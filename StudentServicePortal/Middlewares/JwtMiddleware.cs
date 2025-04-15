using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using StudentServicePortal.Services;
using StudentServicePortal.Services.Interfaces;

namespace StudentServicePortal.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var logoutService = context.RequestServices.GetRequiredService<ILogoutService>();

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (!string.IsNullOrEmpty(token))
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                try
                {
                    var jwtToken = jwtHandler.ReadJwtToken(token);
                    var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == "jti")?.Value;

                    if (!string.IsNullOrEmpty(jti) && logoutService.IsTokenRevoked(jti))
                    {
                        _logger.LogWarning("Token has been revoked: {Token}", token);
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token has been revoked");
                        return;
                    }

                    var exp = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;
                    if (exp != null && DateTime.UtcNow > DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).UtcDateTime)
                    {
                        _logger.LogWarning("Token has expired: {Token}", token);
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token has expired");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing JWT token: {Token}", token);
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid token");
                    return;
                }
            }

            await _next(context);
        }
    }
}
