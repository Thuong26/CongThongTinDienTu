using Dapper;
using StudentServicePortal.Models;
using StudentServicePortal.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace StudentServicePortal.Repositories
{
    public class RegistrationFormRepository : IRegistrationFormRepository
    {
        private readonly IDbConnection _dbConnection;

        public RegistrationFormRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        private const string GET_ALL_FORMS = @"
        SELECT
            ddk.MaDon             AS MaDon,
            ddk.MaPB              AS MaPB,
            ddk.TenDon            AS TenDon,
            ddk.MaCB              AS MaCB,
            ddk.MaQL              AS MaQL,
            ddk.ThongTinChiTiet   AS ThongTinChiTiet,
            ddk.ThoiGianDang      AS ThoiGianDang,
            ddk.TrangThai         AS TrangThai,
            pb.TenPB              AS TenPB
        FROM [dbo].[DON_DANG_KY] ddk
        LEFT JOIN [dbo].[PHONG_BAN] pb ON ddk.MaPB = pb.MaPB";
        private const string GET_FORM_BY_ID = @"
        SELECT 
            MaDonCT, MaDon, MaSV, HocKyHienTai, 
            NgayTaoDonCT, ThongTinChiTiet, TrangThaiXuLy 
        FROM [dbo].[DON_DANG_KY_CHI_TIET] 
        WHERE MaDon = @MaDon";
        private const string GET_FORM_ID = @"
        SELECT 
            ddk.*,
            pb.TenPB              AS TenPB
        FROM [dbo].[DON_DANG_KY] ddk
        LEFT JOIN [dbo].[PHONG_BAN] pb ON ddk.MaPB = pb.MaPB
        WHERE ddk.MaDon = @MaDon";
        private const string INSERT_FORM = @"
        INSERT INTO [dbo].[DON_DANG_KY] (
            MaDon, MaPB, TenDon, MaCB, MaQL, 
            ThongTinChiTiet, ThoiGianDang, TrangThai
        ) VALUES (
            @MaDon, @MaPB, @TenDon, @MaCB, @MaQL, 
            @ThongTinChiTiet, @ThoiGianDang, @TrangThai
        )";

        public async Task AddFormAsync(RegistrationForm form)
        {
            // Thiết lập giá trị cho ThoiGianDang là thời gian hệ thống
            var parameters = new DynamicParameters(form);
            parameters.Add("ThoiGianDang", DateTime.Now); // Gán thời gian hệ thống

            await _dbConnection.ExecuteAsync(INSERT_FORM, parameters);
        }


        public async Task<IEnumerable<RegistrationForm>> GetAllFormsAsync()
        {
            return await _dbConnection.QueryAsync<RegistrationForm>(GET_ALL_FORMS);
        }
        public async Task<RegistrationDetail?> GetFormByIdAsync(string maDon)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@MaDon", maDon);

            return await _dbConnection.QueryFirstOrDefaultAsync<RegistrationDetail>(GET_FORM_BY_ID, parameters);
        }
        private const string GET_PENDING_FORMS = @"
        SELECT 
            ddk.*,
            pb.TenPB              AS TenPB
        FROM [dbo].[DON_DANG_KY] ddk
        LEFT JOIN [dbo].[PHONG_BAN] pb ON ddk.MaPB = pb.MaPB
        WHERE ddk.TrangThai = 1";
        public async Task<IEnumerable<RegistrationForm>> GetPendingFormsAsync()
        {
            return await _dbConnection.QueryAsync<RegistrationForm>(GET_PENDING_FORMS);
        }

        public async Task<RegistrationForm> GetByFormIdAsync(string maDon)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@MaDon", maDon);

            return await _dbConnection.QueryFirstOrDefaultAsync<RegistrationForm>(GET_FORM_ID, parameters);
        }
    }
}
