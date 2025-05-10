using StudentServicePortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Services
{
    public interface IRegulationService
    {
        Task<IEnumerable<Regulation>> GetAllRegulations();
        Task<Regulation?> GetRegulationById(string maQD);
        Task<bool> CreateRegulationAsync(Regulation regulation);

        Task<bool> UpdateRegulationAsync(string maQD, Regulation regulation);
    }
}
