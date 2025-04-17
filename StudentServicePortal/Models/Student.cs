using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentServicePortal.Models
{
    [Table("SINH_VIEN")]
    public class Student
    {
        [Key]
        [Column("MaSV")]
        [StringLength(20)]
        public required string MSSV { get; set; }

        [Column("HoTen")]
        [StringLength(255)]
        public required string Hoten { get; set; }

        [Column("NgaySinh")]
        public DateTime Ngaysinh { get; set; }

        [Column("GioiTinh")]
        [StringLength(10)]
        public string? Gioitinh { get; set; }

        [Column("ChuyenNganh")]
        [StringLength(255)]
        public string? Chuyennganh { get; set; }

        [Column("TrinhDoDaoTao")]
        [StringLength(255)]
        public string? Trinhdohoctap { get; set; }

        [Column("HinhThucDaoTao")]
        [StringLength(255)]
        public string? Hinhthuchoctap { get; set; }

        [Column("Lop")]
        [StringLength(10)]
        public string? Lop { get; set; }

        [Column("Khoa")]
        [StringLength(50)]
        public string? Khoa { get; set; }

        [Column("KhoaHoc")]
        [StringLength(50)]
        public string? Khoahoc { get; set; }

        [Column("Email")]
        [StringLength(255)]
        public string? Email { get; set; }
    }
}
