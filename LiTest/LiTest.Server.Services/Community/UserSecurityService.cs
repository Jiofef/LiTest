using LiTest.Server.Core;
using LiTest.Server.Core.Contracts.Data;
using LiTest.Server.Core.Contracts.Security;
using LiTest.Server.Core.Security;
using LiTest.Shared.Core.Community;
using static LiTest.Server.Services.StandardResultErrors;

namespace LiTest.Server.Services.Community
{
    public class UserSecurityService : IUserSecurityService
    {
        private readonly ILiTestRepository _repo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        public UserSecurityService(ILiTestRepository repo, IPasswordHasher hasher, IJwtProvider jwtProvider)
        {
            _repo = repo;
            _passwordHasher = hasher;
            _jwtProvider = jwtProvider;
        }
        public async Task<Result<UserEntity>> IdentificateAsync(string login, UsersIncludeModeEnum includeUserInfoMode = UsersIncludeModeEnum.Min)
        {
            var userGetOptions = new UsersGetOptions() { IncludeMode = UsersIncludeModeEnum.IncludeAll };
            var user = await _repo.TryGetUserByLoginAsync(login, userGetOptions);

            if (user == null)
                return Result<UserEntity>.Failure(ENTITY_NOT_FOUND);

            return Result<UserEntity>.Success(user);
        }

        public async Task<Result<bool>> AuthenticateAsync(UserAuthenticationDto auth)
        {
            var userIdentificate = await IdentificateAsync(auth.Login);

            if (!userIdentificate.IsSuccess)
                return Result<bool>.Failure(userIdentificate.ErrorCodes);

            var user = userIdentificate.Value;
            var realPasswordHash = user!.PasswordHashed;
            bool isPasswordCorrect = _passwordHasher.Verify(auth.Password, realPasswordHash);

            return Result<bool>.Success(isPasswordCorrect);
        }

        public async Task<Result<RefreshTokenRequestResult>> GetOrUpdateRefreshTokenAsync(Guid userId)
        {
            var userEntity = await _repo.TryGetUserAsync(userId);
            if (userEntity == null)
                return Result<RefreshTokenRequestResult>.Failure(ENTITY_NOT_FOUND);

            List<string> operationSuccessMessages = new();

            UserRefreshTokenEntity? oldToken = null;
            UserRefreshTokenEntity? token = null;

            using var transaction = await _repo.BeginTransactionAsync();
            try
            {
                await _repo.ExecuteOnUserAsync(userId, (user) =>
                {
                    oldToken = user.RefreshToken;

                    bool shouldUpdate =
                        oldToken == null
                        || oldToken.ExpiryDate < DateTime.UtcNow.AddDays(14);
                    if (shouldUpdate)
                    {
                        var refreshTokenString = _jwtProvider.GenerateRefreshToken();
                        var refreshTokenExpiry = DateTime.UtcNow.AddDays(31);
                        var refreshToken = new UserRefreshTokenEntity()
                        {
                            Id = Guid.NewGuid(),
                            UserId = user.Id,
                            ExpiryDate = refreshTokenExpiry,
                            Token = refreshTokenString,
                        };
                        user.RefreshToken = refreshToken;
                        token = refreshToken;
                    }
                    else
                        token = oldToken;

                    return Task.CompletedTask;
                });
            }
            catch
            {
                await transaction.RollbackAsync();
            }

            if (token == null)
                return Result<RefreshTokenRequestResult>.Failure(UNKNOWN_ERROR);

            bool isOldTokenExpired = oldToken?.ExpiryDate < DateTime.UtcNow;
            var result = new RefreshTokenRequestResult(token, oldToken, isOldTokenExpired);
            return Result<RefreshTokenRequestResult>.Success(result, operationSuccessMessages.ToArray());
        }

        public async Task<Result<(string, RefreshTokenRequestResult?)>> RequestAccessTokenAsync(Guid userId, string refreshTokenString)
        {
            // This GetOrUpdate method has a side effect: it refreshes the token if it has expired. In this case, the client will not be allowed to proceed;
            // if the client calls the method again without logging in again, the token will fail the validity check. Therefore, this side effect does not compromise security.
            var refreshTokenRequestResult = await GetOrUpdateRefreshTokenAsync(userId);
            if (!refreshTokenRequestResult.IsSuccess)
                return Result<(string, RefreshTokenRequestResult?)>.Failure(refreshTokenRequestResult.ErrorCodes);

            if (refreshTokenRequestResult.Value!.WasOldTokenExpired)
                return Result<(string, RefreshTokenRequestResult?)>.Failure(ACCESS_DENIED);

            // If sended token is outdated
            RefreshTokenRequestResult? updatedRefreshToken = null;

            var oldRefreshToken = refreshTokenRequestResult.Value.OldToken;
            var actualRefreshToken = refreshTokenRequestResult.Value.Token;

            bool isRefreshTokenMathingNewOne = Equals(actualRefreshToken, refreshTokenString);
            bool isRefreshTokenMathing = isRefreshTokenMathingNewOne || oldRefreshToken != null && Equals(oldRefreshToken, refreshTokenString);
            if (!isRefreshTokenMathingNewOne)
            {
                updatedRefreshToken = refreshTokenRequestResult.Value;
            }

            var user = await _repo.TryGetUserAsync(userId);
            if (user == null)
                return Result<(string, RefreshTokenRequestResult?)>.Failure(UNKNOWN_ERROR);

            var accessToken = _jwtProvider.GenerateAccessToken(user);

            var result = Result<(string, RefreshTokenRequestResult?)>.Success(new(accessToken, updatedRefreshToken));
            return result;
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(UserAuthenticationDto auth)
        {
            // 1. Checking the password
            var authCheck = await AuthenticateAsync(auth);
            if (!authCheck.IsSuccess || !authCheck.Value)
                return Result<AuthResponseDto>.Failure(ACCESS_DENIED);

            // 2. Getting user
            var userResult = await IdentificateAsync(auth.Login);
            if (!userResult.IsSuccess) return Result<AuthResponseDto>.Failure(userResult.ErrorCodes);

            var user = userResult.Value;

            // 3. Getting the tokens
            var tokenResult = await RequestAccessTokenAsync(user!.Id, string.Empty);
            if (!tokenResult.IsSuccess) return Result<AuthResponseDto>.Failure(tokenResult.ErrorCodes);

            return Result<AuthResponseDto>.Success(new AuthResponseDto(
                UserId: user.Id,
                AccessToken: tokenResult.Value.Item1,
                RefreshToken: tokenResult.Value.Item2!.Token.Token
                ));
        }
    }
}
