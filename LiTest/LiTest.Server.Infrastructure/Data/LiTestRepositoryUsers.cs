using LiTest.Server.Core.Contracts.Data;
using LiTest.Server.Infrastructure.Testing;
using LiTest.Shared.Core.Community;
using LiTest.Shared.Core.Testing;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace LiTest.Server.Infrastructure.Data
{
    public partial class LiTestRepository
    {
        public async Task<Guid> AddUserAsync(UserEntity user)
        {
            await using var db = _ctxFactory.CreateDbContext();

            db.Users.Add(user);
            await db.SaveChangesAsync();
            return user.Id;
        }

        /// <summary>
        /// Prefer soft delete
        /// </summary>
        public async Task<bool> HardDeleteUserAsync(Guid uid)
        {
            await using var db = _ctxFactory.CreateDbContext();

            var user = await db.Users
                .FirstOrDefaultAsync(r => r.Id == uid);

            if (user != null)
            {
                db.Users.Remove(user);
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> SoftDeleteUserAsync(Guid uid)
        {
            await using var db = _ctxFactory.CreateDbContext();

            var user = await db.Users
                .FirstOrDefaultAsync(r => r.Id == uid);

            if (user != null && user.DeletedAt == null)
            {
                user.DeletedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> RestoreUserAsync(Guid uid)
        {
            await using var db = _ctxFactory.CreateDbContext();

            var user = await db.Users
                .FirstOrDefaultAsync(r => r.Id == uid);

            if (user != null && user.DeletedAt != null)
            {
                user.DeletedAt = null;
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        private IQueryable<UserEntity> GetUsersQueryIncluding(IQueryable<UserEntity> queryable, UsersIncludeModeEnum includeMode)
        {
            var resultQueryable = queryable;
            if (includeMode >= UsersIncludeModeEnum.IncludeRefreshTokens)
            {
                resultQueryable = queryable
                    .Include(u => u.RefreshToken);

                if (includeMode == UsersIncludeModeEnum.IncludeAll)
                {
                    resultQueryable = resultQueryable
                        .Include(u => u.AttemptStatuses)
                        .ThenInclude(a => a.QuestionStatuses)
                        .ThenInclude(qs => qs.Answer)
                        ;

                    resultQueryable = resultQueryable
                        .Include(u => u.AttemptStatuses)
                        .ThenInclude(a => a.QuestionStatuses)
                        .ThenInclude(qs => qs.AnswerCorrectnessDetails);
                }
            }
            return resultQueryable;
        }
        public async Task<List<UserEntity>> GetUsersAsync(IEnumerable<Guid> ids, UsersGetOptions options)
        {
            var ctx = await _ctxFactory.CreateDbContextAsync();

            var result = new List<UserEntity>();

            var usersQuery = ctx.Users
                .Where(u => ids.Contains(u.Id))
                .AsNoTracking();

            usersQuery = GetUsersQueryIncluding(usersQuery, options.IncludeMode);

            var users = await usersQuery.ToListAsync();
            return users;
        }

        public async Task<UserEntity?> TryGetUserAsync(Guid id)
        {
            var ctx = await _ctxFactory.CreateDbContextAsync();

            var result = await ctx.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            return result;
        }

        public async Task<UserEntity?> TryGetUserByLoginAsync(string email)
        {
            var ctx = await _ctxFactory.CreateDbContextAsync();

            var result = await ctx.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Login == email);

            return result;
        }

        public Task ExecuteOnUserAsync(Guid id, Func<UserEntity, Task> action)
            => ExecuteOnUserAsync(id, async r =>
            {
                await (action(r));
                return true; // dummy value
            });
        public async Task<TResult> ExecuteOnUserAsync<TResult>(Guid id, Func<UserEntity, Task<TResult>> action)
        {
            await using var ctx = _ctxFactory.CreateDbContext();

            var queryable = ctx.Users.AsQueryable();
            queryable = GetUsersQueryIncluding(queryable, UsersIncludeModeEnum.IncludeAll);
            var user = await queryable
                .FirstOrDefaultAsync(t => t.Id == id);

            if (user == null)
                throw new KeyNotFoundException();

            var result = await action(user);

            await ctx.SaveChangesAsync();
            return result;
        }
    }
}
