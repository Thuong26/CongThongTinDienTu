using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentServicePortal.Models;
using StudentServicePortal.Services;
using StudentServicePortal.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentServicePortal.Controllers
{
    [Authorize(Roles = "Manager,Staff")]
    [Route("api/staff")]
    public class StaffController : BaseController
    {
        private readonly IStaffService _staffService;
        private readonly IRegistrationFormService _formService;
        private readonly IRegistrationDetailService _registrationDetailService;
        private readonly IFormService _formService2;
        private readonly IRegulationService _regulationService;
        public StaffController(IStaffService staffService, IRegistrationFormService formService, IRegistrationDetailService registrationDetailService, IRegulationService regulationService, IFormService formService2)
        {
            _staffService = staffService;
            _formService = formService;
            _registrationDetailService = registrationDetailService;
            _regulationService = regulationService;
            _formService2 = formService2;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse<StaffDTO>>> GetProfile()
        {
            try
            {
                string maCB = "CB0001"; // Hardcoded để test

                var staff = await _staffService.GetProfileAsync(maCB);

                if (staff == null)
                {
                    return ApiResponse<StaffDTO>(null, "Không tìm thấy thông tin cán bộ", 404, false);
                }

                return ApiResponse(staff);
            }
            catch (Exception ex)
            {
                return ApiResponse<StaffDTO>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpGet("forms")]
        public async Task<ActionResult<ApiResponse<IEnumerable<RegistrationForm>>>> GetPendingForms()
        {
            try
            {
                var forms = await _formService.GetPendingFormsAsync();
                if (forms == null || !forms.Any())
                {
                    return ApiResponse<IEnumerable<RegistrationForm>>(forms, "Không có đơn đăng ký nào đang chờ xử lý", 200, true);
                }
                return ApiResponse(forms);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<RegistrationForm>>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpGet("forms/{maDon}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<RegistrationDetail>>>> GetRegistrationDetails(string maDon)
        {
            if (string.IsNullOrEmpty(maDon))
            {
                return ApiResponse<IEnumerable<RegistrationDetail>>(null, "Mã đơn không hợp lệ", 400, false);
            }
            
            try
            {
                var details = await _registrationDetailService.GetDetailsByFormIdAsync(maDon);
                if (!details.Any())
                {
                    return ApiResponse<IEnumerable<RegistrationDetail>>(null, "Không tìm thấy chi tiết đơn", 404, false);
                }
                return ApiResponse(details);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<RegistrationDetail>>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpPut("forms/{maDon}/status")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateFormStatus(string maDon, [FromBody] UpdateFormStatusRequest request)
        {
            if (string.IsNullOrEmpty(maDon))
            {
                return ApiResponse<string>("", "Mã đơn không hợp lệ", 400, false);
            }
            
            if (request == null || string.IsNullOrEmpty(request.TrangThaiXuLy))
            {
                return ApiResponse<string>("", "Trạng thái xử lý không hợp lệ", 400, false);
            }
            
            try
            {
                var success = await _registrationDetailService.UpdateStatusByMaDonAsync(maDon, request.TrangThaiXuLy);

                if (!success)
                    return ApiResponse<string>("", $"Không tìm thấy đơn có mã {maDon}", 404, false);

                return ApiResponse("", "Cập nhật trạng thái thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>("", $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }

        [HttpPost("templates")]
        public async Task<ActionResult<ApiResponse<string>>> CreateForm([FromBody] Form form)
        {
            try
            {
                if (form == null)
                    return ApiResponse<string>("", "Biểu mẫu không được rỗng", 400, false);
                    
                if (string.IsNullOrEmpty(form.MaBM))
                    return ApiResponse<string>("", "Mã biểu mẫu không được rỗng", 400, false);
                    
                if (string.IsNullOrEmpty(form.TenBM))
                    return ApiResponse<string>("", "Tên biểu mẫu không được rỗng", 400, false);

                var success = await _formService2.CreateFormAsync(form);

                if (success)
                    return ApiResponse("", "Tạo biểu mẫu thành công");

                return ApiResponse<string>("", "Tạo biểu mẫu thất bại", 500, false);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>("", $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpPut("templates/{maBM}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateForm(string maBM, [FromBody] Form form)
        {
            if (string.IsNullOrEmpty(maBM))
            {
                return ApiResponse<string>("", "Mã biểu mẫu không hợp lệ", 400, false);
            }
            
            try
            {
                if (form == null)
                    return ApiResponse<string>("", "Biểu mẫu không được rỗng", 400, false);
                    
                if (string.IsNullOrEmpty(form.TenBM))
                    return ApiResponse<string>("", "Tên biểu mẫu không được rỗng", 400, false);
                    
                var success = await _formService2.UpdateFormAsync(maBM, form);
                if (!success)
                    return ApiResponse<string>("", "Biểu mẫu không tồn tại hoặc cập nhật thất bại.", 404, false);

                return ApiResponse("", "Cập nhật biểu mẫu thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>("", $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateRegulation([FromBody] Regulation regulation)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse<string>("", ModelState.ToString(), 400, false);
                    
                if (regulation == null)
                    return ApiResponse<string>("", "Quy định không được rỗng", 400, false);
                    
                if (string.IsNullOrEmpty(regulation.MaQD))
                    return ApiResponse<string>("", "Mã quy định không được rỗng", 400, false);
                    
                if (string.IsNullOrEmpty(regulation.TenQD))
                    return ApiResponse<string>("", "Tên quy định không được rỗng", 400, false);

                var result = await _regulationService.CreateRegulationAsync(regulation);
                if (!result)
                    return ApiResponse<string>("", "Tạo quy định thất bại.", 500, false);

                return ApiResponse("", "Tạo quy định thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>("", $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpPut("{maQD}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateRegulation(string maQD, [FromBody] Regulation regulation)
        {
            if (string.IsNullOrEmpty(maQD))
            {
                return ApiResponse<string>("", "Mã quy định không hợp lệ", 400, false);
            }
            
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse<string>("", ModelState.ToString(), 400, false);
                    
                if (regulation == null)
                    return ApiResponse<string>("", "Quy định không được rỗng", 400, false);
                    
                if (string.IsNullOrEmpty(regulation.TenQD))
                    return ApiResponse<string>("", "Tên quy định không được rỗng", 400, false);

                var result = await _regulationService.UpdateRegulationAsync(maQD, regulation);
                if (!result)
                    return ApiResponse<string>("", "Không tìm thấy hoặc cập nhật thất bại.", 404, false);

                return ApiResponse("", "Cập nhật quy định thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>("", $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
    }
}
