using Dapper;
using Microsoft.EntityFrameworkCore;
using StudentServicePortal.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace StudentServicePortal.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IDbConnection _dbConnection;

        public StudentRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

                private const string GET_ALL_STUDENTS = @"
        SELECT
            MaSV             AS MSSV,
            HoTen            AS Hoten,
            NgaySinh         AS Ngaysinh,
            GioiTinh         AS Gioitinh,
            ChuyenNganh      AS Chuyennganh,
            TrinhDoDaoTao    AS Trinhdohoctap,
            HinhThucDaoTao   AS Hinhthuchoctap,
            Lop,
            Khoa,
            KhoaHoc          AS Khoahoc,
            Email
        FROM [dbo].[SINH_VIEN]";

                private const string GET_STUDENT_BY_ID = @"
        SELECT
            MaSV             AS MSSV,
            HoTen            AS Hoten,
            NgaySinh         AS Ngaysinh,
            GioiTinh         AS Gioitinh,
            ChuyenNganh      AS Chuyennganh,
            TrinhDoDaoTao    AS Trinhdohoctap,
            HinhThucDaoTao   AS Hinhthuchoctap,
            Lop,
            Khoa,
            KhoaHoc          AS Khoahoc,
            Email
        FROM [dbo].[SINH_VIEN]
        WHERE MaSV = @MSSV";

                private const string INSERT_STUDENT = @"
        INSERT INTO [dbo].[SINH_VIEN]
        (
            MaSV,
            HoTen,
            NgaySinh,
            GioiTinh,
            ChuyenNganh,
            TrinhDoDaoTao,
            HinhThucDaoTao,
            Lop,
            Khoa,
            KhoaHoc,
            Email
        )
        VALUES
        (
            @MSSV,
            @Hoten,
            @Ngaysinh,
            @Gioitinh,
            @Chuyennganh,
            @Trinhdohoctap,
            @Hinhthuchoctap,
            @Lop,
            @Khoa,
            @Khoahoc,
            @Email
        )";

                private const string UPDATE_STUDENT = @"
        UPDATE [dbo].[SINH_VIEN]
        SET
            HoTen           = @Hoten,
            NgaySinh        = @Ngaysinh,
            GioiTinh        = @Gioitinh,
            ChuyenNganh     = @Chuyennganh,
            TrinhDoDaoTao   = @Trinhdohoctap,
            HinhThucDaoTao  = @Hinhthuchoctap,
            Lop             = @Lop,
            Khoa            = @Khoa,
            KhoaHoc         = @Khoahoc,
            Email           = @Email
        WHERE MaSV = @MSSV";

                private const string DELETE_STUDENT = @"
        DELETE FROM [dbo].[SINH_VIEN]
        WHERE MaSV = @MSSV";


        public async Task<IEnumerable<Student>> GetAllStudents()
        {
            return await _dbConnection.QueryAsync<Student>(GET_ALL_STUDENTS);
        }

        public async Task<Student> GetStudentById(string mssv)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@MSSV", mssv);
            return await _dbConnection.QueryFirstOrDefaultAsync<Student>(GET_STUDENT_BY_ID, parameters);
        }

        public async Task AddStudent(Student student)
        {
            var parameters = new DynamicParameters(student);
            await _dbConnection.ExecuteAsync(INSERT_STUDENT, parameters);
        }

        public async Task UpdateStudent(Student student)
        {
            var parameters = new DynamicParameters(student);
            await _dbConnection.ExecuteAsync(UPDATE_STUDENT, parameters);
        }

        public async Task DeleteStudent(string mssv)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@MSSV", mssv);
            await _dbConnection.ExecuteAsync(DELETE_STUDENT, parameters);
        }

        public async Task<Student?> GetStudentProfile(string mssv)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@MSSV", mssv);
            return await _dbConnection
                .QueryFirstOrDefaultAsync<Student>(GET_STUDENT_BY_ID, parameters);
        }

    }
}
