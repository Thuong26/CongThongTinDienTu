using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentServicePortal.Models
{
    [Table("DON_DANG_KY")]
    public class RegistrationForm
    {
        [Key]
        [Column("MaDon")]
        [StringLength(50)]  // Độ dài 50 ký tự cho MaDon
        public string MaDon { get; set; }

        [Column("MaPB")]
        [StringLength(50)]  // Độ dài 50 ký tự cho MaPB
        public string MaPB { get; set; }

        [Column("TenDon")]
        [StringLength(255)]  // Độ dài 255 ký tự cho TenDon
        public string TenDon { get; set; }

        [Column("MaCB")]
        [StringLength(50)]  // Độ dài 50 ký tự cho MaCB
        public string MaCB { get; set; }

        [Column("MaQL")]
        [StringLength(50)]  // Độ dài 50 ký tự cho MaQL
        public string MaQL { get; set; }

        [Column("ThongTinChiTiet")]
        public string ThongTinChiTiet { get; set; }  // Lưu JSON chuỗi thông tin chi tiết của đơn

        [Column("ThoiGianDang")]
        public DateTime ThoiGianDang { get; set; }  // Thời gian đăng đơn

        [Column("TrangThai")]
        public bool TrangThai { get; set; }  // Trạng thái (1: Đang xử lý, 0: Hoàn thành)
    }
}
