using StudentServicePortal.Models;
using StudentServicePortal.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Services
{
    public class RegulationService : IRegulationService
    {
        private readonly IRegulationRepository _regulationRepository;

        public RegulationService(IRegulationRepository regulationRepository)
        {
            _regulationRepository = regulationRepository;
        }

        public async Task<IEnumerable<Regulation>> GetAllRegulations()
        {
            return await _regulationRepository.GetAllRegulations();
        }
        public async Task<Regulation?> GetRegulationById(string maQD)
        {
            return await _regulationRepository.GetRegulationById(maQD);
        }
        public async Task<bool> CreateRegulationAsync(Regulation regulation)
        {
            regulation.ThoiGianDang = DateTime.Now;
            return await _regulationRepository.CreateAsync(regulation);
        }
        public async Task<bool> UpdateRegulationAsync(string maQD, Regulation regulation)
        {
            regulation.ThoiGianDang = DateTime.Now;
            return await _regulationRepository.UpdateAsync(maQD, regulation);
        }

        public async Task<IEnumerable<Regulation>> GetRegulationsByDepartment(string maPB)
        {
            return await _regulationRepository.GetRegulationsByDepartment(maPB);
        }
    }
}
