using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F051501Repository : RepositoryBase<F051501, Wms3plDbContext, F051501Repository>
	{
        public void UpdateStatusByBatcnNo(string dcCode, string gupCode, string custCode, string batchNo, string status)
        {
            var sql = @" UPDATE F051501 SET STATUS = @p0,UPD_DATE = dbo.GetSysDate(),UPD_STAFF=@p1,UPD_NAME= @p2
                    WHERE DC_CODE = @p3 AND GUP_CODE =@p4 AND CUST_CODE =@p5 AND BATCH_NO= @p6 ";
            var parms = new List<object> { status, Current.Staff, Current.StaffName, dcCode, gupCode, custCode, batchNo };
            ExecuteSqlCommand(sql, parms.ToArray());
        }

      
    }
}
