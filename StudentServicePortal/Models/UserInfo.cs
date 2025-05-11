using System.ComponentModel.DataAnnotations;

namespace StudentServicePortal.Models
{
    public class UserInfo
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string UserType { get; set; }
    }
} 