using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F052901Repository : RepositoryBase<F052901, Wms3plDbContext, F052901Repository>
	{

        public IQueryable<F052901> GetDataByPickOrdNo(string dcCode, string gupCode, string custCode, string pickOrdNo)
        {
            var param = new object[] { dcCode, gupCode, custCode, pickOrdNo };
            var sql = @" SELECT A.*
                    FROM F052901 A
                   INNER JOIN F050801 B
                      ON B.DC_CODE = A.DC_CODE
                     AND B.GUP_CODE = A.GUP_CODE
                     AND B.CUST_CODE = A.CUST_CODE
                     AND B.WMS_ORD_NO = A.WMS_ORD_NO
                   WHERE A.DC_CODE = @p0 
                     AND A.GUP_CODE = @p1 
                     AND A.CUST_CODE = @p2 
                     AND B.PICK_ORD_NO = @p3
                    
                     AND NOT EXISTS(  --排除訂單已取消
                      SELECT 1 
                        FROM F05030101 C
                        INNER JOIN F050301 D
                          ON D.DC_CODE = C.DC_CODE
                          AND D.GUP_CODE = C.GUP_CODE
                          AND D.CUST_CODE = C.CUST_CODE
                          AND D.ORD_NO = C.ORD_NO
                        WHERE C.DC_CODE = B.DC_CODE
                          AND C.GUP_CODE = B.GUP_CODE
                          AND C.CUST_CODE = B.CUST_CODE
                          AND C.WMS_ORD_NO = B.WMS_ORD_NO
                          AND D.PROC_FLAG ='9')
                        ";

            var result = SqlQuery<F052901>(sql, param);

            return result;
        }
    }
}
