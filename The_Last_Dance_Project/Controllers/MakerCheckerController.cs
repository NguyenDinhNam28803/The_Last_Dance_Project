using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using The_Last_Dance_Project.Services;

namespace The_Last_Dance_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MakerCheckerController : ControllerBase
    {
        private readonly IMakerCheckerService _service;

        public MakerCheckerController(IMakerCheckerService service)
        {
            _service = service;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "SYSTEM";

        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] MakerCheckerRequest request)
        {
            var success = await _service.SubmitRequestAsync(request.EntityName, request.EntityId, request.Action, GetUserId(), request.Details);
            return success ? Ok("Yêu cầu đã được tạo.") : BadRequest("Tạo yêu cầu thất bại.");
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var success = await _service.ApproveRequestAsync(id, GetUserId());
            return success ? Ok("Đã duyệt thành công.") : BadRequest("Duyệt thất bại.");
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] string reason)
        {
            var success = await _service.RejectRequestAsync(id, GetUserId(), reason);
            return success ? Ok("Đã từ chối.") : BadRequest("Từ chối thất bại.");
        }
    }

    public class MakerCheckerRequest
    {
        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }
}
