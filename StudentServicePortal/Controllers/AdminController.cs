using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentServicePortal.Models;
using StudentServicePortal.Services;
using StudentServicePortal.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Controllers
{
    [Route("api/admin")]
    [Authorize(Roles = "Manager")]
    public class AdminController : BaseController
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
        public async Task<ActionResult<ApiResponse<IEnumerable<Department>>>> GetDepartments()
        {
            try
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
                return ApiResponse(departments);
        }
            catch (Exception)
            {
                return ApiResponse<IEnumerable<Department>>(null, "Lỗi hệ thống", 500, false);
            }
        }
        
        [HttpGet("staff")]
        public async Task<ActionResult<ApiResponse<IEnumerable<StaffDTO>>>> GetStaff()
        {
            try
        {
            var staffList = await _staffService.GetAllStaffAsync();
                return ApiResponse(staffList);
        }
            catch (Exception)
            {
                return ApiResponse<IEnumerable<StaffDTO>>(null, "Lỗi hệ thống", 500, false);
            }
        }
        
        [HttpPost("staff")]
        public async Task<ActionResult<ApiResponse<string>>> CreateStaff([FromBody] Staff staff)
        {
            try
        {
            if (staff == null || string.IsNullOrEmpty(staff.MSCB) || staff.Matkhau == null || staff.Matkhau.Length == 0)
                    return ApiResponse<string>("", "Thông tin cán bộ không hợp lệ.", 400, false);

            var result = await _staffService.CreateStaffAsync(staff);
            if (result)
                    return ApiResponse("", "Thêm cán bộ thành công.");
            else
                    return ApiResponse<string>("", "Thêm cán bộ thất bại.", 500, false);
            }
            catch (Exception)
            {
                return ApiResponse<string>("", "Lỗi hệ thống", 500, false);
        }
        }
        
        [HttpPut("staff/{msCB}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateStaff(string msCB, [FromBody] Staff staff)
        {
            try
        {
            if (string.IsNullOrEmpty(msCB) || staff == null || staff.Matkhau == null || staff.Matkhau.Length == 0)
                    return ApiResponse<string>("", "Thông tin cập nhật không hợp lệ.", 400, false);

            var result = await _staffService.UpdateStaffAsync(msCB, staff);
            if (result)
                    return ApiResponse("", "Cập nhật cán bộ thành công.");
            else
                    return ApiResponse<string>("", "Không tìm thấy cán bộ để cập nhật.", 404, false);
            }
            catch (Exception)
            {
                return ApiResponse<string>("", "Lỗi hệ thống", 500, false);
        }
        }
        
        [HttpGet("reports")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReportDTO>>>> GetReports()
        {
            try
        {
            var reports = await _reportService.GetReportsAsync();
                return ApiResponse(reports);
        }
            catch (Exception)
            {
                return ApiResponse<IEnumerable<ReportDTO>>(null, "Lỗi hệ thống", 500, false);
            }
        }
        
        // Special case for file download - keeping as is
        [HttpPost("reports/export")]
        public async Task<IActionResult> ExportReports()
        {
            try
            {
                var reports = await _reportService.GetReportsAsync();
            var fileBytes = await _reportService.ExportReportsToExcelAsync((List<ReportDTO>)reports);

            var fileName = $"BaoCao_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Lỗi hệ thống",
                    StatusCode = 500
                });
            }
        }
    }
}
