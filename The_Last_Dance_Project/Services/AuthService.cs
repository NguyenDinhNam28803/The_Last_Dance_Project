using The_Last_Dance_Project.Data;
using The_Last_Dance_Project.Dto;
using The_Last_Dance_Project.Interface;
using The_Last_Dance_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace The_Last_Dance_Project.Services
{
    public class AuthService : IAuthInterface
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtInterface _jwtService;

        public AuthService(ApplicationDbContext context, IJwtInterface jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<UserInforReponse> LoginUser(string userName, string password)
        {
            try
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(u => u.UserName == userName);
                if (customer == null || string.IsNullOrEmpty(customer.PasswordHash))
                {
                    throw new Exception("Invalid username or password");
                }

                var checkPassword = BCrypt.Net.BCrypt.Verify(password, customer.PasswordHash);

                if (checkPassword)
                {
                    // Lấy RoleName từ SystemCodeValue nếu RoleId được lưu ở đó, 
                    // hoặc mặc định là "User" nếu không tìm thấy
                    var roleName = "User";
                    if (!string.IsNullOrEmpty(customer.RoleId))
                    {
                        var role = await _context.SystemCodeValues
                            .FirstOrDefaultAsync(v => v.CodeValue == customer.RoleId);
                        if (role != null) roleName = role.DisplayValue;
                    }

                    var token = _jwtService.GenerateJwtTokens(customer.CustId, roleName, customer.UserName ?? customer.Name);
                    
                    var response = new UserInforReponse
                    {
                        Authorization = token,
                        UserId = customer.CustId,
                        UserName = customer.UserName ?? customer.Name,
                        Email = customer.Email ?? "",
                        PhoneNumber = customer.PhoneNumber ?? ""
                    };
                    return response;
                }
                else
                {
                    throw new Exception("Invalid username or password");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserInforReponse> RegisterUser(string userName, string password, string email, string phoneNumber)
        {
            try
            {
                var userExists = await _context.Customers.AnyAsync(u => u.UserName == userName || u.Email == email);
                if (userExists)
                {
                    throw new Exception("User with information already exists");
                }

                if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
                {
                    throw new Exception("Missing required fields");
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                
                var newCustomer = new Customer
                {
                    CustId = Guid.NewGuid().ToString().Substring(0, 20), // Giới hạn 20 ký tự theo StringLength(20)
                    Name = userName, // Sử dụng UserName làm Name mặc định
                    UserName = userName,
                    PasswordHash = hashedPassword,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    RecordStatus = "1", // Mặc định Active
                    Status = "Active",
                    CreatedDate = DateTime.Now
                };

                _context.Customers.Add(newCustomer);
                await _context.SaveChangesAsync();

                var token = _jwtService.GenerateJwtTokens(newCustomer.CustId, "User", newCustomer.UserName);
                return new UserInforReponse
                {
                    Authorization = token,
                    UserId = newCustomer.CustId,
                    UserName = newCustomer.UserName,
                    Email = newCustomer.Email,
                    PhoneNumber = newCustomer.PhoneNumber
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
