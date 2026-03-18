using The_Last_Dance_Project.Dto;

namespace The_Last_Dance_Project.Interface
{
    public interface IAuthInterface
    {
        Task<UserInforReponse> LoginUser(string userName, string password, bool rememberMe = false);
        Task<UserInforReponse> RegisterUser(string userName, string password, string email, string phoneNumber);
        Task<bool> LogoutUser(string token);
        Task<bool> IsTokenBlacklisted(string token);
        Task<UserInforReponse> CheckSession(string userId);
    }
}
