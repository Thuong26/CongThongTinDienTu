using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentServicePortal.Models
{
    [Table("QUAN_LY")]
    public class Manager
    {
        [Key]
        [Column("MaQL")]
        public string MSQL { get; set; }

        [Column("Matkhau")]
        public byte[] Matkhau { get; set; }
    }
}
