namespace StudentServicePortal.Services.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> ExportReportsToExcelAsync(List<ReportDTO> reports);
        Task<IEnumerable<ReportDTO>> GetReportsAsync();

    }
}
