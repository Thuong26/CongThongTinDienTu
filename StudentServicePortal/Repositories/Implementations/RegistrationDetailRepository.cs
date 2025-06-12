using Dapper;
using Microsoft.Extensions.Configuration;
using StudentServicePortal.Models;
using StudentServicePortal.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace StudentServicePortal.Repositories.Implementations
{
    public class RegistrationDetailRepository : IRegistrationDetailRepository
    {
        private readonly IDbConnection _connection;

        public RegistrationDetailRepository(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task<IEnumerable<RegistrationDetail>> GetAllDetailsAsync()
        {
            const string sql = @"
                SELECT 
                    MaDonCT,
                    MaDon,
                    MaSV,
                    HocKyHienTai,
                    ThongTinChiTiet,
                    NgayTaoDonCT,
                    TrangThaiXuLy
                FROM DON_DANG_KY_CHI_TIET
                ORDER BY NgayTaoDonCT DESC";

            return await _connection.QueryAsync<RegistrationDetail>(sql);
        }

        public async Task<RegistrationDetail> GetDetailByIdAsync(string maDonCT)
        {
            const string sql = @"
                SELECT 
                    MaDonCT,
                    MaDon,
                    MaSV,
                    HocKyHienTai,
                    ThongTinChiTiet,
                    NgayTaoDonCT,
                    TrangThaiXuLy
                FROM DON_DANG_KY_CHI_TIET
                WHERE MaDonCT = @MaDonCT";

            return await _connection.QueryFirstOrDefaultAsync<RegistrationDetail>(sql, new { MaDonCT = maDonCT });
        }

        public async Task<RegistrationDetail> AddDetailAsync(RegistrationDetail detail)
        {
            const string sql = @"
                INSERT INTO DON_DANG_KY_CHI_TIET (
                    MaDonCT,
                    MaDon,
                    MaSV,
                    HocKyHienTai,
                    ThongTinChiTiet,
                    NgayTaoDonCT,
                    TrangThaiXuLy
                ) VALUES (
                    @MaDonCT,
                    @MaDon,
                    @MaSV,
                    @HocKyHienTai,
                    @ThongTinChiTiet,
                    @NgayTaoDonCT,
                    @TrangThaiXuLy
                )";

            await _connection.ExecuteAsync(sql, detail);
            return detail;
        }

        public async Task<RegistrationDetail> UpdateDetailAsync(RegistrationDetail detail)
        {
            const string sql = @"
                UPDATE DON_DANG_KY_CHI_TIET
                SET 
                    MaDon = @MaDon,
                    MaSV = @MaSV,
                    HocKyHienTai = @HocKyHienTai,
                    ThongTinChiTiet = @ThongTinChiTiet,
                    NgayTaoDonCT = @NgayTaoDonCT,
                    TrangThaiXuLy = @TrangThaiXuLy
                WHERE MaDonCT = @MaDonCT";

            await _connection.ExecuteAsync(sql, detail);
            return detail;
        }

        public async Task<bool> DeleteDetailAsync(string maDonCT)
        {
            const string sql = "DELETE FROM DON_DANG_KY_CHI_TIET WHERE MaDonCT = @MaDonCT";
            var rowsAffected = await _connection.ExecuteAsync(sql, new { MaDonCT = maDonCT });
            return rowsAffected > 0;
        }

        public async Task<RegistrationDetail> GetLastDetailAsync()
        {
            const string sql = @"
                SELECT TOP 1 
                    MaDonCT,
                    MaDon,
                    MaSV,
                    HocKyHienTai,
                    ThongTinChiTiet,
                    NgayTaoDonCT,
                    TrangThaiXuLy
                FROM DON_DANG_KY_CHI_TIET
                ORDER BY MaDonCT DESC";

            return await _connection.QueryFirstOrDefaultAsync<RegistrationDetail>(sql);
        }

        public async Task<IEnumerable<RegistrationDetail>> GetDetailsByFormIdAsync(string maDon)
        {
            const string sql = @"
                SELECT 
                    MaDonCT,
                    MaDon,
                    MaSV,
                    HocKyHienTai,
                    ThongTinChiTiet,
                    NgayTaoDonCT,
                    TrangThaiXuLy
                FROM DON_DANG_KY_CHI_TIET
                WHERE MaDon = @MaDon
                ORDER BY NgayTaoDonCT DESC";

            return await _connection.QueryAsync<RegistrationDetail>(sql, new { MaDon = maDon });
        }

        public async Task<IEnumerable<RegistrationDetailWithStudentInfo>> GetDetailsByFormIdWithStudentInfoAsync(string maDon)
        {
            const string sql = @"
                SELECT 
                    ddkct.MaDonCT,
                    ddkct.MaDon,
                    ddk.TenDon,
                    ddkct.MaSV,
                    ddkct.HocKyHienTai,
                    ddkct.NgayTaoDonCT,
                    ddkct.ThongTinChiTiet,
                    ddkct.TrangThaiXuLy,
                    sv.HoTen,
                    sv.Lop,
                    sv.Khoa,
                    sv.Email,
                    sv.ChuyenNganh,
                    sv.KhoaHoc
                FROM DON_DANG_KY_CHI_TIET ddkct
                LEFT JOIN SINH_VIEN sv ON ddkct.MaSV = sv.MaSV
                LEFT JOIN DON_DANG_KY ddk ON ddkct.MaDon = ddk.MaDon
                WHERE ddkct.MaDon = @MaDon
                ORDER BY ddkct.NgayTaoDonCT DESC";

            return await _connection.QueryAsync<RegistrationDetailWithStudentInfo>(sql, new { MaDon = maDon });
        }

        public async Task<RegistrationDetailWithStudentInfo> GetDetailByIdWithStudentInfoAsync(string maDonCT)
        {
            const string sql = @"
                SELECT 
                    ddkct.MaDonCT,
                    ddkct.MaDon,
                    ddk.TenDon,
                    ddkct.MaSV,
                    ddkct.HocKyHienTai,
                    ddkct.NgayTaoDonCT,
                    ddkct.ThongTinChiTiet,
                    ddkct.TrangThaiXuLy,
                    sv.HoTen,
                    sv.Lop,
                    sv.Khoa,
                    sv.Email,
                    sv.ChuyenNganh,
                    sv.KhoaHoc
                FROM DON_DANG_KY_CHI_TIET ddkct
                LEFT JOIN SINH_VIEN sv ON ddkct.MaSV = sv.MaSV
                LEFT JOIN DON_DANG_KY ddk ON ddkct.MaDon = ddk.MaDon
                WHERE ddkct.MaDonCT = @MaDonCT";

            return await _connection.QueryFirstOrDefaultAsync<RegistrationDetailWithStudentInfo>(sql, new { MaDonCT = maDonCT });
        }

        public async Task<bool> UpdateStatusByMaDonAsync(string maDon, string newStatus)
        {
            const string sql = @"
                UPDATE DON_DANG_KY_CHI_TIET
                SET TrangThaiXuLy = @TrangThaiXuLy
                WHERE MaDon = @MaDon";

            var rowsAffected = await _connection.ExecuteAsync(sql, new { MaDon = maDon, TrangThaiXuLy = newStatus });
            return rowsAffected > 0;
        }

        public async Task AddAsync(RegistrationDetail detail)
        {
            const string sql = @"
                INSERT INTO DON_DANG_KY_CHI_TIET (
                    MaDonCT,
                    MaDon,
                    MaSV,
                    HocKyHienTai,
                    ThongTinChiTiet,
                    NgayTaoDonCT,
                    TrangThaiXuLy
                ) VALUES (
                    @MaDonCT,
                    @MaDon,
                    @MaSV,
                    @HocKyHienTai,
                    @ThongTinChiTiet,
                    @NgayTaoDonCT,
                    @TrangThaiXuLy
                )";

            await _connection.ExecuteAsync(sql, detail);
        }
    }
}
