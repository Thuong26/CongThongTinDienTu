using Microsoft.AspNetCore.Mvc;
using StudentServicePortal.Models;
using StudentServicePortal.Services;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Controllers
{
    [Route("api/introduction")]
    public class IntroductionController : BaseController
    {
        private readonly IIntroductionService _introductionService;

        public IntroductionController(IIntroductionService introductionService)
        {
            _introductionService = introductionService ?? throw new ArgumentNullException(nameof(introductionService));
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lấy thông tin giới thiệu", Description = "API lấy thông tin giới thiệu và thông tin liên hệ của trường")]
        [SwaggerResponse(200, "Lấy thông tin thành công", typeof(ApiResponse<IntroductionResponse>))]
        [SwaggerResponse(404, "Không tìm thấy thông tin giới thiệu", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Lỗi hệ thống", typeof(ApiResponse<object>))]
        public async Task<ActionResult<ApiResponse<IntroductionResponse>>> GetIntroduction()
        {
            try
            {
                var introduction = await _introductionService.GetIntroductionAsync();

                if (introduction == null)
                {
                    return ApiResponse<IntroductionResponse>(
                        data: null,
                        message: "Không tìm thấy thông tin giới thiệu",
                        statusCode: 404,
                        success: false
                    );
                }

                var response = new IntroductionResponse
                {
                    TieuDe = introduction.Title,
                    NoiDung = introduction.Content,
                    HinhAnh = introduction.Image,
                    ThongTinLienHe = introduction.ContactInfo
                };

                return ApiResponse<IntroductionResponse>(
                    data: response,
                    message: "Lấy thông tin giới thiệu thành công",
                    statusCode: 200,
                    success: true
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IntroductionResponse>(
                    data: null,
                    message: $"Lỗi hệ thống: {ex.Message}",
                    statusCode: 500,
                    success: false
                );
            }
        }
    }

    public class IntroductionResponse
    {
        [SwaggerSchema(Description = "Tiêu đề giới thiệu")]
        public string TieuDe { get; set; }

        [SwaggerSchema(Description = "Nội dung giới thiệu")]
        public string NoiDung { get; set; }

        [SwaggerSchema(Description = "Đường dẫn hình ảnh minh họa")]
        public string HinhAnh { get; set; }

        [SwaggerSchema(Description = "Danh sách thông tin liên hệ")]
        public List<ContactInfo> ThongTinLienHe { get; set; }
    }
}