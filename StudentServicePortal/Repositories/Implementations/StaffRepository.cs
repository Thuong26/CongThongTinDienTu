using Dapper;
using Microsoft.IdentityModel.Tokens;
using StudentServicePortal.Models;
using System.Data;

namespace StudentServicePortal.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly IDbConnection _dbConnection;

        private const string GET_STAFF_BY_ID = @"
        SELECT MaCB AS MSCB, MaPB, MaQL
        FROM CAN_BO 
        WHERE MaCB = @MaCB";


        // Constructor nhận IDbConnection để sử dụng Dapper
        public StaffRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Lấy thông tin cán bộ theo mã
        public async Task<StaffDTO?> GetByIdAsync(string maCB)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@MaCB", maCB);

            var staff = await _dbConnection.QueryFirstOrDefaultAsync<StaffDTO>(GET_STAFF_BY_ID, parameters);
            return staff;
        }
        private const string GET_ALL_STAFF = "SELECT MaCB as MSCB,MaPB,MaQL FROM CAN_BO";

        public async Task<IEnumerable<StaffDTO>> GetAllStaffAsync()
        {
            return await _dbConnection.QueryAsync<StaffDTO>(GET_ALL_STAFF);
        }

        private const string INSERT_STAFF = "INSERT INTO CAN_BO (MaCB,MaPB,MaQL, Matkhau) VALUES (@MSCB,@MaPB,@MaQL, @Matkhau)";

        public async Task<bool> CreateStaffAsync(Staff staff)
        {
            var result = await _dbConnection.ExecuteAsync(INSERT_STAFF, staff);
            return result > 0;
        }
        private const string UPDATE_STAFF = @"
        UPDATE CAN_BO 
        SET Matkhau = @Matkhau, 
            MaPB = @MaPB, 
            MaQL = @MaQL
        WHERE MaCB = @msCB";

        private const string callpass = "SELECT Matkhau FROM CAN_BO WHERE MaCB = @msCB";

        public async Task<bool> UpdateStaffAsync(string msCB, Staff staff)
        {
            // Nếu Matkhau là null, lấy mật khẩu từ cơ sở dữ liệu
            byte[] mk = staff.Matkhau;
            if (mk == null || mk.Length == 0)  // Kiểm tra nếu mật khẩu là null hoặc rỗng
            {
                var result = await _dbConnection.QuerySingleOrDefaultAsync<byte[]>(callpass, new { msCB });
                mk = result ?? mk;  // Nếu mật khẩu từ DB là null, giữ giá trị ban đầu
            }

            // Dùng DynamicParameters để tránh lỗi khi truyền tham số
            var parameters = new
            {
                msCB,
                MaPB = staff.MaPB,
                MaQL = staff.MaQL,
                Matkhau = mk // Gán mật khẩu dưới dạng byte[]
            };

            // Thực hiện truy vấn UPDATE và trả về số dòng bị ảnh hưởng
            var resultUpdate = await _dbConnection.ExecuteAsync(UPDATE_STAFF, parameters);
            return resultUpdate > 0; // Trả về true nếu có ít nhất một dòng được cập nhật
        }




    }
}
