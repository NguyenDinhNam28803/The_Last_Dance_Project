using The_Last_Dance_Project.Dto;

namespace The_Last_Dance_Project.Interface
{
    public interface IAuthInterface
    {
        Task<UserInforReponse> LoginUser(string userName, string password);
        Task<UserInforReponse> RegisterUser(string userName, string password, string email, string phoneNumber);
    }
}
