using System.Threading.Tasks;
using The_Last_Dance_Project.Dtos;

namespace The_Last_Dance_Project.Interfaces
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
