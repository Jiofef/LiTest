using LiTest.Server.Core;
using LiTest.Server.Core.Contracts.Data;
using LiTest.Shared.Core.Community;

namespace LiTest.Server.Services.Community
{
    public interface IUserSecurityService
    {
        Task<Result<bool>> AuthenticateAsync(UserAuthenticationDto auth);
        Task<Result<RefreshTokenRequestResult>> GetOrUpdateRefreshTokenAsync(Guid userId);
        Task<Result<UserEntity>> IdentificateAsync(string login, UsersIncludeModeEnum includeUserInfoMode = UsersIncludeModeEnum.Min);
        Task<Result<AuthResponseDto>> LoginAsync(UserAuthenticationDto auth);
        Task<Result<(string, RefreshTokenRequestResult?)>> RequestAccessTokenAsync(Guid userId, string refreshTokenString);
    }
    public record RefreshTokenRequestResult(UserRefreshTokenEntity Token, UserRefreshTokenEntity? OldToken, bool WasOldTokenExpired)
    {
        public bool IsJustCreated() => OldToken == null;
        public bool IsJustUpdated() => Token != OldToken;
    }
}