using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F051602Repository : RepositoryBase<F051602, Wms3plDbContext, F051602Repository>
	{
        public void InsertF051602ByBatchNo(string dcCode, string gupCode, string custCode, string batchNo)
        {
            var parms = new List<object> { DateTime.Now, Current.Staff, Current.StaffName, Current.Lang, dcCode, gupCode, custCode, batchNo };
            var sql = @" 
      INSERT INTO F051602(DC_CODE,GUP_CODE,CUST_CODE,BATCH_DATE,BATCH_NO,ITEM_CODE,ITEM_NAME,ITEM_UNIT,RETAIL_CODE,RETAIL_NAME,WMS_ORD_NO,CUST_ORD_LIST,LOC_CODE,PLAN_QTY,ORDER_QTY,CAR_PERIOD,DELV_NO,DELV_WAY,CRT_DATE,CRT_STAFF,CRT_NAME)
			SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,D.BATCH_DATE,D.BATCH_NO ,A.ITEM_CODE,E.ITEM_NAME,F.ACC_UNIT_NAME ITEM_UNIT,B.RETAIL_CODE,K.RETAIL_NAME,A.WMS_ORD_NO,L.CUST_ORD_NO AS CUST_ORD_LIST,A.PICK_LOC LOC_CODE,SUM(B_PICK_QTY) PLAN_QTY,SUM(B_PICK_QTY) ORDER_QTY,I.NAME CAR_PERIOD,G.DELV_NO,G.DELV_WAY,@p0 CRT_DATE,@p1 CRT_STAFF,@p2 CRT_NAME
				FROM F051202 A
				JOIN F050801 B
				ON B.DC_CODE = A.DC_CODE
				AND B.GUP_CODE = A.GUP_CODE
				AND B.CUST_CODE = A.CUST_CODE
				AND B.WMS_ORD_NO = A.WMS_ORD_NO
				JOIN F051503 C
				ON C.DC_CODE = A.DC_CODE
				AND C.GUP_CODE = A.GUP_CODE
				AND C.CUST_CODE = A.CUST_CODE
				AND C.PICK_ORD_NO = A.PICK_ORD_NO
				JOIN F0515 D
				ON D.DC_CODE = C.DC_CODE
				AND D.GUP_CODE = C.GUP_CODE
				AND D.CUST_CODE = C.CUST_CODE
				AND D.BATCH_NO = C.BATCH_NO
				JOIN F1903 E
				ON E.GUP_CODE = A.GUP_CODE
				AND E.ITEM_CODE = A.ITEM_CODE
                AND E.CUST_CODE = A.CUST_CODE
				JOIN F91000302 F 
				ON F.ITEM_TYPE_ID='001'
				AND F.ACC_UNIT = E.ITEM_UNIT
				JOIN F19471601 G
				ON G.DC_CODE = A.DC_CODE
				AND G.GUP_CODE = A.GUP_CODE
				AND G.CUST_CODE = A.CUST_CODE
				AND G.RETAIL_CODE = B.RETAIL_CODE
				JOIN F194716 H
				ON H.DC_CODE = G.DC_CODE
				AND H.GUP_CODE = G.GUP_CODE
				AND H.CUST_CODE = G.CUST_CODE
				AND H.DELV_NO = G.DELV_NO
				JOIN VW_F000904_LANG I
					ON I.TOPIC='F194716'
					AND I.SUBTOPIC='CAR_PERIOD'
					AND I.VALUE = H.CAR_PERIOD
					AND I.LANG = @p3
				 JOIN F1909 J
				 ON J.GUP_CODE = A.GUP_CODE
				 AND J.CUST_CODE = A.CUST_CODE
				 JOIN F1910 K
				 ON K.GUP_CODE = B.GUP_CODE
				 AND K.CUST_CODE = CASE WHEN J.ALLOWGUP_RETAILSHARE = '1' THEN '0' ELSE B.CUST_CODE END
				 AND K.RETAIL_CODE = B.RETAIL_CODE
         JOIN (
				 SELECT  A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.WMS_ORD_NO,
							STRING_AGG (B.CUST_ORD_NO, ',')WITHIN GROUP (ORDER BY B.CUST_ORD_NO) AS CUST_ORD_NO
								FROM F05030101 A
								JOIN F050301 B
								ON B.DC_CODE = A.DC_CODE
								AND B.GUP_CODE = A.GUP_CODE
								AND B.CUST_CODE= A.CUST_CODE
								AND B.ORD_NO = A.ORD_NO
								GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.WMS_ORD_NO) L
				ON L.DC_CODE = A.DC_CODE
				AND L.GUP_CODE = A.GUP_CODE
				AND L.CUST_CODE = A.CUST_CODE
				AND L.WMS_ORD_NO = A.WMS_ORD_NO
				WHERE D.DC_CODE= @p4
					AND D.GUP_CODE = @p5
					AND D.CUST_CODE =@p6
					AND D.BATCH_NO =@p7
				GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,D.BATCH_DATE,D.BATCH_NO,A.ITEM_CODE,E.ITEM_NAME,F.ACC_UNIT_NAME,B.RETAIL_CODE,K.RETAIL_NAME,A.WMS_ORD_NO,L.CUST_ORD_NO,A.PICK_LOC ,I.NAME,G.DELV_NO,G.DELV_WAY";
            ExecuteSqlCommand(sql, parms.ToArray());
        }

        public IQueryable<PutReportData> GetPutReportDatas(string dcCode, string gupCode, string custCode, string batchNo)
        {
            var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.ITEM_CODE,A.RETAIL_CODE,A.LOC_CODE ASC) ROWNUM,A.*
										FROM (
										SELECT TOP 100 PERCENT LOC_CODE,ITEM_CODE,ITEM_NAME,RETAIL_CODE,RETAIL_NAME,SUM(PLAN_QTY) PLAN_QTY,SUM(PACK_QTY) ACT_QTY 
																				 FROM F051602 
																				WHERE DC_CODE = @p0
																					AND GUP_CODE = @p1
																					AND CUST_CODE = @p2
																					AND BATCH_NO =  @p3 
                    GROUP BY LOC_CODE,ITEM_CODE,ITEM_NAME,RETAIL_CODE,RETAIL_NAME
										ORDER BY ITEM_CODE,RETAIL_CODE,LOC_CODE) A ";
            var parms = new List<object> { dcCode, gupCode, custCode, batchNo };

            var result = SqlQuery<PutReportData>(sql, parms.ToArray());

            return result;

        }
    }
}
