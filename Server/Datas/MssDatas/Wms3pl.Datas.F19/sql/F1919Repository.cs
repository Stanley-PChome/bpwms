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
    public partial class F1919Repository : RepositoryBase<F1919, Wms3plDbContext, F1919Repository>
    {
        public IQueryable<F1919Data> GetF1919Datas(string dcCode, string gupCode, string custCode, string warehourseId, string areaCode)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0",  dcCode),
                new SqlParameter("@p1", 1),
               new SqlParameter("@p2", Current.Lang)
            };

            var sql = @" 
                        SELECT ROW_NUMBER()OVER(ORDER BY Z.WAREHOUSE_ID,Z.DC_CODE,Z.AREA_CODE,Z.GUP_CODE,Z.CUST_CODE) ROWNUM, Z.* FROM (
						SELECT DISTINCT TOP 100 PERCENT A.*,
							   B.GUP_CODE,
							   B.CUST_CODE,
							   B.FLOOR,
							   B.MINCHANNEL,
							   B.MAXCHANNEL,
							   B.MINPLAIN,
							   B.MAXPLAIN,
							   B.MINLOC_LEVEL,
							   B.MAXLOC_LEVEL,
							   B.MINLOC_TYPE,
							   B.MAXLOC_TYPE,
							   C.ATYPE_NAME,
							   CASE B.GUP_CODE
								  WHEN NULL THEN N''
								  WHEN '0' THEN N'共用'
								  ELSE D.GUP_NAME
							   END
								  GUP_NAME,
							   CASE B.CUST_CODE
								  WHEN NULL THEN N''
								  WHEN '0' THEN N'共用'
								  ELSE E.CUST_NAME
							   END
								  CUST_NAME,
							   G.WAREHOUSE_NAME,
							   F.DC_NAME,
							   I.VALUE AS PICK_TYPE,
							   I.NAME AS PICK_TYPE_NAME,
							   J.VALUE AS PICK_TOOL,
							   J.NAME AS PICK_TOOL_NAME,
							   K.VALUE AS PUT_TOOL,
							   K.NAME AS PUT_TOOL_NAME,
                               H.PICK_UNIT AS PICK_UNIT,
							   H.PICK_MARTERIAL AS PICK_MARTERIAL,
							   H.DELIVERY_MARTERIAL AS DELIVERY_MARTERIAL,
							   H.PICK_SEQ AS PICK_SEQ,
							   H.SORT_BY AS SORT_BY,
							   L.VALUE AS MOVE_TOOL,
							   L.NAME AS MOVE_TOOL_NAME
						  FROM F1919 A
							   LEFT JOIN F191902 H ON A.AREA_CODE = H.AREA_CODE 
										 AND A.WAREHOUSE_ID = H.WAREHOUSE_ID 
										 AND A.DC_CODE = H.DC_CODE
							   LEFT JOIN
							   (SELECT VALUE, NAME
								  FROM VW_F000904_LANG
								 WHERE     TOPIC = 'F191902'
									   AND SUBTOPIC = 'PICK_TYPE'
									   AND ISUSAGE = @p1
									   AND LANG = @p2) I
								  ON H.PICK_TYPE = I.VALUE
							   LEFT JOIN
							   (SELECT VALUE, NAME
								  FROM VW_F000904_LANG
								 WHERE     TOPIC = 'F191902'
									   AND SUBTOPIC = 'PICK_TOOL'
									   AND ISUSAGE = @p1
									   AND LANG = @p2) J
								  ON H.PICK_TOOL = J.VALUE
							   LEFT JOIN
							   (SELECT VALUE, NAME
								  FROM VW_F000904_LANG
								 WHERE     TOPIC = 'F191902'
									   AND SUBTOPIC = 'PUT_TOOL'
									   AND ISUSAGE = @p1
									   AND LANG = @p2) K
								  ON H.PUT_TOOL = K.VALUE
                               LEFT JOIN
							   (SELECT VALUE, NAME
								  FROM VW_F000904_LANG
								 WHERE     TOPIC = 'F191902'
									   AND SUBTOPIC = 'MOVE_TOOL'
									   AND ISUSAGE = '1'
									   AND LANG = 'zh-TW') L
								  ON H.MOVE_TOOL = L.VALUE
							   LEFT JOIN
							   (  SELECT A.DC_CODE,
										 A.WAREHOUSE_ID,
										 A.AREA_CODE,
										 B.GUP_CODE,
										 B.CUST_CODE,
										 MAX (B.FLOOR) FLOOR,
										 MIN (B.CHANNEL) MINCHANNEL,
										 MAX (B.CHANNEL) MAXCHANNEL,
										 MIN (B.PLAIN) MINPLAIN,
										 MAX (B.PLAIN) MAXPLAIN,
										 MIN (B.LOC_LEVEL) MINLOC_LEVEL,
										 MAX (B.LOC_LEVEL) MAXLOC_LEVEL,
										 MIN (B.LOC_TYPE) MINLOC_TYPE,
										 MAX (B.LOC_TYPE) MAXLOC_TYPE
									FROM F1919 A
										 INNER JOIN F1912 B
											ON     A.DC_CODE = B.DC_CODE
											   AND A.WAREHOUSE_ID = B.WAREHOUSE_ID
											   AND A.AREA_CODE = B.AREA_CODE
								GROUP BY A.DC_CODE,
										 A.WAREHOUSE_ID,
										 A.AREA_CODE,
										 B.GUP_CODE,
										 B.CUST_CODE) B
								  ON     A.DC_CODE = B.DC_CODE
									 AND A.WAREHOUSE_ID = B.WAREHOUSE_ID
									 AND A.AREA_CODE = B.AREA_CODE
							   LEFT JOIN F191901 C ON A.ATYPE_CODE = C.ATYPE_CODE
							   LEFT JOIN F1929 D ON B.GUP_CODE = D.GUP_CODE
							   LEFT JOIN F1909 E ON B.CUST_CODE = E.CUST_CODE
							   LEFT JOIN F1901 F ON A.DC_CODE = F.DC_CODE
							   LEFT JOIN F1980 G
								  ON A.DC_CODE = G.DC_CODE AND A.WAREHOUSE_ID = G.WAREHOUSE_ID
						 WHERE A.DC_CODE = @p0 ";
            if (!string.IsNullOrEmpty(warehourseId))
            {
                sql += "     AND A.WAREHOUSE_ID = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, warehourseId));
            }
            if (!string.IsNullOrEmpty(gupCode))
            {
                sql += "    AND B.GUP_CODE     = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, gupCode));
            }
            else //業主全部要去篩選只有此物流中心業主或業主設為共用
            {
                sql += " AND ((B.AREA_CODE IS NULL) OR ((B.GUP_CODE ='0') OR (EXISTS (SELECT 1 FROM F190101 WHERE DC_CODE = A.DC_CODE AND GUP_CODE = B.GUP_CODE)))) ";
            }
            if (!string.IsNullOrEmpty(custCode))
            {
                sql += "    AND B.CUST_CODE    = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, custCode));
            }
            else //雇主全部要去篩選只有此物流中心且為此業主的雇主或雇主設為共用
            {
                sql += " AND ((B.AREA_CODE IS NULL) OR ((B.CUST_CODE ='0') OR (EXISTS (SELECT 1 FROM F190101 WHERE DC_CODE = A.DC_CODE AND GUP_CODE = B.GUP_CODE AND CUST_CODE = B.CUST_CODE)))) ";
            }
            if (!string.IsNullOrEmpty(areaCode))
            {
                sql += " AND A.AREA_CODE = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, areaCode));
            }


            sql += "    ORDER BY A.WAREHOUSE_ID,A.DC_CODE,A.AREA_CODE,B.GUP_CODE,B.CUST_CODE ) Z ";


            var result = SqlQuery<F1919Data>(sql, parameters.ToArray());
            return result;
        }

        public void P710101Delete(string dcCode, string warehouseId, List<string> areaCode)
        {
            var param = new List<object> { dcCode, warehouseId };
            string sql = @"DELETE F1919 WHERE DC_CODE = @p0 AND WAREHOUSE_ID = @p1 ";
            sql += param.CombineSqlInParameters(" AND AREA_CODE", areaCode); ;
            ExecuteSqlCommand(sql, param.ToArray());
        }

        public IQueryable<F1919> GetDatasByCanToShip(string dcCode, string gupCode, string custCode)
        {
            var sql = @" SELECT DISTINCT A.*
										FROM F1919 A
										JOIN F1980 B
										ON B.DC_CODE = A.DC_CODE
										AND B.WAREHOUSE_ID = A.WAREHOUSE_ID
										JOIN F198001 C
										ON C.TYPE_ID = B.WAREHOUSE_TYPE
										JOIN F190002 D
										ON D.WAREHOUSE_TYPE = C.TYPE_ID
										JOIN F190001 E
										ON E.DC_CODE = D.DC_CODE
										AND E.TICKET_ID = D.TICKET_ID
										WHERE E.TICKET_CLASS LIKE 'O%'
										AND E.DC_CODE = @p0
										AND E.GUP_CODE = @p1
										AND E.CUST_CODE = @p2";
            var parms = new List<object> { dcCode, gupCode, custCode };
            return SqlQuery<F1919>(sql, parms.ToArray());
        }
    }
}
