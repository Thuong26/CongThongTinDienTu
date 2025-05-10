using StudentServicePortal.Models;

namespace StudentServicePortal.Repositories
{
    public interface IStaffRepository
    {
        Task<StaffDTO?> GetByIdAsync(string maCB);
        Task<IEnumerable<StaffDTO>> GetAllStaffAsync();
        Task<bool> CreateStaffAsync(Staff staff);
        Task<bool> UpdateStaffAsync(string msCB, Staff staff);

    }
}
