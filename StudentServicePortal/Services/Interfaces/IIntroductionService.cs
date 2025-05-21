using StudentServicePortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentServicePortal.Services
{
    public interface IIntroductionService
    {
        Task<Introduction> GetIntroductionAsync();
    }
} 