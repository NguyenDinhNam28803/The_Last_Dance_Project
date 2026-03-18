using System.Security.Claims;
using The_Last_Dance_Project.Dto;

namespace The_Last_Dance_Project.Interface
{
    public interface IJwtInterface
    {
        JwtTokenDto GenerateJwtTokens(string userId, string roleName, string userName, bool rememberMe = false);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        JwtTokenDto RefreshAccessToken(string accessToken, string refreshToken);

    }
}
