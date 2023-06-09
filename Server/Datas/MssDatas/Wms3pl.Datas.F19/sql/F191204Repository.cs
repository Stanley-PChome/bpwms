using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F191204Repository : RepositoryBase<F191204, Wms3plDbContext, F191204Repository>
	{
        public void DeleteDataByAllocation(string gupCode, string custCode, string dcCode, List<string> allocationNos)
        {
            var parms = new List<object> { dcCode, gupCode, custCode };
            var sql = @"DELETE F191204 WHERE DC_CODE = @p0 AND GUP_CODE = @p1 AND CUST_CODE = @p2";
            sql += parms.CombineSqlInParameters(" AND ALLOCATION_NO", allocationNos);
            ExecuteSqlCommand(sql, parms.ToArray());
        }
    }
}
