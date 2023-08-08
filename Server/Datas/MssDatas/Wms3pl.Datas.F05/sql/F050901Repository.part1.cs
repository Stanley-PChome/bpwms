using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;

namespace Wms3pl.Datas.F05
{
    public partial class F050901Repository : RepositoryBase<F050901, Wms3plDbContext, F050901Repository>
	{
        //P0503060000  出貨回傳檔下載
        public IQueryable<GetF050901CSV> GetF050901CSV(string dcCode, string gupCode, string custCode, DateTime? begCrtDate, DateTime? endCrtDate)
        {
            var parameter = new List<SqlParameter>
            {
                new SqlParameter("@p0",dcCode),
                new SqlParameter("@p1",gupCode),
                new SqlParameter("@p2",custCode),
            };

            var sql = @" 
           SELECT B.CUST_ORD_NO ,B.CUST_NAME,'3' ALL_ID,D.CONSIGN_NO,A.DC_CODE,A.GUP_CODE,A.CUST_CODE
             FROM F05030101 A
             JOIN F050301 B ON A.ORD_NO = B.ORD_NO AND A.CUST_CODE = B.CUST_CODE AND A.GUP_CODE = B.GUP_CODE AND A.DC_CODE = B.DC_CODE
             JOIN F050801 C ON A.WMS_ORD_NO = C.WMS_ORD_NO AND A.CUST_CODE = C.CUST_CODE AND A.GUP_CODE = C.GUP_CODE AND A.DC_CODE = C.DC_CODE
             JOIN F050901 D ON A.GUP_CODE = D.GUP_CODE AND A.CUST_CODE = D.CUST_CODE AND A.DC_CODE = D.DC_CODE AND A.WMS_ORD_NO = D.WMS_NO 
            WHERE  A.DC_CODE = @p0 AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2 AND C.APPROVE_DATE IS NOT NULL AND C.ORD_PROP = 'O3'
                        ";
            if (begCrtDate.HasValue)
            {
                sql += "    AND C.APPROVE_DATE >= @p" + parameter.Count;
                parameter.Add(new SqlParameter("@p" + parameter.Count, begCrtDate.Value.Date));
            }
            if (endCrtDate.HasValue)
            {
                sql += "    AND C.APPROVE_DATE < @p" + parameter.Count;
                parameter.Add(new SqlParameter("@p" + parameter.Count, endCrtDate.Value.Date.AddDays(1)));
            }
            // var param = new object[] { dcCode, gupCode, custCode };
            var result = SqlQuery<GetF050901CSV>(sql, parameter.ToArray());

            return result;
        }
    }
}
