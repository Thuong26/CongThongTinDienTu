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
        public async Task<RegistrationDetail> CreateAsync(RegistrationDetail detail)
        {
            // Gán ngày tạo và trạng thái mặc định
            detail.NgayTaoDonCT = DateTime.UtcNow;
            detail.TrangThaiXuLy = detail.TrangThaiXuLy ?? "Đang xử lý";

            await _repository.AddAsync(detail);
            return detail;
        }
        public async Task<RegistrationDetail> GetByIdAsync(string maDonCT)
        {
            return await _repository.GetByIdAsync(maDonCT);
        }
    }
}
