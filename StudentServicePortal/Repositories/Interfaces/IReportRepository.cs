namespace StudentServicePortal.Repositories.Interfaces
{
    // IReportRepository.cs
    public interface IReportRepository
    {
        Task<IEnumerable<ReportDTO>> GetReportsAsync();
    }

}
