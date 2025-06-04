using System.Collections.Generic;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace StudentServicePortal.Models
{
    public class ContactInfo
    {
        [SwaggerSchema(Description = "Tên phòng ban")]
        [JsonPropertyName("department")]
        public string Department { get; set; }
        
        [SwaggerSchema(Description = "Danh sách số điện thoại của phòng ban")]
        [JsonPropertyName("phones")]
        public List<string> Phones { get; set; }
        
        [SwaggerSchema(Description = "Email liên hệ của phòng ban")]
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
} 