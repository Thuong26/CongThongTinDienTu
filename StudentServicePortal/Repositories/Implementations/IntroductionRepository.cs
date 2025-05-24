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
                SELECT TOP 1 
                    MaQL,
                    TieuDe,
                    NoiDung,
                    HinhAnh,
                    ThongTinLienHe
                FROM GIOI_THIEU
                WHERE MaQL = 'QL_ADMIN'";

            var result = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(query);
            
            if (result == null)
            {
                return new Introduction
                {
                    ManagerId = "QL_ADMIN",
                    Title = string.Empty,
                    Content = string.Empty,
                    Image = string.Empty,
                    ContactInfoJson = "[]"
                };
            }

            return new Introduction
            {
                ManagerId = result.MaQL,
                Title = result.TieuDe,
                Content = result.NoiDung,
                Image = result.HinhAnh,
                ContactInfoJson = result.ThongTinLienHe
            };
        }
    }
} 