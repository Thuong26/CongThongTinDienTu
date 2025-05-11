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
        [SwaggerResponse(200, "Đăng nhập thành công", typeof(ApiResponse<LoginResponse>))]
        [SwaggerResponse(400, "Thiếu thông tin hoặc sai định dạng", typeof(ApiResponse<object>))]
        [SwaggerResponse(401, "Sai tên đăng nhập hoặc mật khẩu", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Thiếu thông tin hoặc sai định dạng",
                    StatusCode = 400
                });

            var result = await _authService.LoginAsync(request.Username, request.Password);
            if (result == null)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Sai tên đăng nhập hoặc mật khẩu.",
                    StatusCode = 401
                });

            var (accessToken, refreshToken) = result.Value;
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(accessToken);
            var userType = jwt.Claims.FirstOrDefault(c => c.Type == "UserType")?.Value;

            return Ok(new ApiResponse<LoginResponse>
            {
                Success = true,
                Message = "Đăng nhập thành công",
                Data = new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    UserType = userType
                },
                StatusCode = 200
            });
        }

        [Authorize]
        [HttpPost("refresh-token")]
        [SwaggerOperation(Summary = "Làm mới token", Description = "Sử dụng refresh token để lấy access token mới")]
        [SwaggerResponse(200, "Làm mới token thành công", typeof(ApiResponse<RefreshTokenResponse>))]
        [SwaggerResponse(400, "Thiếu thông tin hoặc sai định dạng", typeof(ApiResponse<object>))]
        [SwaggerResponse(401, "Refresh token không hợp lệ hoặc đã hết hạn", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Refresh token không được để trống",
                    StatusCode = 400
                });
            }

            var result = await _authService.RefreshTokenAsync(request.RefreshToken);

            if (result.AccessToken == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Refresh token không hợp lệ hoặc đã hết hạn.",
                    StatusCode = 401
                });
            }

            return Ok(new ApiResponse<RefreshTokenResponse>
            {
                Success = true,
                Message = "Làm mới token thành công",
                Data = new RefreshTokenResponse
                {
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken
                },
                StatusCode = 200
            });
        }

        [Authorize]
        [HttpPost("change-password")]
        [SwaggerOperation(Summary = "Đổi mật khẩu", Description = "Đổi mật khẩu cho tài khoản đang đăng nhập")]
        [SwaggerResponse(200, "Đổi mật khẩu thành công", typeof(ApiResponse<object>))]
        [SwaggerResponse(400, "Thông tin không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(401, "Không xác định được người dùng", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Không xác định được người dùng.",
                    StatusCode = 401
                });

            if (request.NewPassword != request.ConfirmNewPassword)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Mật khẩu xác nhận không khớp với mật khẩu mới.",
                    StatusCode = 400
                });

            var result = await _authService.ChangePasswordAsync(username, request.OldPassword, request.NewPassword);
            return result
                ? Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Đổi mật khẩu thành công.",
                    StatusCode = 200
                })
                : BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Mật khẩu cũ không chính xác hoặc có lỗi xảy ra.",
                    StatusCode = 400
                });
        }

        [HttpPost("forgot-password")]
        [SwaggerOperation(Summary = "Quên mật khẩu", Description = "Gửi mã OTP qua email để đặt lại mật khẩu.")]
        [SwaggerResponse(200, "Mã OTP đã được gửi qua email", typeof(ApiResponse<object>))]
        [SwaggerResponse(400, "Email không hợp lệ hoặc không tồn tại", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Email không hợp lệ",
                    StatusCode = 400
                });

            var otpToken = await _authService.GeneratePasswordResetTokenAsync(request.Email);
            if (otpToken == null)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Email không hợp lệ hoặc không tồn tại.",
                    StatusCode = 400
                });

            try
            {
                await _emailService.SendEmailAsync(request.Email, "Mã đặt lại mật khẩu",
                    $"<p>Mã OTP của bạn là: <b>{otpToken}</b></p><p>OTP sẽ hết hạn sau 10 phút.</p>");
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Mã OTP đã được gửi qua email.",
                    StatusCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Lỗi gửi email: {ex.Message}",
                    StatusCode = 500
                });
            }
        }

        [HttpPost("verify-otp")]
        [SwaggerOperation(Summary = "Xác thực mã OTP", Description = "Xác thực mã OTP được gửi qua email trước khi đặt lại mật khẩu.")]
        [SwaggerResponse(200, "Mã OTP hợp lệ", typeof(ApiResponse<VerifyOtpResponse>))]
        [SwaggerResponse(400, "Mã OTP không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ",
                    StatusCode = 400
                });

            var isValid = await _authService.VerifyOtpTokenAsync(request.Token);
            if (!isValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Mã OTP không hợp lệ hoặc đã hết hạn.",
                    StatusCode = 400
                });

            return Ok(new ApiResponse<VerifyOtpResponse>
            {
                Success = true,
                Message = "Mã OTP hợp lệ.",
                Data = new VerifyOtpResponse
                {
                    IsValid = true
                },
                StatusCode = 200
            });
        }

        [HttpPost("reset-password")]
        [SwaggerOperation(Summary = "Đặt lại mật khẩu", Description = "Đặt lại mật khẩu sau khi đã xác thực mã OTP thành công.")]
        [SwaggerResponse(200, "Đặt lại mật khẩu thành công", typeof(ApiResponse<object>))]
        [SwaggerResponse(400, "Thông tin không hợp lệ", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ",
                    StatusCode = 400
                });

            try
            {
                var success = await _authService.ResetPasswordAsync(request.NewPassword);
                if (!success)
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Không thể đặt lại mật khẩu. Vui lòng thử lại.",
                        StatusCode = 400
                    });

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Đặt lại mật khẩu thành công.",
                    StatusCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Lỗi hệ thống: {ex.Message}",
                    StatusCode = 500
                });
            }
        }

        [HttpGet("user/profile")]
        [Authorize]
        [SwaggerOperation(Summary = "Lấy thông tin người dùng hiện tại", Description = "Lấy thông tin username và loại người dùng (Student/Staff/Manager) của tài khoản đang đăng nhập")]
        [SwaggerResponse(200, "Lấy thông tin thành công", typeof(ApiResponse<UserInfo>))]
        [SwaggerResponse(401, "Không có quyền truy cập", typeof(ApiResponse<object>))]
        [SwaggerResponse(404, "Không tìm thấy người dùng", typeof(ApiResponse<object>))]
        public async Task<IActionResult> GetUserProfile()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Không xác định được người dùng",
                    StatusCode = 401
                });

            var userInfo = await _authService.GetUserInfoAsync(username);
            if (userInfo == null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Không tìm thấy người dùng",
                    StatusCode = 404
                });

            return Ok(new ApiResponse<UserInfo>
            {
                Success = true,
                Data = userInfo,
                StatusCode = 200
            });
        }
    }
}
