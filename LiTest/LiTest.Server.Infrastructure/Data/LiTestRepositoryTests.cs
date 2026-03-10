using LiTest.Server.Infrastructure.Testing;
using LiTest.Shared.Core.Community;
using LiTest.Shared.Core.Testing;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace LiTest.Server.Infrastructure.Data
{
    public partial class LiTestRepository
    {
        public async Task<Guid> AddTestAsync(LiTestEntity test)
        {
            await using var db = _ctxFactory.CreateDbContext();

            db.Tests.Add(test);
            await db.SaveChangesAsync();
            return test.Id;
        }
        /// <summary>
        /// Prefer soft delete
        /// </summary>
        public async Task<bool> HardDeleteTestAsync(Guid uid)
        {
            await using var db = _ctxFactory.CreateDbContext();

            var test = await db.Tests
                .FirstOrDefaultAsync(r => r.Id == uid);

            if (test != null)
            {
                db.Tests.Remove(test);
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> SoftDeleteTestAsync(Guid uid)
        {
            await using var db = _ctxFactory.CreateDbContext();

            var test = await db.Tests
                .FirstOrDefaultAsync(r => r.Id == uid);

            if (test != null && test.DeletedAt == null)
            {
                test.DeletedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> RestoreTestAsync(Guid uid)
        {
            await using var db = _ctxFactory.CreateDbContext();

            var test = await db.Tests
                .FirstOrDefaultAsync(r => r.Id == uid);

            if (test != null && test.DeletedAt != null)
            {
                test.DeletedAt = null;
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public enum TestsSortEnum { Passers, Likes, Latest, Earliest }
        public enum TestsIncludeModeEnum { Min, IncludeQuestions, IncludeAll }
        public class LiTestGetOptions
        {
            public LiTestGetOptions(TestsIncludeModeEnum getMode = TestsIncludeModeEnum.Min, TestsSortEnum sortMode = TestsSortEnum.Passers)
            {
                IncludeMode = getMode;
                SortMode = sortMode;
            }
            public TestsIncludeModeEnum IncludeMode { get; set; }
            public TestsSortEnum SortMode { get; set; }
        }
        private IQueryable<LiTestEntity> GetLitestQueryIncluding(IQueryable<LiTestEntity> queryable, TestsIncludeModeEnum includeMode)
        {
            var resultQueryable = queryable;
            if (includeMode >= TestsIncludeModeEnum.IncludeQuestions)
            {
                var questionQuery = resultQueryable.
                    Include(lt => lt.Questions).ThenInclude(q => q.CorrectAnswer);

                if (includeMode == TestsIncludeModeEnum.IncludeAll)
                    resultQueryable = questionQuery
                        .Include(lt => lt.Questions)
                        .ThenInclude(q => (q.Content as QuestionOptionsContentEntity)!.Options);
                else
                    resultQueryable = questionQuery;
            }
            return resultQueryable;
        }
        private IQueryable<LiTestEntity> GetLitestQuerySortingBy(IQueryable<LiTestEntity> queryable, TestsSortEnum sortMode)
        {
            var resultQueryable = queryable;
            switch (sortMode)
            {
                case TestsSortEnum.Passers:
                    return queryable.OrderByDescending(lt => lt.PassersCount);
                case TestsSortEnum.Likes:
                    return queryable.OrderByDescending(lt => lt.LikesCount);
                case TestsSortEnum.Latest:
                    return queryable.OrderByDescending(lt => lt.PublishedAt)
                        .ThenByDescending(lt => lt.Id);
                default:
                    return queryable;
            }
        }
        public async Task<List<LiTestEntity>> GetTests(IEnumerable<Guid> ids, LiTestGetOptions options)
        {
            var ctx = await _ctxFactory.CreateDbContextAsync();

            IQueryable<LiTestEntity> queryable = ctx.Tests
                .Where(lt => ids.Contains(lt.Id))
                .AsNoTracking();

            queryable = GetLitestQueryIncluding(queryable, options.IncludeMode);
            queryable = GetLitestQuerySortingBy(queryable, options.SortMode);

            return await queryable.ToListAsync();
        }

        public async Task<List<LiTestEntity>> SearchTests(string query, int lastId, LiTestGetOptions options, int limit = 20)
        {
            var ctx = await _ctxFactory.CreateDbContextAsync();

            var queryable = ctx.Tests
                .Where(lt =>
                EF.Functions.ILike(lt.Name, $"%{query}%") ||
                lt.Tags.Any(t => t == query)
                );

            queryable = GetLitestQueryIncluding(queryable, options.IncludeMode);
            queryable = GetLitestQuerySortingBy(queryable, options.SortMode);

            return await queryable.ToListAsync();
        }

        public Task ExecuteOnTestAsync(Guid id, Func<LiTestEntity, Task> action)
            => ExecuteOnTestAsync(id, async r =>
            {
                await (action(r));
                return true; // dummy value
            });
        public async Task<TResult> ExecuteOnTestAsync<TResult>(Guid id, Func<LiTestEntity, Task<TResult>> action)
        {
            await using var ctx = _ctxFactory.CreateDbContext();

            var queryable = ctx.Tests.AsQueryable();
            queryable = GetLitestQueryIncluding(queryable, TestsIncludeModeEnum.IncludeAll);
            var test = await queryable
                .FirstOrDefaultAsync(t => t.Id == id);

            if (test == null)
                throw new KeyNotFoundException();

            var result = await action(test);

            await ctx.SaveChangesAsync();
            return result;
        }
    }
}
