using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StudentServicePortal.Models
{
    [Table("GIOI_THIEU")]
    public class Introduction
    {
        [Key]
        [Column("MaQL")]
        [JsonIgnore]
        public string ManagerId { get; set; }  // Link to QUAN_LY (Manager)

        [Column("TieuDe")]
        [JsonPropertyName("tieuDe")]
        public string Title { get; set; }  // Title

        [Column("NoiDung")]
        [JsonPropertyName("noiDung")]
        public string Content { get; set; }  // Content of the introduction

        [Column("HinhAnh")]
        [JsonPropertyName("hinhAnh")]
        public string Image { get; set; }  // Image link

        [Column("ThongTinLienHe")]
        [JsonIgnore]
        public string ContactInfoJson { get; set; }  // JSON string for contact information

        // Thuộc tính không ánh xạ đến cơ sở dữ liệu
        [NotMapped]
        [JsonPropertyName("thongTinLienHe")]
        public List<ContactInfo> ContactInfo
        {
            get
            {
                if (string.IsNullOrEmpty(ContactInfoJson))
                    return new List<ContactInfo>();
                
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    return JsonSerializer.Deserialize<List<ContactInfo>>(ContactInfoJson, options) ?? new List<ContactInfo>();
                }
                catch (Exception ex)
                {
                    // Log error if needed
                    return new List<ContactInfo>();
                }
            }
        }
    }
}
