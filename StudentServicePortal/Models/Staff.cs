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

        [Column("Matkhau")]
        public byte[] Matkhau { get; set; }
    }
}
