using Microsoft.AspNetCore.Mvc;
using StudentServicePortal.Models;
using StudentServicePortal.Services;
using StudentServicePortal.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<ActionResult<ApiResponse<IEnumerable<RegistrationForm>>>> GetAllForms()
        {
            try
            {
                var forms = await _formService.GetAllForms();
                return ApiResponse(forms);
            }
            catch (Exception)
            {
                return ApiResponse<IEnumerable<RegistrationForm>>(null, "Lỗi hệ thống", 500, false);
            }
        }
        
        [HttpGet("details/{maDon}")]
        public async Task<ActionResult<ApiResponse<RegistrationDetail>>> GetFormById(string maDon)
        {
            try
            {
                var form = await _formService.GetFormById(maDon);
                if (form == null)
                    return ApiResponse<RegistrationDetail>(null, $"No application found with code: {maDon}", 404, false);

                return ApiResponse(form);
            }
            catch (Exception)
            {
                return ApiResponse<RegistrationDetail>(null, "Lỗi hệ thống", 500, false);
            }
        }
        
        [HttpGet("{maDon}")]
        public async Task<ActionResult<ApiResponse<RegistrationForm>>> GetByFormIdAsync(string maDon)
        {
            try
            {
                var form = await _formService.GetByFormIdAsync(maDon);
                if (form == null)
                    return ApiResponse<RegistrationForm>(null, $"No application found with code: {maDon}", 404, false);

                return ApiResponse(form);
            }
            catch (Exception)
            {
                return ApiResponse<RegistrationForm>(null, "Lỗi hệ thống", 500, false);
            }
        }
        
        [HttpPost]
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
        
        [HttpPost("detail")]
        public async Task<ActionResult<ApiResponse<RegistrationDetail>>> Create([FromBody] RegistrationDetail detail)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse<RegistrationDetail>(null, "Invalid model state", 400, false);

                var created = await _service.CreateAsync(detail);
                return ApiResponse(created, "Registration detail created successfully");
            }
            catch (Exception)
            {
                return ApiResponse<RegistrationDetail>(null, "Lỗi hệ thống", 500, false);
            }
        }
        
        [HttpGet("{maDonCT}")]
        public async Task<ActionResult<ApiResponse<RegistrationDetail>>> GetById(string maDonCT)
        {
            try
            {
                var detail = await _service.GetByIdAsync(maDonCT);
                if (detail == null)
                    return ApiResponse<RegistrationDetail>(null, "Registration detail not found", 404, false);
                
                return ApiResponse(detail);
            }
            catch (Exception)
            {
                return ApiResponse<RegistrationDetail>(null, "Lỗi hệ thống", 500, false);
            }
        }
    }
}
