using StudentServicePortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Repositories.Interfaces
{
    public interface IRegistrationDetailRepository
    {
        Task<IEnumerable<RegistrationDetail>> GetAllDetailsAsync();
        Task<RegistrationDetail> GetDetailByIdAsync(string maDonCT);
        Task<RegistrationDetail> AddDetailAsync(RegistrationDetail detail);
        Task<RegistrationDetail> UpdateDetailAsync(RegistrationDetail detail);
        Task<bool> DeleteDetailAsync(string maDonCT);
        Task<IEnumerable<RegistrationDetail>> GetDetailsByFormIdAsync(string maDon);
        Task<bool> UpdateStatusByMaDonAsync(string maDon, string newStatus);
        Task AddAsync(RegistrationDetail detail);
        Task<RegistrationDetail> GetLastDetailAsync();
        Task<IEnumerable<RegistrationDetailWithStudentInfo>> GetDetailsByFormIdWithStudentInfoAsync(string maDon);
        Task<RegistrationDetailWithStudentInfo> GetDetailByIdWithStudentInfoAsync(string maDonCT);
    }
}
