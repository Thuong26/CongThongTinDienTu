using Dapper;
using StudentServicePortal.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace StudentServicePortal.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly IDbConnection _dbConnection;

        public DepartmentRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        private const string GET_ALL_DEPARTMENTS = "SELECT MaPB, TenPB FROM PHONG_BAN";

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _dbConnection.QueryAsync<Department>(GET_ALL_DEPARTMENTS);
        }
    }
}
