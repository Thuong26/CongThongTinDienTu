using StudentServicePortal.Models;
using StudentServicePortal.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Services
{
    public class FormService : IFormService
    {
        private readonly IFormRepository _formRepository;

        public FormService(IFormRepository formRepository)
        {
            _formRepository = formRepository;
        }

        public async Task<IEnumerable<Form>> GetAllForms()
        {
            return await _formRepository.GetAllForms();
        }
        public async Task<Form?> GetFormById(string maBM)
        {
            return await _formRepository.GetFormById(maBM);
        }
        public async Task<bool> CreateFormAsync(Form form)
        {
            form.ThoiGianDang = DateTime.Now;
            return await _formRepository.CreateFormAsync(form);
        }

        public async Task<bool> UpdateFormAsync(string maBM, Form form)
        {
            var existingForm = await _formRepository.GetFormById(maBM);
            if (existingForm == null)
                return false;

            return await _formRepository.UpdateAsync(maBM, form);
        }

        public async Task<bool> DeleteFormAsync(string maBM)
        {
            var existingForm = await _formRepository.GetFormById(maBM);
            if (existingForm == null)
                return false;

            return await _formRepository.DeleteFormAsync(maBM);
        }
    }
}
