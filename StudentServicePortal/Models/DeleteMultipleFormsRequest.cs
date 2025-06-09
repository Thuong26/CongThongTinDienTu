using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentServicePortal.Models
{
    public class DeleteMultipleFormsRequest
    {
        [Required(ErrorMessage = "Danh sách mã biểu mẫu không được rỗng")]
        public IEnumerable<string> MaBMList { get; set; }
    }
} 