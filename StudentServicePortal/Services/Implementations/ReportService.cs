using OfficeOpenXml;
using StudentServicePortal.Repositories.Interfaces;
using StudentServicePortal.Services.Interfaces;
using OfficeOpenXml.Style;
using System.IO;
using System.Drawing;

namespace StudentServicePortal.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repository;

        public ReportService(IReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ReportDTO>> GetReportsAsync()
        {
            return await _repository.GetReportsAsync();
        }
        public async Task<byte[]> ExportReportsToExcelAsync(List<ReportDTO> reports)
        {
            ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Reports");

                // Tiêu đề cột
                worksheet.Cells[1, 1].Value = "Mã đơn";
                worksheet.Cells[1, 2].Value = "Tên đơn";
                worksheet.Cells[1, 3].Value = "Phòng ban";
                worksheet.Cells[1, 4].Value = "Cán bộ";
                worksheet.Cells[1, 5].Value = "Quản lý";
                worksheet.Cells[1, 6].Value = "Thời gian đăng";
                worksheet.Cells[1, 7].Value = "Trạng thái";

                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                // Dữ liệu
                for (int i = 0; i < reports.Count; i++)
                {
                    var report = reports[i];
                    worksheet.Cells[i + 2, 1].Value = report.MaDon;
                    worksheet.Cells[i + 2, 2].Value = report.TenDon;
                    worksheet.Cells[i + 2, 3].Value = report.TenPB;
                    worksheet.Cells[i + 2, 4].Value = report.MaCB;
                    worksheet.Cells[i + 2, 5].Value = report.MaQL;
                    worksheet.Cells[i + 2, 6].Value = report.ThoiGianDang.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[i + 2, 7].Value = report.TrangThai ? "Đang xử lý" : "Hoàn thành";
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return await Task.FromResult(package.GetAsByteArray());
            }
        }
    }
}
