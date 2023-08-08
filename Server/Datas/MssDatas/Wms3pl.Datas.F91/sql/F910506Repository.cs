using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
    public partial class F910506Repository : RepositoryBase<F910506, Wms3plDbContext, F910506Repository>
    {
        
        public ExecuteResult UpdateBoxNoForP910103(string dcCode, string gupCode, string custCode, string processNo, string clientIp, string boxNo)
        {
            var sql = @"
                        UPDATE F910506
                        SET    BOX_NO = @p0,
                               UPD_STAFF = @p1,
                               UPD_NAME = @p2,
                               UPD_DATE = @p3
                        WHERE  DC_CODE = @p4
                           AND GUP_CODE = @p5
                           AND CUST_CODE = @p6
                           AND PROCESS_NO = @p7
                           AND PROCESS_IP = @p8
                           AND BOX_NO IS NULL
                           AND STATUS = '0'
                           AND ISPASS = '1' 
                        ";
            var param = new List<object>
            {
                boxNo,
                Current.Staff,
                Current.StaffName,
                DateTime.Now,
                dcCode,
                gupCode,
                custCode,
                processNo,
                clientIp
            };

            ExeSqlCmdCountMustGreaterZero(sql, "刷讀紀錄已綁定盒號，不可裝盒", param.ToArray());
            return new ExecuteResult(true);
        }

        

       

       
    }
}
