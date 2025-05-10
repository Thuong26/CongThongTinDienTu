using Dapper;
using StudentServicePortal.Repositories.Interfaces;
using System.Data;

namespace StudentServicePortal.Repositories.Implementations
{
    // ReportRepository.cs
    public class ReportRepository : IReportRepository
    {
        private readonly IDbConnection _dbConnection;

        public ReportRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        private const string GET_REPORTS_SQL = @"
        SELECT 
            dk.MaDon, dk.TenDon, dk.MaPB, pb.TenPB,
            dk.MaCB, dk.MaQL,
            dk.ThoiGianDang, dk.TrangThai
        FROM DON_DANG_KY dk
        JOIN PHONG_BAN pb ON dk.MaPB = pb.MaPB";

        public async Task<IEnumerable<ReportDTO>> GetReportsAsync()
        {
            return await _dbConnection.QueryAsync<ReportDTO>(GET_REPORTS_SQL);
        }
    }

}
