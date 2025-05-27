using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentServicePortal.Models;
using StudentServicePortal.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace StudentServicePortal.Controllers
{
    [Authorize]
    [Route("api/students")]
    public class StudentController : BaseController
    {
        private readonly IStudentService _studentService;
        private readonly IFormService _formService;
        private readonly IRegulationService _regulationService;

        public StudentController(IStudentService studentService, IFormService formService, IRegulationService regulationService)
        {
            _studentService = studentService;
            _formService = formService;
            _regulationService = regulationService;
        }

        //[HttpGet]
        //public async Task<ActionResult<ApiResponse<IEnumerable<Student>>>> GetAllStudents()
        //{
        //    var students = await _studentService.GetAllStudents();
        //    return ApiResponse(students);
        //}

        //[HttpGet("{mssv}")]
        //public async Task<ActionResult<ApiResponse<Student>>> GetStudentById(string mssv)
        //{
        //    var student = await _studentService.GetStudentById(mssv);
        //    if (student == null)
        //        return ApiResponse<Student>(null, "Student not found", 404, false);
        //    return ApiResponse(student);
        //}
        //[HttpPost]
        //public async Task<ActionResult<ApiResponse<Student>>> AddStudent(Student student)
        //{
        //    await _studentService.AddStudent(student);
        //    return ApiResponse(student, "Student added successfully");
        //}
        
        [HttpPut]
        [SwaggerOperation(Summary = "Cập nhật thông tin sinh viên", Description = "API cho phép sinh viên cập nhật thông tin cá nhân của mình")]
        [SwaggerResponse(200, "Cập nhật thành công", typeof(ApiResponse<string>))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(401, "Không xác định được người dùng", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<string>>> UpdateStudent(Student student)
        {
            if (student == null)
                return ApiResponse<string>("", "Dữ liệu không hợp lệ", 400, false);
                
            var mssv = User.FindFirst("MSSV")?.Value;
            if (mssv != student.MSSV)
                return ApiResponse<string>("", "MSSV không khớp với người dùng hiện tại", 400, false);

            try 
            {
                await _studentService.UpdateStudent(student);
                return ApiResponse("", "Cập nhật thông tin sinh viên thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>("", $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }

        //[HttpDelete("{mssv}")]
        //public async Task<ActionResult<ApiResponse<string>>> DeleteStudent(string mssv)
        //{
        //    await _studentService.DeleteStudent(mssv);
        //    return ApiResponse("", "Student deleted successfully");
        //}
        
        [HttpGet("profile")]
        [SwaggerOperation(Summary = "Lấy thông tin sinh viên", Description = "API trả về thông tin chi tiết của sinh viên đang đăng nhập")]
        [SwaggerResponse(200, "Lấy thông tin thành công", typeof(ApiResponse<Student>))]
        [SwaggerResponse(401, "Không xác định được người dùng", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy thông tin sinh viên", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<Student>>> GetStudentProfile()
        {
            var mssv = User.FindFirst("MSSV")?.Value;
            if (string.IsNullOrWhiteSpace(mssv))
                return ApiResponse<Student>(null, "Không xác định được người dùng", 401, false);
            try
            {
                var profile = await _studentService.GetStudentProfile(mssv);
                if (profile == null)
                    return ApiResponse<Student>(null, $"Không tìm thấy thông tin sinh viên với mã số = {mssv}.", 404, false);
                return ApiResponse(profile, "Lấy thông tin sinh viên thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Student>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }

        [HttpGet("templates")]
        [SwaggerOperation(Summary = "Lấy danh sách biểu mẫu", Description = "API trả về danh sách các biểu mẫu đăng ký dành cho sinh viên")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ApiResponse<IEnumerable<Form>>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<Form>>>> GetAllFormTemplates()
        {
            try
            {
                var forms = await _formService.GetAllForms();
                return ApiResponse(forms, "Lấy danh sách biểu mẫu thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<Form>>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpGet("templates/{maBM}")]
        [SwaggerOperation(Summary = "Lấy biểu mẫu theo mã", Description = "API trả về thông tin chi tiết của một biểu mẫu dựa trên mã biểu mẫu cung cấp")]
        [SwaggerResponse(200, "Lấy thông tin thành công", typeof(ApiResponse<Form>))]
        [SwaggerResponse(400, "Mã biểu mẫu không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy biểu mẫu", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<Form>>> GetFormById(string maBM)
        {
            if (string.IsNullOrWhiteSpace(maBM))
                return ApiResponse<Form>(null, "Mã biểu mẫu không hợp lệ", 400, false);
            try
            {
                var form = await _formService.GetFormById(maBM);
                if (form == null)
                    return ApiResponse<Form>(null, $"Không tìm thấy biểu mẫu với mã {maBM}", 404, false);
                return ApiResponse(form, "Lấy thông tin biểu mẫu thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Form>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpGet("regulations")]
        [SwaggerOperation(Summary = "Lấy danh sách quy định", Description = "API trả về danh sách các quy định dành cho sinh viên")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ApiResponse<IEnumerable<Regulation>>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<Regulation>>>> GetAllRegulations()
        {
            try
            {
                var regulations = await _regulationService.GetAllRegulations();
                return ApiResponse(regulations, "Lấy danh sách quy định thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<Regulation>>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpGet("regulations/{maQD}")]
        [SwaggerOperation(Summary = "Lấy quy định theo mã", Description = "API trả về thông tin chi tiết của một quy định dựa trên mã quy định cung cấp")]
        [SwaggerResponse(200, "Lấy thông tin thành công", typeof(ApiResponse<Regulation>))]
        [SwaggerResponse(400, "Mã quy định không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy quy định", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<Regulation>>> GetRegulationById(string maQD)
        {
            if (string.IsNullOrWhiteSpace(maQD))
                return ApiResponse<Regulation>(null, "Mã quy định không hợp lệ", 400, false);
            try
            {
                var regulation = await _regulationService.GetRegulationById(maQD);
                if (regulation == null)
                    return ApiResponse<Regulation>(null, $"Không tìm thấy quy định với mã {maQD}", 404, false);
                return ApiResponse(regulation, "Lấy thông tin quy định thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Regulation>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
    }
}
