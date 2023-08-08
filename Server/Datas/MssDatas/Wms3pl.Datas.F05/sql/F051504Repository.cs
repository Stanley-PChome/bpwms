using System;
using System.Collections.Generic;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F051504Repository : RepositoryBase<F051504, Wms3plDbContext, F051504Repository>
	{
        public void UpdateStatusByBatchNo(string dcCode, string gupCode, string custCode, string batchNo, string status)
        {
            var sql = @" UPDATE F051504
									SET STATUS=@p0,UPD_DATE =@p1,UPD_STAFF=@p2,UPD_NAME =@p3 
									WHERE DC_CODE = @p4
										AND GUP_CODE = @p5
										AND CUST_CODE = @p6
										AND BATCH_NO = @p7";

            var parms = new List<object> { status, DateTime.Now, Current.Staff, Current.StaffName, dcCode, gupCode, custCode, batchNo };

            ExecuteSqlCommand(sql, parms.ToArray());
        }
    }
}
