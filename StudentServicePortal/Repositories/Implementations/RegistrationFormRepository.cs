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
            MaDon             AS MaDon,
            MaPB              AS MaPB,
            TenDon            AS TenDon,
            MaCB              AS MaCB,
            MaQL              AS MaQL,
            ThongTinChiTiet   AS ThongTinChiTiet,
            ThoiGianDang      AS ThoiGianDang,
            TrangThai         AS TrangThai
        FROM [dbo].[DON_DANG_KY]";
        private const string GET_FORM_BY_ID = @"
        SELECT 
            MaDonCT, MaDon, MaSV, HocKyHienTai, 
            NgayTaoDonCT, ThongTinChiTiet, TrangThaiXuLy 
        FROM [dbo].[DON_DANG_KY_CHI_TIET] 
        WHERE MaDon = @MaDon";
        private const string GET_FORM_ID = @"
        SELECT *
        FROM [dbo].[DON_DANG_KY] 
        WHERE MaDon = @MaDon";
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
            SELECT * FROM DON_DANG_KY
            WHERE TrangThai = 1";
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
