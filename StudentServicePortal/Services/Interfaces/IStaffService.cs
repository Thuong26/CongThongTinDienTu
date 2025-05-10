
using StudentServicePortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Services
{
    public interface IStaffService
    {
        Task<StaffDTO?> GetProfileAsync(string maCB);
        Task<IEnumerable<StaffDTO>> GetAllStaffAsync();
        Task<bool> CreateStaffAsync(Staff staff);
        Task<bool> UpdateStaffAsync(string msCB, Staff staff);

    }
}
