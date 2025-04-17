using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentServicePortal.Models
{
    [Table("GIOI_THIEU")]
    public class Introduction
    {
        [Key]
        [Column("MaQL")]
        public string ManagerId { get; set; }  // Link to QUAN_LY (Manager)

        [Required]
        [Column("TieuDe")]
        public string Title { get; set; }  // Title

        [Column("NoiDung")]
        public string? Content { get; set; }  // Content of the introduction

        [Column("HinhAnh")]
        public string? ImageLink { get; set; }  // Image link

        [Column("ThongTinLienHe")]
        public string? ContactInformation { get; set; }  // JSON string for contact information
    }
}
