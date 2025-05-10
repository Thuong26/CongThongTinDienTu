using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentServicePortal.Models;
using StudentServicePortal.Services;
using StudentServicePortal.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Controllers.Admin
{
    [Route("api/admin")]
    [Authorize(Roles = "Manager")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly IStaffService _staffService;
        private readonly IReportService _reportService;
        public AdminController(IDepartmentService departmentService, IStaffService staffService, IReportService reportService)
        {
            _departmentService = departmentService;
            _staffService = staffService;
            _reportService = reportService;
        }
        //[Authorize]
        //[HttpGet("debug/claims")]
        //public IActionResult GetClaims()
        //{
        //    var claims = User.Claims
        //        .Select(c => new { c.Type, c.Value })
        //        .ToList();
        //    return Ok(claims);
        //}

        [HttpGet("departments")]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return Ok(departments);
        }
        [HttpGet("staff")]
        public async Task<ActionResult<IEnumerable<Staff>>> GetStaff()
        {
            var staffList = await _staffService.GetAllStaffAsync();
            return Ok(staffList);
        }
        [HttpPost("staff")]
        public async Task<IActionResult> CreateStaff([FromBody] Staff staff)
        {
            if (staff == null || string.IsNullOrEmpty(staff.MSCB) || staff.Matkhau == null || staff.Matkhau.Length == 0)
                return BadRequest("Thông tin cán bộ không hợp lệ.");

            var result = await _staffService.CreateStaffAsync(staff);
            if (result)
                return Ok(new { message = "Thêm cán bộ thành công." });
            else
                return StatusCode(500, "Thêm cán bộ thất bại.");
        }
        [HttpPut("staff/{msCB}")]
        public async Task<IActionResult> UpdateStaff(string msCB, [FromBody] Staff staff)
        {
            if (string.IsNullOrEmpty(msCB) || staff == null || staff.Matkhau == null || staff.Matkhau.Length == 0)
                return BadRequest("Thông tin cập nhật không hợp lệ.");

            var result = await _staffService.UpdateStaffAsync(msCB, staff);
            if (result)
                return Ok(new { message = "Cập nhật cán bộ thành công." });
            else
                return NotFound(new { message = "Không tìm thấy cán bộ để cập nhật." });
        }
        [HttpGet("reports")]
        public async Task<IActionResult> GetReports()
        {
            var reports = await _reportService.GetReportsAsync();
            return Ok(reports);
        }
        [HttpPost("reports/export")]
        public async Task<IActionResult> ExportReports()
        {
            var reports = await _reportService.GetReportsAsync(); // Lấy danh sách báo cáo (ví dụ từ database)
            var fileBytes = await _reportService.ExportReportsToExcelAsync((List<ReportDTO>)reports);

            var fileName = $"BaoCao_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }


    }
}
