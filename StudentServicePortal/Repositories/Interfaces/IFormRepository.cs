using StudentServicePortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Repositories
{
    public interface IFormRepository
    {
        Task<IEnumerable<Form>> GetAllForms();
        Task<Form?> GetFormById(string maBM);
        Task<bool> CreateFormAsync(Form form);
        Task<bool> UpdateAsync(string maBM, Form form);
    }
}
