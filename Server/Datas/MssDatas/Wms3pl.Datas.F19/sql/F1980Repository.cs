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
    public partial class F1980Repository : RepositoryBase<F1980, Wms3plDbContext, F1980Repository>
    {
        public IQueryable<F1980Data> GetF1980Datas(string dcCode, string gupCode, string custCode, string typeId, string account)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0",  dcCode),
            };
            var sql = @"SELECT
	                        ROW_NUMBER() OVER (
                        ORDER BY
	                        A.WAREHOUSE_ID,
	                        A.DC_CODE,
	                        B.GUP_CODE,
	                        B.CUST_CODE ) AS ROWNUM,
	                        A.*,
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
	                        C.LOC_TYPE_NAME,
	                        C.HANDY,
	                        CASE
		                        B.GUP_CODE WHEN NULL THEN N''
		                        WHEN '0' THEN N'共用'
		                        ELSE D.GUP_NAME
	                        END GUP_NAME,
	                        CASE
		                        B.CUST_CODE WHEN NULL THEN N''
		                        WHEN '0' THEN N'共用'
		                        ELSE E.CUST_NAME
	                        END CUST_NAME,
	                        F.DC_NAME
                        FROM
	                        F1980 A
                        LEFT JOIN (
	                        SELECT
		                        A.WAREHOUSE_ID,
		                        A.DC_CODE,
		                        B.GUP_CODE,
		                        B.CUST_CODE,
		                        MAX(B.FLOOR) FLOOR,
		                        MIN(B.CHANNEL) MINCHANNEL,
		                        MAX(B.CHANNEL) MAXCHANNEL,
		                        MIN(B.PLAIN) MINPLAIN,
		                        MAX(B.PLAIN) MAXPLAIN,
		                        MIN(B.LOC_LEVEL) MINLOC_LEVEL,
		                        MAX(B.LOC_LEVEL) MAXLOC_LEVEL,
		                        MIN(B.LOC_TYPE) MINLOC_TYPE,
		                        MAX(B.LOC_TYPE) MAXLOC_TYPE
	                        FROM
		                        F1980 A
	                        INNER JOIN F1912 B ON
		                        A.WAREHOUSE_ID = B.WAREHOUSE_ID AND A.DC_CODE =B.DC_CODE 
	                        GROUP BY
		                        A.WAREHOUSE_ID,
		                        A.DC_CODE,
		                        B.GUP_CODE,
		                        B.CUST_CODE ) B ON
	                        A.WAREHOUSE_ID = B.WAREHOUSE_ID
	                        AND B.GUP_CODE = B.GUP_CODE
	                        AND B.CUST_CODE = B.CUST_CODE
	                        AND A.DC_CODE =B.DC_CODE 
                        LEFT JOIN F1942 C ON
	                        A.LOC_TYPE_ID = C.LOC_TYPE_ID
                        LEFT JOIN F1929 D ON
	                        B.GUP_CODE = D.GUP_CODE
                        LEFT JOIN F1909 E ON
	                        B.CUST_CODE = E.CUST_CODE
                        LEFT JOIN F1901 F ON
	                        A.DC_CODE = F.DC_CODE
                        WHERE
	                        A.DC_CODE = @p0";
            if (!string.IsNullOrEmpty(typeId))
            {
                sql += "     AND A.WAREHOUSE_TYPE = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, typeId));
            }
            if (!string.IsNullOrEmpty(gupCode))
            {
                sql += "    AND B.GUP_CODE     = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, gupCode));
            }
            else //業主全部要去篩選只有此物流中心業主或業主設為共用
            {
            sql += string.Format(@" AND ((B.GUP_CODE ='0') OR (EXISTS (SELECT 1 
									FROM F190101 aa 
									INNER JOIN (SELECT * 
												FROM F192402 
												WHERE EMP_ID = @p{0}) bb 
                                        ON aa.DC_CODE = bb.DC_CODE AND aa.GUP_CODE = bb.GUP_CODE 
									WHERE aa.DC_CODE = A.DC_CODE AND aa.GUP_CODE = B.GUP_CODE))) ", parameters.Count);
                parameters.Add(new SqlParameter("@p" + parameters.Count, account));
            }
            if (!string.IsNullOrEmpty(custCode))
            {
                sql += "    AND B.CUST_CODE    = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, custCode));
            }
            else //雇主全部要去篩選只有此物流中心且為此業主的雇主或雇主設為共用
            {
                sql += string.Format(@" AND ((B.CUST_CODE ='0') OR (EXISTS (SELECT 1 
                                        FROM F190101 cc 
										INNER JOIN (SELECT * 
                                                    FROM F192402 
													WHERE EMP_ID = @p{0}) dd 
                                            ON cc.DC_CODE = dd.DC_CODE AND cc.GUP_CODE = dd.GUP_CODE AND cc.CUST_CODE = dd.CUST_CODE
										WHERE cc.DC_CODE = A.DC_CODE AND cc.GUP_CODE = B.GUP_CODE AND cc.CUST_CODE = B.CUST_CODE))) ", parameters.Count);
                parameters.Add(new SqlParameter("@p" + parameters.Count, account));
            }

            sql += "    ORDER BY A.WAREHOUSE_ID,A.DC_CODE,B.GUP_CODE,B.CUST_CODE ";

            var result = SqlQuery<F1980Data>(sql, parameters.ToArray());
            return result;
        }

        public IQueryable<F1980Data> GetF1980CheckDatas(string dcCode)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0",  dcCode),
            };
            var sql = " SELECT  ROW_NUMBER()OVER(ORDER BY A.WAREHOUSE_ID,A.DC_CODE,B.GUP_CODE,B.CUST_CODE) ROWNUM,A.*, " +
                                "        B.GUP_CODE,B.CUST_CODE, " +
                                "        B.FLOOR,B.MINCHANNEL,B.MAXCHANNEL,B.MINPLAIN,B.MAXPLAIN,B.MINLOC_LEVEL,B.MAXLOC_LEVEL," +
                                "        B.MINLOC_TYPE,B.MAXLOC_TYPE,C.LOC_TYPE_NAME,C.HANDY, " +
                                "        CASE B.GUP_CODE WHEN NULL THEN N'' WHEN '0' THEN N'共用' ELSE D.GUP_NAME END GUP_NAME, " +
                                "        CASE B.CUST_CODE WHEN NULL THEN N'' WHEN '0' THEN N'共用' ELSE E.CUST_NAME END CUST_NAME, " +
                                "        F.DC_NAME " +
                                "   FROM F1980 A " +
                                "   LEFT JOIN ( " +
                                "          SELECT A.WAREHOUSE_ID,B.GUP_CODE,B.CUST_CODE,A.DC_CODE," +
                                "                 MAX(B.FLOOR) FLOOR, " +
                                "                 MIN(B.CHANNEL) MINCHANNEL,MAX(B.CHANNEL) MAXCHANNEL, " +
                                "                 MIN(B.PLAIN) MINPLAIN,MAX(B.PLAIN) MAXPLAIN, " +
                                "                 MIN(B.LOC_LEVEL) MINLOC_LEVEL,MAX(B.LOC_LEVEL) MAXLOC_LEVEL, " +
                                "                 MIN(B.LOC_TYPE) MINLOC_TYPE, MAX(B.LOC_TYPE) MAXLOC_TYPE " +
                                "            FROM F1980 A " +
                                "           INNER JOIN F1912 B ON A.DC_CODE = b.DC_CODE AND A.WAREHOUSE_ID = B.WAREHOUSE_ID " +
                                "           GROUP BY A.WAREHOUSE_ID,B.GUP_CODE,B.CUST_CODE,A.DC_CODE " +
                                "            ) B  ON A.WAREHOUSE_ID = B.WAREHOUSE_ID " +
                                "                AND B.GUP_CODE = B.GUP_CODE " +
                                "                AND B.CUST_CODE = B.CUST_CODE " +
								"				AND A.DC_CODE = B.DC_CODE" +
                                "   LEFT JOIN F1942 C ON A.LOC_TYPE_ID  = C.LOC_TYPE_ID  " +
                                "	  LEFT JOIN F1929 D ON B.GUP_CODE = D.GUP_CODE " +
                                "   LEFT JOIN F1909 E ON B.CUST_CODE = E.CUST_CODE " +
                                "   LEFT JOIN F1901 F ON A.DC_CODE = F.DC_CODE " +
                                "  WHERE A.DC_CODE      = @p0 ";
            sql += "    ORDER BY A.WAREHOUSE_ID,A.DC_CODE,B.GUP_CODE,B.CUST_CODE ";
            var result = SqlQuery<F1980Data>(sql, parameters.ToArray());
            return result;
        }

        public bool CheckAutoWarehouse(string dcCode, string warehouseId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0",  dcCode),
                new SqlParameter("@p1", string.IsNullOrWhiteSpace(warehouseId) ? (object)DBNull.Value : warehouseId )
            };
            var sql = @" SELECT * FROM F1980
                         WHERE DC_CODE = @p0
                         AND WAREHOUSE_ID = @p1
                         AND ISNULL(DEVICE_TYPE,'0') <> '0'
                        ";

            var result = SqlQuery<F1980>(sql, parameters.ToArray()).FirstOrDefault();
            return result != null;
        }
				public IQueryable<F1980> GetAutoWarehouseList(string dcCode)
				{
						var parameters = new List<SqlParameter>
									{
											new SqlParameter("@p0",  dcCode)
									};
						var sql = @" SELECT * FROM F1980
															 WHERE DC_CODE = @p0
															 AND ISNULL(DEVICE_TYPE,'0') <> '0'
															";

						return SqlQuery<F1980>(sql, parameters.ToArray());
				}
	}
}
