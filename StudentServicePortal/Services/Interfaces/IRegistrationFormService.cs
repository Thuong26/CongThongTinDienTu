using StudentServicePortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Services
{
    public interface IRegistrationFormService
    {
        Task<IEnumerable<RegistrationForm>> GetAllForms();
        Task<RegistrationDetail?> GetFormById(string maDon);

        Task AddForm(RegistrationForm form);
        Task<RegistrationForm> GetByFormIdAsync(string maDon);

        Task<IEnumerable<RegistrationForm>> GetPendingFormsAsync();
    }
}