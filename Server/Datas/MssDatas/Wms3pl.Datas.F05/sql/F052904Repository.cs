using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F052904Repository : RepositoryBase<F052904, Wms3plDbContext, F052904Repository>
	{
		public F052904 FindOtherContainerNotFinish(string dcCode,string gupCode,string custCode,DateTime delvDate,string pickTIme,string moveOutTarget)
		{
			var parms = new object[] { dcCode, gupCode, custCode, delvDate, pickTIme, moveOutTarget };
			var sql = @" SELECT *
                   FROM F052904
                  WHERE DC_CODE = @p0
                    AND GUP_CODE = @p1
                    AND CUST_CODE = @p2
                    AND DELV_DATE = @p3
                    AND PICK_TIME = @p4
                    AND MOVE_OUT_TARGET = @p5
                    AND STATUS = '0'";
			return SqlQuery<F052904>(sql, parms).FirstOrDefault();
		}

		public ContainerPickInfo GetContainerPickInfo(string dcCode, string containerCode)
		{
			var parms = new object[] { dcCode, containerCode };

			var sql = @" SELECT 0 AS Id,C.PICK_ORD_NO PickOrdNo,D.DELV_DATE DelvDate,D.PICK_TIME PickTime, 
													D.ATFL_N_PICK_CNT+D.ATFL_B_PICK_CNT+D.ATFL_S_PICK_CNT+
													D.ATFL_NP_PICK_CNT+D.ATFL_BP_PICK_CNT+D.ATFL_SP_PICK_CNT+
													D.AUTO_N_PICK_CNT+D.AUTO_S_PICK_CNT+D.REPICK_CNT  BatchPickCnt,
													D.PICK_CNT BatchPickQty,
													E.B_PICK_QTY PickQty,
                          C.MOVE_OUT_TARGET MoveOutTarget,
													G.CROSS_NAME MoveOutTargetName,
                          ISNULL(F.CancelOrderCnt,0) CancelOrderCnt,
                          H.AllOrderCnt - ISNULL(F.CancelOrderCnt,0) NormalOrderCnt
										 FROM F052904 A
										 JOIN F051201 C
											 ON C.DC_CODE = A.DC_CODE
										  AND C.GUP_CODE = A.GUP_CODE
											AND C.CUST_CODE = A.CUST_CODE
											AND C.PICK_ORD_NO = A.PICK_ORD_NO
										JOIN F0513 D
											ON D.DC_CODE = C.DC_CODE
										 AND D.GUP_CODE = C.GUP_CODE
										 AND D.CUST_CODE = C.CUST_CODE
										 AND D.DELV_DATE = C.DELV_DATE
										 AND D.PICK_TIME = C.PICK_TIME
										JOIN (
											SELECT DC_CODE,GUP_CODE,CUST_CODE,PICK_ORD_NO,SUM(B_PICK_QTY) B_PICK_QTY
											FROM F051202
                      WHERE PICK_STATUS <>'9'
											GROUP BY DC_CODE,GUP_CODE,CUST_CODE,PICK_ORD_NO
										) E
										  ON E.DC_CODE = C.DC_CODE
										 AND E.GUP_CODE = C.GUP_CODE
										 AND E.CUST_CODE = C.CUST_CODE
										 AND E.PICK_ORD_NO = C.PICK_ORD_NO
                     LEFT JOIN (SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.PICK_ORD_NO,COUNT(DISTINCT A.WMS_ORD_NO) CancelOrderCnt
														FROM F051202 A
														JOIN F050801 B
														ON B.DC_CODE = A.DC_CODE
														AND B.GUP_CODE = A.GUP_CODE
														AND B.CUST_CODE = A.CUST_CODE
														AND B.WMS_ORD_NO = A.WMS_ORD_NO
														WHERE B.STATUS ='9' AND A.PICK_STATUS<>'9'
														GROUP BY  A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.PICK_ORD_NO) F
                       ON F.DC_CODE = C.DC_CODE
                      AND F.GUP_CODE = C.GUP_CODE
                      AND F.CUST_CODE = C.CUST_CODE
                      AND F.PICK_ORD_NO = C.PICK_ORD_NO
                    LEFT JOIN F0001 G
                      ON G.CROSS_CODE = C.MOVE_OUT_TARGET
                    JOIN (SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.PICK_ORD_NO,COUNT(DISTINCT A.WMS_ORD_NO) AllOrderCnt
														FROM F051202 A
														JOIN F050801 B
													  	ON B.DC_CODE = A.DC_CODE
														 AND B.GUP_CODE = A.GUP_CODE
													 	 AND B.CUST_CODE = A.CUST_CODE
														 AND B.WMS_ORD_NO = A.WMS_ORD_NO
                           WHERE A.PICK_STATUS <> '9'
													 GROUP BY  A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.PICK_ORD_NO) H
                       ON H.DC_CODE = C.DC_CODE
                      AND H.GUP_CODE = C.GUP_CODE
                      AND H.CUST_CODE = C.CUST_CODE
                      AND H.PICK_ORD_NO = C.PICK_ORD_NO
									 WHERE A.DC_CODE = @p0
										 AND A.CONTAINER_CODE = @p1
										 AND C.NEXT_STEP = '6'
										 AND A.STATUS ='0'
									 ORDER BY A.CRT_DATE DESC";

			return SqlQuery<ContainerPickInfo>(sql, parms).FirstOrDefault();
		}

		public F052904 GetDataByPickContianerCode(string dcCode,string gupCode,string custCode,string pickOrdNo,string containerCode)
		{
			var parms = new object[] { dcCode, gupCode, custCode, pickOrdNo,containerCode };
			var sql = @" SELECT *
                   FROM F052904
                  WHERE DC_CODE = @p0
                    AND GUP_CODE = @p1
                    AND CUST_CODE = @p2
                    AND PICK_ORD_NO = @p3
                    AND CONTAINER_CODE = @p4 ";
			return SqlQuery<F052904>(sql, parms).FirstOrDefault();
		}
	}
}
