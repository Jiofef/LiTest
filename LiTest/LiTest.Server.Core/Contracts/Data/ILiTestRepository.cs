using LiTest.Server.Infrastructure.Testing;
using LiTest.Shared.Core.Community;

namespace LiTest.Server.Core.Contracts.Data
{
    public enum UsersIncludeModeEnum { Min, IncludeRefreshTokens, IncludeAll }
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
    public class UsersGetOptions
    {
        public UsersGetOptions(UsersIncludeModeEnum getMode = UsersIncludeModeEnum.Min)
        {
            IncludeMode = getMode;
        }
        public UsersIncludeModeEnum IncludeMode { get; set; }
    }
    public interface ILiTestRepository
    {
        Task<Guid> AddTestAsync(LiTestEntity test);
        Task<Guid> AddUserAsync(UserEntity user);
        Task<ITransactionWrapper> BeginTransactionAsync();
        Task ExecuteOnTestAsync(Guid id, Func<LiTestEntity, Task> action);
        Task<TResult> ExecuteOnTestAsync<TResult>(Guid id, Func<LiTestEntity, Task<TResult>> action);
        Task ExecuteOnUserAsync(Guid id, Func<UserEntity, Task> action);
        Task<TResult> ExecuteOnUserAsync<TResult>(Guid id, Func<UserEntity, Task<TResult>> action);
        Task<List<LiTestEntity>> GetTestsAsync(IEnumerable<Guid> ids, LiTestGetOptions options);
        Task<List<UserEntity>> GetUsersAsync(IEnumerable<Guid> ids, UsersGetOptions? options = null);
        Task<bool> HardDeleteTestAsync(Guid uid);
        Task<bool> HardDeleteUserAsync(Guid uid);
        Task<bool> RestoreTestAsync(Guid uid);
        Task<bool> RestoreUserAsync(Guid uid);
        Task SaveChangesAsync();
        Task<List<LiTestEntity>> SearchTestsAsync(string query, int lastId, LiTestGetOptions options, int limit = 20);
        Task<bool> SoftDeleteTestAsync(Guid uid);
        Task<bool> SoftDeleteUserAsync(Guid uid);
        Task<LiTestEntity?> TryGetTestAsync(Guid id);
        Task<UserEntity?> TryGetUserAsync(Guid id, UsersGetOptions? options = null);
        Task<UserEntity?> TryGetUserByLoginAsync(string login, UsersGetOptions? options = null);
    }
}