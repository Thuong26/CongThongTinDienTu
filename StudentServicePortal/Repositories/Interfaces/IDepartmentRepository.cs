using StudentServicePortal.Models;
using System.Collections.Generic;

namespace StudentServicePortal.Repositories
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
    }
}
