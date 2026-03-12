using LiTest.Shared.Core.Community;

namespace LiTest.Server.Core.Security
{
    public interface IJwtProvider
    {
        string GenerateAccessToken(UserEntity user);
        string GenerateRefreshToken();
    }
}