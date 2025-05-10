using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentServicePortal.Models;
using StudentServicePortal.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/students")]
    public class StudentController : ControllerBase
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
        //public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        //{
        //    var students = await _studentService.GetAllStudents();
        //    return Ok(students);
        //}

        //[HttpGet("{mssv}")]
        //public async Task<ActionResult<Student>> GetStudentById(string mssv)
        //{
        //    var student = await _studentService.GetStudentById(mssv);
        //    if (student == null)
        //        return NotFound("Student not found");
        //    return Ok(student);
        //}
        //[HttpPost]
        //public async Task<IActionResult> AddStudent(Student student)
        //{
        //    await _studentService.AddStudent(student);
        //    return CreatedAtAction(nameof(GetStudentById), new { mssv = student.MSSV }, student);
        //}
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateStudent(Student student)
        {
            var mssv = User.FindFirst("MSSV")?.Value;
            if (mssv != student.MSSV)
                return BadRequest("MSSV mismatch");

            await _studentService.UpdateStudent(student);
            return NoContent();
        }

        //[HttpDelete("{mssv}")]
        //public async Task<IActionResult> DeleteStudent(string mssv)
        //{
        //    await _studentService.DeleteStudent(mssv);
        //    return NoContent();
        //}
        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<Student>> GetStudentProfile()
        {
            var mssv = User.FindFirst("MSSV")?.Value;

            if (string.IsNullOrWhiteSpace(mssv))
                return Unauthorized("Không thể xác định sinh viên đang đăng nhập.");

            var profile = await _studentService.GetStudentProfile(mssv);

            if (profile == null)
                return NotFound($"Không tìm thấy thông tin sinh viên với mã số = {mssv}.");

            return Ok(profile);
        }

        [HttpGet("templates")]
        public async Task<ActionResult<IEnumerable<Form>>> GetAllFormTemplates()
        {
            var forms = await _formService.GetAllForms();
            return Ok(forms);
        }
        [HttpGet("templates/{maBM}")]
        public async Task<ActionResult<Form>> GetFormById(string maBM)
        {
            var form = await _formService.GetFormById(maBM);
            if (form == null)
                return NotFound($"No form found with code {maBM}");
            return Ok(form);
        }
        [HttpGet("regulations")]
        public async Task<ActionResult<IEnumerable<Regulation>>> GetAllRegulations()
        {
            var regulations = await _regulationService.GetAllRegulations();
            return Ok(regulations);
        }
        [HttpGet("regulations/{maQD}")]
        public async Task<ActionResult<Regulation>> GetRegulationById(string maQD)
        {
            var regulation = await _regulationService.GetRegulationById(maQD);
            if (regulation == null)
            {
                return NotFound(new { message = "No rule found with the code provided." });
            }

            return Ok(regulation);
        }

    }
}
