using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1913Repository : RepositoryBase<F1913, Wms3plDbContext, F1913Repository>
    {
        public IQueryable<StockQuery> GetStockQuerys(string gupCode, string custCode, List<string> dcCodes, List<string> locCodes, List<string> itemCodes)
        {
            var parameters = new List<object>
            {
              gupCode,custCode
            };
            var sql = @"SELECT A.DC_CODE,B.WAREHOUSE_ID,A.LOC_CODE,A.ITEM_CODE,SUM(A.QTY) QTY
										      FROM F1913 A   
										      JOIN F1912 B On A.DC_CODE = B.DC_CODE And A.LOC_CODE = B.LOC_CODE
												 WHERE A.GUP_CODE =@p0 AND A.CUST_CODE=@p1							
													 AND A.QTY > 0 
													 AND B.NOW_STATUS_ID <> '04'
													 AND B.NOW_STATUS_ID <> '03' ";
            sql += parameters.CombineSqlInParameters("AND A.DC_CODE ", dcCodes);
            sql += parameters.CombineSqlInParameters("AND A.LOC_CODE ", locCodes);
            sql += parameters.CombineSqlInParameters("AND A.ITEM_CODE ", itemCodes);

            sql += @" GROUP BY A.DC_CODE,B.WAREHOUSE_ID,A.LOC_CODE,A.ITEM_CODE ";

            return SqlQuery<StockQuery>(sql, parameters.ToArray());

        }
    }
}
