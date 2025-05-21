using Dapper;
using StudentServicePortal.Models;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StudentServicePortal.Repositories
{
    public class IntroductionRepository : IIntroductionRepository
    {
        private readonly IDbConnection _dbConnection;

        public IntroductionRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Introduction> GetIntroductionAsync()
        {
            const string query = @"
                SELECT TOP 1 MaQL, TieuDe, NoiDung, HinhAnh, ThongTinLienHe
                FROM GIOI_THIEU
                ORDER BY MaQL";

            var introduction = await _dbConnection.QueryFirstOrDefaultAsync<Introduction>(query);
            return introduction;
        }
    }
} 