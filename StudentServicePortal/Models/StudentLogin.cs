using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentServicePortal.Models
{
    [Table("DANG_NHAP_SV")]
    public class StudentLogin
    {
        [Key]
        [Column("MaSV")]
        public string MSSV { get; set; }

        [Column("Matkhau")]
        public byte[] Matkhau { get; set; }
    }
}

