using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using The_Last_Dance_Project.Dtos;
using The_Last_Dance_Project.Interfaces;
using The_Last_Dance_Project.Models;

namespace The_Last_Dance_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerContactController : ControllerBase
    {
        private readonly ICustomerContactService _contactService;
        private readonly IMakerCheckerService _makerCheckerService;

        public CustomerContactController(ICustomerContactService contactService, IMakerCheckerService makerCheckerService)
        {
            _contactService = contactService;
            _makerCheckerService = makerCheckerService;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "SYSTEM";

        [HttpGet]
        [Authorize(Roles = "Administrator")]
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

            if (currentUserRole != "Administrator" && currentUserId != contact.CustId)
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

            if (currentUserRole != "Administrator" && currentUserId != custId)
            {
                return Forbid("Bạn không có quyền xem thông tin liên hệ của người khác.");
            }

            var contacts = await _contactService.GetByCustomerIdAsync(custId);
            return Ok(contacts);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerContactCreateDto dto)
        {
            var currentUserId = GetUserId();
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "Administrator" && currentUserId != dto.CustId)
            {
                return Forbid("Bạn không có quyền thêm liên hệ cho người khác.");
            }

            // GỬI YÊU CẦU DUYỆT THAY VÌ TẠO TRỰC TIẾP
            var details = System.Text.Json.JsonSerializer.Serialize(dto);
            var success = await _makerCheckerService.SubmitRequestAsync("CustomerContact", dto.ContactId, "CREATE", currentUserId, details);
            
            return success ? Ok("Yêu cầu tạo mới đã được gửi và chờ duyệt.") : BadRequest("Gửi yêu cầu thất bại.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CustomerContactUpdateDto dto)
        {
            var existing = await _contactService.GetByIdAsync(id);
            if (existing == null) return NotFound("Không tìm thấy liên hệ để cập nhật.");

            var currentUserId = GetUserId();
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "Administrator" && currentUserId != existing.CustId)
            {
                return Forbid("Bạn không có quyền cập nhật liên hệ của người khác.");
            }

            // GỬI YÊU CẦU DUYỆT THAY VÌ CẬP NHẬT TRỰC TIẾP
            var updateObj = new CustomerContact
            {
                ContactId = id,
                CustId = existing.CustId,
                CountryId = dto.CountryId,
                AddType = dto.AddType,
                InfoType = dto.InfoType,
                Contact = dto.Contact,
                FaxAttention = dto.FaxAttention,
                IsDefault = dto.IsDefault,
                Description = dto.Description
            };
            var details = System.Text.Json.JsonSerializer.Serialize(updateObj);
            var success = await _makerCheckerService.SubmitRequestAsync("CustomerContact", id, "UPDATE", currentUserId, details);

            return success ? Ok("Yêu cầu cập nhật đã được gửi và chờ duyệt.") : BadRequest("Gửi yêu cầu thất bại.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _contactService.GetByIdAsync(id);
            if (existing == null) return NotFound("Không tìm thấy liên hệ để xóa.");

            var currentUserId = GetUserId();
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "Administrator" && currentUserId != existing.CustId)
            {
                return Forbid("Bạn không có quyền xóa liên hệ của người khác.");
            }

            // GỬI YÊU CẦU DUYỆT THAY VÌ XÓA TRỰC TIẾP
            var success = await _makerCheckerService.SubmitRequestAsync("CustomerContact", id, "DELETE", currentUserId, id);

            return success ? Ok("Yêu cầu xóa đã được gửi và chờ duyệt.") : BadRequest("Gửi yêu cầu thất bại.");
        }

        [HttpPatch("{id}/set-default")]
        public async Task<IActionResult> SetDefault(string id)
        {
            var existing = await _contactService.GetByIdAsync(id);
            if (existing == null) return NotFound("Không tìm thấy liên hệ.");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "Administrator" && currentUserId != existing.CustId)
            {
                return Forbid("Bạn không có quyền thay đổi thông tin của người khác.");
            }

            var success = await _contactService.SetDefaultAsync(id);
            if (!success) return BadRequest("Thay đổi trạng thái mặc định thất bại.");

            return Ok("Đã đặt làm liên hệ mặc định thành công.");
        }
    }
}
