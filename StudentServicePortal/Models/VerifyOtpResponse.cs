namespace StudentServicePortal.Models
{
    public class VerifyOtpResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public bool IsValid { get; set; }
        public int StatusCode { get; set; }
    }
} 