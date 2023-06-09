using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F194707Repository : RepositoryBase<F194707, Wms3plDbContext, F194707Repository>
    {
        public IQueryable<F194707Ex> GetP710507SearchData(string dcCode, string allId, string accKind,
          string inTax, string logiType, string custType, string status)
        {
            var sql = @"
SELECT ROW_NUMBER ()OVER(ORDER BY A.ALL_ID,A.ACC_AREA_ID,A.DC_CODE,A.DELV_EFFIC,A.DELV_TMPR,A.CUST_TYPE,A.LOGI_TYPE,A.ACC_KIND,A.ACC_DELVNUM_ID,A.ACC_TYPE,A.BASIC_VALUE) ROWNUM,
       A.*,
       B.ALL_COMP,
       C.ACC_AREA,
       D.NUM
  FROM F194707 A
       LEFT OUTER JOIN F1947 B
          ON A.ALL_ID = B.ALL_ID AND A.DC_CODE = B.DC_CODE
       LEFT OUTER JOIN F194708 C
          ON     A.ACC_AREA_ID = C.ACC_AREA_ID
             AND A.ALL_ID = C.ALL_ID
             AND A.DC_CODE = C.DC_CODE
       LEFT OUTER JOIN F194709 D
          ON     A.ACC_DELVNUM_ID = D.ACC_DELVNUM_ID
             AND A.ALL_ID = D.ALL_ID
             AND A.DC_CODE = D.DC_CODE
 WHERE     A.DC_CODE = @p0
";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
            };

            if (!string.IsNullOrWhiteSpace(allId))
            {
                sql += " AND A.ALL_ID = @p1 ";
                parameters.Add(new SqlParameter("@p1", allId));
            }

            if (!string.IsNullOrWhiteSpace(accKind))
            {
                sql += " AND A.ACC_KIND = @p2 ";
                parameters.Add(new SqlParameter("@p2", accKind));
            }

            if (!string.IsNullOrWhiteSpace(inTax))
            {
                sql += " AND A.IN_TAX = @p3 ";
                parameters.Add(new SqlParameter("@p3", inTax));
            }

            if (!string.IsNullOrWhiteSpace(logiType))
            {
                sql += " AND A.LOGI_TYPE = @p4 ";
                parameters.Add(new SqlParameter("@p4", logiType));
            }

            if (!string.IsNullOrWhiteSpace(custType))
            {
                sql += " AND A.CUST_TYPE = @p5 ";
                parameters.Add(new SqlParameter("@p5", custType));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                sql += " AND A.STATUS = @p6 ";
                parameters.Add(new SqlParameter("@p6", status));
            }

            var query = SqlQuery<F194707Ex>(sql, parameters.ToArray()).ToList();
            return query.AsQueryable();
        }

        public IQueryable<QuoteData> GetQuoteDatas()
        {
            var sql = @"SELECT ROW_NUMBER ()OVER(ORDER BY A.ALL_ID,A.ACC_AREA_ID,A.DC_CODE,A.DELV_EFFIC,A.DELV_TMPR,A.CUST_TYPE,A.LOGI_TYPE,A.ACC_KIND,A.ACC_DELVNUM_ID,A.ACC_TYPE,A.BASIC_VALUE) ROWNUM,
											 A.*,
											 B.NUM DELVNUM,
											 'ORDER' ACC_UNIT_NAME,
											 A.BASIC_VALUE ACC_NUM,
											 A.FEE APPROV_BASIC_FEE,
											 A.OVER_UNIT_FEE APPROV_OVER_FEE
									FROM F194707 A
											 JOIN F194709 B
													ON     A.DC_CODE = B.DC_CODE
														 AND A.ALL_ID = B.ALL_ID
														 AND A.ACC_DELVNUM_ID = B.ACC_DELVNUM_ID
								 WHERE A.STATUS = '0' ";
            var result = SqlQuery<QuoteData>(sql).ToList();
            return result.AsQueryable();
        }

        public IQueryable<F194707WithF19470801> GetF194707WithF19470801s(string dcCode, IEnumerable<string> allIds, IEnumerable<string> zipCodes)
        {
            var sql = @"SELECT DISTINCT A.ALL_ID,
										A.DC_CODE,
										A.ACC_AREA_ID,
										A.DELV_EFFIC,
										A.DELV_TMPR,
										A.CUST_TYPE,
										A.LOGI_TYPE,
										A.ACC_KIND,
										A.ACC_DELVNUM_ID,
										A.ACC_TYPE,
										A.BASIC_VALUE,
										A.MAX_WEIGHT,
										A.FEE,
										OVER_VALUE,
										A.OVER_UNIT_FEE,
										B.ZIP_CODE
						  FROM F194707 A
							   JOIN F19470801 B
								  ON     A.ACC_AREA_ID = B.ACC_AREA_ID
									 AND A.ALL_ID = B.ALL_ID
									 AND A.DC_CODE = B.DC_CODE
						 WHERE     A.STATUS = '0' AND A.LOGI_TYPE = '01'	-- 正物流
							   AND A.DC_CODE = @p0";

            var paramList = new List<object> { dcCode };
            sql += paramList.CombineSqlInParameters("AND A.ALL_ID", allIds);
            sql += paramList.CombineSqlInParameters("AND B.ZIP_CODE", zipCodes);
            return SqlQuery<F194707WithF19470801>(sql, paramList.ToArray());
        }
    }
}
