using Dapper;
using StudentServicePortal.Models;
using StudentServicePortal.Repositories.Interfaces;
using System.Data;

public class RegistrationDetailRepository : IRegistrationDetailRepository
{
    private readonly IDbConnection _dbConnection;

    public RegistrationDetailRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<RegistrationDetail>> GetDetailsByFormIdAsync(string maDon)
    {
        string query = "SELECT * FROM DON_DANG_KY_CHI_TIET WHERE MaDon = @MaDon";
        return await _dbConnection.QueryAsync<RegistrationDetail>(query, new { MaDon = maDon });
    }
    private const string UPDATE_STATUS_BY_MADON = @"
        UPDATE DON_DANG_KY_CHI_TIET 
        SET TrangThaiXuLy = @TrangThaiXuLy 
        WHERE MaDon = @MaDon";
    private const string UPDATE_STATUS_IN_FORM = @"
        UPDATE DON_DANG_KY 
        SET TrangThai = @TrangThaiXuLy 
        WHERE MaDon = @MaDon";

    public async Task<bool> UpdateStatusByMaDonAsync(string maDon, string newStatus)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@MaDon", maDon);
        parameters.Add("@TrangThaiXuLy", newStatus);

        var rowsAffectedDetail = await _dbConnection.ExecuteAsync(UPDATE_STATUS_BY_MADON, parameters);
        var rowsAffectedForm = await _dbConnection.ExecuteAsync(UPDATE_STATUS_IN_FORM, parameters);

        return rowsAffectedDetail > 0 && rowsAffectedForm > 0;
    }

    public async Task AddAsync(RegistrationDetail detail)
    {
        const string sql = @"
INSERT INTO DON_DANG_KY_CHI_TIET
    (MaDonCT, MaDon, MaSV, HocKyHienTai, NgayTaoDonCT, ThongTinChiTiet, TrangThaiXuLy)
VALUES
    (@MaDonCT, @MaDon, @MaSV, @HocKyHienTai, @NgayTaoDonCT, @ThongTinChiTiet, @TrangThaiXuLy)";

        await _dbConnection.ExecuteAsync(sql, new
        {
            detail.MaDonCT,
            detail.MaDon,
            detail.MaSV,
            detail.HocKyHienTai,
            detail.NgayTaoDonCT,
            detail.ThongTinChiTiet,
            detail.TrangThaiXuLy
        });
    }
    public async Task<RegistrationDetail> GetByIdAsync(string maDonCT)
    {
        const string sql = @"
SELECT MaDonCT, MaDon, MaSV, HocKyHienTai, NgayTaoDonCT, ThongTinChiTiet, TrangThaiXuLy
FROM DON_DANG_KY_CHI_TIET
WHERE MaDonCT = @MaDonCT";

        return await _dbConnection.QuerySingleOrDefaultAsync<RegistrationDetail>(sql, new { MaDonCT = maDonCT });
    }
}
