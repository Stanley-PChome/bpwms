using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
    public partial class F000904Repository : RepositoryBase<F000904, Wms3plDbContext, F000904Repository>
    {
        public IQueryable<F000904DelvAccType> GetDelvAccTypes(string itemTypeId, string accItemKindId)
        {
            var sql = @"SELECT 
                        row_number()over(order by b.VALUE) as ROWNUM,
                        b.VALUE as DELV_ACC_TYPE ,
                        b.NAME as DELV_ACC_TYPE_NAME
										FROM F91000301 a 
							 LEFT JOIN (SELECT VALUE,NAME 
														FROM VW_F000904_LANG 
													 WHERE TOPIC = 'F91000301' AND SUBTOPIC = 'DELV_ACC_TYPE' AND LANG = @p2)  
										b ON a.DELV_ACC_TYPE = b.VALUE
									 WHERE a.ITEM_TYPE_ID = @p0 AND a.ACC_ITEM_KIND_ID = @p1 ";

            var param = new List<SqlParameter> {
                new SqlParameter("@p0", itemTypeId),
                new SqlParameter("@p1", accItemKindId),
                new SqlParameter("@p2", Current.Lang)
            };

            return SqlQuery<F000904DelvAccType>(sql, param.ToArray()).AsQueryable();
        }

        public IQueryable<WorkList> GetWorkListDatas(string dcCode, string gupCode, string custCode, string apType, string account)
        {
            var sql = @" 
                     SELECT 
	                           ROW_NUMBER()OVER(order by Z.VALUE) ROWNUM,
		                       Z.VALUE,
		                       Z.NAME,
		                       Z.FUNC_ID,
		                       Y.FUN_NAME FUNC_NAME,
		                       Z.COUNTS,
		                       Z.COUNTS_B,
		                       Z.COUNTS_C
	                      FROM (
		                    SELECT	 a.VALUE,
				                     a.NAME,
				                     b.FUNC_ID,
				                     b.FUNC_NAME,
				                     ISNULL (b.COUNTS, 0) COUNTS,
				                     ISNULL (b.COUNT_B, 0) COUNTS_B,
				                     ISNULL (b.COUNT_C, 0) COUNTS_C
		                    FROM VW_F000904_LANG a With(nolock)
		                    LEFT JOIN 
							(SELECT '01' AS WORK_TYPE, 'office' AS APP_TYPE, 'P0102010000' AS FUNC_ID,'進倉單維護' AS FUNC_NAME,ISNULL(SUM(CASE WHEN STATUS <>'9' AND STATUS <>'2' THEN 1 ELSE 0 END),0) COUNTS,ISNULL(SUM(CASE WHEN STATUS <>'9' AND CRT_DATE > CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_B,ISNULL(SUM(CASE WHEN STATUS <>'9' AND STATUS <>'2' AND CRT_DATE > CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_C 
		                       FROM F010201 With(nolock)
		                      WHERE 1=1
			                    AND DC_CODE =@p0
			                    AND GUP_CODE= @p1
			                    AND CUST_CODE= @p2
		                      
		                      UNION
			                    SELECT '02' AS WORK_TYPE, 'office' AS APP_TYPE, 'P1601010000' AS FUNC_ID,'退貨單維護' AS FUNC_NAME,ISNULL(SUM(CASE WHEN STATUS ='0' THEN 1 ELSE 0 END),0) COUNTS,ISNULL(SUM(CASE WHEN STATUS <>'9' AND RETURN_DATE >= CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_B,ISNULL(SUM(CASE WHEN STATUS <>'9' AND STATUS <>'2' AND RETURN_DATE >= CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_C 
			                    FROM F161201 With(nolock)
			                    WHERE 1=1
			                    AND DC_CODE =@p0
			                    AND GUP_CODE= @p1
			                    AND CUST_CODE= @p2
			                    
			                    UNION
			                    SELECT '04' AS WORK_TYPE, 'office' AS APP_TYPE, 'P1502010000' AS FUNC_ID,'調撥單維護' AS FUNC_NAME,ISNULL(SUM(CASE WHEN STATUS <>'9' AND STATUS <>'5' THEN 1 ELSE 0 END),0) COUNTS,ISNULL(SUM(CASE WHEN STATUS <>'9' AND CRT_DATE > CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_B,ISNULL(SUM(CASE WHEN STATUS <>'9' AND STATUS <>'5' AND CRT_DATE > CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_C 
			                    FROM F151001 With(nolock)
			                    WHERE 1=1
			                    AND DC_CODE =@p0
			                    AND GUP_CODE= @p1
			                    AND CUST_CODE= @p2
			                    
			                    UNION
			                    SELECT '05' AS WORK_TYPE, 'office' AS APP_TYPE, 'P1401010000' AS FUNC_ID,'盤點單維護' AS FUNC_NAME,ISNULL(SUM(CASE WHEN STATUS <>'9' AND STATUS ='0' THEN 1 ELSE 0 END),0) COUNTS,ISNULL(SUM(CASE WHEN STATUS <>'9' AND CRT_DATE > CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_B,ISNULL(SUM(CASE WHEN STATUS <>'9' AND STATUS ='0' AND CRT_DATE > CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_C 
			                    FROM F140101 With(nolock)
			                    WHERE 1=1
			                    AND DC_CODE =@p0
			                    AND GUP_CODE= @p1
			                    AND CUST_CODE= @p2
			                    
			                    UNION
			                    SELECT '06' AS WORK_TYPE,'office' AS APP_TYPE,'P0503020000' AS FUNC_ID,'訂單維護' AS FUNC_NAME,ISNULL(SUM(CASE WHEN A.STATUS<>'9' AND C.WMS_ORD_NO IS  NULL THEN 1 ELSE 0 END),0) COUNTS,ISNULL(SUM(CASE WHEN A.STATUS<>'9' AND A.CRT_DATE > CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_B,ISNULL(SUM(CASE WHEN A.STATUS<>'9' AND C.WMS_ORD_NO IS  NULL AND A.CRT_DATE > CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_C 
			                    FROM F050101 A With(nolock)
			                    LEFT JOIN F05030101 B With(nolock)
			                    ON B.DC_CODE = A.DC_CODE
			                    AND B.GUP_CODE = A.GUP_CODE
			                    AND B.CUST_CODE = A.CUST_CODE
			                    AND B.ORD_NO = A.ORD_NO
			                    LEFT JOIN F050801 C With(nolock)
			                    ON C.DC_CODE = B.DC_CODE
			                    AND C.GUP_CODE = B.GUP_CODE
			                    AND C.CUST_CODE = B.CUST_CODE
			                    AND C.WMS_ORD_NO = B.WMS_ORD_NO
			                    WHERE 1=1
			                    AND A.DC_CODE =@p0
			                    AND A.GUP_CODE= @p1
			                    AND A.CUST_CODE= @p2
			                    UNION
			                    SELECT '06' AS WORK_TYPE, 'office' AS APP_TYPE, 'P1602010000' AS FUNC_ID,'廠退單維護' AS FUNC_NAME,ISNULL(SUM(CASE WHEN STATUS ='0' THEN 1 ELSE 0 END),0) COUNTS,ISNULL(SUM(CASE WHEN STATUS <>'9' AND RTN_VNR_DATE >= CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_B,ISNULL(SUM(CASE WHEN STATUS <>'9' AND STATUS <> '2' AND RTN_VNR_DATE >= CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_C 
			                    FROM F160201 With(nolock)
			                    WHERE 1=1
			                    AND DC_CODE =@p0
			                    AND GUP_CODE= @p1
			                    AND CUST_CODE= @p2) b 
		                    ON a.VALUE = b.WORK_TYPE
		                    WHERE 
								a.TOPIC = 'P2115010000' AND a.SUBTOPIC = 'WORK_TYPE' AND b.APP_TYPE = @p3 AND a.LANG = @p4
		                    
							) Z INNER JOIN F1954_I18N Y With(nolock) ON Z.FUNC_ID = Y.FUN_CODE AND Y.LANG = @p4 
                   ";
            var param = new List<SqlParameter> {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", apType),
                new SqlParameter("@p4", Current.Lang),
                new SqlParameter("@p5", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };

            //補 ROWNUM、
            //var dataList = _wms3plDbContext.SqlQuery<WorkList>(sql, param.ToArray()).ToList(); 
            //for (int i = 0; i < dataList.Count(); i++) 
            //    dataList[i].ROWNUM = i+1;
            //補 order by
            return SqlQuery<WorkList>(sql, param.ToArray()).AsQueryable();
        }

        public IQueryable<WorkList> GetChartListDatas(string dcCode, string gupCode, string custCode, string apType, string account)
        {
            var sql = @"
                SELECT ROW_NUMBER()OVER(ORDER BY Z.VALUE)ROWNUM,
		                   Z.VALUE,
		                   Z.NAME,
		                   Z.FUNC_ID,
		                   Y.FUN_NAME FUNC_NAME,
		                   Z.COUNTS,
		                   Z.COUNTS_B,
		                   Z.COUNTS_C
	                  FROM (
		                SELECT		 a.VALUE,
					                 a.NAME,
					                 b.FUNC_ID,
					                 b.FUNC_NAME,
					                 ISNULL (b.COUNTS, 0) COUNTS,
					                 ISNULL (b.COUNT_B, 0) COUNTS_B,
					                 ISNULL (b.COUNT_C, 0) COUNTS_C
		                FROM VW_F000904_LANG a With(nolock)
			                LEFT JOIN 
			                (
				                SELECT '03' AS WORK_TYPE, 'office' AS APP_TYPE, 'P0501040000' AS FUNC_ID,'揀貨單列印' AS FUNC_NAME,ISNULL(SUM(CASE WHEN PICK_STATUS <>'9' AND PICK_STATUS <>'2' THEN 1 ELSE 0 END),0) COUNTS,ISNULL(SUM(CASE WHEN PICK_STATUS <>'9' AND CRT_DATE > CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_B,ISNULL(SUM(CASE WHEN PICK_STATUS <>'9' AND PICK_STATUS <>'2' AND CRT_DATE > CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_C 
				                FROM F051201 With(nolock)
				                WHERE 1=1
				                AND DC_CODE =@p0
				                AND GUP_CODE= @p1
				                AND CUST_CODE= @p2
				                UNION
				                SELECT '06' AS WORK_TYPE, 'office' AS APP_TYPE, 'P0807010000' AS FUNC_ID,'出貨包裝/包裝站' AS FUNC_NAME,ISNULL(SUM(CASE WHEN STATUS <>'9' AND STATUS ='0' THEN 1 ELSE 0 END),0) COUNTS,ISNULL(SUM(CASE WHEN STATUS <>'9'  AND CRT_DATE > CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_B,ISNULL(SUM(CASE WHEN STATUS <>'9' AND STATUS ='0' AND CRT_DATE > CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_C 
				                FROM F050801 With(nolock)
				                WHERE 1=1
				                AND DC_CODE =@p0
				                AND GUP_CODE= @p1
				                AND CUST_CODE= @p2
				                UNION
				                SELECT '06' AS WORK_TYPE, 'office' AS APP_TYPE, 'P0808030000' AS FUNC_ID,'宅單扣帳' AS FUNC_NAME,ISNULL(SUM(CASE WHEN STATUS <>'9' AND STATUS ='2' THEN 1 ELSE 0 END),0) COUNTS,ISNULL(SUM(CASE WHEN STATUS <>'9' AND STATUS IN('2','5','6') AND CRT_DATE > CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_B,ISNULL(SUM(CASE WHEN STATUS <>'9' AND STATUS ='2' AND CRT_DATE > CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_C 
				                FROM F050801 With(nolock)
				                WHERE 1=1
				                AND DC_CODE =@p0
				                AND GUP_CODE= @p1
				                AND CUST_CODE= @p2
				               
				                UNION
			                SELECT '06' AS WORK_TYPE,'office' AS APP_TYPE,'P0503020000' AS FUNC_ID,'訂單維護' AS FUNC_NAME,ISNULL(SUM(CASE WHEN A.STATUS<>'9' AND C.WMS_ORD_NO IS  NULL THEN 1 ELSE 0 END),0) COUNTS,ISNULL(SUM(CASE WHEN A.STATUS<>'9' AND A.CRT_DATE > CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_B,ISNULL(SUM(CASE WHEN A.STATUS<>'9' AND C.WMS_ORD_NO IS  NULL AND A.CRT_DATE > CONVERT(date, @p5) THEN 1 ELSE 0 END),0) COUNT_C 
				                FROM F050101 A With(nolock)
				                LEFT JOIN F05030101 B With(nolock)
				                ON B.DC_CODE = A.DC_CODE
				                AND B.GUP_CODE = A.GUP_CODE
				                AND B.CUST_CODE = A.CUST_CODE
				                AND B.ORD_NO = A.ORD_NO
				                LEFT JOIN F050801 C With(nolock)
				                ON C.DC_CODE = B.DC_CODE
				                AND C.GUP_CODE = B.GUP_CODE
				                AND C.CUST_CODE = B.CUST_CODE
				                AND C.WMS_ORD_NO = B.WMS_ORD_NO
				                WHERE 1=1
				                AND A.DC_CODE =@p0
				                AND A.GUP_CODE= @p1
				                AND A.CUST_CODE= @p2) b
			                ON a.VALUE = b.WORK_TYPE
			                WHERE     a.TOPIC = 'P2115010000'
					                AND a.SUBTOPIC = 'WORK_TYPE'
					                AND b.APP_TYPE = @p3
					                AND a.LANG = @p4
			                ) Z  INNER JOIN F1954_I18N Y With(nolock) ON Z.FUNC_ID = Y.FUN_CODE AND Y.LANG = @p4 
	                
                            ";
            var param = new List<SqlParameter> {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", apType),
                new SqlParameter("@p4", Current.Lang),
                new SqlParameter("@p5", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };

            return SqlQuery<WorkList>(sql, param.ToArray()).AsQueryable();
        }

        public string GetWorkStationGroupCode()
        {
            var sql = @"
                SELECT TOP(1) VALUE
	                FROM F000904
                 WHERE TOPIC = 'F1946'
                   AND SUBTOPIC  = 'GROUP'
                   AND NAME = '驗貨區'
                            ";

            return SqlQuery<string>(sql).SingleOrDefault();
        }

        public string GetF010201StatusName(string value)
        {
            var sqlParameter = new List<SqlParameter>()
            {
                new SqlParameter("@p0", value) { SqlDbType = SqlDbType.VarChar },
            };
            var sql = @"
                SELECT TOP(1) NAME
	                FROM F000904
                 WHERE TOPIC = 'F010201'
                   AND SUBTOPIC  = 'STATUS'
                   AND VALUE = @p0
                ";

            return SqlQuery<string>(sql,sqlParameter.ToArray()).FirstOrDefault();
        }
    }
}
