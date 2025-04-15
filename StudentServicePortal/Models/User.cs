namespace StudentServicePortal.Models
{
    public class User
    {
        public string Id { get; set; }
        public byte[] PasswordHash { get; set; }
    }
}
