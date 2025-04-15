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
        public required string MSSV { get; set; }

        [Column("HoTen")]
        public required string Hoten { get; set; }

        [Column("NgaySinh")]
        public DateTime Ngaysinh { get; set; }

        [Column("GioiTinh")]
        public string? Gioitinh { get; set; }

        [Column("ChuyenNganh")]
        public string? Chuyennganh { get; set; }

        [Column("TrinhDoHocTap")]
        public string? Trinhdohoctap { get; set; }

        [Column("HinhThucHocTap")]
        public string? Hinhthuchoctap { get; set; }

        [Column("Lop")]
        public string? Lop { get; set; }

        [Column("Khoa")]
        public string? Khoa { get; set; }

        [Column("KhoaHoc")]
        public string? Khoahoc { get; set; }

        [Column("Email")]
        public string? Email { get; set; }
    }
}
