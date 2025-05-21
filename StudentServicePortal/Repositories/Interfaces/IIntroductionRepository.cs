using StudentServicePortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Repositories
{
    public interface IIntroductionRepository
    {
        Task<Introduction> GetIntroductionAsync();
    }
} 