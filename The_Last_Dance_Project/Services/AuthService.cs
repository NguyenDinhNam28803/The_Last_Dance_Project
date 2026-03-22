using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using The_Last_Dance_Project.Data;
using The_Last_Dance_Project.Dtos;
using The_Last_Dance_Project.Interfaces;
using The_Last_Dance_Project.Models;

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

        private bool ValidatePassword(string password)
        {
            // Regex: Minimum 8 characters, at least 1 uppercase, 1 lowercase, 1 number and 1 special character
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            return regex.IsMatch(password);
        }

        public async Task<UserInforReponse> LoginUser(string userName, string password, bool rememberMe = false)
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
                    var roleName = "User";
                    if (!string.IsNullOrEmpty(customer.RoleId))
                    {
                        var role = await _context.SystemCodeValues
                            .FirstOrDefaultAsync(v => v.CodeValue == customer.RoleId);
                        if (role != null) roleName = role.DisplayValue;
                    }

                    var token = _jwtService.GenerateJwtTokens(customer.CustId, roleName, customer.UserName ?? customer.Name, rememberMe);

                    var response = new UserInforReponse
                    {
                        Authorization = token,
                        UserId = customer.CustId,
                        UserName = customer.UserName ?? customer.Name,
                        Email = customer.Email ?? "",
                        PhoneNumber = customer.PhoneNumber ?? "",
                        Role = roleName
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
                if (!ValidatePassword(password))
                {
                    throw new Exception("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");
                }

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
                    CustId = Guid.NewGuid().ToString().Substring(0, 20),
                    Name = userName,
                    UserName = userName,
                    PasswordHash = hashedPassword,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    RecordStatus = "1",
                    Status = "Active",
                    CreatedDate = DateTime.UtcNow
                };

                _context.Customers.Add(newCustomer);
                await _context.SaveChangesAsync();

                return await LoginUser(userName, password);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> LogoutUser(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token)) return false;

                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var expiry = jwtToken.ValidTo;

                var blacklistEntry = new TokenBlacklist
                {
                    Token = token,
                    ExpiryDate = expiry
                };

                _context.TokenBlacklists.Add(blacklistEntry);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> IsTokenBlacklisted(string token)
        {
            return await _context.TokenBlacklists.AnyAsync(t => t.Token == token);
        }

        public async Task<UserInforReponse> CheckSession(string userId)
        {
            try
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(u => u.CustId == userId);
                if (customer == null) throw new Exception("User not found");

                var roleName = "User";
                if (!string.IsNullOrEmpty(customer.RoleId))
                {
                    var role = await _context.SystemCodeValues
                        .FirstOrDefaultAsync(v => v.CodeValue == customer.RoleId);
                    if (role != null) roleName = role.DisplayValue;
                }

                return new UserInforReponse
                {
                    UserId = customer.CustId,
                    UserName = customer.UserName ?? customer.Name,
                    Email = customer.Email ?? "",
                    PhoneNumber = customer.PhoneNumber ?? "",
                    Role = roleName
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

