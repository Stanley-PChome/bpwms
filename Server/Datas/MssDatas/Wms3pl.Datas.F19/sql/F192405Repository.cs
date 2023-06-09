using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F192405Repository : RepositoryBase<F192405, Wms3plDbContext, F192405Repository>
    {
        public void Delete(string empId, string scheduleId = null)
        {
            var sql = @"
                        DELETE F192405
                        WHERE  EMP_ID = @p0
                               AND ( CASE
                                       WHEN @p1 = '' THEN '1'
                                       ELSE SCHEDULE_ID
                                     END ) = ( CASE
                                                 WHEN @p1 = '' THEN '1'
                                                 ELSE @p1
                                               END ) 
                        ";

            var paramers = new[]
            {
                new SqlParameter("@p0", empId),
                new SqlParameter("@p1", scheduleId ?? string.Empty)
            };

            ExecuteSqlCommand(sql, paramers);
        }
    }
}
