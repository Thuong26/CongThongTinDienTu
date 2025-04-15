namespace StudentServicePortal.Models
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string UserType { get; set; }
    }
}
