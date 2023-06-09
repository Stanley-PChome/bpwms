using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F190901Repository : RepositoryBase<F190901, Wms3plDbContext, F190901Repository>
	{
        public ExecuteResult UpdateInvoNo(string gupCode, string custCode, string yearCode, string monthCode, string invoTitle, string invoNo)
        {
            var result = new ExecuteResult() { IsSuccessed = false };
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0",  invoNo),
                new SqlParameter("@p1",  gupCode),
                new SqlParameter("@p2",  custCode),
                new SqlParameter("@p3",  yearCode),
                new SqlParameter("@p4",  monthCode),
                new SqlParameter("@p5",  invoTitle),
                new SqlParameter("@p6",  Current.Staff),
                new SqlParameter("@p7",  Current.StaffName)
            };
            var sql = @"
						update F190901 set INVO_NO =@p0 
										   , UPD_DATE =dbo.GetSysDate()  , UPD_STAFF =@p6  , UPD_NAME =@p7                           
                        where  GUP_CODE =@p1 and CUST_CODE = @p2
								and INVO_YEAR =@p3 and INVO_MON =@p4 and INVO_TITLE =@p5
						";

            ExecuteSqlCommand(sql, parameters.ToArray());
            result.IsSuccessed = true;
            return result;
        }
    }
}