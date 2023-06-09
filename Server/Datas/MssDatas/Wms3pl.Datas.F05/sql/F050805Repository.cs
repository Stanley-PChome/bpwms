using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F050805Repository : RepositoryBase<F050805, Wms3plDbContext, F050805Repository>
	{
        public IQueryable<F050805Data> GetF050805Datas(string dcCode, string gupCode, string custCode, string calNo)
        {
            var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.ITEM_CODE ASC) ROWNUM,
                                 A.ID,
                                 A.DC_CODE,
                                 A.GUP_CODE,
                                 A.CUST_CODE,
                                 A.CAL_NO,
                                 A.TYPE_ID,
                                 C.TYPE_NAME,
                                 A.ITEM_CODE,
                                 B.ITEM_NAME,
                                 A.TTL_PICK_STOCK_QTY,
                                 A.TTL_RESUPPLY_STOCK_QTY,
                                 A.TTL_PICK_STOCK_QTY+A.TTL_RESUPPLY_STOCK_QTY+TTL_VIRTUAL_STOCK_QTY TTL_STOCK_QTY,
                                 A.TTL_VIRTUAL_STOCK_QTY,
                                 A.TTL_ORD_QTY,
                                 A.TTL_OUTSTOCK_QTY,
                                 A.SUG_RESUPPLY_STOCK_QTY,
                                 A.SUG_VIRTUAL_STOCK_QTY,
                                 A.MAKE_NO,
                                 A.SERIAL_NO
						FROM F050805 A
						LEFT JOIN F1903 B
						  ON B.GUP_CODE = A.GUP_CODE
						 AND B.CUST_CODE = A.CUST_CODE
						 AND B.ITEM_CODE = A.ITEM_CODE
						JOIN F198001 C
						  ON C.TYPE_ID = A.TYPE_ID
					    WHERE A.DC_CODE = @p0
                          AND A.GUP_CODE = @p1
                          AND A.CUST_CODE = @p2
                          AND A.CAL_NO= @p3
                        ORDER BY A.ITEM_CODE ";

            var result = SqlQuery<F050805Data>(sql, new object[] { dcCode, gupCode, custCode, calNo });

            return result;
        }
    }
}
