using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentServicePortal.Models
{
    [Table("BIEU_MAU")]
    public class Form
    {
        [Key]
        [Column("MaBM")]
        [StringLength(50)]  // Độ dài 50 ký tự cho MaBM
        public string MaBM { get; set; }

        [Column("MaCB")]
        [StringLength(50)]  // Độ dài 50 ký tự cho MaCB
        public string MaCB { get; set; }

        [Column("MaPB")]
        [StringLength(50)]  // Độ dài 50 ký tự cho MaPB
        public string MaPB { get; set; }

        [Column("TenBM")]
        [StringLength(255)]  // Độ dài 255 ký tự cho TenBM
        public string TenBM { get; set; }

        [Column("LienKet")]
        [StringLength(1000)]  // Độ dài 1000 ký tự cho LienKet
        public string LienKet { get; set; }

        [Column("ThoiGianDang")]
        public DateTime ThoiGianDang { get; set; }  // Thời gian đăng tải

        // Mối quan hệ với CAN_BO (Cán bộ)
        [ForeignKey("MaCB")]
        public virtual Manager? Manager { get; set; }

        // Mối quan hệ với PHONG_BAN (Phòng ban)
        [ForeignKey("MaPB")]
        public virtual Department? Department { get; set; }
    }
}
