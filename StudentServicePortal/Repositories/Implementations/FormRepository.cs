using Dapper;
using StudentServicePortal.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace StudentServicePortal.Repositories
{
    public class FormRepository : IFormRepository
    {
        private readonly IDbConnection _dbConnection;
        public FormRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        private const string GET_ALL_FORMS = @"
            SELECT MaBM, MaCB, MaPB, TenBM, LienKet, ThoiGianDang 
            FROM [dbo].[BIEU_MAU]";
        private const string GET_FORM_BY_ID = "SELECT * FROM BIEU_MAU WHERE MaBM = @MaBM";
        private const string INSERT_FORM = @"
        INSERT INTO BIEU_MAU (MaBM, MaCB, MaPB, TenBM, LienKet, ThoiGianDang)
        VALUES (@MaBM, @MaCB, @MaPB, @TenBM, @LienKet, @ThoiGianDang)";
        private const string UPDATE_FORM = @"
        UPDATE BIEU_MAU 
        SET MaCB = @MaCB, MaPB = @MaPB, TenBM = @TenBM, 
            LienKet = @LienKet, ThoiGianDang = @ThoiGianDang
        WHERE MaBM = @MaBM";
        public async Task<IEnumerable<Form>> GetAllForms()
        {
            return await _dbConnection.QueryAsync<Form>(GET_ALL_FORMS);
        }

        public async Task<Form?> GetFormById(string maBM)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@MaBM", maBM);

            return await _dbConnection.QueryFirstOrDefaultAsync<Form>(GET_FORM_BY_ID, parameters);
        }

        public async Task<bool> CreateFormAsync(Form form)
        {
            form.ThoiGianDang = DateTime.Now; // Gán thời gian hệ thống tại đây

            var result = await _dbConnection.ExecuteAsync(INSERT_FORM, form);
            return result > 0;
        }


        public async Task<bool> UpdateAsync(string maBM, Form form)
        {
            form.ThoiGianDang = DateTime.Now;
            var parameters = new
            {
                form.MaCB,
                form.MaPB,
                form.TenBM,
                form.LienKet,
                form.ThoiGianDang,
                MaBM = maBM
            };

            var affectedRows = await _dbConnection.ExecuteAsync(UPDATE_FORM, parameters);
            return affectedRows > 0;
        }
    }
}
