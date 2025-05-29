using Microsoft.AspNetCore.Mvc;
using StudentServicePortal.Models;
using System;

namespace StudentServicePortal.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected ActionResult<ApiResponse<T>> ApiResponse<T>(T data, string message = "", int statusCode = 200, bool success = true)
        {
            var response = new ApiResponse<T>
            {
                Success = success,
                Message = message,
                Data = data,
                StatusCode = statusCode
            };

            return statusCode switch
            {
                200 => Ok(response),
                400 => BadRequest(response),
                401 => Unauthorized(response),
                403 => Forbid(),
                404 => NotFound(response),
                500 => StatusCode(500, response),
                _ => StatusCode(statusCode, response)
            };
        }

        protected ActionResult<ApiResponse<string>> ApiError(string message, int statusCode = 500)
        {
            return ApiResponse<string>("", message, statusCode, false);
        }
    }
} 