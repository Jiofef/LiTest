using LiTest.Server.Core.Contracts.Data;
using LiTest.Server.Infrastructure.Testing;
using LiTest.Shared.Core.Community;
using Microsoft.EntityFrameworkCore;

namespace LiTest.Server.Infrastructure.Data
{
    public partial class LiTestRepository
    {
        public async Task<Guid> AddUserAsync(UserEntity user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task<bool> HardDeleteUserAsync(Guid uid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(r => r.Id == uid);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> SoftDeleteUserAsync(Guid uid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(r => r.Id == uid);
            if (user != null && user.DeletedAt == null)
            {
                user.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RestoreUserAsync(Guid uid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(r => r.Id == uid);
            if (user != null && user.DeletedAt != null)
            {
                user.DeletedAt = null;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<UserEntity>> GetUsersAsync(IEnumerable<Guid> ids, UsersGetOptions? options = null)
        {
            options ??= new();
            var query = _context.Users.Where(u => ids.Contains(u.Id)).AsNoTracking();
            query = GetUsersQueryIncluding(query, options.IncludeMode);
            return await query.ToListAsync();
        }

        public async Task<UserEntity?> TryGetUserAsync(Guid id, UsersGetOptions? options = null)
        {
            options ??= new();
            var query = _context.Users.AsNoTracking();
            query = GetUsersQueryIncluding(query, options.IncludeMode);
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<UserEntity?> TryGetUserByLoginAsync(string login, UsersGetOptions? options = null)
        {
            options ??= new();
            var query = _context.Users.AsNoTracking();
            query = GetUsersQueryIncluding(query, options.IncludeMode);
            return await query.FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<TResult> ExecuteOnUserAsync<TResult>(Guid id, Func<UserEntity, Task<TResult>> action)
        {
            var queryable = _context.Users.AsQueryable();
            queryable = GetUsersQueryIncluding(queryable, UsersIncludeModeEnum.IncludeAll);
            var user = await queryable.FirstOrDefaultAsync(t => t.Id == id);

            if (user == null) throw new KeyNotFoundException();

            var result = await action(user);
            await _context.SaveChangesAsync();
            return result;
        }
        // Overload for actions without return value
        public Task ExecuteOnUserAsync(Guid id, Func<UserEntity, Task> action)
            => ExecuteOnUserAsync(id, async r =>
            {
                await action(r);
                return true;
            });

        // Query modifying
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
    }
}