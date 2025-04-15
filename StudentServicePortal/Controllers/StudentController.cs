using Microsoft.AspNetCore.Mvc;
using StudentServicePortal.Models;
using StudentServicePortal.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {
            var students = await _studentService.GetAllStudents();
            return Ok(students);
        }

        [HttpGet("{mssv}")]
        public async Task<ActionResult<Student>> GetStudentById(string mssv)
        {
            var student = await _studentService.GetStudentById(mssv);
            if (student == null)
                return NotFound("Student not found");
            return Ok(student);
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent(Student student)
        {
            await _studentService.AddStudent(student);
            return CreatedAtAction(nameof(GetStudentById), new { mssv = student.MSSV }, student);
        }

        [HttpPut("{mssv}")]
        public async Task<IActionResult> UpdateStudent(string mssv, Student student)
        {
            if (mssv != student.MSSV)
                return BadRequest("MSSV mismatch");

            await _studentService.UpdateStudent(student);
            return NoContent();
        }

        [HttpDelete("{mssv}")]
        public async Task<IActionResult> DeleteStudent(string mssv)
        {
            await _studentService.DeleteStudent(mssv);
            return NoContent();
        }
    }
}
