using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using The_Last_Dance_Project.Dtos;
using The_Last_Dance_Project.Interfaces;

namespace The_Last_Dance_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthInterface _authService;
        private readonly IJwtInterface _jwtService;

        public AuthController(IAuthInterface authService, IJwtInterface jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
        {
            try
            {
                var result = await _authService.RegisterUser(
                    registerRequest.UserName,
                    registerRequest.Password,
                    registerRequest.Email,
                    registerRequest.PhoneNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var result = await _authService.LoginUser(request.UserName, request.Password, request.RememberMe);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token)) return BadRequest();

            var result = await _authService.LogoutUser(token);
            if (result) return Ok(new { message = "Logged out successfully" });
            return BadRequest(new { message = "Logout failed" });
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] TokenRefreshRequestDto tokenRequest)
        {
            try
            {
                var result = _jwtService.RefreshAccessToken(tokenRequest.AccessToken, tokenRequest.RefreshToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (await _authService.IsTokenBlacklisted(token)) return Unauthorized(new { message = "Token has been revoked" });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                var result = await _authService.CheckSession(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

