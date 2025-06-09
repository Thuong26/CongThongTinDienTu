using StudentServicePortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Repositories.Interfaces
{
    public interface IRegistrationFormRepository
    {
        Task<IEnumerable<RegistrationForm>> GetAllFormsAsync();
        Task<RegistrationDetail?> GetFormByIdAsync(string maDon);
        Task AddFormAsync(RegistrationForm form);
        Task<RegistrationForm> GetByFormIdAsync(string maDon);

        Task<IEnumerable<RegistrationForm>> GetPendingFormsAsync();
        Task<IEnumerable<RegistrationForm>> GetFormsByDepartmentAsync(string maPB);
        Task<IEnumerable<RegistrationForm>> GetPendingFormsByDepartmentAsync(string maPB);
    }
}
