using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using The_Last_Dance_Project.Dtos;
using The_Last_Dance_Project.Interfaces;

namespace The_Last_Dance_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu đăng nhập cho tất cả các endpoint
    public class UserController : ControllerBase
    {
        private readonly ICustomerService _userService;

        public UserController(ICustomerService userService)
        {
            _userService = userService;
        }

        // Lấy danh sách người dùng - Chỉ ADMIN
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetUserManagementListAsync();
            return Ok(users);
        }

        // Xem chi tiết người dùng
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            // Kiểm tra quyền: Admin hoặc chính chủ mới được xem
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "ADMIN" && currentUserId != id)
            {
                return Forbid("Bạn không có quyền xem thông tin người dùng khác.");
            }

            var user = await _userService.GetUserDetailByIdAsync(id);
            if (user == null) return NotFound("Không tìm thấy người dùng.");
            return Ok(user);
        }

        // Admin tạo User mới
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "SYSTEM";
            var success = await _userService.AdminCreateUserAsync(dto, adminId);
            if (!success) return BadRequest("Không thể tạo người dùng. Có thể CustId hoặc UserName đã tồn tại.");
            return Ok("Tạo người dùng thành công.");
        }

        // Admin cập nhật thông tin User
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateDto dto)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "SYSTEM";
            var success = await _userService.AdminUpdateUserAsync(id, dto, adminId);
            if (!success) return NotFound("Không tìm thấy người dùng để cập nhật.");
            return Ok("Cập nhật thành công.");
        }

        // Admin đổi Role
        [HttpPatch("{id}/role")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ChangeRole(string id, [FromBody] ChangeRoleDto dto)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "SYSTEM";
            var success = await _userService.ChangeUserRoleAsync(id, dto.RoleId, adminId);
            if (!success) return BadRequest("Thay đổi vai trò thất bại. Kiểm tra lại ID người dùng hoặc ID vai trò.");
            return Ok("Thay đổi vai trò thành công.");
        }

        // Admin bật/tắt trạng thái
        [HttpPatch("{id}/toggle-status")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "SYSTEM";
            var success = await _userService.ToggleUserStatusAsync(id, adminId);
            if (!success) return NotFound("Không tìm thấy người dùng.");
            return Ok("Thay đổi trạng thái thành công.");
        }
    }
}

