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
                // Lấy MSSV từ token
                var mssv = User.FindFirst("MSSV")?.Value;
                if (string.IsNullOrEmpty(mssv))
                {
                    return ApiResponse<IEnumerable<RegistrationForm>>(null, "Không xác định được sinh viên", 401, false);
                }

                // Lấy tất cả đơn đăng ký
                var allForms = await _formService.GetAllForms();
                
                // Lọc chỉ lấy đơn của sinh viên hiện tại
                var studentForms = allForms.Where(f => 
                {
                    var details = _service.GetAllDetailsAsync().Result;
                    return details.Any(d => d.MaDon == f.MaDon && d.MaSV == mssv);
                });

                return ApiResponse(studentForms, "Lấy danh sách đơn đăng ký thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<RegistrationForm>>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpGet("details")]
        [SwaggerOperation(Summary = "Lấy danh sách chi tiết đơn đăng ký", Description = "API trả về toàn bộ danh sách chi tiết đơn đăng ký của sinh viên")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ApiResponse<IEnumerable<RegistrationDetail>>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<RegistrationDetail>>>> GetAllDetails()
        {
            try
            {
                // Lấy MSSV từ token
                var mssv = User.FindFirst("MSSV")?.Value;
                if (string.IsNullOrEmpty(mssv))
                {
                    return ApiResponse<IEnumerable<RegistrationDetail>>(null, "Không xác định được sinh viên", 401, false);
                }

                // Lấy tất cả chi tiết đơn
                var allDetails = await _service.GetAllDetailsAsync();
                
                // Lọc chỉ lấy chi tiết đơn của sinh viên hiện tại
                var studentDetails = allDetails.Where(d => d.MaSV == mssv);

                return ApiResponse(studentDetails, "Lấy danh sách chi tiết đơn đăng ký thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<RegistrationDetail>>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpGet("{maDon}")]
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
