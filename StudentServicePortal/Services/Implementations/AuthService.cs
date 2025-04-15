using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudentServicePortal.Configurations;
using StudentServicePortal.Data;
using StudentServicePortal.Models;
using StudentServicePortal.Services.Interfaces;



namespace StudentServicePortal.Services
{
    public class AuthService : IAuthService
    {
        private readonly StudentPortalDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(StudentPortalDbContext context, IConfiguration configuration, IOptions<JwtSettings> jwtOptions)
        {
            _context = context;
            _configuration = configuration;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<(string AccessToken, string RefreshToken)?> LoginAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null) return null;

            var hashedPassword = HashPassword(password);
            string userType = null;

            if (user is StudentLogin student)
            {
                if (hashedPassword.Length == student.Matkhau.Length &&
                    hashedPassword.SequenceEqual(student.Matkhau))
                    userType = "Student";
            }
            else if (user is Staff staff)
            {
                if (hashedPassword.Length == staff.Matkhau.Length &&
                    hashedPassword.SequenceEqual(staff.Matkhau))
                    userType = "Staff";
            }
            else if (user is Manager manager)
            {
                if (hashedPassword.Length == manager.Matkhau.Length &&
                    hashedPassword.SequenceEqual(manager.Matkhau))
                    userType = "Manager";
            }

            if (userType == null) return null;

            string accessToken = GenerateJwtToken(username, userType);
            string refreshToken = GenerateRefreshToken(username, userType);

            return (accessToken, refreshToken);
        }
        public async Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword)
        {
            if (!IsValidPassword(newPassword)) throw new Exception("Mật khẩu không đủ mạnh.");

            var user = await GetUserByUsernameAsync(username);
            if (user == null)
            {
                Console.WriteLine($"Không tìm thấy người dùng với username: {username}");
                return false;
            }

            var oldHashed = HashPassword(oldPassword);
            var newHashed = HashPassword(newPassword);

            if (user is StudentLogin student)
            {
                if (!oldHashed.SequenceEqual(student.Matkhau)) return false;
                await _context.StudentLogins
                    .Where(u => u.MSSV == username)
                    .ExecuteUpdateAsync(u => u.SetProperty(p => p.Matkhau, newHashed));
            }
            else if (user is Staff staff)
            {
                if (!oldHashed.SequenceEqual(staff.Matkhau)) return false;
                await _context.Staffs
                    .Where(u => u.MSCB == username)
                    .ExecuteUpdateAsync(u => u.SetProperty(p => p.Matkhau, newHashed));
            }
            else if (user is Manager manager)
            {
                if (!oldHashed.SequenceEqual(manager.Matkhau)) return false;
                await _context.Managers
                    .Where(u => u.MSQL == username)
                    .ExecuteUpdateAsync(u => u.SetProperty(p => p.Matkhau, newHashed));
            }

            return true;
        }


        public async Task<string?> GeneratePasswordResetTokenAsync(string email)
        {
            try
            {
                // Kiểm tra _context
                if (_context == null)
                {
                    Console.WriteLine("_context là null");
                    return null;
                }

                // Kiểm tra email có hợp lệ không
                if (!IsValidEmail(email))
                {
                    Console.WriteLine("Email không hợp lệ");
                    return null;
                }

                // Kiểm tra bảng SINH_VIEN có tồn tại không
                if (_context.Students == null)
                {
                    Console.WriteLine("DbSet SINH_VIEN là null");
                    return null;
                }

                var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
                if (student == null)
                {
                    Console.WriteLine("Không tìm thấy sinh viên với email này");
                    return null;
                }

                Console.WriteLine($"Tìm thấy sinh viên: MaSV={student.MSSV}, Email={student.Email}");

                if (_context.PasswordResetTokens == null)
                {
                    Console.WriteLine("DbSet PasswordResetTokens là null");
                    return null;
                }

                var otpToken = GenerateOtpToken();
                Console.WriteLine($"Đã tạo OTP: {otpToken}");

                var resetToken = new PasswordResetToken
                {
                    MSSV = student.MSSV,
                    Token = otpToken,
                    ThoiGianHetHan = DateTime.UtcNow.AddMinutes(10),
                    SuDung = false
                };

                await _context.PasswordResetTokens.AddAsync(resetToken);
                await _context.SaveChangesAsync();
                Console.WriteLine("Đã lưu token vào DB");

                if (_emailService == null)
                {
                    Console.WriteLine("_emailService là null");
                    return otpToken;
                }

                await _emailService.SendEmailAsync(email, "Mã đặt lại mật khẩu",
                    $"<p>Mã OTP của bạn là: <b>{otpToken}</b></p><p>OTP sẽ hết hạn sau 10 phút.</p>");
                Console.WriteLine("Đã gửi email");

                return otpToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong GeneratePasswordResetTokenAsync: {ex}");
                throw new Exception($"Lỗi hệ thống khi tạo mã đặt lại mật khẩu. Chi tiết: {ex.Message}");
            }
        }

        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }

        private string GenerateOtpToken()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private async Task<object?> GetUserByUsernameAsync(string username)
        {
            if (username.StartsWith("211"))
                return await _context.StudentLogins.FirstOrDefaultAsync(u => u.MSSV == username);
            if (username.StartsWith("CB"))
                return await _context.Staffs.FirstOrDefaultAsync(u => u.MSCB == username);
            return await _context.Managers.FirstOrDefaultAsync(u => u.MSQL == username);
        }

        private byte[] HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private string GenerateJwtToken(string username, string userType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimTypes.Name, username),
                new Claim("UserType", userType),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken(string username, string userType)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim("UserType", userType),
                new Claim("TokenId", Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            try
            {
                var principal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true
                }, out SecurityToken validatedToken);

                var username = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var userType = principal.Claims.FirstOrDefault(c => c.Type == "UserType")?.Value;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userType))
                {
                    return (null, null);
                }
                var newAccessToken = GenerateJwtToken(username, userType);

                return (newAccessToken, refreshToken);
            }
            catch (SecurityTokenExpiredException)
            {
                Console.WriteLine("Refresh token đã hết hạn");
                return (null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi xử lý refresh token: {ex.Message}");
                return (null, null);
            }
        }

        private bool IsValidPassword(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(c => "!@#$%^&*()-_=+[]{};:'\",.<>?/".Contains(c));
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            if (!IsValidPassword(newPassword))
                throw new Exception("Mật khẩu không đủ mạnh.");

            var tokenEntry = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == token && !t.SuDung && t.ThoiGianHetHan > DateTime.UtcNow);

            if (tokenEntry == null)
                throw new Exception("Token không hợp lệ hoặc đã hết hạn.");

            var student = await _context.StudentLogins.FirstOrDefaultAsync(s => s.MSSV == tokenEntry.MSSV);
            if (student == null)
                throw new Exception("Không tìm thấy người dùng tương ứng với token.");

            var hashedPassword = HashPassword(newPassword);

            student.Matkhau = hashedPassword;
            tokenEntry.SuDung = true;

            await _context.SaveChangesAsync();

            return true;
        }


        public Task<string> GeneratePasswordResetToken(string email)
        {
            throw new NotImplementedException();
        }

        public Task<string?> ValidateOtpToken(string otpToken)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetEmailFromOtpToken(string otpToken)
        {
            throw new NotImplementedException();
        }
    }
}
