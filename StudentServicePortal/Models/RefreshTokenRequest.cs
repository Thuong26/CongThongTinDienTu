using System.ComponentModel.DataAnnotations;

namespace StudentServicePortal.Models
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Refresh token không được để trống")]
        public string RefreshToken { get; set; }
    }
}
