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
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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

        [HttpGet("forms/department")]
        [SwaggerOperation(Summary = "Lấy danh sách đơn đăng ký theo phòng ban", Description = "API trả về danh sách tất cả đơn đăng ký thuộc phòng ban của cán bộ đang đăng nhập")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ApiResponse<IEnumerable<RegistrationForm>>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy thông tin cán bộ", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<RegistrationForm>>>> GetFormsByDepartment()
        {
            try
            {
                // Lấy mã cán bộ từ token
                var maCB = User.FindFirst("MSSV")?.Value;
                if (string.IsNullOrEmpty(maCB))
                {
                    return ApiResponse<IEnumerable<RegistrationForm>>(null, "Không xác định được cán bộ", 401, false);
                }

                // Lấy thông tin phòng ban của cán bộ
                var staff = await _staffService.GetProfileAsync(maCB);
                if (staff == null)
                {
                    return ApiResponse<IEnumerable<RegistrationForm>>(null, "Không tìm thấy thông tin cán bộ", 404, false);
                }

                // Lấy danh sách đơn đăng ký theo phòng ban
                var forms = await _formService.GetFormsByDepartmentAsync(staff.MaPB);
                if (forms == null || !forms.Any())
                {
                    return ApiResponse<IEnumerable<RegistrationForm>>(forms, "Không có đơn đăng ký nào trong phòng ban này", 200, true);
                }

                return ApiResponse(forms, "Lấy danh sách đơn đăng ký theo phòng ban thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<RegistrationForm>>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }

        [HttpGet("forms/department/pending")]
        [SwaggerOperation(Summary = "Lấy danh sách đơn đăng ký chờ xử lý theo phòng ban", Description = "API trả về danh sách các đơn đăng ký đang chờ xử lý thuộc phòng ban của cán bộ đang đăng nhập")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ApiResponse<IEnumerable<RegistrationForm>>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy thông tin cán bộ", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<RegistrationForm>>>> GetPendingFormsByDepartment()
        {
            try
            {
                // Lấy mã cán bộ từ token
                var maCB = User.FindFirst("MSSV")?.Value;
                if (string.IsNullOrEmpty(maCB))
                {
                    return ApiResponse<IEnumerable<RegistrationForm>>(null, "Không xác định được cán bộ", 401, false);
                }

                // Lấy thông tin phòng ban của cán bộ
                var staff = await _staffService.GetProfileAsync(maCB);
                if (staff == null)
                {
                    return ApiResponse<IEnumerable<RegistrationForm>>(null, "Không tìm thấy thông tin cán bộ", 404, false);
                }

                // Lấy danh sách đơn đăng ký chờ xử lý theo phòng ban
                var forms = await _formService.GetPendingFormsByDepartmentAsync(staff.MaPB);
                if (forms == null || !forms.Any())
                {
                    return ApiResponse<IEnumerable<RegistrationForm>>(forms, "Không có đơn đăng ký nào đang chờ xử lý trong phòng ban này", 200, true);
                }

                return ApiResponse(forms, "Lấy danh sách đơn đăng ký chờ xử lý theo phòng ban thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<RegistrationForm>>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }

        [HttpGet("forms/by-department/{maPB}")]
        [SwaggerOperation(Summary = "Lấy danh sách đơn đăng ký theo mã phòng ban", Description = "API trả về danh sách tất cả đơn đăng ký thuộc phòng ban được chỉ định")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ApiResponse<IEnumerable<RegistrationForm>>))]
        [SwaggerResponse(400, "Mã phòng ban không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<RegistrationForm>>>> GetFormsByDepartmentCode(string maPB)
        {
            try
            {
                if (string.IsNullOrEmpty(maPB))
                {
                    return ApiResponse<IEnumerable<RegistrationForm>>(null, "Mã phòng ban không được rỗng", 400, false);
                }

                // Lấy danh sách đơn đăng ký theo mã phòng ban
                var forms = await _formService.GetFormsByDepartmentAsync(maPB);
                if (forms == null || !forms.Any())
                {
                    return ApiResponse<IEnumerable<RegistrationForm>>(forms, $"Không có đơn đăng ký nào trong phòng ban {maPB}", 200, true);
                }

                return ApiResponse(forms, $"Lấy danh sách đơn đăng ký của phòng ban {maPB} thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<RegistrationForm>>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }

        [HttpGet("forms/by-department/{maPB}/pending")]
        [SwaggerOperation(Summary = "Lấy danh sách đơn đăng ký chờ xử lý theo mã phòng ban", Description = "API trả về danh sách các đơn đăng ký đang chờ xử lý thuộc phòng ban được chỉ định")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ApiResponse<IEnumerable<RegistrationForm>>))]
        [SwaggerResponse(400, "Mã phòng ban không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<RegistrationForm>>>> GetPendingFormsByDepartmentCode(string maPB)
        {
            try
            {
                if (string.IsNullOrEmpty(maPB))
                {
                    return ApiResponse<IEnumerable<RegistrationForm>>(null, "Mã phòng ban không được rỗng", 400, false);
                }

                // Lấy danh sách đơn đăng ký chờ xử lý theo mã phòng ban
                var forms = await _formService.GetPendingFormsByDepartmentAsync(maPB);
                if (forms == null || !forms.Any())
                {
                    return ApiResponse<IEnumerable<RegistrationForm>>(forms, $"Không có đơn đăng ký nào đang chờ xử lý trong phòng ban {maPB}", 200, true);
                }

                return ApiResponse(forms, $"Lấy danh sách đơn đăng ký chờ xử lý của phòng ban {maPB} thành công");
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
        [SwaggerOperation(Summary = "Tạo mới biểu mẫu", Description = "API cho phép cán bộ tạo mới một biểu mẫu đăng ký với file đính kèm")]
        [SwaggerResponse(200, "Tạo biểu mẫu thành công", typeof(ApiResponse<string>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<string>>> CreateForm([FromForm] FormCreateRequest request)
        {
            try
            {
                if (request == null)
                    return ApiResponse<string>("", "Biểu mẫu không được rỗng", 400, false);
                    
                if (string.IsNullOrEmpty(request.TenBM))
                    return ApiResponse<string>("", "Tên biểu mẫu không được rỗng", 400, false);

                // Kiểm tra file upload
                if (request.File == null || request.File.Length == 0)
                    return ApiResponse<string>("", "Vui lòng chọn file để upload", 400, false);

                // Kiểm tra định dạng file (chỉ cho phép PDF, DOC, DOCX)
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                    return ApiResponse<string>("", "Chỉ cho phép upload file PDF, DOC, DOCX", 400, false);

                // Kiểm tra kích thước file (tối đa 10MB)
                if (request.File.Length > 10 * 1024 * 1024)
                    return ApiResponse<string>("", "Kích thước file không được vượt quá 10MB", 400, false);

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

                // Tạo thư mục lưu file nếu chưa tồn tại
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "forms");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Tạo tên file unique
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu file vào server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }

                // Tạo đối tượng Form từ request
                var form = new Form
                {
                    MaBM = await _codeGenerator.GenerateMaDonCTAsync(), // Tự động tạo mã biểu mẫu
                    MaCB = maCB,
                    MaPB = staff.MaPB,
                    TenBM = request.TenBM,
                    LienKet = $"/uploads/forms/{fileName}", // Lưu đường dẫn file
                    ThoiGianDang = DateTime.Now // Lấy thời gian hiện tại
                };

                var success = await _formService2.CreateFormAsync(form);

                if (success)
                    return ApiResponse("", "Tạo biểu mẫu thành công");

                // Nếu tạo thất bại, xóa file đã upload
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

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
        [SwaggerResponse(200, "Tạo quy định thành công", typeof(ApiResponse<Regulation>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<Regulation>>> CreateRegulation([FromBody] Regulation regulation)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse<Regulation>(null, ModelState.ToString(), 400, false);
                    
                if (regulation == null)
                    return ApiResponse<Regulation>(null, "Quy định không được rỗng", 400, false);
                    
                if (string.IsNullOrEmpty(regulation.TenQD))
                    return ApiResponse<Regulation>(null, "Tên quy định không được rỗng", 400, false);

                // Tự động tạo mã quy định nếu chưa có
                if (string.IsNullOrEmpty(regulation.MaQD))
                {
                    regulation.MaQD = await _codeGenerator.GenerateMaQDAsync();
                }

                // Lấy thông tin cán bộ để gán MaCB và MaPB nếu chưa có
                var staffId = User.FindFirst("StaffId")?.Value;
                if (!string.IsNullOrEmpty(staffId))
                {
                    var staff = await _staffService.GetProfileAsync(staffId);
                    if (staff != null)
                    {
                        if (string.IsNullOrEmpty(regulation.MaCB))
                            regulation.MaCB = staff.MSCB;
                        if (string.IsNullOrEmpty(regulation.MaPB))
                            regulation.MaPB = staff.MaPB;
                    }
                }

                var result = await _regulationService.CreateRegulationAsync(regulation);
                if (!result)
                    return ApiResponse<Regulation>(null, "Tạo quy định thất bại.", 500, false);

                // Lấy lại quy định vừa tạo với thông tin đầy đủ (bao gồm TenPB)
                var createdRegulation = await _regulationService.GetRegulationById(regulation.MaQD);
                
                return ApiResponse(createdRegulation, "Tạo quy định thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Regulation>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpPut("{maQD}")]
        [SwaggerOperation(Summary = "Cập nhật quy định", Description = "API cho phép cán bộ cập nhật thông tin một quy định")]
        [SwaggerResponse(200, "Cập nhật quy định thành công", typeof(ApiResponse<Regulation>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy quy định", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<Regulation>>> UpdateRegulation(string maQD, [FromBody] Regulation regulation)
        {
            if (string.IsNullOrEmpty(maQD))
            {
                return ApiResponse<Regulation>(null, "Mã quy định không hợp lệ", 400, false);
            }
            
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse<Regulation>(null, ModelState.ToString(), 400, false);
                    
                if (regulation == null)
                    return ApiResponse<Regulation>(null, "Quy định không được rỗng", 400, false);
                    
                if (string.IsNullOrEmpty(regulation.TenQD))
                    return ApiResponse<Regulation>(null, "Tên quy định không được rỗng", 400, false);

                var result = await _regulationService.UpdateRegulationAsync(maQD, regulation);
                if (!result)
                    return ApiResponse<Regulation>(null, "Không tìm thấy hoặc cập nhật thất bại.", 404, false);

                // Lấy lại quy định vừa cập nhật với thông tin đầy đủ (bao gồm TenPB)
                var updatedRegulation = await _regulationService.GetRegulationById(maQD);

                return ApiResponse(updatedRegulation, "Cập nhật quy định thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Regulation>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
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

                // Xóa file vật lý nếu tồn tại
                if (!string.IsNullOrEmpty(form.LienKet) && form.LienKet.StartsWith("/uploads/"))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", form.LienKet.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
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

        [HttpDelete("templates/multiple")]
        [SwaggerOperation(Summary = "Xóa nhiều biểu mẫu", Description = "API cho phép cán bộ xóa nhiều biểu mẫu cùng lúc")]
        [SwaggerResponse(200, "Xóa biểu mẫu thành công", typeof(ApiResponse<string>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy biểu mẫu", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<string>>> DeleteMultipleForms([FromBody] DeleteMultipleFormsRequest request)
        {
            try
            {
                if (request == null || request.MaBMList == null || !request.MaBMList.Any())
                {
                    return ApiResponse<string>("", "Danh sách mã biểu mẫu không được rỗng", 400, false);
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

                // Lấy tất cả biểu mẫu và lọc ra những biểu mẫu thuộc phòng ban của cán bộ
                var allForms = await _formService2.GetAllForms();
                var departmentForms = allForms.Where(f => f.MaPB == staff.MaPB).ToList();
                
                // Kiểm tra quyền xóa và lọc ra những biểu mẫu hợp lệ
                var validFormIds = request.MaBMList
                    .Where(maBM => departmentForms.Any(f => f.MaBM == maBM))
                    .ToList();

                if (!validFormIds.Any())
                {
                    return ApiResponse<string>("", "Không tìm thấy biểu mẫu hợp lệ hoặc bạn không có quyền xóa các biểu mẫu này", 404, false);
                }

                // Xóa các biểu mẫu hợp lệ
                var success = await _formService2.DeleteMultipleFormsAsync(validFormIds);
                if (!success)
                {
                    return ApiResponse<string>("", "Xóa biểu mẫu thất bại", 500, false);
                }

                var deletedCount = validFormIds.Count;
                var totalRequested = request.MaBMList.Count();
                var message = deletedCount == totalRequested 
                    ? $"Xóa thành công {deletedCount} biểu mẫu"
                    : $"Xóa thành công {deletedCount}/{totalRequested} biểu mẫu (một số biểu mẫu không tồn tại hoặc không có quyền xóa)";

                return ApiResponse("", message);
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

        [HttpGet("templates/{maBM}/download")]
        [SwaggerOperation(Summary = "Tải xuống file biểu mẫu", Description = "API cho phép tải xuống file biểu mẫu")]
        [SwaggerResponse(200, "Tải xuống thành công")]
        [SwaggerResponse(404, "Không tìm thấy file", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<IActionResult> DownloadFormFile(string maBM)
        {
            try
            {
                if (string.IsNullOrEmpty(maBM))
                {
                    return BadRequest("Mã biểu mẫu không hợp lệ");
                }

                // Tìm biểu mẫu
                var forms = await _formService2.GetAllForms();
                var form = forms.FirstOrDefault(f => f.MaBM == maBM);
                if (form == null)
                {
                    return NotFound("Không tìm thấy biểu mẫu");
                }

                // Kiểm tra file tồn tại
                if (string.IsNullOrEmpty(form.LienKet) || !form.LienKet.StartsWith("/uploads/"))
                {
                    return NotFound("Không tìm thấy file");
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", form.LienKet.TrimStart('/'));
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File không tồn tại trên server");
                }

                // Lấy content type dựa trên extension
                var extension = Path.GetExtension(filePath).ToLowerInvariant();
                var contentType = extension switch
                {
                    ".pdf" => "application/pdf",
                    ".doc" => "application/msword",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    _ => "application/octet-stream"
                };

                // Tạo tên file download
                var fileName = $"{form.TenBM}{extension}";

                // Đọc file và trả về
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }
    }

    public class FormRequest
    {
        public string TenBM { get; set; }
        public string LienKet { get; set; }
    }

    public class FormCreateRequest
    {
        public string TenBM { get; set; }
        public IFormFile File { get; set; }
    }
}
