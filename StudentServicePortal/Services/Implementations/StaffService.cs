using StudentServicePortal.Models;
using StudentServicePortal.Repositories;
using StudentServicePortal.Services;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;

    public StaffService(IStaffRepository staffRepository)
    {
        _staffRepository = staffRepository;
    }

    public async Task<StaffDTO?> GetProfileAsync(string maCB)
    {
        return await _staffRepository.GetByIdAsync(maCB);
    }
    public async Task<IEnumerable<StaffDTO>> GetAllStaffAsync()
    {
        return await _staffRepository.GetAllStaffAsync();
    }
    public async Task<bool> CreateStaffAsync(Staff staff)
    {
        return await _staffRepository.CreateStaffAsync(staff);
    }
    public async Task<bool> UpdateStaffAsync(string msCB, Staff staff)
    {
        return await _staffRepository.UpdateStaffAsync(msCB, staff);
    }

}
