using StudentServicePortal.Models;

namespace StudentServicePortal.Repositories.Interfaces
{
    public interface IRegistrationDetailRepository
    {
        Task<IEnumerable<RegistrationDetail>> GetDetailsByFormIdAsync(string maDon);

        Task<bool> UpdateStatusByMaDonAsync(string maDon, string newStatus);
        Task AddAsync(RegistrationDetail detail);
        Task<RegistrationDetail> GetByIdAsync(string maDonCT);
    }
}
