using Dapper;
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

        private const string GET_ALL_STUDENTS = "SELECT * FROM SINHVIEN";
        private const string GET_STUDENT_BY_ID = "SELECT * FROM SINHVIEN WHERE MSSV = @MSSV";
        private const string INSERT_STUDENT = @"
            INSERT INTO SINHVIEN (MSSV, Hoten, Ngaysinh, Gioitinh, Chuyennganh, Trinhdohoctap, Hinhthuchoctap, Lop, Khoa, Khoahoc) 
            VALUES (@MSSV, @Hoten, @Ngaysinh, @Gioitinh, @Chuyennganh, @Trinhdohoctap, @Hinhthuchoctap, @Lop, @Khoa, @Khoahoc)";
        private const string UPDATE_STUDENT = @"
            UPDATE SINHVIEN 
            SET Hoten = @Hoten, Ngaysinh = @Ngaysinh, Gioitinh = @Gioitinh, 
                Chuyennganh = @Chuyennganh, Trinhdohoctap = @Trinhdohoctap, 
                Hinhthuchoctap = @Hinhthuchoctap, Lop = @Lop, Khoa = @Khoa, Khoahoc = @Khoahoc 
            WHERE MSSV = @MSSV";
        private const string DELETE_STUDENT = "DELETE FROM SINHVIEN WHERE MSSV = @MSSV";

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
    }
}
