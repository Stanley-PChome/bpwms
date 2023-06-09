using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F20;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
namespace Wms3pl.Datas.F20
{
    public partial class F200101Repository : RepositoryBase<F200101, Wms3plDbContext, F200101Repository>
    {

        public IQueryable<F200101Data> GetF200101Datas(string dcCode, string gupCode, string custCode, string adjustNo,
            string adjustType, string workType, string begAdjustDate, string endAdjustDate)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
            };
            var sql = $@" SELECT 
	ROW_NUMBER()OVER(ORDER BY A.ADJUST_NO)AS ROWNUM,
	A.ADJUST_NO,A.ADJUST_TYPE,B.NAME AS ADJUST_TYPE_NAME,A.WORK_TYPE,ISNULL(C.NAME,'') AS WORK_TYPE_NAME, 
								        CASE WHEN D.ADJUST_NO IS NULL THEN 1 ELSE 0 END IsCanEdit,  -- IsCanEdit  0:不可修改(不是包裝前訂單) 1:可修改(包裝前訂單)
								        A.CRT_STAFF,A.CRT_DATE,A.CRT_NAME,A.UPD_STAFF,A.UPD_DATE,A.UPD_NAME, 
								        A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ADJUST_DATE 
					     FROM F200101 A 
					    INNER JOIN VW_F000904_LANG  B 
					       ON B.TOPIC = 'F200101' 
					      AND B.SUBTOPIC='ADJUST_TYPE' 
					      AND B.VALUE = A.ADJUST_TYPE 
						  AND B.LANG = '{Current.Lang}'
								   LEFT JOIN VW_F000904_LANG  C 
								     ON C.TOPIC = 'F200101' 
								    AND C.SUBTOPIC ='WORK_TYPE' 
								    AND C.VALUE = A.WORK_TYPE 
										AND C.LANG = '{Current.Lang}'
					     LEFT JOIN (  -- 包裝前訂單 有找到代表已為已包裝訂單 沒找到才是包裝前訂單 
					          SELECT DISTINCT B.DC_CODE,B.GUP_CODE,B.CUST_CODE,B.ADJUST_NO 
					            FROM F200102 B 
					           INNER JOIN F050301 C ON C.DC_CODE = B.DC_CODE AND C.GUP_CODE = B.GUP_CODE AND C.CUST_CODE = B.CUST_CODE AND C.ORD_NO = B.ORD_NO 
					           INNER JOIN F05030101 D ON D.DC_CODE = C.DC_CODE AND D.GUP_CODE = C.GUP_CODE AND D.CUST_CODE = C.CUST_CODE AND D.ORD_NO = C.ORD_NO 
					           INNER JOIN F050801 E ON  E.DC_CODE = D.DC_CODE AND E.GUP_CODE = D.GUP_CODE AND E.CUST_CODE = D.CUST_CODE AND E.WMS_ORD_NO = D.WMS_ORD_NO 
					           INNER JOIN F055001 F ON  F.DC_CODE = E.DC_CODE AND F.GUP_CODE = E.GUP_CODE AND F.CUST_CODE = E.CUST_CODE AND F.WMS_ORD_NO = E.WMS_ORD_NO 
					               ) D ON D.DC_CODE = A.DC_CODE 
					  								 AND D.GUP_CODE = A.GUP_CODE 
					  								 AND D.CUST_CODE = A.CUST_CODE 
					  								 AND D.ADJUST_NO = A.ADJUST_NO 
					    WHERE A.DC_CODE = @p0 
					      AND A.GUP_CODE = @p1 
					      AND A.CUST_CODE = @p2 
					      AND A.STATUS <> '9' "; // 排除已刪除調整單


            if (!string.IsNullOrEmpty(adjustNo))
            {
                sql += "    AND A.ADJUST_NO = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, adjustNo));
            }
            if (!string.IsNullOrEmpty(adjustType))
            {
                sql += "    AND A.ADJUST_TYPE = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, adjustType));
            }
            if (!string.IsNullOrEmpty(workType))
            {
                sql += "    AND A.WORK_TYPE = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, workType));
            }
            if (!string.IsNullOrEmpty(begAdjustDate))
            {
                sql += "    AND convert(varchar,A.ADJUST_DATE,111) >= convert(varchar,@p" + parameters.Count + ",111) ";
                parameters.Add(new SqlParameter("@p" + parameters.Count, begAdjustDate));
            }
            if (!string.IsNullOrEmpty(endAdjustDate))
            {
                sql += "    AND convert(varchar,A.ADJUST_DATE,111) <=  convert(varchar,@p" + parameters.Count + ",111)";
                parameters.Add(new SqlParameter("@p" + parameters.Count, endAdjustDate));
            }
            sql += " ORDER BY A.ADJUST_NO ";
            return SqlQuery<F200101Data>(sql, parameters.ToArray());
        }

        public IQueryable<F200101Data> GetF200101DatasByAdjustType1Or2(string dcCode, string gupCode, string custCode, string adjustNo,
            string adjustType, string workType, string begAdjustDate, string endAdjustDate)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
            };
            var sql = $@" SELECT ROW_NUMBER()OVER(ORDER BY A.ADJUST_NO )AS ROWNUM,A.ADJUST_NO,A.ADJUST_TYPE,B.NAME AS ADJUST_TYPE_NAME,A.WORK_TYPE,'' AS WORK_TYPE_NAME, 
								       1 AS IsCanEdit, 
								       A.CRT_STAFF,A.CRT_DATE,A.CRT_NAME,A.UPD_STAFF,A.UPD_DATE,A.UPD_NAME, 
								       A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ADJUST_DATE,A.SOURCE_TYPE,A.SOURCE_NO,C.SOURCE_NAME 
								  FROM F200101 A 
								 INNER JOIN VW_F000904_LANG  B 
								    ON B.TOPIC = 'F200101' 
								   AND B.SUBTOPIC='ADJUST_TYPE' 
								   AND B.VALUE = A.ADJUST_TYPE 
								   AND B.LANG = '{Current.Lang}' 
                  LEFT JOIN F000902 C ON C.SOURCE_TYPE = A.SOURCE_TYPE
								 WHERE A.DC_CODE = @p0 
								   AND A.GUP_CODE = @p1 
								   AND A.CUST_CODE = @p2 
								   AND A.STATUS <> '9' "; // 排除已刪除調整單


            if (!string.IsNullOrEmpty(adjustNo))
            {
                sql += "    AND A.ADJUST_NO = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, adjustNo));
            }
            if (!string.IsNullOrEmpty(adjustType))
            {
                sql += "    AND A.ADJUST_TYPE = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, adjustType));
            }
            if (!string.IsNullOrEmpty(workType))
            {
                sql += "    AND A.WORK_TYPE = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, workType));
            }
            if (!string.IsNullOrEmpty(begAdjustDate))
            {
                sql += "    AND convert(varchar,A.ADJUST_DATE,111) >= convert(varchar,@p" + parameters.Count + ",111) ";
                parameters.Add(new SqlParameter("@p" + parameters.Count, begAdjustDate));
            }
            if (!string.IsNullOrEmpty(endAdjustDate))
            {
                sql += "    AND convert(varchar,A.ADJUST_DATE,111) <=  convert(varchar,@p" + parameters.Count + ",111) ";
                parameters.Add(new SqlParameter("@p" + parameters.Count, endAdjustDate));
            }
            return SqlQuery<F200101Data>(sql, parameters.ToArray());
        }
    }
}
