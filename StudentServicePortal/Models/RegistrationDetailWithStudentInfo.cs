using System;

namespace StudentServicePortal.Models
{
    public class RegistrationDetailWithStudentInfo
    {
        // Thông tin đơn đăng ký chi tiết
        public string MaDonCT { get; set; }
        public string MaDon { get; set; }
        public string TenDon { get; set; }
        public string MaSV { get; set; }
        public string HocKyHienTai { get; set; }
        public DateTime NgayTaoDonCT { get; set; }
        public string ThongTinChiTiet { get; set; }
        public string TrangThaiXuLy { get; set; }
        
        // Thông tin sinh viên
        public string HoTen { get; set; }
        public string Lop { get; set; }
        public string Khoa { get; set; }
        public string Email { get; set; }
        public string ChuyenNganh { get; set; }
        public string KhoaHoc { get; set; }
    }
} 