using StudentServicePortal.Repositories.Interfaces;
using StudentServicePortal.Services.Interfaces;
using StudentServicePortal.Repositories;
using System;
using System.Threading.Tasks;

namespace StudentServicePortal.Services.Implementations
{
    public class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly IRegistrationDetailRepository _registrationDetailRepository;
        private readonly IRegulationRepository _regulationRepository;

        public CodeGeneratorService(IRegistrationDetailRepository registrationDetailRepository, IRegulationRepository regulationRepository)
        {
            _registrationDetailRepository = registrationDetailRepository ?? throw new ArgumentNullException(nameof(registrationDetailRepository));
            _regulationRepository = regulationRepository ?? throw new ArgumentNullException(nameof(regulationRepository));
        }

        public async Task<string> GenerateMaDonCTAsync()
        {
            // Lấy mã đơn chi tiết cuối cùng
            var lastDetail = await _registrationDetailRepository.GetLastDetailAsync();
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

        public async Task<string> GenerateMaQDAsync()
        {
            // Lấy mã quy định cuối cùng
            var lastRegulation = await _regulationRepository.GetLastRegulationAsync();
            string newMaQD;

            if (lastRegulation == null)
            {
                // Nếu chưa có quy định nào, bắt đầu từ QD001
                newMaQD = "QD001";
            }
            else
            {
                // Lấy số từ mã quy định cuối và tăng lên 1
                string lastNumber = lastRegulation.MaQD.Substring(2);
                int nextNumber = int.Parse(lastNumber) + 1;
                newMaQD = $"QD{nextNumber:D3}";
            }

            return newMaQD;
        }
    }
} 