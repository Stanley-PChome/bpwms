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
    public partial class F910102Repository : RepositoryBase<F910102, Wms3plDbContext, F910102Repository>
    {
        // <summary>
        // 非組合商品也要找出加工倉的庫存, 此時回傳的ITEM_CODE, ITEM_CODE_BOM, MATERIAL_CODE皆相同
        // BOM_QTY一律為0
        // ToDo: 還沒加上WAREHOUSE_TYPE的判斷
        // </summary>
        // <param name="dcCode"></param>
        // <param name="gupCode"></param>
        // <param name="custCode"></param>
        // <param name="processNo"></param>
        // <returns></returns>
        public IQueryable<BomQtyData> GetBomQtyData2(string dcCode, string gupCode, string custCode, string processNo)
        {
            var sql = $@"SELECT S.*,ROW_NUMBER ()OVER(ORDER BY DC_CODE, GUP_CODE, CUST_CODE, ITEM_CODE, ITEM_CODE_BOM, PROCESS_QTY) ROWNUM
				  FROM (SELECT C.DC_CODE, C.GUP_CODE, C.CUST_CODE, C.ITEM_CODE, C.ITEM_CODE_BOM,
				               0 BOM_QTY, C.PROCESS_QTY, C.PROCESS_QTY AS NEED_QTY, Sum(G.QTY) AS AVAILABLE_QTY, 'W' AS WAREHOUSE_TYPE
				          FROM F910201 C /* 加工單, 用來取出所需的數量 */ 
				          LEFT JOIN (SELECT D.*
				                       FROM F1913 D
				                       JOIN F1912 E On D.DC_CODE=E.DC_CODE And D.LOC_CODE=E.LOC_CODE
				                       JOIN F1980 F On E.DC_CODE=F.DC_CODE And E.WAREHOUSE_ID =F.WAREHOUSE_ID And F.WAREHOUSE_TYPE = 'W'
				                    ) G
				            ON C.GUP_CODE = G.GUP_CODE AND C.CUST_CODE = G.CUST_CODE And C.DC_CODE=G.DC_CODE And C.ITEM_CODE = G.ITEM_CODE 
				         WHERE C.DC_CODE = @p0
				           AND C.GUP_CODE = @p1
				           AND C.CUST_CODE = @p2
				           And C.PROCESS_NO= @p3
				         GROUP BY C.DC_CODE, C.GUP_CODE, C.CUST_CODE, C.ITEM_CODE, C.ITEM_CODE_BOM, PROCESS_QTY
				       ) S
				       ";

            var param = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", processNo)
            };

            var result = SqlQuery<BomQtyData>(sql, param.ToArray());
            return result;
        }
    }
}
