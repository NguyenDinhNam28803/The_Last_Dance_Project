using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using The_Last_Dance_Project.Data;
using The_Last_Dance_Project.Models;
using The_Last_Dance_Project.Services;

namespace The_Last_Dance_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemCodeController : ControllerBase
    {
        private readonly ISystemCodeService _service;

        public SystemCodeController(ISystemCodeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetSystemCodes()
        {
            return Ok(await _service.GetSystemCodesAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateSystemCode([FromBody] SystemCode systemCode)
        {
            await _service.AddSystemCodeAsync(systemCode);
            return Ok(systemCode);
        }
    }
}
