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
        [Required(ErrorMessage = "Mã đơn chi tiết không được để trống")]
        public string MaDonCT { get; set; }

        [Column("MaDon")]
        [StringLength(50)]  // Độ dài 50 ký tự cho MaDon
        [Required(ErrorMessage = "Mã đơn không được để trống")]
        public string MaDon { get; set; }

        [Column("MaSV")]
        [StringLength(20)]  // Độ dài 20 ký tự cho MaSV
        [Required(ErrorMessage = "Mã sinh viên không được để trống")]
        public string MaSV { get; set; }

        [Column("HocKyHienTai")]
        [StringLength(8)]  // Độ dài 8 ký tự cho HocKyHienTai (ví dụ: "Học kỳ 1")
        [Required(ErrorMessage = "Học kỳ hiện tại không được để trống")]
        public string HocKyHienTai { get; set; }

        [Column("NgayTaoDonCT")]
        public DateTime NgayTaoDonCT { get; set; }  // Ngày tạo đơn chi tiết

        [Column("ThongTinChiTiet")]
        [Required(ErrorMessage = "Thông tin chi tiết không được để trống")]
        public string ThongTinChiTiet { get; set; }  // Lưu chuỗi JSON thông tin chi tiết

        [Column("TrangThaiXuLy")]
        [StringLength(50)]  // Độ dài 50 ký tự cho TrangThaiXuLy
        public string TrangThaiXuLy { get; set; } = "Đang xử lý";
    }
}
