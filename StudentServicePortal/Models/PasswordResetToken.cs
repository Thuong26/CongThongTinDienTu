using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentServicePortal.Models
{
    [Table("DOI_MAT_KHAU")]
    public class PasswordResetToken
    {
        [Key]
        [Column("MaDMK")]
        public Guid MaDMK { get; set; }  // Mã Đổi mật khẩu (Uniqueidentifier trong SQL)

        [Column("MaSV")]
        [StringLength(20)]  // Độ dài 20 ký tự cho Mã Sinh viên
        public string MaSV { get; set; }  // Mã sinh viên (khóa ngoại)

        [Column("Token")]
        [StringLength(100)]  // Độ dài 100 ký tự cho mã OTP
        public string Token { get; set; }  // Mã OTP (dùng để xác thực)

        [Column("ThoiGianHetHan")]
        public DateTime ThoiGianHetHan { get; set; }  // Thời gian hết hạn của mã OTP

        [Column("SuDung")]
        public bool SuDung { get; set; }  // Token đã sử dụng hay chưa (0 hoặc 1)

        // Mối quan hệ với bảng SINH_VIEN (Sinh viên)
        [ForeignKey("MaSV")]
        public virtual Student Student { get; set; }
    }
}
