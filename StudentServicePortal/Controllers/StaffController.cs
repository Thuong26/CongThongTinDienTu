using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentServicePortal.Models;
using StudentServicePortal.Services;
using StudentServicePortal.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

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
        private readonly ICodeGeneratorService _codeGenerator;

        public StaffController(
            IStaffService staffService, 
            IRegistrationFormService formService, 
            IRegistrationDetailService registrationDetailService, 
            IRegulationService regulationService, 
            IFormService formService2,
            ICodeGeneratorService codeGenerator)
        {
            _staffService = staffService;
            _formService = formService;
            _registrationDetailService = registrationDetailService;
            _regulationService = regulationService;
            _formService2 = formService2;
            _codeGenerator = codeGenerator;
        }

        [HttpGet("profile")]
        [SwaggerOperation(Summary = "Lấy thông tin cán bộ", Description = "API trả về thông tin chi tiết của cán bộ đang đăng nhập")]
        [SwaggerResponse(200, "Lấy thông tin thành công", typeof(ApiResponse<StaffDTO>))]
        [SwaggerResponse(404, "Không tìm thấy thông tin cán bộ", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
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
        [SwaggerOperation(Summary = "Lấy danh sách đơn đăng ký chờ xử lý", Description = "API trả về danh sách các đơn đăng ký đang chờ cán bộ xử lý")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ApiResponse<IEnumerable<RegistrationForm>>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
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
        [SwaggerOperation(Summary = "Lấy chi tiết đơn đăng ký theo mã đơn", Description = "API trả về chi tiết các mục trong đơn đăng ký dựa trên mã đơn cung cấp")]
        [SwaggerResponse(200, "Lấy chi tiết thành công", typeof(ApiResponse<IEnumerable<RegistrationDetail>>))]
        [SwaggerResponse(400, "Mã đơn không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy chi tiết đơn", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<RegistrationDetail>>>> GetRegistrationDetails(string maDon)
        {
            if (string.IsNullOrEmpty(maDon))
            {
                return ApiResponse<IEnumerable<RegistrationDetail>>(null, "Mã đơn không hợp lệ", 400, false);
            }
            
            try
            {
                var allDetails = await _registrationDetailService.GetAllDetailsAsync();
                var details = allDetails.Where(d => d.MaDon == maDon);
                
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
        [SwaggerOperation(Summary = "Cập nhật trạng thái đơn đăng ký", Description = "API cho phép cán bộ cập nhật trạng thái xử lý của đơn đăng ký")]
        [SwaggerResponse(200, "Cập nhật trạng thái thành công", typeof(ApiResponse<string>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy đơn đăng ký", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
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
                var allDetails = await _registrationDetailService.GetAllDetailsAsync();
                var detail = allDetails.FirstOrDefault(d => d.MaDon == maDon);

                if (detail == null)
                    return ApiResponse<string>("", $"Không tìm thấy đơn có mã {maDon}", 404, false);

                detail.TrangThaiXuLy = request.TrangThaiXuLy;
                await _registrationDetailService.UpdateDetailAsync(detail);

                return ApiResponse("", "Cập nhật trạng thái thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>("", $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }

        [HttpGet("templates")]
        [SwaggerOperation(Summary = "Lấy danh sách biểu mẫu", Description = "API trả về danh sách biểu mẫu của phòng ban mà cán bộ đang làm việc")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ApiResponse<IEnumerable<Form>>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<Form>>>> GetAllFormTemplates()
        {
            try
            {
                // Lấy mã cán bộ từ token
                var maCB = User.FindFirst("MSSV")?.Value;
                if (string.IsNullOrEmpty(maCB))
                {
                    return ApiResponse<IEnumerable<Form>>(null, "Không xác định được cán bộ", 401, false);
                }

                // Lấy thông tin phòng ban của cán bộ
                var staff = await _staffService.GetProfileAsync(maCB);
                if (staff == null)
                {
                    return ApiResponse<IEnumerable<Form>>(null, "Không tìm thấy thông tin cán bộ", 404, false);
                }

                // Lấy danh sách biểu mẫu của phòng ban
                var forms = await _formService2.GetAllForms();
                var departmentForms = forms.Where(f => f.MaPB == staff.MaPB);

                return ApiResponse(departmentForms, "Lấy danh sách biểu mẫu thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<Form>>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }

        [HttpPost("templates")]
        [SwaggerOperation(Summary = "Tạo mới biểu mẫu", Description = "API cho phép cán bộ tạo mới một biểu mẫu đăng ký")]
        [SwaggerResponse(200, "Tạo biểu mẫu thành công", typeof(ApiResponse<string>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<string>>> CreateForm([FromBody] FormRequest request)
        {
            try
            {
                if (request == null)
                    return ApiResponse<string>("", "Biểu mẫu không được rỗng", 400, false);
                    
                if (string.IsNullOrEmpty(request.TenBM))
                    return ApiResponse<string>("", "Tên biểu mẫu không được rỗng", 400, false);

                // Lấy mã cán bộ từ token
                var maCB = User.FindFirst("MSSV")?.Value;
                if (string.IsNullOrEmpty(maCB))
                {
                    return ApiResponse<string>("", "Không xác định được cán bộ", 401, false);
                }

                // Lấy thông tin phòng ban của cán bộ
                var staff = await _staffService.GetProfileAsync(maCB);
                if (staff == null)
                {
                    return ApiResponse<string>("", "Không tìm thấy thông tin cán bộ", 404, false);
                }

                // Tạo đối tượng Form từ request
                var form = new Form
                {
                    MaBM = await _codeGenerator.GenerateMaDonCTAsync(), // Tự động tạo mã biểu mẫu
                    MaCB = maCB,
                    MaPB = staff.MaPB,
                    TenBM = request.TenBM,
                    LienKet = request.LienKet,
                    ThoiGianDang = DateTime.Now // Lấy thời gian hiện tại
                };

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
        [SwaggerOperation(Summary = "Cập nhật biểu mẫu", Description = "API cho phép cán bộ cập nhật thông tin một biểu mẫu đăng ký")]
        [SwaggerResponse(200, "Cập nhật biểu mẫu thành công", typeof(ApiResponse<string>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy biểu mẫu", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
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
        [SwaggerOperation(Summary = "Tạo mới quy định", Description = "API cho phép cán bộ tạo mới một quy định")]
        [SwaggerResponse(200, "Tạo quy định thành công", typeof(ApiResponse<string>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
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
        [SwaggerOperation(Summary = "Cập nhật quy định", Description = "API cho phép cán bộ cập nhật thông tin một quy định")]
        [SwaggerResponse(200, "Cập nhật quy định thành công", typeof(ApiResponse<string>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy quy định", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
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

        [HttpDelete("templates/{maBM}")]
        [SwaggerOperation(Summary = "Xóa biểu mẫu", Description = "API cho phép cán bộ xóa một biểu mẫu của phòng ban")]
        [SwaggerResponse(200, "Xóa biểu mẫu thành công", typeof(ApiResponse<string>))]
        [SwaggerResponse(400, "Mã biểu mẫu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy biểu mẫu", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<string>>> DeleteForm(string maBM)
        {
            try
            {
                if (string.IsNullOrEmpty(maBM))
                {
                    return ApiResponse<string>("", "Mã biểu mẫu không hợp lệ", 400, false);
                }

                // Lấy mã cán bộ từ token
                var maCB = User.FindFirst("MSSV")?.Value;
                if (string.IsNullOrEmpty(maCB))
                {
                    return ApiResponse<string>("", "Không xác định được cán bộ", 401, false);
                }

                // Lấy thông tin phòng ban của cán bộ
                var staff = await _staffService.GetProfileAsync(maCB);
                if (staff == null)
                {
                    return ApiResponse<string>("", "Không tìm thấy thông tin cán bộ", 404, false);
                }

                // Kiểm tra xem biểu mẫu có thuộc phòng ban của cán bộ không
                var forms = await _formService2.GetAllForms();
                var form = forms.FirstOrDefault(f => f.MaBM == maBM && f.MaPB == staff.MaPB);
                if (form == null)
                {
                    return ApiResponse<string>("", "Không tìm thấy biểu mẫu hoặc bạn không có quyền xóa biểu mẫu này", 404, false);
                }

                // Xóa biểu mẫu
                var success = await _formService2.DeleteFormAsync(maBM);
                if (!success)
                {
                    return ApiResponse<string>("", "Xóa biểu mẫu thất bại", 500, false);
                }

                return ApiResponse("", "Xóa biểu mẫu thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>("", $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }

        [HttpGet("regulations/department")]
        [SwaggerOperation(Summary = "Lấy danh sách quy định theo phòng ban", Description = "API trả về danh sách các quy định của phòng ban")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ApiResponse<IEnumerable<Regulation>>))]
        [SwaggerResponse(400, "Mã phòng ban không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<Regulation>>>> GetRegulationsByDepartment()
        {
            try
            {
                // Lấy mã phòng ban từ token
                var staffId = User.FindFirst("StaffId")?.Value;
                if (string.IsNullOrEmpty(staffId))
                    return ApiResponse<IEnumerable<Regulation>>(null, "Không tìm thấy thông tin cán bộ", 400, false);

                var staff = await _staffService.GetProfileAsync(staffId);
                if (staff == null)
                    return ApiResponse<IEnumerable<Regulation>>(null, "Không tìm thấy thông tin cán bộ", 400, false);

                var regulations = await _regulationService.GetRegulationsByDepartment(staff.MaPB);
                return ApiResponse(regulations, "Lấy danh sách quy định thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<Regulation>>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
    }

    public class FormRequest
    {
        public string TenBM { get; set; }
        public string LienKet { get; set; }
    }
}
