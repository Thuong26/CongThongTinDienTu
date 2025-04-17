using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentServicePortal.Models
{
    [Table("PHONG_BAN")]
    public class Department
    {
        [Key]
        [Column("MaPB")]
        public string MaPB { get; set; }

        [Column("TenPB")]
        public string TenPB { get; set; }
    }
}
