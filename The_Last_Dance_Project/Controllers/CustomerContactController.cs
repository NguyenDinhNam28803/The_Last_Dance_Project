using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using The_Last_Dance_Project.Dtos;
using The_Last_Dance_Project.Interfaces;

namespace The_Last_Dance_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerContactController : ControllerBase
    {
        private readonly ICustomerContactService _contactService;

        public CustomerContactController(ICustomerContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAll()
        {
            var contacts = await _contactService.GetAllAsync();
            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var contact = await _contactService.GetByIdAsync(id);
            if (contact == null) return NotFound("Không tìm thấy thông tin liên hệ.");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "ADMIN" && currentUserId != contact.CustId)
            {
                return Forbid("Bạn không có quyền xem thông tin liên hệ của người khác.");
            }

            return Ok(contact);
        }

        [HttpGet("customer/{custId}")]
        public async Task<IActionResult> GetByCustomerId(string custId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "ADMIN" && currentUserId != custId)
            {
                return Forbid("Bạn không có quyền xem thông tin liên hệ của người khác.");
            }

            var contacts = await _contactService.GetByCustomerIdAsync(custId);
            return Ok(contacts);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerContactCreateDto dto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "ADMIN" && currentUserId != dto.CustId)
            {
                return Forbid("Bạn không có quyền thêm liên hệ cho người khác.");
            }

            var result = await _contactService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.ContactId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CustomerContactUpdateDto dto)
        {
            var existing = await _contactService.GetByIdAsync(id);
            if (existing == null) return NotFound("Không tìm thấy liên hệ để cập nhật.");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "ADMIN" && currentUserId != existing.CustId)
            {
                return Forbid("Bạn không có quyền cập nhật liên hệ của người khác.");
            }

            var result = await _contactService.UpdateAsync(id, dto);
            if (result == null) return BadRequest("Cập nhật liên hệ thất bại.");

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _contactService.GetByIdAsync(id);
            if (existing == null) return NotFound("Không tìm thấy liên hệ để xóa.");

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

        [HttpPatch("{id}/set-default")]
        public async Task<IActionResult> SetDefault(string id)
        {
            var existing = await _contactService.GetByIdAsync(id);
            if (existing == null) return NotFound("Không tìm thấy liên hệ.");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "ADMIN" && currentUserId != existing.CustId)
            {
                return Forbid("Bạn không có quyền thay đổi thông tin của người khác.");
            }

            var success = await _contactService.SetDefaultAsync(id);
            if (!success) return BadRequest("Thay đổi trạng thái mặc định thất bại.");

            return Ok("Đã đặt làm liên hệ mặc định thành công.");
        }
    }
}
