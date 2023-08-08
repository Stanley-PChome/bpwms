using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F05080503Repository : RepositoryBase<F05080503, Wms3plDbContext, F05080503Repository>
    {
        public IQueryable<P0503050000CalHead> GetCalHeadList(string dcCode, string gupCode, string custCode,
           DateTime? calDateBegin, DateTime? calDateEnd, string calNo)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode)
            };
            string temp = string.Empty;


            if (calDateBegin.HasValue)
                temp += parameters.Combine(" AND A.CRT_DATE >= @p{0}", calDateBegin);
            if (calDateEnd.HasValue)
                temp += parameters.Combine(" AND A.CRT_DATE <= @p{0}", calDateEnd);
            temp += parameters.CombineNotNullOrEmpty(" AND A.CAL_NO = @p{0}", calNo);

            if ((!calDateBegin.HasValue || !calDateEnd.HasValue))
                return null;

            var sql = $@"SELECT ROW_NUMBER()OVER(ORDER BY B.CAL_NO ASC) ROWNUM, B.*
				FROM (
				SELECT TOP 100 PERCENT A.CAL_NO,
				CONVERT(varchar, A.TTL_A_ORD_CNT) + '/' + CONVERT(varchar, A.TTL_B_ORD_CNT) AS ORD_CNT,
				CONVERT(varchar, A.TTL_A_ITEM_CNT) + '/' + CONVERT(varchar, A.TTL_B_ITEM_CNT) AS ITEM_CNT,
				CONVERT(varchar, A.TTL_A_RETAIL_CNT) + '/' + CONVERT(varchar, A.TTL_B_RETAIL_CNT) AS RETAIL_CNT,
				CONVERT(varchar, A.TTL_A_DELV_QTY) + '/' + CONVERT(varchar, A.TTL_B_DELV_QTY) AS DELV_QTY,
				A.TTL_A_SHELF_CNT AS SHELF_CNT
				FROM F05080503 A
                WHERE A.DC_CODE = @p0
				AND A.GUP_CODE = @p1
				AND A.CUST_CODE = @p2
                {temp}
				AND A.TTL_B_ORD_CNT > 0 
                ORDER BY A.CAL_NO) B";

            var result = SqlQuery<P0503050000CalHead>(sql, parameters.ToArray());

            return result;
        }
    }
}
