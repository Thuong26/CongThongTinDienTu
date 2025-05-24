using StudentServicePortal.Models;

namespace StudentServicePortal.Services.Interfaces
{
    public interface IRegistrationDetailService
    {
        Task<IEnumerable<RegistrationDetail>> GetDetailsByFormIdAsync(string maDon);
        Task<bool> UpdateStatusByMaDonAsync(string maDon, string newStatus);
    }
}
