using System.ComponentModel.DataAnnotations;

namespace StudentServicePortal.Models
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

}
