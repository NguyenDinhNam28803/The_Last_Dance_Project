using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using The_Last_Dance_Project.Dtos;
using The_Last_Dance_Project.Services;

namespace The_Last_Dance_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu đăng nhập mặc định cho tất cả các endpoint
    public class CustomerContactController : ControllerBase
    {
        private readonly ICustomerContactService _contactService;

        public CustomerContactController(ICustomerContactService contactService)
        {
            _contactService = contactService;
        }

        // Lấy toàn bộ danh sách liên hệ (Thường dùng cho Admin)
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAll()
        {
            var contacts = await _contactService.GetAllAsync();
            return Ok(contacts);
        }

        // Lấy chi tiết một thông tin liên hệ theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var contact = await _contactService.GetByIdAsync(id);
            if (contact == null) return NotFound("Không tìm thấy thông tin liên hệ.");

            // Kiểm tra quyền: Admin hoặc chính chủ mới được xem
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "ADMIN" && currentUserId != contact.CustId)
            {
                return Forbid("Bạn không có quyền xem thông tin liên hệ của người khác.");
            }

            return Ok(contact);
        }

        // Lấy danh sách liên hệ theo Customer ID
        [HttpGet("customer/{custId}")]
        public async Task<IActionResult> GetByCustomerId(string custId)
        {
            // Kiểm tra quyền: Admin hoặc chính chủ
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "ADMIN" && currentUserId != custId)
            {
                return Forbid("Bạn không có quyền xem thông tin liên hệ của người khác.");
            }

            var contacts = await _contactService.GetByCustomerIdAsync(custId);
            return Ok(contacts);
        }

        // Thêm mới liên hệ
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerContactCreateDto dto)
        {
            // Kiểm tra quyền: Admin hoặc chính chủ đang tạo cho mình
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "ADMIN" && currentUserId != dto.CustId)
            {
                return Forbid("Bạn không có quyền thêm liên hệ cho người khác.");
            }

            var success = await _contactService.CreateAsync(dto);
            if (!success) return BadRequest("Không thể thêm liên hệ. Vui lòng kiểm tra lại CustId hoặc ContactId.");

            return Ok("Thêm liên hệ thành công.");
        }

        // Cập nhật liên hệ
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CustomerContactUpdateDto dto)
        {
            var existing = await _contactService.GetByIdAsync(id);
            if (existing == null) return NotFound("Không tìm thấy liên hệ để cập nhật.");

            // Kiểm tra quyền: Admin hoặc chính chủ
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "ADMIN" && currentUserId != existing.CustId)
            {
                return Forbid("Bạn không có quyền cập nhật liên hệ của người khác.");
            }

            var success = await _contactService.UpdateAsync(id, dto);
            if (!success) return BadRequest("Cập nhật liên hệ thất bại.");

            return Ok("Cập nhật thành công.");
        }

        // Xóa liên hệ
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _contactService.GetByIdAsync(id);
            if (existing == null) return NotFound("Không tìm thấy liên hệ để xóa.");

            // Kiểm tra quyền: Admin hoặc chính chủ
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "ADMIN" && currentUserId != existing.CustId)
            {
                return Forbid("Bạn không có quyền xóa liên hệ của người khác.");
            }

            var success = await _contactService.DeleteAsync(id);
            if (!success) return BadRequest("Xóa liên hệ thất bại.");

            return Ok("Xóa thành công.");
        }

        // Đặt liên hệ làm mặc định
        [HttpPatch("{id}/set-default")]
        public async Task<IActionResult> SetDefault(string id)
        {
            var existing = await _contactService.GetByIdAsync(id);
            if (existing == null) return NotFound("Không tìm thấy liên hệ.");

            // Kiểm tra quyền: Admin hoặc chính chủ
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "ADMIN" && currentUserId != existing.CustId)
            {
                return Forbid("Bạn không có quyền thay đổi thông tin của người khác.");
            }

            var success = await _contactService.SetDefaultContactAsync(id);
            if (!success) return BadRequest("Thay đổi trạng thái mặc định thất bại.");

            return Ok("Đã đặt làm liên hệ mặc định thành công.");
        }
    }
}
