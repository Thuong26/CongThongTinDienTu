using StudentServicePortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Services
{
    public interface IFormService
    {
        Task<IEnumerable<Form>> GetAllForms();

        Task<Form?> GetFormById(string maBM);

        Task<bool> CreateFormAsync(Form form);

        Task<bool> UpdateFormAsync(string maBM, Form form);
    }
}
