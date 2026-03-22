using The_Last_Dance_Project.Dtos;

namespace The_Last_Dance_Project.Interfaces
{
    public interface IJwtInterface
    {
        JwtTokenDto GenerateJwtTokens(string userId, string role, string userName, bool rememberMe);
        JwtTokenDto RefreshAccessToken(string accessToken, string refreshToken);
    }
}
