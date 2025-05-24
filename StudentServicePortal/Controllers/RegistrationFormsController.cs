using Microsoft.AspNetCore.Mvc;
using StudentServicePortal.Models;
using StudentServicePortal.Services;
using StudentServicePortal.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace StudentServicePortal.Controllers
{
    [Route("api/students/forms")]
    public class RegistrationFormsController : BaseController
    {
        private readonly IRegistrationFormService _formService;
        private readonly IRegistrationDetailService _service;
        public RegistrationFormsController(IRegistrationFormService formService, IRegistrationDetailService service)
        {
            _formService = formService;
            _service = service;
        }
        
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách đơn đăng ký", Description = "API trả về toàn bộ danh sách đơn đăng ký của sinh viên")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ApiResponse<IEnumerable<RegistrationForm>>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<RegistrationForm>>>> GetAllForms()
        {
            try
            {
                var forms = await _formService.GetAllForms();
                return ApiResponse(forms, "Lấy danh sách đơn đăng ký thành công");
            }
            catch (Exception)
            {
                return ApiResponse<IEnumerable<RegistrationForm>>(null, "Lỗi hệ thống", 500, false);
            }
        }
        
        [HttpGet("details/{maDon}")]
        [SwaggerOperation(Summary = "Lấy chi tiết đơn đăng ký theo mã đơn", Description = "API trả về chi tiết đơn đăng ký dựa trên mã đơn cung cấp")]
        [SwaggerResponse(200, "Lấy chi tiết thành công", typeof(ApiResponse<RegistrationDetail>))]
        [SwaggerResponse(404, "Không tìm thấy đơn đăng ký", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<RegistrationDetail>>> GetFormById(string maDon)
        {
            try
            {
                var form = await _formService.GetFormById(maDon);
                if (form == null)
                    return ApiResponse<RegistrationDetail>(null, $"No application found with code: {maDon}", 404, false);
                return ApiResponse(form, "Lấy chi tiết đơn đăng ký thành công");
            }
            catch (Exception)
            {
                return ApiResponse<RegistrationDetail>(null, "Lỗi hệ thống", 500, false);
            }
        }
        
        [HttpGet("{maDon}")]
        [SwaggerOperation(Summary = "Lấy đơn đăng ký theo mã đơn", Description = "API trả về thông tin đơn đăng ký dựa trên mã đơn cung cấp")]
        [SwaggerResponse(200, "Lấy thông tin thành công", typeof(ApiResponse<RegistrationForm>))]
        [SwaggerResponse(404, "Không tìm thấy đơn đăng ký", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<RegistrationForm>>> GetByFormIdAsync(string maDon)
        {
            try
            {
                var form = await _formService.GetByFormIdAsync(maDon);
                if (form == null)
                    return ApiResponse<RegistrationForm>(null, $"No application found with code: {maDon}", 404, false);
                return ApiResponse(form, "Lấy đơn đăng ký thành công");
            }
            catch (Exception)
            {
                return ApiResponse<RegistrationForm>(null, "Lỗi hệ thống", 500, false);
            }
        }
        
        [HttpPost]
        [SwaggerOperation(Summary = "Thêm mới đơn đăng ký", Description = "API cho phép thêm mới một đơn đăng ký cho sinh viên")]
        [SwaggerResponse(200, "Thêm đơn đăng ký thành công", typeof(ApiResponse<RegistrationForm>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<RegistrationForm>>> AddForm([FromBody] RegistrationForm form)
        {
            try
            {
                if (form == null || string.IsNullOrEmpty(form.MaDon))
                {
                    return ApiResponse<RegistrationForm>(null, "Invalid data.", 400, false);
                }

                form.ThoiGianDang = form.ThoiGianDang == default ? DateTime.Now : form.ThoiGianDang;
                form.TrangThai = true;

                await _formService.AddForm(form);

                return ApiResponse(form, "Registration form added successfully");
            }
            catch (Exception)
            {
                return ApiResponse<RegistrationForm>(null, "Lỗi hệ thống", 500, false);
            }
        }
    }
}
