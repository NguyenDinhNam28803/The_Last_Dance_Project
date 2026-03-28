using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using The_Last_Dance_Project.Interfaces;

namespace The_Last_Dance_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MakerCheckerController : ControllerBase
    {
        private readonly IMakerCheckerService _service;

        public MakerCheckerController(IMakerCheckerService service)
        {
            _service = service;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "SYSTEM";

        [HttpGet("pending")]
        [Authorize(Roles = "Checker,Administrator")]
        public async Task<IActionResult> GetPending()
        {
            var requests = await _service.GetPendingRequestsAsync();
            return Ok(requests);
        }

        [HttpPost("submit")]
        [Authorize(Roles = "Maker,Administrator")]
        public async Task<IActionResult> Submit([FromBody] MakerCheckerRequest request)
        {
            var success = await _service.SubmitRequestAsync(request.EntityName, request.EntityId, request.Action, GetUserId(), request.Details);
            return success ? Ok("Yêu cầu đã được tạo.") : BadRequest("Tạo yêu cầu thất bại.");
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Checker,Administrator")]
        public async Task<IActionResult> Approve(int id)
        {
            var success = await _service.ApproveRequestAsync(id, GetUserId());
            return success ? Ok("Đã duyệt thành công.") : BadRequest("Duyệt thất bại.");
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Checker,Administrator")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectRequest request)
        {
            var success = await _service.RejectRequestAsync(id, GetUserId(), request.Reason);
            return success ? Ok("Đã từ chối.") : BadRequest("Từ chối thất bại.");
        }

        [HttpPost("{id}/cancel")]
        [Authorize(Roles = "Maker,Administrator")]
        public async Task<IActionResult> Cancel(int id)
        {
            var success = await _service.CancelRequestAsync(id, GetUserId());
            return success ? Ok("Đã hủy yêu cầu.") : BadRequest("Hủy yêu cầu thất bại.");
        }
    }

    public class MakerCheckerRequest
    {
        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }

    public class RejectRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}
