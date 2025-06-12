using StudentServicePortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Services.Interfaces
{
    public interface IRegistrationDetailService
    {
        Task<RegistrationDetail> GetDetailByIdAsync(string maDonCT);
        Task<IEnumerable<RegistrationDetail>> GetAllDetailsAsync();
        Task<RegistrationDetail> AddDetailAsync(RegistrationDetail detail);
        Task<RegistrationDetail> UpdateDetailAsync(RegistrationDetail detail);
        Task<bool> DeleteDetailAsync(string maDonCT);
        Task<string> GenerateMaDonCTAsync();
        Task<IEnumerable<RegistrationDetailWithStudentInfo>> GetDetailsByFormIdWithStudentInfoAsync(string maDon);
        Task<RegistrationDetailWithStudentInfo> GetDetailByIdWithStudentInfoAsync(string maDonCT);
    }
}
