using StudentServicePortal.Models;
using StudentServicePortal.Repositories.Interfaces;
using StudentServicePortal.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Services.Implementations
{
    public class RegistrationDetailService : IRegistrationDetailService
    {
        private readonly IRegistrationDetailRepository _repository;

        public RegistrationDetailService(IRegistrationDetailRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IEnumerable<RegistrationDetail>> GetAllDetailsAsync()
        {
            return await _repository.GetAllDetailsAsync();
        }

        public async Task<RegistrationDetail> GetDetailByIdAsync(string maDonCT)
        {
            if (string.IsNullOrEmpty(maDonCT))
                throw new ArgumentException("Mã đơn chi tiết không được để trống", nameof(maDonCT));

            return await _repository.GetDetailByIdAsync(maDonCT);
        }

        public async Task<RegistrationDetail> AddDetailAsync(RegistrationDetail detail)
        {
            if (detail == null)
                throw new ArgumentNullException(nameof(detail));

            return await _repository.AddDetailAsync(detail);
        }

        public async Task<RegistrationDetail> UpdateDetailAsync(RegistrationDetail detail)
        {
            if (detail == null)
                throw new ArgumentNullException(nameof(detail));

            return await _repository.UpdateDetailAsync(detail);
        }

        public async Task<bool> DeleteDetailAsync(string maDonCT)
        {
            if (string.IsNullOrEmpty(maDonCT))
                throw new ArgumentException("Mã đơn chi tiết không được để trống", nameof(maDonCT));

            return await _repository.DeleteDetailAsync(maDonCT);
        }

        public async Task<IEnumerable<RegistrationDetail>> GetDetailsByFormIdAsync(string maDon)
        {
            if (string.IsNullOrEmpty(maDon))
                throw new ArgumentException("Mã đơn không được để trống", nameof(maDon));

            return await _repository.GetDetailsByFormIdAsync(maDon);
        }

        public async Task<bool> UpdateStatusByMaDonAsync(string maDon, string newStatus)
        {
            if (string.IsNullOrEmpty(maDon))
                throw new ArgumentException("Mã đơn không được để trống", nameof(maDon));
            if (string.IsNullOrEmpty(newStatus))
                throw new ArgumentException("Trạng thái mới không được để trống", nameof(newStatus));

            return await _repository.UpdateStatusByMaDonAsync(maDon, newStatus);
        }

        public async Task<IEnumerable<RegistrationDetailWithStudentInfo>> GetDetailsByFormIdWithStudentInfoAsync(string maDon)
        {
            if (string.IsNullOrEmpty(maDon))
                throw new ArgumentException("Mã đơn không được để trống", nameof(maDon));

            return await _repository.GetDetailsByFormIdWithStudentInfoAsync(maDon);
        }

        public async Task<RegistrationDetailWithStudentInfo> GetDetailByIdWithStudentInfoAsync(string maDonCT)
        {
            if (string.IsNullOrEmpty(maDonCT))
                throw new ArgumentException("Mã đơn chi tiết không được để trống", nameof(maDonCT));

            return await _repository.GetDetailByIdWithStudentInfoAsync(maDonCT);
        }

        public async Task<string> GenerateMaDonCTAsync()
        {
            var lastDetail = await _repository.GetLastDetailAsync();
            if (lastDetail == null)
                return "DCT001";

            var lastNumber = int.Parse(lastDetail.MaDonCT.Substring(3));
            return $"DCT{(lastNumber + 1).ToString("D3")}";
        }
    }
}
