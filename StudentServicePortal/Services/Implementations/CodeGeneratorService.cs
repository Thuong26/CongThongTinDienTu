using StudentServicePortal.Repositories.Interfaces;
using StudentServicePortal.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace StudentServicePortal.Services.Implementations
{
    public class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly IRegistrationDetailRepository _repository;

        public CodeGeneratorService(IRegistrationDetailRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<string> GenerateMaDonCTAsync()
        {
            // Lấy mã đơn chi tiết cuối cùng
            var lastDetail = await _repository.GetLastDetailAsync();
            string newMaDonCT;

            if (lastDetail == null)
            {
                // Nếu chưa có đơn nào, bắt đầu từ DCT001
                newMaDonCT = "DCT001";
            }
            else
            {
                // Lấy số từ mã đơn cuối và tăng lên 1
                string lastNumber = lastDetail.MaDonCT.Substring(3);
                int nextNumber = int.Parse(lastNumber) + 1;
                newMaDonCT = $"DCT{nextNumber:D3}";
            }

            return newMaDonCT;
        }
    }
} 