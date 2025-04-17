using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentServicePortal.Models
{
    [Table("LICH_SU")]
    public class History
    {
        [Key]
        [Column("MaLichSu")]
        public int MaLichSu { get; set; }  // Mã lịch sử (khóa chính, tự tăng)

        [Column("MaSV")]
        [StringLength(20)]  // Độ dài 20 ký tự cho Mã Sinh viên
        public string MaSV { get; set; }  // Mã sinh viên

        [Column("MaBM")]
        [StringLength(50)]  // Độ dài 50 ký tự cho Mã Biểu mẫu
        public string MaBM { get; set; }  // Mã biểu mẫu

        // Mối quan hệ với bảng SINH_VIEN (Sinh viên)
        [ForeignKey("MaSV")]
        public virtual Student Student { get; set; }

        // Mối quan hệ với bảng BIEU_MAU (Biểu mẫu)
        [ForeignKey("MaBM")]
        public virtual Form Form { get; set; }
    }
}
