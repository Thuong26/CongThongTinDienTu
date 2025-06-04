using Dapper;
using StudentServicePortal.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace StudentServicePortal.Repositories
{
    public class RegulationRepository : IRegulationRepository
    {
        private readonly IDbConnection _dbConnection;

        public RegulationRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        private const string GET_ALL_REGULATIONS = @"SELECT qd.*, pb.TenPB FROM QUY_DINH qd LEFT JOIN PHONG_BAN pb ON qd.MaPB = pb.MaPB";

        public async Task<IEnumerable<Regulation>> GetAllRegulations()
        {
            return await _dbConnection.QueryAsync<Regulation>(GET_ALL_REGULATIONS);
        }

        private const string GET_REGULATION_BY_ID = @"SELECT qd.*, pb.TenPB FROM QUY_DINH qd LEFT JOIN PHONG_BAN pb ON qd.MaPB = pb.MaPB WHERE qd.MaQD = @MaQD";

        public async Task<Regulation?> GetRegulationById(string maQD)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@MaQD", maQD);
            return await _dbConnection.QueryFirstOrDefaultAsync<Regulation>(GET_REGULATION_BY_ID, parameters);
        }

        private const string GET_REGULATIONS_BY_DEPARTMENT = @"
            SELECT qd.*, pb.TenPB 
            FROM QUY_DINH qd 
            LEFT JOIN PHONG_BAN pb ON qd.MaPB = pb.MaPB 
            WHERE qd.MaPB = @MaPB 
            ORDER BY qd.ThoiGianDang DESC";

        public async Task<IEnumerable<Regulation>> GetRegulationsByDepartment(string maPB)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@MaPB", maPB);
            return await _dbConnection.QueryAsync<Regulation>(GET_REGULATIONS_BY_DEPARTMENT, parameters);
        }

        private const string INSERT = @"
        INSERT INTO QUY_DINH (
            MaQD, TenQD, MaCB, MaPB, LienKet, LoaiVanBan, 
            NoiBanHanh, NgayBanHanh, NgayCoHieuLuc, HieuLuc, ThoiGianDang
        )
        VALUES (
            @MaQD, @TenQD, @MaCB, @MaPB, @LienKet, @LoaiVanBan, 
            @NoiBanHanh, @NgayBanHanh, @NgayCoHieuLuc, @HieuLuc, @ThoiGianDang
        )";
        public async Task<bool> CreateAsync(Regulation regulation)
        {
            var rows = await _dbConnection.ExecuteAsync(INSERT, regulation);
            return rows > 0;
        }
        private const string UPDATE = @"
        UPDATE QUY_DINH SET
            TenQD = @TenQD,
            MaCB = @MaCB,
            MaPB = @MaPB,
            LienKet = @LienKet,
            LoaiVanBan = @LoaiVanBan,
            NoiBanHanh = @NoiBanHanh,
            NgayBanHanh = @NgayBanHanh,
            NgayCoHieuLuc = @NgayCoHieuLuc,
            HieuLuc = @HieuLuc,
            ThoiGianDang = @ThoiGianDang
        WHERE MaQD = @MaQD";

        public async Task<bool> UpdateAsync(string maQD, Regulation regulation)
        {
            regulation.MaQD = maQD;
            var rows = await _dbConnection.ExecuteAsync(UPDATE, regulation);
            return rows > 0;
        }
    }
}
