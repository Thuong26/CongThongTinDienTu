using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentServicePortal.Models
{
    [Table("DON_DANG_KY_CHI_TIET")]
    public class RegistrationDetail
    {
        [Key]
        [Column("MaDonCT")]
        [StringLength(50)]  // Độ dài 50 ký tự cho MaDonCT
        public string MaDonCT { get; set; }

        [Column("MaDon")]
        [StringLength(50)]  // Độ dài 50 ký tự cho MaDon
        public string MaDon { get; set; }

        [Column("MaSV")]
        [StringLength(20)]  // Độ dài 20 ký tự cho MaSV
        public string MaSV { get; set; }

        [Column("HocKyHienTai")]
        [StringLength(8)]  // Độ dài 8 ký tự cho HocKyHienTai (ví dụ: "Học kỳ 1")
        public string HocKyHienTai { get; set; }

        [Column("NgayTaoDonCT")]
        public DateTime NgayTaoDonCT { get; set; }  // Ngày tạo đơn chi tiết

        [Column("ThongTinChiTiet")]
        public string ThongTinChiTiet { get; set; }  // Lưu chuỗi JSON thông tin chi tiết

        [Column("TrangThaiXuLy")]
        [StringLength(50)]  // Độ dài 50 ký tự cho TrangThaiXuLy
        public string TrangThaiXuLy { get; set; }  // Trạng thái xử lý (mặc định: 'Đang xử lý')

        // Mối quan hệ với DON_DANG_KY (Đơn đăng ký)
        [ForeignKey("MaDon")]
        public virtual RegistrationForm RegistrationForm { get; set; }

        // Mối quan hệ với SINH_VIEN (Sinh viên)
        [ForeignKey("MaSV")]
        public virtual Student Student { get; set; }
    }
}
