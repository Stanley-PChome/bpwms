using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F05080502Repository : RepositoryBase<F05080502, Wms3plDbContext, F05080502Repository>
	{
        public IQueryable<F05080502Data> GetF05080502Datas(string dcCode, string gupCode, string custCode, string calNo)
        {
            var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.ORD_NO,A.ORD_SEQ,A.ITEM_CODE ASC) ROWNUM,
                                A.DC_CODE,
                                A.GUP_CODE,
                                A.CUST_CODE,
                                A.CAL_NO,
                                A.ORD_NO,
                                A.ORD_SEQ,
                                A.ITEM_CODE,
                                B.ITEM_NAME,
                                A.ORD_QTY,
                                A.ALLOT_QTY,
                                ISNULL(C.CUST_ORD_NO,D.CUST_ORD_NO) CUST_ORD_NO
					 FROM F05080502 A
					 LEFT JOIN F1903 B
					   ON B.GUP_CODE = A.GUP_CODE
					  AND B.CUST_CODE= A.CUST_CODE
					  AND B.ITEM_CODE = A.ITEM_CODE 
                     LEFT JOIN F050001 C
                       ON C.DC_CODE = A.DC_CODE
                      AND C.GUP_CODE = A.GUP_CODE
                      AND C.CUST_CODE = A.CUST_CODE
                      AND C.ORD_NO = A.ORD_NO
                     LEFT JOIN F050301 D
                       ON D.DC_CODE = A.DC_CODE
                      AND D.GUP_CODE = A.GUP_CODE
                      AND D.CUST_CODE = A.CUST_CODE
                      AND D.ORD_NO = A.ORD_NO
                    WHERE A.DC_CODE = @p0
                      AND A.GUP_CODE = @p1
                      AND A.CUST_CODE =@p2
                      AND A.CAL_NO =@p3
                    ORDER BY A.ORD_NO,A.ORD_SEQ,A.ITEM_CODE ";

            var result = SqlQuery<F05080502Data>(sql, new object[] { dcCode, gupCode, custCode, calNo });

            return result;
        }
    }
}
