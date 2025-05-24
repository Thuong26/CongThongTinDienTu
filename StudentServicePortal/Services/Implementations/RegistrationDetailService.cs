using StudentServicePortal.Models;
using StudentServicePortal.Repositories.Interfaces;
using StudentServicePortal.Services.Interfaces;

namespace StudentServicePortal.Services.Implementations
{
    public class RegistrationDetailService : IRegistrationDetailService
    {
        private readonly IRegistrationDetailRepository _repository;

        public RegistrationDetailService(IRegistrationDetailRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RegistrationDetail>> GetDetailsByFormIdAsync(string maDon)
        {
            return await _repository.GetDetailsByFormIdAsync(maDon);
        }
        public async Task<bool> UpdateStatusByMaDonAsync(string maDon, string newStatus)
        {
            return await _repository.UpdateStatusByMaDonAsync(maDon, newStatus);
        }
    }
}
