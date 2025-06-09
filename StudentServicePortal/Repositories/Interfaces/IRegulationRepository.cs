using StudentServicePortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Repositories
{
    public interface IRegulationRepository
    {
        Task<IEnumerable<Regulation>> GetAllRegulations();
        Task<Regulation?> GetRegulationById(string maQD);
        Task<bool> CreateAsync(Regulation regulation);

        Task<bool> UpdateAsync(string maQD, Regulation regulation);
        Task<IEnumerable<Regulation>> GetRegulationsByDepartment(string maPB);
        Task<Regulation> GetLastRegulationAsync();
    }
}
