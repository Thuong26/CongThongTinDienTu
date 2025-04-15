using System.ComponentModel.DataAnnotations;

namespace StudentServicePortal.Models
{
    public class ForgotPasswordRequest
    {
        [Required]
        public string Email { get; set; }
    }

}
