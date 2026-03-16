using FluentValidation;
using LiTest.Server.Core;
using LiTest.Server.Core.Contracts.Data;
using LiTest.Server.Core.Contracts.Security;
using LiTest.Server.Core.Security;
using LiTest.Server.Core.Services;
using LiTest.Shared.Core.Community;
using static LiTest.Server.Services.StandardResultErrors;

namespace LiTest.Server.Services.Community
{
    public class UserMainService : IUserMainService
    {
        private readonly ILiTestRepository _repo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IValidator<UserRegistrationDto> _userRegistrationValidator;
        private readonly IUserSecurityService _userSecurityService;
        public UserMainService(ILiTestRepository repo, IPasswordHasher hasher, IJwtProvider jwtProvider, IValidator<UserRegistrationDto> urv, IUserSecurityService uss)
        {
            _repo = repo;
            _passwordHasher = hasher;
            _userRegistrationValidator = urv;
            _userSecurityService = uss;
        }
        public async Task<Result<Guid>> RegisterAsync(UserRegistrationDto info)
        {
            var validationResult = _userRegistrationValidator.Validate(info);
            if (!validationResult.IsValid)
                return Result<Guid>.Failure(
                    validationResult.Errors.Select(e => e.ErrorCode).ToArray(),
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());

            // Use transaction for atomic registration
            using var transaction = await _repo.BeginTransactionAsync();
            try
            {
                var existingUser = await _repo.TryGetUserByLoginAsync(info.Login);
                if (existingUser != null)
                    return Result<Guid>.Failure(ENTITY_ALREADY_EXISTS);

                var passwordHash = _passwordHasher.Generate(info.Password);
                var newUser = new UserEntity()
                {
                    Id = Guid.NewGuid(),
                    Nickname = info.Nickname,
                    Login = info.Login,
                    PasswordHashed = passwordHash,
                };

                var userId = await _repo.AddUserAsync(newUser);
                await transaction.CommitAsync();

                return Result<Guid>.Success(userId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Result<AuthResponseDto>> RegisterAndLoginAsync(UserRegistrationDto dto)
        {
            var registrationResult = await RegisterAsync(dto);
            if (!registrationResult.IsSuccess)
                return Result<AuthResponseDto>.Failure(registrationResult.ErrorCodes, registrationResult.ErrorMessages);

            var authenticationDto = new UserAuthenticationDto(dto.Login, dto.Password);
            var loginResult = await _userSecurityService.LoginAsync(authenticationDto);
            if (!registrationResult.IsSuccess)
                return Result<AuthResponseDto>.Failure(registrationResult.ErrorCodes, registrationResult.ErrorMessages);

            return Result<AuthResponseDto>.Success(loginResult.Value!);
        }
        public async Task<Result<bool>> DeleteAsync(Guid realUserId, Guid userToDeleteId)
        {
            if (realUserId != userToDeleteId)
                return Result<bool>.Failure(ACCESS_DENIED);

            await _repo.SoftDeleteUserAsync(realUserId);

            return Result<bool>.Success(true);
        }
        public async Task<Result<bool>> RecoverAsync(UserAuthenticationDto auth, Guid userToRecoverId)
        {
            var isAuthSuccess = await _userSecurityService.AuthenticateAsync(auth);

            if (!isAuthSuccess.Value)
                return Result<bool>.Failure(ACCESS_DENIED);

            await _repo.RestoreUserAsync(userToRecoverId);

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> UpdateProfileInfoAsync(Guid realUserId, UserDto dto)
        {
            if (realUserId != dto.Id)
                return Result<bool>.Failure(ACCESS_DENIED);

            return await _repo.ExecuteOnUserAsync(realUserId, (user) =>
            {
                user.Nickname = dto.Nickname;

                var result = Result<bool>.Success(true);
                return Task.FromResult(result);
            });
        }
    }
}
