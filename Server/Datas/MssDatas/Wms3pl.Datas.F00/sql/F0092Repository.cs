using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Enums;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
    public partial class F0092Repository : RepositoryBase<F0092, Wms3plDbContext, F0092Repository>
    {
        public void InsertF0092(WmsLogProcType procType, string batchNo, string procMsg)
        {
            var parm = new List<SqlParameter>()
            {
                new SqlParameter("@p0",SqlDbType.VarChar){ Value= procType.ToString() },
                new SqlParameter("@p1", SqlDbType.VarChar){Value = batchNo },
                new SqlParameter("@p2", SqlDbType.NVarChar){Value =  procMsg.Length>200 ? procMsg.Substring(0,200) : procMsg },
								new SqlParameter("@p3", SqlDbType.DateTime2){Value = DateTime.Now },
								new SqlParameter("@p4", SqlDbType.VarChar){Value = Current.Staff },
								new SqlParameter("@p5", SqlDbType.NVarChar){Value = Current.StaffName}
            };
            var sql = @"
                            INSERT INTO [F0092]
                                        ([PROC_TYPE],
                                         [BATCH_NO],
                                         [PROC_MSG],
                                         [CRT_DATE],
                                         [CRT_STAFF],
                                         [CRT_NAME])
                            VALUES      (@p0,
                                         @p1,
                                         @p2,
                                         @p3,
                                         @p4,
                                         @p5);  
                        ";
             ExecuteSqlCommand(sql, parm.ToArray());
        }
    }
}
