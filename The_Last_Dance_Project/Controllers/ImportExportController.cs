using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using The_Last_Dance_Project.Interfaces;

namespace The_Last_Dance_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bắt buộc đăng nhập
    public class ImportExportController : ControllerBase
    {
        private readonly IImportExportService _service;

        public ImportExportController(IImportExportService service)
        {
            _service = service;
        }

        [HttpGet("template")]
        public IActionResult GetTemplate()
        {
            var file = _service.GenerateTemplate();
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Template_Import_Client.xlsx");
        }

        [HttpGet("export-customers")]
        public async Task<IActionResult> ExportCustomers()
        {
            var file = await _service.ExportCustomersAsync();
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Customers_Export_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }

        [HttpPost("import-customers")]
        // Chỉ Maker (hoặc Admin) mới được phép Import
        [Authorize(Roles = "ADMIN,MAKER")]
        public async Task<IActionResult> ImportCustomers(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Không có file nào được tải lên.");

            var ext = Path.GetExtension(file.FileName).ToLower();
            if (ext != ".xlsx" && ext != ".xls")
                return BadRequest("Chỉ hỗ trợ file định dạng Excel (.xlsx, .xls).");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "SYSTEM";

            using var stream = file.OpenReadStream();
            var result = await _service.ImportCustomersAsync(stream, userId);

            return Ok(result);
        }
    }
}
