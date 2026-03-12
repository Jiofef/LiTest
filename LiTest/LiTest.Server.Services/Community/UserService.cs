using FluentValidation;
using LiTest.Server.Infrastructure.Data;
using LiTest.Shared.Core.Community;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Text;
using static LiTest.Server.Services.StandardResultErrors;
using LiTest.Server.Core.Contracts.Data;
using LiTest.Server.Core.Contracts.Security;
using LiTest.Server.Core.Security;

namespace LiTest.Server.Services.Community
{
    public class UserService
    {
        private readonly ILiTestRepository _repo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly IValidator<UserRegistrationDto> _userRegistrationValidator;
        public UserService(ILiTestRepository repo, IPasswordHasher hasher, IJwtProvider jwtProvider, IValidator<UserRegistrationDto> urv) 
        {
            _repo = repo;
            _passwordHasher = hasher;
            _jwtProvider = jwtProvider;
            _userRegistrationValidator = urv;
        }
        public async Task<Result<AuthResponseDto>> RegisterAsync(UserRegistrationDto info) 
        {
            // Validation
            var validationResult = _userRegistrationValidator.Validate(info);

            if (!validationResult.IsValid)
                return Result<AuthResponseDto>.Failure(
                    validationResult.Errors.Select(e => e.ErrorCode).ToArray(),
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());

            var existingUser = await _repo.TryGetUserByLoginAsync(info.Login);
            if (existingUser != null)
                return Result<AuthResponseDto>.Failure(ENTITY_ALREADY_EXISTS);

            // Registration
            var passwordHash = _passwordHasher.Generate(info.Password);
            var newUser = new UserEntity()
            {
                Id = Guid.NewGuid(),
                Nickname = info.Nickname,
                Login = info.Login,
                PasswordHashed = passwordHash,
            };

            var userId = await _repo.AddUserAsync(newUser);

            // Generating access and refresh tokens
            var accessToken = _jwtProvider.GenerateAccessToken(newUser);
            var refreshTokenString = _jwtProvider.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(31);
            var refreshToken = new UserRefreshTokenEntity() 
            {
                Id = Guid.NewGuid(), 
                UserId = userId,  
                ExpiryDate = refreshTokenExpiry,
                Token = refreshTokenString,
            };

            await _repo.ExecuteOnUserAsync(userId, (user =>
            {
                user.RefreshToken = refreshToken;
                return Task.CompletedTask;
            }));

            var result = new AuthResponseDto(accessToken, refreshTokenString);
            return Result<AuthResponseDto>.Success(result);
        }
        public async Task<bool> DeleteAsync() { throw new NotImplementedException(); }
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
