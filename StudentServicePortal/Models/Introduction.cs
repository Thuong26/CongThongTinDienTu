using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

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
        public string Content { get; set; }  // Content of the introduction

        [Column("HinhAnh")]
        public string Image { get; set; }  // Image link

        [Column("ThongTinLienHe")]
        public string ContactInfoJson { get; set; }  // JSON string for contact information

        // Thuộc tính không ánh xạ đến cơ sở dữ liệu
        [NotMapped]
        public List<ContactInfo> ContactInfo
        {
            get
            {
                if (string.IsNullOrEmpty(ContactInfoJson))
                    return new List<ContactInfo>();
                
                try
                {
                    return JsonSerializer.Deserialize<List<ContactInfo>>(ContactInfoJson);
                }
                catch
                {
                    return new List<ContactInfo>();
                }
            }
        }
    }
}
