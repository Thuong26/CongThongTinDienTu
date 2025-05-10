using System.ComponentModel.DataAnnotations;

namespace StudentServicePortal.Models
{
    public class VerifyOtpRequest
    {
        [Required(ErrorMessage = "Mã OTP không được để trống")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải có đúng 6 ký tự")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Mã OTP phải là 6 chữ số")]
        public string Token { get; set; }
    }
} 