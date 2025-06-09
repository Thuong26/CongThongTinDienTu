using Microsoft.AspNetCore.Mvc;
using StudentServicePortal.Models;
using StudentServicePortal.Services;
using StudentServicePortal.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace StudentServicePortal.Controllers
{
    [Authorize(Roles = "Student")]
    [Route("api/students/forms")]
    public class RegistrationFormsController : BaseController
    {
        private readonly IRegistrationFormService _formService;
        private readonly IRegistrationDetailService _service;
        private readonly ICodeGeneratorService _codeGenerator;

        public RegistrationFormsController(
            IRegistrationFormService formService, 
            IRegistrationDetailService service,
            ICodeGeneratorService codeGenerator)
        {
            _formService = formService;
            _service = service;
            _codeGenerator = codeGenerator;
        }
        
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách đơn đăng ký", Description = "API trả về toàn bộ danh sách đơn đăng ký của sinh viên")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ApiResponse<IEnumerable<RegistrationForm>>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<RegistrationForm>>>> GetAllForms()
        {
            try
            {
                // Lấy tất cả đơn đăng ký
                var allForms = await _formService.GetAllForms();
                return ApiResponse<IEnumerable<RegistrationForm>>(allForms, "Lấy danh sách đơn đăng ký thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<RegistrationForm>>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpGet("details")]
        [SwaggerOperation(Summary = "Lấy danh sách chi tiết đơn đăng ký", Description = "API trả về toàn bộ danh sách chi tiết đơn đăng ký của sinh viên kèm thông tin sinh viên")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ApiResponse<IEnumerable<RegistrationDetailWithStudentInfo>>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<RegistrationDetailWithStudentInfo>>>> GetAllDetails()
        {
            try
            {
                var mssv = User.FindFirst("MSSV")?.Value;
                if (string.IsNullOrEmpty(mssv))
                {
                    return ApiResponse<IEnumerable<RegistrationDetailWithStudentInfo>>(null, "Không xác định được sinh viên", 401, false);
                }

                var allDetails = await _service.GetAllDetailsAsync();
                var studentDetails = allDetails.Where(d => d.MaSV == mssv).ToList();

                var resultList = new List<RegistrationDetailWithStudentInfo>();

                foreach (var detail in studentDetails)
                {
                    var detailsWithStudentInfo = await _service.GetDetailsByFormIdWithStudentInfoAsync(detail.MaDon);
                    var studentDetail = detailsWithStudentInfo.FirstOrDefault(d => d.MaDonCT == detail.MaDonCT);
                    
                    if (studentDetail != null)
                    {
                        resultList.Add(studentDetail);
                    }
                }

                return ApiResponse(resultList, "Lấy danh sách chi tiết đơn đăng ký thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<RegistrationDetailWithStudentInfo>>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpGet("detail/{maDonCT}")]
        [SwaggerOperation(Summary = "Lấy chi tiết đơn đăng ký theo mã", Description = "API trả về thông tin chi tiết đơn đăng ký kèm thông tin sinh viên")]
        [SwaggerResponse(200, "Lấy chi tiết thành công", typeof(ApiResponse<RegistrationDetailWithStudentInfo>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy đơn", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<RegistrationDetailWithStudentInfo>>> GetDetailById(string maDonCT)
        {
            try
            {
                var mssv = User.FindFirst("MSSV")?.Value;
                if (string.IsNullOrEmpty(mssv))
                {
                    return ApiResponse<RegistrationDetailWithStudentInfo>(null, "Không xác định được sinh viên", 401, false);
                }

                var detail = await _service.GetDetailByIdWithStudentInfoAsync(maDonCT);
                if (detail == null)
                {
                    return ApiResponse<RegistrationDetailWithStudentInfo>(null, "Không tìm thấy đơn đăng ký", 404, false);
                }

                // Kiểm tra quyền truy cập (chỉ sinh viên tạo đơn mới được xem)
                if (detail.MaSV != mssv)
                {
                    return ApiResponse<RegistrationDetailWithStudentInfo>(null, "Bạn không có quyền xem đơn này", 401, false);
                }

                return ApiResponse(detail, "Lấy chi tiết đơn đăng ký thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<RegistrationDetailWithStudentInfo>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpGet("by-id/{maDon}")]
        [SwaggerOperation(Summary = "Lấy đơn đăng ký theo mã đơn", Description = "API trả về thông tin đơn đăng ký dựa trên mã đơn cung cấp")]
        [SwaggerResponse(200, "Lấy thông tin thành công", typeof(ApiResponse<RegistrationForm>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy đơn đăng ký", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<RegistrationForm>>> GetByFormIdAsync(string maDon)
        {
            try
            {
                // Lấy MSSV từ token
                var mssv = User.FindFirst("MSSV")?.Value;
                if (string.IsNullOrEmpty(mssv))
                {
                    return ApiResponse<RegistrationForm>(null, "Không xác định được sinh viên", 401, false);
                }

                if (string.IsNullOrEmpty(maDon))
                    return ApiResponse<RegistrationForm>(null, "Mã đơn không được để trống", 400, false);

                // Kiểm tra xem đơn có thuộc về sinh viên này không
                var details = await _service.GetAllDetailsAsync();
                var detail = details.FirstOrDefault(d => d.MaDon == maDon && d.MaSV == mssv);
                if (detail == null)
                {
                    return ApiResponse<RegistrationForm>(null, "Bạn không có quyền xem đơn này", 401, false);
                }

                var form = await _formService.GetByFormIdAsync(maDon);
                if (form == null)
                    return ApiResponse<RegistrationForm>(null, $"Không tìm thấy đơn đăng ký với mã: {maDon}", 404, false);
                
                return ApiResponse(form, "Lấy đơn đăng ký thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<RegistrationForm>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpPost]
        [SwaggerOperation(Summary = "Thêm mới đơn đăng ký chi tiết", Description = "API cho phép thêm mới một đơn đăng ký chi tiết cho sinh viên")]
        [SwaggerResponse(200, "Thêm đơn đăng ký thành công", typeof(ApiResponse<RegistrationDetail>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<RegistrationDetail>>> AddForm([FromBody] RegistrationDetailRequest request)
        {
            try
            {
                // Lấy MSSV từ token
                var mssv = User.FindFirst("MSSV")?.Value;
                if (string.IsNullOrEmpty(mssv))
                {
                    return ApiResponse<RegistrationDetail>(null, "Không xác định được sinh viên", 401, false);
                }

                if (request == null)
                    return ApiResponse<RegistrationDetail>(null, "Dữ liệu không được để trống", 400, false);

                // Kiểm tra xem MSSV trong request có khớp với MSSV trong token không
                if (request.MaSV != mssv)
                {
                    return ApiResponse<RegistrationDetail>(null, "Bạn không có quyền tạo đơn cho sinh viên khác", 401, false);
                }

                var validationContext = new ValidationContext(request, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(x => x.ErrorMessage);
                    return ApiResponse<RegistrationDetail>(null, string.Join(", ", errorMessages), 400, false);
                }

                // Tạo đối tượng RegistrationDetail từ request
                var detail = new RegistrationDetail
                {
                    MaDonCT = await _codeGenerator.GenerateMaDonCTAsync(), // Tự động tạo mã đơn chi tiết
                    MaDon = request.MaDon,
                    MaSV = request.MaSV,
                    HocKyHienTai = request.HocKyHienTai,
                    ThongTinChiTiet = request.ThongTinChiTiet,
                    NgayTaoDonCT = DateTime.Now,
                    TrangThaiXuLy = "Đang xử lý"
                };

                await _service.AddDetailAsync(detail);

                return ApiResponse(detail, "Thêm đơn đăng ký thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<RegistrationDetail>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
    }
}
