using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentServicePortal.Models;
using StudentServicePortal.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

                return ApiResponse(profile);
            }
            catch (Exception ex)
            {
                return ApiResponse<Student>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }

        [HttpGet("templates")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Form>>>> GetAllFormTemplates()
        {
            try
            {
                var forms = await _formService.GetAllForms();
                return ApiResponse(forms);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<Form>>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpGet("templates/{maBM}")]
        public async Task<ActionResult<ApiResponse<Form>>> GetFormById(string maBM)
        {
            if (string.IsNullOrWhiteSpace(maBM))
                return ApiResponse<Form>(null, "Mã biểu mẫu không hợp lệ", 400, false);
                
            try
            {
                var form = await _formService.GetFormById(maBM);
                if (form == null)
                    return ApiResponse<Form>(null, $"Không tìm thấy biểu mẫu với mã {maBM}", 404, false);
                    
                return ApiResponse(form);
            }
            catch (Exception ex)
            {
                return ApiResponse<Form>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpGet("regulations")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Regulation>>>> GetAllRegulations()
        {
            try
            {
                var regulations = await _regulationService.GetAllRegulations();
                return ApiResponse(regulations);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<Regulation>>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
        
        [HttpGet("regulations/{maQD}")]
        public async Task<ActionResult<ApiResponse<Regulation>>> GetRegulationById(string maQD)
        {
            if (string.IsNullOrWhiteSpace(maQD))
                return ApiResponse<Regulation>(null, "Mã quy định không hợp lệ", 400, false);
                
            try
            {
                var regulation = await _regulationService.GetRegulationById(maQD);
                if (regulation == null)
                {
                    return ApiResponse<Regulation>(null, $"Không tìm thấy quy định với mã {maQD}", 404, false);
                }

                return ApiResponse(regulation);
            }
            catch (Exception ex)
            {
                return ApiResponse<Regulation>(null, $"Lỗi hệ thống: {ex.Message}", 500, false);
            }
        }
    }
}
