using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentServicePortal.Models
{
    [Table("CAN_BO")]
    public class Staff
    {
        [Key]
        [Column("MaCB")]
        public string MSCB { get; set; }

        public string MaPB { get; set; }
        public string MaQL { get; set; }

        [Column("Matkhau")]
        public byte[] Matkhau { get; set; }
    }
    public class StaffDTO // Dành cho public API
    {
        [Key]
        [Column("MaCB")]
        public string MSCB { get; set; }
        public string MaPB { get; set; }
        public string MaQL { get; set; }
    }
}
