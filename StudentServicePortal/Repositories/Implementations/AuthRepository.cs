using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentServicePortal.Models;

namespace StudentServicePortal.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbConnection _dbConnection;

        public AuthRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<StudentLogin> GetUserByUsernameAsync(string username)
        {
            // Tìm sinh viên
            const string sqlStudent = @"SELECT MSSV AS Username, Matkhau FROM DANG_NHAP_SV WHERE MSSV = @u";
            var student = await _dbConnection.QuerySingleOrDefaultAsync<StudentLogin>(sqlStudent, new { u = username });
            string userType = null;
            if (student != null)
            {
                userType = "Student";
                return student;
            }

            // Tìm cán bộ
            const string sqlStaff = @"SELECT Username, Matkhau FROM CAN_BO WHERE Username = @u"; 
            var staff = await _dbConnection.QuerySingleOrDefaultAsync<StudentLogin>(sqlStaff, new { u = username });
            if (staff != null)
            {
                userType = "Staff";
                return staff;
            }

            // Tìm quản lý
            const string sqlManager = @"SELECT Username, Matkhau FROM QUAN_LY WHERE Username = @u"; 
            var manager = await _dbConnection.QuerySingleOrDefaultAsync<StudentLogin>(sqlManager, new { u = username });
            if (manager != null)
            {
                userType = "Manager";
                return manager;
            }

            return null;
        }
        public async Task<(bool, string)> ValidateUserAsync(string username, string password)
        {
            var hashedPassword = HashPassword(password); 

            string sql = @"
                        SELECT COUNT(*) 
                        FROM DANGNHAP_SV 
                        WHERE MSSV = @MSSV 
                        AND Matkhau = HASHBYTES('SHA2_256', @Password)";

            int count = await _dbConnection.ExecuteScalarAsync<int>(sql, new { MSSV = username, Password = password });

            if (count > 0)
            {
                string roleSql = "SELECT 'Student'";
                string role = await _dbConnection.ExecuteScalarAsync<string>(roleSql);
                return (true, role);
            }

            return (false, null);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower(); 
            }
        }

    }
}
