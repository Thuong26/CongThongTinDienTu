using StudentServicePortal.Models;
using StudentServicePortal.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Services
{
    public class RegistrationFormService : IRegistrationFormService
    {
        private readonly IRegistrationFormRepository _formRepository;

        public RegistrationFormService(IRegistrationFormRepository formRepository)
        {
            _formRepository = formRepository;
        }

        public async Task<IEnumerable<RegistrationForm>> GetAllForms()
        {
            return await _formRepository.GetAllFormsAsync();
        }
        public async Task<RegistrationDetail?> GetFormById(string maDon)
        {
            return await _formRepository.GetFormByIdAsync(maDon);
        }
        public async Task AddForm(RegistrationForm form)
        {
            await _formRepository.AddFormAsync(form);
        }
        public async Task<IEnumerable<RegistrationForm>> GetPendingFormsAsync()
        {
            return await _formRepository.GetPendingFormsAsync();
        }

        public async Task<IEnumerable<RegistrationForm>> GetFormsByDepartmentAsync(string maPB)
        {
            return await _formRepository.GetFormsByDepartmentAsync(maPB);
        }

        public async Task<IEnumerable<RegistrationForm>> GetPendingFormsByDepartmentAsync(string maPB)
        {
            return await _formRepository.GetPendingFormsByDepartmentAsync(maPB);
        }

        public async Task<RegistrationForm> GetByFormIdAsync(string maDon)
        {
            return await _formRepository.GetByFormIdAsync(maDon);
        }
    }
}
