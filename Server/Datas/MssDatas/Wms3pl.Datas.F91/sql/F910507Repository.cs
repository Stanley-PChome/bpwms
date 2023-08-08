using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
    public partial class F910507Repository : RepositoryBase<F910507, Wms3plDbContext, F910506Repository>
    {
        public ExecuteResult UpdateCaseNoForP910103(string dcCode, string gupCode, string custCode, string processNo, string clientIp, string caseNo)
        {
            // 2. 更新case no
            var sql = @"
                        UPDATE F910507
                        SET    CASE_NO = @p5,
                               UPD_DATE = @p8,
                               UPD_STAFF = @p6,
                               UPD_NAME = @p7
                        WHERE  DC_CODE = @p0
                           AND GUP_CODE = @p1
                           AND CUST_CODE = @p2
                           AND PROCESS_NO = @p3
                           AND PROCESS_IP = @p4
                           AND ( CASE_NO = ''
                                  OR CASE_NO IS NULL )
                           AND STATUS = '0'
                           AND ISPASS = '1' 
                        ";

            var param = new[] {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", processNo),
                new SqlParameter("@p4", clientIp),
                new SqlParameter("@p5", caseNo),
                new SqlParameter("@p6", Current.Staff),
                new SqlParameter("@p7", Current.StaffName),
                new SqlParameter("@p8", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };
            ExecuteSqlCommand(sql, param);
            return new ExecuteResult(true, caseNo);
        }

        public ExecuteResult ClearCaseNoForP910103(string dcCode, string gupCode, string custCode, string processNo, string clientIp, string caseNo)
        {
            var sql = @"
                        UPDATE F910507
                        SET    CASE_NO = '',
                               UPD_DATE = @p8,
                               UPD_STAFF = @p6,
                               UPD_NAME = @p7
                        WHERE  DC_CODE = @p0
                           AND GUP_CODE = @p1
                           AND CUST_CODE = @p2
                           AND PROCESS_NO = @p3
                           AND PROCESS_IP = @p4
                           AND CASE_NO = @p5 
                        ";

            var param = new[]
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", processNo),
                new SqlParameter("@p4", clientIp),
                new SqlParameter("@p5", caseNo),
                new SqlParameter("@p6", Current.Staff),
                new SqlParameter("@p7", Current.StaffName),
                new SqlParameter("@p8", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };

            ExecuteSqlCommand(sql, param);
            return new ExecuteResult() { IsSuccessed = true };
        }

        

       
    }
}
