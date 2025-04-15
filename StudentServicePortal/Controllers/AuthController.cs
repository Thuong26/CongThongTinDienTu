using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using StudentServicePortal.Models;
using StudentServicePortal.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace StudentServicePortal.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public AuthController(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Đăng nhập", Description = "Đăng nhập hệ thống bằng tài khoản sinh viên, cán bộ hoặc quản lý.")]
        [SwaggerResponse(200, "Đăng nhập thành công", typeof(LoginResponse))]
        [SwaggerResponse(400, "Thiếu thông tin hoặc sai định dạng")]
        [SwaggerResponse(401, "Sai tên đăng nhập hoặc mật khẩu")]
        [SwaggerResponse(500, "Lỗi hệ thống")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Thiếu thông tin hoặc sai định dạng");

            var result = await _authService.LoginAsync(request.Username, request.Password);
            if (result == null)
                return Unauthorized("Sai tên đăng nhập hoặc mật khẩu.");

            var (accessToken, refreshToken) = result.Value;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(accessToken);
            var userType = jwt.Claims.FirstOrDefault(c => c.Type == "UserType")?.Value;

            return Ok(new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserType = userType
            });
        }

        [Authorize]
        [HttpPost("refresh-token")]
        [SwaggerOperation(Summary = "Làm mới token", Description = "Sử dụng refresh token để lấy access token mới")]
        [SwaggerResponse(200, "Làm mới token thành công", typeof(RefreshTokenResponse))]
        [SwaggerResponse(400, "Thiếu thông tin hoặc sai định dạng")]
        [SwaggerResponse(401, "Refresh token không hợp lệ hoặc đã hết hạn")]
        [SwaggerResponse(500, "Lỗi hệ thống")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new
                {
                    Message = "Refresh token không được để trống"
                });
            }

            var result = await _authService.RefreshTokenAsync(request.RefreshToken);

            if (result.AccessToken == null)
            {
                return Unauthorized(new
                {
                    Message = "Refresh token không hợp lệ hoặc đã hết hạn."
                });
            }

            return Ok(new
            {
                AccessToken = result.AccessToken
            });
        }


        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var username = User.Identity?.Name;
            Console.WriteLine("Username: " + username);

            if (string.IsNullOrWhiteSpace(username))
                return Unauthorized("Không xác định được người dùng.");

            if (request.NewPassword != request.ConfirmNewPassword)
                return BadRequest("Mật khẩu xác nhận không khớp với mật khẩu mới.");

            var result = await _authService.ChangePasswordAsync(username, request.OldPassword, request.NewPassword);
            return result
                ? Ok("Đổi mật khẩu thành công.")
                : BadRequest("Mật khẩu cũ không chính xác hoặc có lỗi xảy ra.");
        }

        [HttpPost("forgot-password")]
        [SwaggerOperation(Summary = "Quên mật khẩu", Description = "Gửi mã OTP qua email để đặt lại mật khẩu.")]
        [SwaggerResponse(200, "Mã OTP đã được gửi qua email.")]
        [SwaggerResponse(400, "Email không hợp lệ hoặc không tồn tại")]
        [SwaggerResponse(500, "Lỗi hệ thống")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email không hợp lệ");

            var otpToken = await _authService.GeneratePasswordResetTokenAsync(request.Email);
            if (otpToken == null)
                return BadRequest("Email không hợp lệ hoặc không tồn tại.");

            try
            {
                await _emailService.SendEmailAsync(request.Email, "Mã đặt lại mật khẩu",
                    $"<p>Mã OTP của bạn là: <b>{otpToken}</b></p><p>OTP sẽ hết hạn sau 10 phút.</p>");
                return Ok("Mã OTP đã được gửi qua email.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi gửi email", error = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        [SwaggerOperation(Summary = "Đặt lại mật khẩu", Description = "Đặt lại mật khẩu bằng mã OTP được gửi qua email.")]
        [SwaggerResponse(200, "Đặt lại mật khẩu thành công.")]
        [SwaggerResponse(400, "Token không hợp lệ hoặc đã hết hạn")]
        [SwaggerResponse(500, "Lỗi hệ thống")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest("Thiếu thông tin hoặc sai định dạng");

            var success = await _authService.ResetPasswordAsync(request.Token, request.NewPassword);
            return success ? Ok("Đặt lại mật khẩu thành công.") : BadRequest("Token không hợp lệ hoặc đã hết hạn.");
        }
    }
}
