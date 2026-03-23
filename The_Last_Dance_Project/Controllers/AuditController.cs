using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using The_Last_Dance_Project.Data;
using The_Last_Dance_Project.Services;

namespace The_Last_Dance_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLogs()
        {
            var logs = await _auditService.GetAllLogsAsync();
            return Ok(logs);
        }
    }
}
