using StudentServicePortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Repositories
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllStudents();
        Task<Student> GetStudentById(string mssv);
        Task AddStudent(Student student);
        Task UpdateStudent(Student student);
        Task DeleteStudent(string mssv);
    }
}
