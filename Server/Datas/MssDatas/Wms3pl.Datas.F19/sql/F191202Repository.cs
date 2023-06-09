using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F191202Repository : RepositoryBase<F191202, Wms3plDbContext, F191202Repository>
	{
        /// <summary>
        /// Log F1912的各項變更
        /// </summary>
        /// <param name="data">F1912原始資料</param>
        /// <param name="userId">異動人員</param>
        /// <param name="transStatus">異動作業類型(0新增,1刪除,2屬性修改)</param>
        /// <param name="transWay">異動狀態(0修改前資料,1修改後資料)</param>
        public void Log(F1912 data, string userId, string transStatus, string transWay)
        {
            var tmp = Mapper.DynamicMap<F191202>(data);
            tmp.TRANS_DATE = DateTime.Now;
            tmp.TRANS_STAFF = userId;
            tmp.TRANS_STATUS = transStatus;
            tmp.TRANS_WAY = transWay;
            this.Add(tmp, "TRANS_NO");
        }

        /// <summary>
        /// P7103010000 取得異動記錄
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="locCode"></param>
        /// <param name="startDt"></param>
        /// <param name="endDt"></param>
        /// <param name="locStatus"></param>
        /// <param name="warehouseType"></param>
        /// <returns></returns>
        public IQueryable<F191202Ex> GetLogs(string dcCode, string gupCode, string custCode
            , string locCode, DateTime startDt, DateTime endDt, string locStatus, string warehouseType, string account)
        {

            var parameters = new List<SqlParameter> {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", locCode),
                new SqlParameter("@p4", startDt.ToString("yyyy/MM/dd")),
                new SqlParameter("@p5", endDt.ToString("yyyy/MM/dd")),
                new SqlParameter("@p6", locStatus),
                new SqlParameter("@p7", warehouseType)
            };

            string sql = @"
        		  SELECT ROW_NUMBER()OVER(ORDER BY TRANS_NO) AS ROW_NUM, LOC.TRANS_DATE
                             , CASE LOC.CUST_CODE WHEN '0' THEN N'共用' ELSE CUST.CUST_NAME END CUST_NAME, 
                             WAREHOUSE.TYPE_NAME AS WAREHOUSE_TYPE_NAME
							 , LOC.LOC_CODE
                             , CASE LOC.TRANS_STATUS WHEN '0' THEN '新增' WHEN '1' THEN '刪除' WHEN '2' THEN '屬性修改' END 
                               + CASE LOC.TRANS_WAY WHEN '0' THEN '前' WHEN '1' THEN '後' ELSE '(未知)' END AS TRANS_STATUS
                             , LOCSTATUS.LOC_STATUS_NAME
                             , STAFF.EMP_NAME
                        FROM F191202 LOC
						left join F1943 LOCSTATUS on LOC.NOW_STATUS_ID = LOCSTATUS.LOC_STATUS_ID
						left join F1909 CUST ON LOC.GUP_CODE = CUST.GUP_CODE AND LOC.CUST_CODE = CUST.CUST_CODE
						left join F1924 STAFF ON LOC.TRANS_STAFF = STAFF.EMP_ID
						LEFT JOIN (SELECT WH.DC_CODE, WH.WAREHOUSE_ID, WH.WAREHOUSE_TYPE, WHT.TYPE_NAME
                                  FROM F1980 WH
							  left join F198001 WHT on WH.WAREHOUSE_TYPE = WHT.TYPE_ID
                               ) WAREHOUSE 
							   on LOC.WAREHOUSE_ID = WAREHOUSE.WAREHOUSE_ID  AND LOC.DC_CODE = WAREHOUSE.DC_CODE  
                       WHERE 
                         LOC.DC_CODE = @p0
                        AND (LOC.GUP_CODE = @p1 OR @p1 = '0')
                         AND (LOC.CUST_CODE = @p2 OR @p2 = '0')
                         AND (LOC.LOC_CODE = @p3 OR @p3 ='')
                         AND (LOC.TRANS_DATE) >= (@p4) AND LOC.TRANS_DATE < (@p5)
                         AND (LOC.NOW_STATUS_ID = @p6 OR @p6 = '0')
                      AND (WAREHOUSE.WAREHOUSE_TYPE = @p7 OR @p7 = '0')
        	";

            if (gupCode == "0")
            {
                sql += string.Format(@" AND (EXISTS (SELECT 1 
        																           FROM F190101 aa 
        																     INNER JOIN (SELECT * 
        																     				       FROM F192402 
        																     						  WHERE EMP_ID = @p{0}) bb ON aa.DC_CODE = bb.DC_CODE AND aa.GUP_CODE = bb.GUP_CODE 
        																     		  WHERE aa.DC_CODE = LOC.DC_CODE AND aa.GUP_CODE = LOC.GUP_CODE)) ", parameters.Count);
                parameters.Add(new SqlParameter("@p" + parameters.Count, account));
            }
            if (custCode == "0")
            {
                sql += string.Format(@" AND (EXISTS (SELECT 1 
        																						FROM F190101 cc 
        																			INNER JOIN (SELECT * 
        																    								  FROM F192402 
        																    								 WHERE EMP_ID = @p{0}) dd ON cc.DC_CODE = dd.DC_CODE AND cc.GUP_CODE = dd.GUP_CODE 
        																    					WHERE cc.DC_CODE = LOC.DC_CODE AND cc.GUP_CODE = LOC.GUP_CODE AND cc.CUST_CODE = LOC.CUST_CODE)) ", parameters.Count);
                parameters.Add(new SqlParameter("@p" + parameters.Count, account));
            }
            var result = SqlQuery<F191202Ex>(sql, parameters.ToArray()).AsQueryable();

            return result;

        }
    }
}
