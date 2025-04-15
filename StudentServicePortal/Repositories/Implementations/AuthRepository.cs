using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace StudentServicePortal.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbConnection _dbConnection;

        public AuthRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<(bool, string)> ValidateUserAsync(string username, string password)
        {
            var hashedPassword = HashPassword(password); // ✅ Hash mật khẩu nhập vào

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
                return BitConverter.ToString(hash).Replace("-", "").ToLower(); // Chuyển sang dạng hex
            }
        }

    }
}
