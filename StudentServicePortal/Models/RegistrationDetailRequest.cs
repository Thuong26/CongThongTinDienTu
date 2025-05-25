using System.ComponentModel.DataAnnotations;

namespace StudentServicePortal.Models
{
    public class RegistrationDetailRequest
    {
        [Required(ErrorMessage = "Mã đơn không được để trống")]
        [StringLength(50)]
        public string MaDon { get; set; }

        [Required(ErrorMessage = "Mã sinh viên không được để trống")]
        [StringLength(20)]
        public string MaSV { get; set; }

        [Required(ErrorMessage = "Học kỳ hiện tại không được để trống")]
        [StringLength(8)]
        public string HocKyHienTai { get; set; }

        [Required(ErrorMessage = "Thông tin chi tiết không được để trống")]
        public string ThongTinChiTiet { get; set; }
    }
} 