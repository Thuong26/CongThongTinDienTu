using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentServicePortal.Models
{
    [Table("DOI_MAT_KHAU")]
    public class PasswordResetToken
    {
        [Key]
        [Column("IDDMK")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("MaSV")]
        public string MSSV { get; set; }

        [Required]
        [Column("Token")]
        public string Token { get; set; }

        [Required]
        [Column("ThoiGianHetHan")]
        public DateTime ThoiGianHetHan { get; set; }

        [Column("SuDung")]
        public bool SuDung { get; set; } = false;
    }
}
