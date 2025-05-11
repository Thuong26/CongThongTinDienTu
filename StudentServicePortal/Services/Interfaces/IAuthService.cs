using System.Security.Claims;
using StudentServicePortal.Models;

public interface IAuthService
{
    Task<(string AccessToken, string RefreshToken)?> LoginAsync(string username, string password);
    Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken);
    Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword);
    Task<string?> GeneratePasswordResetTokenAsync(string email);
    Task<string?> ValidateOtpToken(string otpToken);
    Task<string?> GetEmailFromOtpToken(string otpToken);
    Task<bool> ResetPasswordAsync(string newPassword);
    Task<bool> VerifyOtpTokenAsync(string token);
    Task<UserInfo?> GetUserInfoAsync(string username);
}
