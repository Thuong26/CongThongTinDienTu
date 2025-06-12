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
        [SwaggerOperation(Summary = "Lấy danh sách đơn đăng ký chờ xử lý theo phòng ban", Description = "API trả về danh sách các đơn đăng ký đang chờ xử lý thuộc phòng ban của cán bộ đang đăng nhập")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ApiResponse<IEnumerable<RegistrationForm>>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy thông tin cán bộ", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<RegistrationForm>>>> GetPendingForms()
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
                var pendingForms = await _formService.GetPendingFormsByDepartmentAsync(staff.MaPB);
                if (pendingForms == null || !pendingForms.Any())
                {
                    return ApiResponse<IEnumerable<RegistrationForm>>(Enumerable.Empty<RegistrationForm>(), "Không có đơn đăng ký nào đang chờ xử lý trong phòng ban này", 200, true);
                }
                
                return ApiResponse(pendingForms, "Lấy danh sách đơn đăng ký chờ xử lý theo phòng ban thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<RegistrationForm>>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }

        [HttpGet("forms/department")]
        [SwaggerOperation(Summary = "Lấy danh sách đơn đăng ký chi tiết theo phòng ban", Description = "API trả về danh sách tất cả đơn đăng ký chi tiết thuộc phòng ban của cán bộ đang đăng nhập, kèm thông tin sinh viên")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ApiResponse<IEnumerable<RegistrationDetailWithStudentInfo>>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy thông tin cán bộ", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<RegistrationDetailWithStudentInfo>>>> GetFormsByDepartment()
        {
            try
            {
                // Lấy mã cán bộ từ token
                var maCB = User.FindFirst("MSSV")?.Value;
                if (string.IsNullOrEmpty(maCB))
                {
                    return ApiResponse<IEnumerable<RegistrationDetailWithStudentInfo>>(null, "Không xác định được cán bộ", 401, false);
                }

                // Lấy thông tin phòng ban của cán bộ
                var staff = await _staffService.GetProfileAsync(maCB);
                if (staff == null)
                {
                    return ApiResponse<IEnumerable<RegistrationDetailWithStudentInfo>>(null, "Không tìm thấy thông tin cán bộ", 404, false);
                }

                // Lấy danh sách đơn đăng ký theo phòng ban
                var forms = await _formService.GetFormsByDepartmentAsync(staff.MaPB);
                if (forms == null || !forms.Any())
                {
                    return ApiResponse<IEnumerable<RegistrationDetailWithStudentInfo>>(Enumerable.Empty<RegistrationDetailWithStudentInfo>(), "Không có đơn đăng ký nào trong phòng ban này", 200, true);
                }

                // Lấy chi tiết đơn đăng ký với thông tin sinh viên cho mỗi đơn
                var detailsList = new List<RegistrationDetailWithStudentInfo>();
                foreach (var form in forms)
                {
                    var details = await _registrationDetailService.GetDetailsByFormIdWithStudentInfoAsync(form.MaDon);
                    if (details != null && details.Any())
                    {
                        detailsList.AddRange(details);
                    }
                }

                return ApiResponse<IEnumerable<RegistrationDetailWithStudentInfo>>(detailsList, "Lấy danh sách đơn đăng ký chi tiết theo phòng ban thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<RegistrationDetailWithStudentInfo>>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
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
        
        [HttpPut("forms/{maDonCT}/status")]
        [SwaggerOperation(Summary = "Cập nhật trạng thái đơn đăng ký chi tiết", Description = "API cho phép cán bộ cập nhật trạng thái xử lý của đơn đăng ký chi tiết")]
        [SwaggerResponse(200, "Cập nhật trạng thái thành công", typeof(ApiResponse<UpdateStatusResponse>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy đơn đăng ký chi tiết", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<UpdateStatusResponse>>> UpdateFormStatus(string maDonCT, [FromBody] UpdateFormStatusRequest request)
        {
            if (string.IsNullOrEmpty(maDonCT))
            {
                return ApiResponse<UpdateStatusResponse>(null, "Mã đơn chi tiết không hợp lệ", 400, false);
            }
            
            if (request == null || string.IsNullOrEmpty(request.TrangThaiXuLy))
            {
                return ApiResponse<UpdateStatusResponse>(null, "Trạng thái xử lý không hợp lệ", 400, false);
            }
            
            try
            {
                var allDetails = await _registrationDetailService.GetAllDetailsAsync();
                var detail = allDetails.FirstOrDefault(d => d.MaDonCT == maDonCT);

                if (detail == null)
                    return ApiResponse<UpdateStatusResponse>(null, $"Không tìm thấy đơn chi tiết có mã {maDonCT}", 404, false);

                detail.TrangThaiXuLy = request.TrangThaiXuLy;
                await _registrationDetailService.UpdateDetailAsync(detail);

                // Tạo response object với thông tin chi tiết
                var response = new UpdateStatusResponse
                {
                    MaDonCT = detail.MaDonCT,
                    MaDon = detail.MaDon,
                    TrangThaiXuLy = detail.TrangThaiXuLy,
                    ThoiGianCapNhat = DateTime.Now
                };

                return ApiResponse(response, "Cập nhật trạng thái thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<UpdateStatusResponse>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
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

                if (string.IsNullOrEmpty(request.LienKet))
                    return ApiResponse<string>("", "Liên kết không được rỗng", 400, false);

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
        [SwaggerOperation(Summary = "Cập nhật biểu mẫu", Description = "API cho phép cán bộ cập nhật thông tin một biểu mẫu đăng ký (chỉ các trường được phép)")]
        [SwaggerResponse(200, "Cập nhật biểu mẫu thành công", typeof(ApiResponse<string>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy biểu mẫu", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<string>>> UpdateForm(string maBM, [FromBody] FormUpdateRequest request)
        {
            if (string.IsNullOrEmpty(maBM))
            {
                return ApiResponse<string>("", "Mã biểu mẫu không hợp lệ", 400, false);
            }
            
            try
            {
                if (request == null)
                    return ApiResponse<string>("", "Dữ liệu cập nhật không được rỗng", 400, false);
                    
                if (string.IsNullOrEmpty(request.TenBM))
                    return ApiResponse<string>("", "Tên biểu mẫu không được rỗng", 400, false);

                // Lấy biểu mẫu hiện tại
                var existingForms = await _formService2.GetAllForms();
                var existingForm = existingForms.FirstOrDefault(f => f.MaBM == maBM);
                if (existingForm == null)
                    return ApiResponse<string>("", "Không tìm thấy biểu mẫu", 404, false);

                // Chỉ cập nhật các trường được phép
                existingForm.TenBM = request.TenBM;
                existingForm.LienKet = request.LienKet ?? existingForm.LienKet;
                // MaBM, MaCB, MaPB, ThoiGianDang không được phép cập nhật
                    
                var success = await _formService2.UpdateFormAsync(maBM, existingForm);
                if (!success)
                    return ApiResponse<string>("", "Cập nhật biểu mẫu thất bại", 500, false);

                return ApiResponse("", "Cập nhật biểu mẫu thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>("", $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpPost("regulations")]
        [SwaggerOperation(Summary = "Tạo mới quy định", Description = "API cho phép cán bộ tạo mới một quy định")]
        [SwaggerResponse(200, "Tạo quy định thành công", typeof(ApiResponse<RegulationResponse>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<RegulationResponse>>> CreateRegulation([FromBody] CreateRegulationRequest request)
        {
            try
            {
                if (request == null)
                    return ApiResponse<RegulationResponse>(null, "Dữ liệu quy định không được rỗng", 400, false);
                    
                if (string.IsNullOrWhiteSpace(request.TenQD))
                    return ApiResponse<RegulationResponse>(null, "Tên quy định không được rỗng", 400, false);

                if (string.IsNullOrWhiteSpace(request.LoaiVanBan))
                    return ApiResponse<RegulationResponse>(null, "Loại văn bản không được rỗng", 400, false);

                // Lấy thông tin cán bộ từ token
                var maCB = User.FindFirst("MSSV")?.Value;
                if (string.IsNullOrEmpty(maCB))
                {
                    return ApiResponse<RegulationResponse>(null, "Không xác định được cán bộ", 401, false);
                }

                var staff = await _staffService.GetProfileAsync(maCB);
                if (staff == null)
                {
                    return ApiResponse<RegulationResponse>(null, "Không tìm thấy thông tin cán bộ", 404, false);
                }

                // Xử lý mã quy định
                string maQD;
                if (!string.IsNullOrWhiteSpace(request.MaQD))
                {
                    // Kiểm tra trùng lặp mã quy định
                    var existingRegulation = await _regulationService.GetRegulationById(request.MaQD);
                    if (existingRegulation != null)
                    {
                        return ApiResponse<RegulationResponse>(null, $"Mã quy định '{request.MaQD}' đã tồn tại", 400, false);
                    }
                    maQD = request.MaQD;
                }
                else
                {
                    // Tự động tạo mã nếu không được cung cấp
                    maQD = await _codeGenerator.GenerateMaQDAsync();
                }

                // Tạo đối tượng Regulation từ request
                var regulation = new Regulation
                {
                    MaQD = maQD,
                    TenQD = request.TenQD,
                    MaCB = staff.MSCB, // Lấy từ token đăng nhập
                    MaPB = staff.MaPB, // Lấy từ thông tin cán bộ
                    LienKet = request.LienKet, // Optional
                    LoaiVanBan = request.LoaiVanBan,
                    NoiBanHanh = "", // Để trống vì đã bỏ từ request
                    NgayBanHanh = request.NgayBanHanh ?? DateTime.Now, // Optional, default hôm nay
                    NgayCoHieuLuc = request.NgayCoHieuLuc ?? DateTime.Now, // Optional, default hôm nay
                    HieuLuc = request.HieuLuc ?? true, // Optional, default true
                    ThoiGianDang = DateTime.Now // Tự động
                };

                var result = await _regulationService.CreateRegulationAsync(regulation);
                if (!result)
                    return ApiResponse<RegulationResponse>(null, "Tạo quy định thất bại.", 500, false);

                // Tạo response object chỉ chứa các trường cần thiết
                var response = new RegulationResponse
                {
                    MaQD = regulation.MaQD,
                    TenQD = regulation.TenQD,
                    LienKet = regulation.LienKet,
                    LoaiVanBan = regulation.LoaiVanBan,
                    NgayBanHanh = regulation.NgayBanHanh,
                    NgayCoHieuLuc = regulation.NgayCoHieuLuc,
                    HieuLuc = regulation.HieuLuc
                };
                
                return ApiResponse(response, "Tạo quy định thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<RegulationResponse>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpPut("{maQD}")]
        [SwaggerOperation(Summary = "Cập nhật quy định", Description = "API cho phép cán bộ cập nhật thông tin một quy định (chỉ các trường được phép)")]
        [SwaggerResponse(200, "Cập nhật quy định thành công", typeof(ApiResponse<Regulation>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy quy định", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<Regulation>>> UpdateRegulation(string maQD, [FromBody] RegulationUpdateRequest request)
        {
            if (string.IsNullOrEmpty(maQD))
            {
                return ApiResponse<Regulation>(null, "Mã quy định không hợp lệ", 400, false);
            }
            
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse<Regulation>(null, ModelState.ToString(), 400, false);
                    
                if (request == null)
                    return ApiResponse<Regulation>(null, "Dữ liệu cập nhật không được rỗng", 400, false);
                    
                if (string.IsNullOrEmpty(request.TenQD))
                    return ApiResponse<Regulation>(null, "Tên quy định không được rỗng", 400, false);

                // Lấy quy định hiện tại
                var existingRegulation = await _regulationService.GetRegulationById(maQD);
                if (existingRegulation == null)
                    return ApiResponse<Regulation>(null, "Không tìm thấy quy định", 404, false);

                // Chỉ cập nhật các trường được phép
                existingRegulation.MaQD = request.MaQD ?? existingRegulation.MaQD;
                existingRegulation.TenQD = request.TenQD;
                existingRegulation.MaCB = request.MaCB ?? existingRegulation.MaCB;
                existingRegulation.LienKet = request.LienKet ?? existingRegulation.LienKet;
                existingRegulation.LoaiVanBan = request.LoaiVanBan ?? existingRegulation.LoaiVanBan;
                existingRegulation.NgayBanHanh = request.NgayBanHanh ?? existingRegulation.NgayBanHanh;
                existingRegulation.NgayCoHieuLuc = request.NgayCoHieuLuc ?? existingRegulation.NgayCoHieuLuc;
                existingRegulation.HieuLuc = request.HieuLuc ?? existingRegulation.HieuLuc;
                existingRegulation.ThoiGianDang = request.ThoiGianDang ?? existingRegulation.ThoiGianDang;
                // MaPB không được phép cập nhật

                var result = await _regulationService.UpdateRegulationAsync(maQD, existingRegulation);
                if (!result)
                    return ApiResponse<Regulation>(null, "Cập nhật thất bại", 500, false);

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
                // Lấy mã cán bộ từ token
                var maCB = User.FindFirst("MSSV")?.Value;
                if (string.IsNullOrEmpty(maCB))
                    return ApiResponse<IEnumerable<Regulation>>(null, "Không xác định được cán bộ", 401, false);

                var staff = await _staffService.GetProfileAsync(maCB);
                if (staff == null)
                    return ApiResponse<IEnumerable<Regulation>>(null, "Không tìm thấy thông tin cán bộ", 404, false);

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

        [HttpPost("registration-forms")]
        [SwaggerOperation(Summary = "Thêm đơn đăng ký mới", Description = "API cho phép cán bộ tạo mới một đơn đăng ký hoàn chỉnh")]
        [SwaggerResponse(200, "Tạo đơn đăng ký thành công", typeof(ApiResponse<RegistrationForm>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<RegistrationForm>>> AddNewRegistrationForm([FromBody] CreateRegistrationFormRequest request)
        {
            try
            {
                // Lấy mã cán bộ từ token
                var maCB = User.FindFirst("MSSV")?.Value;
                if (string.IsNullOrEmpty(maCB))
                {
                    return ApiResponse<RegistrationForm>(null, "Không xác định được cán bộ", 401, false);
                }

                if (request == null)
                    return ApiResponse<RegistrationForm>(null, "Dữ liệu đơn đăng ký không được để trống", 400, false);

                // Validate required fields
                if (string.IsNullOrWhiteSpace(request.TenDon))
                    return ApiResponse<RegistrationForm>(null, "Tên đơn không được để trống", 400, false);

                if (string.IsNullOrWhiteSpace(request.MaPB))
                    return ApiResponse<RegistrationForm>(null, "Mã phòng ban không được để trống", 400, false);

                // Lấy thông tin cán bộ để kiểm tra quyền
                var staff = await _staffService.GetProfileAsync(maCB);
                if (staff == null)
                {
                    return ApiResponse<RegistrationForm>(null, "Không tìm thấy thông tin cán bộ", 404, false);
                }

                // Tạo đối tượng RegistrationForm từ request
                var newForm = new RegistrationForm
                {
                    MaDon = await GenerateMaDonAsync(), // Tự động tạo
                    MaPB = request.MaPB,
                    TenDon = request.TenDon,
                    MaCB = maCB, // Lấy từ token đăng nhập
                    MaQL = request.MaQL, // Optional, có thể null
                    ThongTinChiTiet = request.ThongTinChiTiet, // Optional
                    ThoiGianDang = DateTime.Now, // Tự động
                    TrangThai = true // Mặc định đang xử lý
                };

                // Thêm đơn đăng ký mới
                await _formService.AddForm(newForm);

                return ApiResponse(newForm, "Tạo đơn đăng ký mới thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<RegistrationForm>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }

        [HttpDelete("registration-forms/{maDon}")]
        [SwaggerOperation(Summary = "Xóa đơn đăng ký", Description = "API cho phép cán bộ xóa một đơn đăng ký đã tạo")]
        [SwaggerResponse(200, "Xóa đơn đăng ký thành công", typeof(ApiResponse<object>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy đơn đăng ký", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<object>>> DeleteRegistrationForm(string maDon)
        {
            try
            {
                // Lấy mã cán bộ từ token
                var maCB = User.FindFirst("MSSV")?.Value;
                if (string.IsNullOrEmpty(maCB))
                {
                    return ApiResponse<object>(null, "Không xác định được cán bộ", 401, false);
                }

                if (string.IsNullOrWhiteSpace(maDon))
                    return ApiResponse<object>(null, "Mã đơn không được để trống", 400, false);

                // Kiểm tra xem đơn đăng ký có tồn tại không
                var existingForm = await _formService.GetByFormIdAsync(maDon);
                if (existingForm == null)
                {
                    return ApiResponse<object>(null, "Không tìm thấy đơn đăng ký", 404, false);
                }

                // Lấy thông tin cán bộ để kiểm tra quyền
                var staff = await _staffService.GetProfileAsync(maCB);
                if (staff == null)
                {
                    return ApiResponse<object>(null, "Không tìm thấy thông tin cán bộ", 404, false);
                }

                // Kiểm tra quyền truy cập - chỉ được xóa đơn thuộc phòng ban của mình
                if (existingForm.MaPB != staff.MaPB)
                {
                    return ApiResponse<object>(null, "Bạn không có quyền xóa đơn này (không thuộc phòng ban của bạn)", 401, false);
                }

                // Kiểm tra xem đơn có đang được xử lý không (có thể có chi tiết đơn)
                var allDetails = await _registrationDetailService.GetAllDetailsAsync();
                var relatedDetails = allDetails.Where(d => d.MaDon == maDon).ToList();
                
                if (relatedDetails.Any())
                {
                    return ApiResponse<object>(null, "Không thể xóa đơn đăng ký vì đã có sinh viên đăng ký chi tiết", 400, false);
                }

                // Xóa đơn đăng ký
                var isDeleted = await _formService.DeleteFormAsync(maDon);
                if (!isDeleted)
                {
                    return ApiResponse<object>(null, "Không thể xóa đơn đăng ký", 500, false);
                }

                return ApiResponse<object>(null, "Xóa đơn đăng ký thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }

        [HttpDelete("registration-forms/multiple")]
        [SwaggerOperation(Summary = "Xóa nhiều đơn đăng ký", Description = "API cho phép cán bộ xóa nhiều đơn đăng ký cùng lúc")]
        [SwaggerResponse(200, "Xóa đơn đăng ký thành công", typeof(ApiResponse<object>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy đơn đăng ký", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<object>>> DeleteMultipleRegistrationForms([FromBody] DeleteMultipleRegistrationFormsRequest request)
        {
            try
            {
                if (request == null || request.MaDonList == null || !request.MaDonList.Any())
                {
                    return ApiResponse<object>(null, "Danh sách mã đơn không được rỗng", 400, false);
                }

                // Lấy mã cán bộ từ token
                var maCB = User.FindFirst("MSSV")?.Value;
                if (string.IsNullOrEmpty(maCB))
                {
                    return ApiResponse<object>(null, "Không xác định được cán bộ", 401, false);
                }

                // Lấy thông tin cán bộ để kiểm tra quyền
                var staff = await _staffService.GetProfileAsync(maCB);
                if (staff == null)
                {
                    return ApiResponse<object>(null, "Không tìm thấy thông tin cán bộ", 404, false);
                }

                // Lấy tất cả đơn đăng ký và lọc ra những đơn thuộc phòng ban của cán bộ
                var allForms = await _formService.GetAllForms();
                var departmentForms = allForms.Where(f => f.MaPB == staff.MaPB).ToList();
                
                // Kiểm tra quyền xóa và lọc ra những đơn hợp lệ
                var validFormIds = request.MaDonList
                    .Where(maDon => departmentForms.Any(f => f.MaDon == maDon))
                    .ToList();

                if (!validFormIds.Any())
                {
                    return ApiResponse<object>(null, "Không tìm thấy đơn đăng ký hợp lệ hoặc bạn không có quyền xóa các đơn này", 404, false);
                }

                // Kiểm tra xem có đơn nào đã có chi tiết đăng ký không
                var allDetails = await _registrationDetailService.GetAllDetailsAsync();
                var formsWithDetails = validFormIds.Where(maDon => 
                    allDetails.Any(d => d.MaDon == maDon)).ToList();

                if (formsWithDetails.Any())
                {
                    return ApiResponse<object>(null, 
                        $"Không thể xóa một số đơn đăng ký vì đã có sinh viên đăng ký chi tiết. Mã đơn: {string.Join(", ", formsWithDetails)}", 
                        400, false);
                }

                // Xóa các đơn đăng ký hợp lệ
                int deletedCount = 0;
                var failedDeletions = new List<string>();

                foreach (var maDon in validFormIds)
                {
                    var isDeleted = await _formService.DeleteFormAsync(maDon);
                    if (isDeleted)
                    {
                        deletedCount++;
                    }
                    else
                    {
                        failedDeletions.Add(maDon);
                    }
                }

                if (failedDeletions.Any())
                {
                    return ApiResponse<object>(null, 
                        $"Xóa thành công {deletedCount} đơn. Thất bại: {string.Join(", ", failedDeletions)}", 
                        500, false);
                }

                var totalRequested = request.MaDonList.Count();
                var message = deletedCount == totalRequested 
                    ? $"Xóa thành công {deletedCount} đơn đăng ký"
                    : $"Xóa thành công {deletedCount}/{totalRequested} đơn đăng ký (một số đơn không tồn tại hoặc không có quyền xóa)";

                return ApiResponse<object>(null, message);
            }
            catch (Exception ex)
            {
                return ApiResponse<object>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }

        [HttpDelete("regulations/multiple")]
        [SwaggerOperation(Summary = "Xóa nhiều quy định", Description = "API cho phép cán bộ xóa nhiều quy định cùng lúc")]
        [SwaggerResponse(200, "Xóa quy định thành công", typeof(ApiResponse<object>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy quy định", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<object>>> DeleteMultipleRegulations([FromBody] DeleteMultipleRegulationsRequest request)
        {
            try
            {
                if (request == null || request.MaQDList == null || !request.MaQDList.Any())
                {
                    return ApiResponse<object>(null, "Danh sách mã quy định không được rỗng", 400, false);
                }

                // Lấy mã cán bộ từ token
                var maCB = User.FindFirst("MSSV")?.Value;
                if (string.IsNullOrEmpty(maCB))
                {
                    return ApiResponse<object>(null, "Không xác định được cán bộ", 401, false);
                }

                // Lấy thông tin cán bộ để kiểm tra quyền
                var staff = await _staffService.GetProfileAsync(maCB);
                if (staff == null)
                {
                    return ApiResponse<object>(null, "Không tìm thấy thông tin cán bộ", 404, false);
                }

                // Lấy tất cả quy định và lọc ra những quy định thuộc phòng ban của cán bộ
                var allRegulations = await _regulationService.GetAllRegulations();
                var departmentRegulations = allRegulations.Where(r => r.MaPB == staff.MaPB).ToList();
                
                // Kiểm tra quyền xóa và lọc ra những quy định hợp lệ
                var validRegulationIds = request.MaQDList
                    .Where(maQD => departmentRegulations.Any(r => r.MaQD == maQD))
                    .ToList();

                if (!validRegulationIds.Any())
                {
                    return ApiResponse<object>(null, "Không tìm thấy quy định hợp lệ hoặc bạn không có quyền xóa các quy định này", 404, false);
                }

                // Xóa các quy định hợp lệ
                var isDeleted = await _regulationService.DeleteMultipleRegulationsAsync(validRegulationIds);
                if (!isDeleted)
                {
                    return ApiResponse<object>(null, "Không thể xóa quy định", 500, false);
                }

                var totalRequested = request.MaQDList.Count();
                var deletedCount = validRegulationIds.Count;
                var message = deletedCount == totalRequested 
                    ? $"Xóa thành công {deletedCount} quy định"
                    : $"Xóa thành công {deletedCount}/{totalRequested} quy định (một số quy định không tồn tại hoặc không có quyền xóa)";

                return ApiResponse<object>(null, message);
            }
            catch (Exception ex)
            {
                return ApiResponse<object>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }

        private async Task<string> GenerateMaDonAsync()
        {
            try
            {
                // Lấy tất cả đơn đăng ký để tìm mã cuối cùng
                var allForms = await _formService.GetAllForms();
                
                if (!allForms.Any())
                {
                    return "DD001";
                }

                // Tìm mã đơn lớn nhất
                var maxMaDon = allForms
                    .Where(f => f.MaDon.StartsWith("DD") && f.MaDon.Length == 5)
                    .Select(f => f.MaDon)
                    .OrderByDescending(x => x)
                    .FirstOrDefault();

                if (maxMaDon == null)
                {
                    return "DD001";
                }

                // Lấy số từ mã đơn cuối và tăng lên 1
                string numberPart = maxMaDon.Substring(2);
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    int nextNumber = lastNumber + 1;
                    return $"DD{nextNumber:D3}";
                }

                return "DD001";
            }
            catch
            {
                // Nếu có lỗi, trả về mã mặc định
                return $"DD{DateTime.Now.Ticks % 1000:D3}";
            }
        }
    }

    public class FormRequest
    {
        public string TenBM { get; set; }
        public string LienKet { get; set; }
    }

    public class FormUpdateRequest
    {
        public string TenBM { get; set; }
        public string? LienKet { get; set; } // Optional
        // Lưu ý: MaBM, MaCB, MaPB, ThoiGianDang không được phép cập nhật
    }

    public class FormCreateRequest
    {
        public string TenBM { get; set; }
        public IFormFile File { get; set; }
    }

    public class RegulationUpdateRequest
    {
        public string? MaQD { get; set; }
        public string TenQD { get; set; }
        public string? MaCB { get; set; }
        public string? LienKet { get; set; }
        public string? LoaiVanBan { get; set; }
        public DateTime? NgayBanHanh { get; set; }
        public DateTime? NgayCoHieuLuc { get; set; }
        public bool? HieuLuc { get; set; }
        public DateTime? ThoiGianDang { get; set; }
    }

    public class DeleteMultipleRegistrationFormsRequest
    {
        public List<string> MaDonList { get; set; }
    }

    public class CreateRegistrationFormRequest
    {
        public string MaPB { get; set; }
        public string TenDon { get; set; }
        public string? MaQL { get; set; } // Optional
        public string? ThongTinChiTiet { get; set; } // Optional
    }

    public class DeleteMultipleRegulationsRequest
    {
        public List<string> MaQDList { get; set; }
    }

    public class CreateRegulationRequest
    {
        public string? MaQD { get; set; } // Optional - nếu không có sẽ tự động tạo
        public string TenQD { get; set; }
        public string LoaiVanBan { get; set; }
        public string? LienKet { get; set; } // Optional
        public DateTime? NgayBanHanh { get; set; } // Optional, default hôm nay
        public DateTime? NgayCoHieuLuc { get; set; } // Optional, default hôm nay  
        public bool? HieuLuc { get; set; } // Optional, default true
    }

    public class RegulationResponse
    {
        public string MaQD { get; set; }
        public string TenQD { get; set; }
        public string? LienKet { get; set; }
        public string LoaiVanBan { get; set; }
        public DateTime NgayBanHanh { get; set; }
        public DateTime NgayCoHieuLuc { get; set; }
        public bool HieuLuc { get; set; }
    }

    public class UpdateStatusResponse
    {
        public string MaDonCT { get; set; }
        public string MaDon { get; set; }
        public string TrangThaiXuLy { get; set; }
        public DateTime ThoiGianCapNhat { get; set; }
    }
}
