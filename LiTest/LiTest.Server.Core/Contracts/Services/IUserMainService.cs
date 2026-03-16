using LiTest.Server.Core;
using LiTest.Shared.Core.Community;

namespace LiTest.Server.Core.Services
{
    public interface IUserMainService
    {
        Task<Result<bool>> DeleteAsync(Guid realUserId, Guid userToDeleteId);
        Task<Result<bool>> RecoverAsync(UserAuthenticationDto auth, Guid userToRecoverId);
        Task<Result<AuthResponseDto>> RegisterAndLoginAsync(UserRegistrationDto dto);
        Task<Result<Guid>> RegisterAsync(UserRegistrationDto info);
        Task<Result<bool>> UpdateProfileInfoAsync(Guid realUserId, UserDto dto);
    }
}