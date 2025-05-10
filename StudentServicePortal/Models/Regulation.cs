using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentServicePortal.Models
{
    [Table("QUY_DINH")]
    public class Regulation
    {
        [Key]
        [Column("MaQD")]
        [StringLength(50)]  // Độ dài 50 ký tự cho MaQD
        public string MaQD { get; set; }

        [Column("TenQD")]
        [StringLength(255)]  // Độ dài 255 ký tự cho TenQD
        public string TenQD { get; set; }

        [Column("MaCB")]
        [StringLength(50)]  // Độ dài 50 ký tự cho MaCB
        public string MaCB { get; set; }

        [Column("MaPB")]
        [StringLength(50)]  // Độ dài 50 ký tự cho MaPB
        public string MaPB { get; set; }

        [Column("LienKet")]
        [StringLength(255)]  // Độ dài 255 ký tự cho LienKet
        public string LienKet { get; set; }

        [Column("LoaiVanBan")]
        [StringLength(100)]  // Độ dài 100 ký tự cho LoaiVanBan
        public string LoaiVanBan { get; set; }

        [Column("NoiBanHanh")]
        [StringLength(255)]  // Độ dài 255 ký tự cho NoiBanHanh
        public string NoiBanHanh { get; set; }

        [Column("NgayBanHanh")]
        public DateTime NgayBanHanh { get; set; }

        [Column("NgayCoHieuLuc")]
        public DateTime NgayCoHieuLuc { get; set; }

        [Column("HieuLuc")]
        public bool HieuLuc { get; set; }  // 1: Còn hiệu lực, 0: Hết hiệu lực

        [Column("ThoiGianDang")]
        public DateTime ThoiGianDang { get; set; }  // Thời điểm đăng

        
    }
}
