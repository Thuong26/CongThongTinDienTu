namespace StudentServicePortal.Services.Interfaces
{
    public interface ICodeGeneratorService
    {
        Task<string> GenerateMaDonCTAsync();
        Task<string> GenerateMaQDAsync();
    }
} 