using StudentServicePortal.Models;
using StudentServicePortal.Repositories;
using System.Threading.Tasks;

namespace StudentServicePortal.Services
{
    public class IntroductionService : IIntroductionService
    {
        private readonly IIntroductionRepository _introductionRepository;

        public IntroductionService(IIntroductionRepository introductionRepository)
        {
            _introductionRepository = introductionRepository;
        }

        public async Task<Introduction> GetIntroductionAsync()
        {
            return await _introductionRepository.GetIntroductionAsync();
        }
    }
} 