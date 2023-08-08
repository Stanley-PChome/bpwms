using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F05080504Repository : RepositoryBase<F05080504, Wms3plDbContext, F05080504Repository>
	{
        public IQueryable<F05080504Data> GetF05080504Datas(string dcCode, string gupCode, string custCode, string calNo)
        {
            var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.CAL_NO, A.WAREHOUSE_ID, A.AREA_CODE ASC) ROWNUM, A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.WAREHOUSE_ID,
								A.WAREHOUSE_ID + ' ' + B.WAREHOUSE_NAME WAREHOUSE_NAME,
								A.AREA_CODE, A.AREA_CODE + ' ' + C.AREA_NAME AREA_NAME,
								A.DELV_QTY,A.DELV_RATIO
                      FROM F05080504 A
                      JOIN F1980 B 
                        ON B.DC_CODE = A.DC_CODE
                       AND B.WAREHOUSE_ID = A.WAREHOUSE_ID
                      LEFT JOIN F1919 C
                        ON C.DC_CODE = A.DC_CODE
                       AND C.WAREHOUSE_ID = A.WAREHOUSE_ID
                       AND C.AREA_CODE = A.AREA_CODE 
                     WHERE A.DC_CODE = @p0
                       AND A.GUP_CODE = @p1 
                       AND A.CUST_CODE = @p2
                       AND A.CAL_NO = @p3 ";

            var result = SqlQuery<F05080504Data>(sql, new object[] { dcCode, gupCode, custCode, calNo });

            return result;
        }
    }
}
