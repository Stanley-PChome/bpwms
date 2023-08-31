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
    public partial class F1947Repository : RepositoryBase<F1947, Wms3plDbContext, F1947Repository>
    {
        public IQueryable<F1947WithF194701> GetF1947WithF194701Datas(string dcCode, string gupCode, string custCode, string delvTime)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", delvTime)
            };
            var sql = @"Select A.DC_CODE,A.ALL_ID,A.ALL_COMP,A.PIER_CODE,B.DELV_TIME
										From (
                                                SELECT F1947.*
                                                FROM F1947
                                                LEFT JOIN F194704
                                                    ON F1947.DC_CODE = F194704.DC_CODE
                                                    AND F1947.ALL_ID = F194704.ALL_ID
                                                WHERE F1947.DC_CODE = @p0
                                                AND (F194704.ALL_ID IS NULL OR F194704.GUP_CODE = @p1 AND F194704.CUST_CODE = @p2)
                                            ) A
										left Join (
                                                SELECT DC_CODE, ALL_ID, DELV_TIME,PAST_TYPE
                                                FROM F194701
                                                GROUP BY DC_CODE, ALL_ID, DELV_TIME,PAST_TYPE 
                                            ) B
											On A.DC_CODE=B.DC_CODE AND A.ALL_ID=B.ALL_ID
									 Where B.PAST_TYPE = '0' AND (B.DELV_TIME >= @p3 OR @p3 ='')
                                     ORDER BY B.DELV_TIME";

            return SqlQuery<F1947WithF194701>(sql, parameters.ToArray());
        }

        public IQueryable<F1947Ex> GetF1947ExQuery(string dcCode, string gupCode, string custCode, string allID, string allComp)
        {
            var paramList = new List<object>
                    {
                        dcCode,
                        gupCode,
                        custCode,
                        allID,
                         allID,
                        allID
                    };

            var sql = $@"SELECT A.DC_CODE, D.DC_NAME, A.ALL_ID, A.ALL_COMP, A.PIER_CODE,A.CHECK_ROUTE,A.TYPE,E.NAME AS TYPENAME,A.ALLOW_ROUND_PIECE
						FROM (
                               SELECT F1947.*
                               FROM F1947
                               LEFT JOIN F194704
                                 ON F1947.DC_CODE = F194704.DC_CODE
                                 AND F1947.ALL_ID = F194704.ALL_ID
                               WHERE F1947.DC_CODE = @p0
                               AND (F194704.ALL_ID IS NULL OR F194704.GUP_CODE = @p1 AND F194704.CUST_CODE = @p2)
                             ) A
						LEFT JOIN F1901 D ON A.DC_CODE = D.DC_CODE
						LEFT JOIN VW_F000904_LANG E ON E.TOPIC='F1947' AND E.SUBTOPIC='TYPE' AND VALUE = A.TYPE AND E.LANG = '{Current.Lang}'
             WHERE A.ALL_ID = CASE WHEN @p3 = '' or @p3 is null THEN A.ALL_ID ELSE @p4 END ";

            if (!string.IsNullOrWhiteSpace(allComp))
            {
                sql += string.Format(" AND A.ALL_COMP LIKE @p{0}", paramList.Count);
                paramList.Add("%" + allComp + "%");
            }

            return SqlQuery<F1947Ex>(sql, paramList.ToArray());
        }

        #region 商品召回
        public IQueryable<DistributionData> GetDistributionData(string dcCode, string gupCode, string custCode)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode)
            };
            //AND F194701.ZIP_CODE = F1934.ZIP_CODE(+)
            var sql = @"SELECT DISTINCT F1934.COUDIV_ID,F1933.COUDIV_NAME, F194701.ALL_ID, F1947.ALL_COMP
						  FROM (SELECT F1947.*
								  FROM F1947
									   LEFT JOIN F194704
										  ON     F1947.DC_CODE = F194704.DC_CODE
											 AND F1947.ALL_ID = F194704.ALL_ID
								 WHERE     F1947.DC_CODE = @p0
									   AND (   F194704.ALL_ID IS NULL
											OR     (F194704.GUP_CODE = @p1 AND F194704.CUST_CODE = @p2))) F1947,
							   F194701,F1933 left join F1934 on F1934.COUDIV_ID = F1933.COUDIV_ID
						 WHERE     F1947.DC_CODE = F194701.DC_CODE
							   
							   AND F1947.ALL_ID = F194701.ALL_ID
							   AND F194701.DC_CODE = @p0
							   AND F194701.PAST_TYPE = '1'
						";

            var result = SqlQuery<DistributionData>(sql, parameters.ToArray()).AsQueryable();
            return result;
        }
        #endregion

        /// <summary>
		/// 取出貨單配送商
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <returns></returns>
		public F1947 GetAllIdByWmsOrdNo(string wmsOrdNo, string gupCode, string custCode, string dcCode)
        {
            var sql = @"SELECT TOP(1) D.*
						FROM F700102 A
							INNER JOIN F700101 C
                                ON A.DISTR_CAR_NO = C.DISTR_CAR_NO AND A.DC_CODE = C.DC_CODE
                            INNER JOIN F1947 D
                                ON C.ALL_ID = D.ALL_ID AND C.DC_CODE = D.DC_CODE
                        WHERE A.WMS_NO = @p0
                        AND A.GUP_CODE = @p2
                        AND A.CUST_CODE = @p3
                        AND A.DC_CODE = @p1";

            var parameters = new object[]
            {
                wmsOrdNo,
                gupCode,
                custCode,
                dcCode,
            };

            return SqlQuery<F1947>(sql, parameters).SingleOrDefault();
        }

        public IQueryable<F1947JoinF194701> GetF1947JoinF194701Datas(string ALL_ID, string DC_CODE)
        {
            List<SqlParameter> Parameters = new List<SqlParameter>();
            string sql = $@"select b.DELV_TIME
                                 ,b.DELV_EFFIC
                                 ,b.DELV_TMPR  
                                 ,(select  NAME from VW_F000904_LANG where TOPIC='F194701' and SUBTOPIC='DELV_TMPR' and VALUE=b.DELV_TMPR  AND LANG = 'zh-TW') DELV_TMPR_NAME          
                                 ,(select  NAME from VW_F000904_LANG where TOPIC='F190102' and SUBTOPIC='DELV_EFFIC' and VALUE=b.DELV_EFFIC  AND LANG = 'zh-TW') DELV_EFFIC_NAME
                                 ,CASE WHEN charindex('0',DELV_FREQ) =0 THEN '0' ELSE '1' END AS Sun
																 ,CASE WHEN charindex('1',DELV_FREQ) =0 THEN '0' ELSE '1' END AS Mon
															   ,CASE WHEN charindex('2',DELV_FREQ) =0 THEN '0' ELSE '1' END AS Tue
																 ,CASE WHEN charindex('3',DELV_FREQ) =0 THEN '0' ELSE '1' END AS Wed
																 ,CASE WHEN charindex('4',DELV_FREQ) =0 THEN '0' ELSE '1' END AS Thu
																 ,CASE WHEN charindex('5',DELV_FREQ) =0 THEN '0' ELSE '1' END AS Fri
																 ,CASE WHEN charindex('6',DELV_FREQ) =0 THEN '0' ELSE '1' END AS Sat     
																 ,b.PAST_TYPE, b.DELV_TIMES
                           from F1947 a join F194701 b
                           on a.ALL_ID=b.ALL_ID 
                           and a.DC_CODE=b.DC_CODE 
                           where a.ALL_ID=@p0
                           and a.DC_CODE=@p1  ";

            Parameters.Add(new SqlParameter("@p0", ALL_ID));
            Parameters.Add(new SqlParameter("@p1", DC_CODE));

            var result = SqlQuery<F1947JoinF194701>(sql.ToString(), Parameters.ToArray()).AsQueryable();
            return result;

        }

        /// <summary>
        /// 取得配送商資料
        /// </summary>
        /// <param name="dcCode"></param>
        /// <returns></returns>
        public IQueryable<F1947> GetDatas(string dcCode)
        {
            List<SqlParameter> Parameters = new List<SqlParameter>();
            Parameters.Add(new SqlParameter("@p0", dcCode) { SqlDbType = System.Data.SqlDbType.VarChar });

            string sql = @"SELECT * FROM F1947 WHERE DC_CODE = @p0 ";

            return SqlQuery<F1947>(sql.ToString(), Parameters.ToArray());
        }
    }
}
