using StudentServicePortal.Models;
using StudentServicePortal.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<IEnumerable<Student>> GetAllStudents()
        {
            return await _studentRepository.GetAllStudents();
        }

        public async Task<Student> GetStudentById(string mssv)
        {
            return await _studentRepository.GetStudentById(mssv);
        }

        public async Task AddStudent(Student student)
        {
            await _studentRepository.AddStudent(student);
        }

        public async Task UpdateStudent(Student student)
        {
            await _studentRepository.UpdateStudent(student);
        }

        public async Task DeleteStudent(string mssv)
        {
            await _studentRepository.DeleteStudent(mssv);
        }
    }
}
