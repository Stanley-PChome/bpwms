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
    public partial class F199007Repository : RepositoryBase<F199007, Wms3plDbContext, F199007Repository>
    {
        public IQueryable<F199007Data> GetProjectValuation(string dcCode, string gupCode, string custCode, DateTime? creDateS, DateTime? creDateE,
            string accProjectNo, DateTime? enableD, DateTime? disableD, string quoteNo, string status, string accProjectName)
        {
            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode)
            };

            var sql = @"SELECT a.*,CONVERT(varchar,a.ENABLE_DATE,111) + '~' + CONVERT(varchar,a.DISABLE_DATE,111) as PROJECT_DATE 
										FROM F199007 a
									 WHERE a.DC_CODE = @p0 
										 AND a.GUP_CODE = @p1
										 AND a.CUST_CODE = @p2 ";
            if (creDateS != null)
            {
                sql += string.Format("AND a.CRT_DATE >= @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(string.Format("@p{0}", sqlParameters.Count), creDateS));
            }

            if (creDateE != null)
            {
                sql += string.Format("AND a.CRT_DATE < @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(string.Format("@p{0}", sqlParameters.Count), creDateE.Value.AddDays(1)));
            }

            if (!string.IsNullOrEmpty(accProjectNo))
            {
                sql += string.Format("AND a.ACC_PROJECT_NO = @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(string.Format("@p{0}", sqlParameters.Count), accProjectNo));
            }

            if (enableD != null)
            {
                sql += string.Format("AND a.ENABLE_DATE >= @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(string.Format("@p{0}", sqlParameters.Count), enableD));
            }

            if (disableD != null)
            {
                sql += string.Format("AND a.DISABLE_DATE < @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(string.Format("@p{0}", sqlParameters.Count), disableD.Value.AddDays(1)));
            }

            if (!string.IsNullOrEmpty(quoteNo))
            {
                sql += string.Format("AND a.QUOTE_NO = @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(string.Format("@p{0}", sqlParameters.Count), quoteNo));
            }

            if (!string.IsNullOrEmpty(status))
            {
                sql += string.Format("AND a.STATUS = @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(string.Format("@p{0}", sqlParameters.Count), status));
            }
            else
            {
                sql += "AND a.STATUS <> '9' ";
            }

            if (!string.IsNullOrEmpty(accProjectName))
            {
                sql += string.Format("AND a.ACC_PROJECT_NAME = @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(string.Format("@p{0}", sqlParameters.Count), accProjectName));
            }

            var result =  SqlQuery<F199007Data>(sql, sqlParameters.ToArray()).AsQueryable();
            return result;
        }

        public IQueryable<QuoteData> GetQuoteDatas(string dcCode, string gupCode, string custCode, List<string> quotes)
        {
            var parameters = new List<object> {
                 dcCode,
                dcCode,
                gupCode,
                custCode
            };
            var inSql = parameters.CombineSqlInParameters("AND A.ACC_PROJECT_NO", quotes);
            var sql = @"SELECT A.DC_CODE,
						A.GUP_CODE,
						A.CUST_CODE,
						A.ACC_PROJECT_NO QUOTE_NO,
						A.ENABLE_DATE,
						A.DISABLE_DATE,
						A.FEE,
						A.ACC_KIND ,A.FEE APPROV_FEE,A.ACC_PROJECT_NAME ACC_ITEM_NAME,'01' DELV_ACC_TYPE
					    FROM F199007 A
					    WHERE (A.DC_CODE = @p0 OR @p1 = '000') AND A.GUP_CODE = @p2 AND A.CUST_CODE = @p3 " + inSql;
            var result = SqlQuery<QuoteData>(sql, parameters.ToArray()).ToList();
            for (int i = 0; i < result.Count; i++) { result[i].ROWNUM = i + 1; }
            return result.AsQueryable();
        }

        /// <summary>
		/// 結算關帳 專案計價項目回押結案
		/// </summary>
		public void UpdateF199007Status(DateTime cntDate, string gupCode, string custCode)
        {
            var sql = @"

						UPDATE F199007  SET STATUS ='2'
						WHERE EXISTS (
										SELECT * FROM F500201 S1,F199007 A WHERE S1.ITEM_TYPE_ID ='007'
																		AND S1.QUOTE_NO = A.QUOTE_NO
																		AND S1.CNT_DATE = @p0 
																		AND S1.GUP_CODE = @p1 
																		AND S1.CUST_CODE = @p2 
																		AND S1.STATUS = '0'
									  )
					";

            var param = new[] {
                new SqlParameter("@p0", cntDate),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode)
            };
            ExecuteSqlCommand(sql, param);
        }
    }
}
