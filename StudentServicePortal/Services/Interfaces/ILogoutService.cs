namespace StudentServicePortal.Services.Interfaces
{
    public interface ILogoutService
    {
        void RevokeToken(string jti);
        bool IsTokenRevoked(string jti);
    }
}
