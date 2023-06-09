using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F15
{
	public partial class F151001Repository : RepositoryBase<F151001, Wms3plDbContext, F151001Repository>
	{
        public IQueryable<GetF150201CSV> GetF150201CSV(string gupCode, string custCode, string SourceDcCode, string TargetDcCode, DateTime CRTDateS, DateTime CRTDateE, string TxtSearchAllocationNo, DateTime? PostingDateS, DateTime? PostingDateE, string SourceWarehouseList, string TargetWarehouseList, string StatusList, string TxtSearchSourceNo)
        {


            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", gupCode),
                new SqlParameter("@p1", custCode),
                new SqlParameter("@p2", SourceDcCode),
                new SqlParameter("@p3", TargetDcCode)
            };



            var sql = @"SELECT 
                               A.GUP_CODE,A.CUST_CODE,A.SRC_DC_CODE,A.TAR_DC_CODE,A.SRC_WAREHOUSE_ID,A.TAR_WAREHOUSE_ID,
							   E.ITEM_CODE,A.SRC_LOC_CODE,A.TAR_LOC_CODE,A.SRC_QTY                                              
						  FROM (  SELECT B.SRC_DC_CODE,                                                                     -- 序號綁儲位
										 B.TAR_DC_CODE,
										 B.SRC_WAREHOUSE_ID,
										 B.TAR_WAREHOUSE_ID,
										 C.ALLOCATION_NO,                -- 調撥單明細顯示以畫面上顯示的欄位做 Group By
                     B.ALLOCATION_DATE,
										 B.STATUS,
                                         B.POSTING_DATE,
                                         B.CRT_ALLOCATION_DATE,
										 C.DC_CODE,
										 C.GUP_CODE,
										 C.CUST_CODE,
										 C.ITEM_CODE,                                            -- 品號
										 CASE WHEN B.SRC_WAREHOUSE_ID IS NULL THEN '' ELSE C.SRC_LOC_CODE END AS SRC_LOC_CODE,    -- 來源儲位
										 CASE WHEN B.TAR_WAREHOUSE_ID IS NULL THEN '' ELSE C.SUG_LOC_CODE END AS SUG_LOC_CODE,   -- 建議上架儲位
										 CASE WHEN B.TAR_WAREHOUSE_ID IS NULL THEN '' ELSE C.TAR_LOC_CODE END AS TAR_LOC_CODE,   -- 實際上架儲位
										 CASE WHEN B.TAR_WAREHOUSE_ID IS NULL THEN NULL ELSE SUM (C.TAR_QTY) END AS TAR_QTY,         -- 上架數量
										 CASE WHEN B.TAR_WAREHOUSE_ID IS NULL THEN NULL ELSE SUM (C.A_TAR_QTY) END AS A_TAR_QTY,  -- 實際上架數量
										 CASE WHEN B.SRC_WAREHOUSE_ID IS NULL THEN NULL ELSE SUM (C.SRC_QTY) END AS SRC_QTY,       -- 下架數量
                     CASE WHEN B.SRC_WAREHOUSE_ID IS NULL THEN NULL ELSE SUM (C.A_SRC_QTY) END AS A_SRC_QTY,    --實際下架數量
										 C.SRC_STAFF,                                          -- 下架人員
										 C.SRC_NAME,                                           -- 下架人名
										 C.TAR_STAFF,                                          -- 上架人員
										 C.TAR_NAME,                                           -- 上架人名
										 MAX (C.SRC_DATE) AS SRC_DATE,                    -- 取最後一次下架時間
										 MAX (C.TAR_DATE) AS TAR_DATE,                    -- 取最後一次上架時間
										 MIN (C.CHECK_SERIALNO) AS CHECK_SERIALNO,            -- 序號已刷讀
                     B.SOURCE_NO
									FROM F151001 B
										 JOIN F151002 C
											ON     B.ALLOCATION_NO = C.ALLOCATION_NO
											   AND B.DC_CODE = C.DC_CODE
											   AND B.GUP_CODE = C.GUP_CODE
											   AND B.CUST_CODE = C.CUST_CODE
								GROUP BY B.SRC_DC_CODE,
										 B.TAR_DC_CODE,
										 B.SRC_WAREHOUSE_ID,
										 B.TAR_WAREHOUSE_ID,
										 C.ALLOCATION_NO,
										 B.ALLOCATION_DATE,
										 B.STATUS,
                                         B.POSTING_DATE,
                                         B.CRT_ALLOCATION_DATE,
										 C.DC_CODE,
										 C.GUP_CODE,
										 C.CUST_CODE,
										 C.ITEM_CODE,
										 C.SRC_LOC_CODE,
										 C.SUG_LOC_CODE,
										 C.TAR_LOC_CODE,
										 C.SRC_STAFF,
										 C.SRC_NAME,
										 C.TAR_STAFF,
										 C.TAR_NAME,B.SOURCE_NO) A
							   LEFT JOIN F1903 E
								  ON     A.GUP_CODE = E.GUP_CODE
									 AND A.CUST_CODE = E.CUST_CODE
									 AND A.ITEM_CODE = E.ITEM_CODE
						     WHERE  
							       A.GUP_CODE = @p0
							   AND A.CUST_CODE = @p1
                               AND A.SRC_DC_CODE = @p2
                               AND A.TAR_DC_CODE = @p3";

            if (TxtSearchAllocationNo != " " && TxtSearchAllocationNo != "")
            {
                sql += " AND A.ALLOCATION_NO = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, TxtSearchAllocationNo));
            }
            if (TxtSearchSourceNo != " " && TxtSearchSourceNo != "")
            {
                sql += " AND A.SOURCE_NO = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, TxtSearchSourceNo));
            }
            if (StatusList != "")
            {
                sql += " AND A.STATUS = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, StatusList));
            }
            if (SourceWarehouseList != "")
            {
                sql += " AND A.SRC_WAREHOUSE_ID = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, SourceWarehouseList));
            }
            if (TargetWarehouseList != "")
            {
                sql += " AND A.TAR_WAREHOUSE_ID =@p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, TargetWarehouseList));
            }
            if (PostingDateS.HasValue)
            {
                sql += " AND A.POSTING_DATE >= @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, PostingDateS.Value.Date));
            }
            if (PostingDateE.HasValue)
            {
                sql += " AND A.POSTING_DATE < @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, PostingDateE.Value.Date.AddDays(1)));
            }

            if (CRTDateS != null)
            {
                sql += " AND A.CRT_ALLOCATION_DATE >= @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, CRTDateS.Date));
            }
            if (CRTDateE != null)
            {
                sql += " AND A.CRT_ALLOCATION_DATE < @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, CRTDateE.Date.AddDays(1)));
            }

            var result = SqlQuery<GetF150201CSV>(sql, parameters.ToArray());
            return result; ;

        }
    }
}
