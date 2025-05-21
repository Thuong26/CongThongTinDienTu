using System.Collections.Generic;
using Swashbuckle.AspNetCore.Annotations;

namespace StudentServicePortal.Models
{
    public class ContactInfo
    {
        [SwaggerSchema(Description = "Tên phòng ban")]
        public string Department { get; set; }
        
        [SwaggerSchema(Description = "Danh sách số điện thoại của phòng ban")]
        public List<string> Phones { get; set; }
        
        [SwaggerSchema(Description = "Email liên hệ của phòng ban")]
        public string Email { get; set; }
    }
} 