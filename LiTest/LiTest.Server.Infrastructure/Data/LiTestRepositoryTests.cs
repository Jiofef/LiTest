using LiTest.Server.Core.Contracts.Data;
using LiTest.Server.Infrastructure.Testing;
using LiTest.Shared.Core.Community;
using LiTest.Shared.Core.Testing;
using Microsoft.EntityFrameworkCore;

namespace LiTest.Server.Infrastructure.Data
{
    public partial class LiTestRepository
    {
        public async Task<Guid> AddTestAsync(LiTestEntity test)
        {
            _context.Tests.Add(test);
            await _context.SaveChangesAsync();
            return test.Id;
        }

        public async Task<bool> HardDeleteTestAsync(Guid uid)
        {
            var test = await _context.Tests
                .FirstOrDefaultAsync(r => r.Id == uid);

            if (test != null)
            {
                _context.Tests.Remove(test);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> SoftDeleteTestAsync(Guid uid)
        {
            var test = await _context.Tests
                .FirstOrDefaultAsync(r => r.Id == uid);

            if (test != null && test.DeletedAt == null)
            {
                test.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RestoreTestAsync(Guid uid)
        {
            var test = await _context.Tests
                .FirstOrDefaultAsync(r => r.Id == uid);

            if (test != null && test.DeletedAt != null)
            {
                test.DeletedAt = null;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<LiTestEntity>> GetTestsAsync(IEnumerable<Guid> ids, LiTestGetOptions options)
        {
            IQueryable<LiTestEntity> queryable = _context.Tests
                .Where(lt => ids.Contains(lt.Id))
                .AsNoTracking();

            queryable = GetLitestQueryIncluding(queryable, options.IncludeMode);
            queryable = GetLitestQuerySortingBy(queryable, options.SortMode);

            return await queryable.ToListAsync();
        }

        public async Task<LiTestEntity?> TryGetTestAsync(Guid id)
        {
            return await _context.Tests
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<LiTestEntity>> SearchTestsAsync(string query, int lastId, LiTestGetOptions options, int limit = 20)
        {
            var queryable = _context.Tests
                .Where(lt =>
                    EF.Functions.ILike(lt.Name, $"%{query}%") ||
                    lt.Tags.Any(t => t == query)
                );

            queryable = GetLitestQueryIncluding(queryable, options.IncludeMode);
            queryable = GetLitestQuerySortingBy(queryable, options.SortMode);

            return await queryable.ToListAsync();
        }

        public async Task<TResult> ExecuteOnTestAsync<TResult>(Guid id, Func<LiTestEntity, Task<TResult>> action)
        {
            var queryable = _context.Tests.AsQueryable();
            queryable = GetLitestQueryIncluding(queryable, TestsIncludeModeEnum.IncludeAll);

            var test = await queryable.FirstOrDefaultAsync(t => t.Id == id);

            if (test == null)
                throw new KeyNotFoundException();

            var result = await action(test);

            await _context.SaveChangesAsync();
            return result;
        }

        // Overload for actions without return value
        public Task ExecuteOnTestAsync(Guid id, Func<LiTestEntity, Task> action)
            => ExecuteOnTestAsync(id, async r =>
            {
                await action(r);
                return true;
            });


        // Query modifying
        private IQueryable<LiTestEntity> GetLitestQueryIncluding(IQueryable<LiTestEntity> queryable, TestsIncludeModeEnum includeMode)
        {
            var resultQueryable = queryable;
            if (includeMode >= TestsIncludeModeEnum.IncludeQuestions)
            {
                resultQueryable = resultQueryable.
                    Include(lt => lt.Questions).ThenInclude(q => q.CorrectAnswer);

                if (includeMode == TestsIncludeModeEnum.IncludeAll)
                {
                    resultQueryable = resultQueryable
                        .Include(lt => lt.Questions)
                        .ThenInclude(q => (q.Content as QuestionOptionsContentEntity)!.Options);
                }
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
    }
}