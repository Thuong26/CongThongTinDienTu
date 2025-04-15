using System.Threading.Tasks;

namespace StudentServicePortal.Repositories
{
    public interface IAuthRepository
    {
        Task<(bool, string)> ValidateUserAsync(string username, string password);
    }
}
