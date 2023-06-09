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
    public partial class F191001Repository : RepositoryBase<F191001, Wms3plDbContext, F191001Repository>
	{

        public IQueryable<ItemLimitValidDay> GeItemLimitValidDays(string gupCode, string custCode, string retialCode, List<string> itemCodes)
        {
            var sql = @" 
                    SELECT DISTINCT A.GUP_CODE, A.CUST_CODE, A.RETAIL_CODE, C.ITEM_CODE, B.DELIVERY_DAY
										                     FROM F1910 A
										                     JOIN F191001 B
											                     ON B.GUP_CODE = A.GUP_CODE
											                    AND B.CUST_CODE = A.CUST_CODE
											                    AND B.CHANNEL = A.CHANNEL
										                     JOIN F1903 C
											                     ON C.GUP_CODE = B.GUP_CODE
											                    AND C.LTYPE = B.LTYPE
											                    AND C.MTYPE = B.MTYPE
											                    AND C.STYPE = B.STYPE
											                    AND C.CUST_CODE = A.CUST_CODE
										                    WHERE A.GUP_CODE = @p0
											                    AND A.CUST_CODE = @p1
											                    AND A.RETAIL_CODE = @p2 
                    ";
            var parms = new List<object> { gupCode, custCode, retialCode };
            sql += parms.CombineSqlInParameters(" AND C.ITEM_CODE ", itemCodes);
            return SqlQuery<ItemLimitValidDay>(sql, parms.ToArray());
        }
    }
}
