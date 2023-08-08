using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F16
{
	public partial class F161601Repository : RepositoryBase<F161601, Wms3plDbContext, F161601Repository>
	{
		public IQueryable<F161601DetailDatas> GetF161601DetailDatas(string dcCode, string gupCode, string custCode, string rtnApplyNo)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", rtnApplyNo)
			};
			var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY A.DC_CODE,
								A.GUP_CODE,
								A.CUST_CODE,
								A.RTN_APPLY_NO)ROWNUM,
							   A.DC_CODE,
							   A.GUP_CODE,
							   A.CUST_CODE,
							   A.RTN_APPLY_NO,
							   A.RTN_APPLY_DATE,
							   A.STATUS,
							   A.MEMO,
							   B.RTN_APPLY_SEQ,
							   B.ITEM_CODE,
							   B.SRC_LOC,
							   B.LOC_QTY,
							   B.WAREHOUSE_ID,
							   D.WAREHOUSE_NAME,
							   B.MOVED_QTY,
							   B.POST_QTY,
							   C.ITEM_NAME,
							   C.ITEM_SIZE,
							   C.ITEM_SPEC,
							   C.ITEM_COLOR,
							   B.VNR_CODE,
							   V.VNR_NAME,
							   B.MAKE_NO,
							   B.BOX_CTRL_NO,
							   B.PALLET_CTRL_NO,
							   B.TAR_MAKE_NO,
							   B.TAR_BOX_CTRL_NO,
							   B.TAR_PALLET_CTRL_NO,
							   B.TAR_VALID_DATE ,
                               B.VALID_DATE,
                               B.ENTER_DATE,
							   (CASE C.TMPR_TYPE
								   WHEN '02' THEN '01'
								   WHEN '03' THEN '02'
								   WHEN '04' THEN '03'
								   ELSE C.TMPR_TYPE
								END)
								  AS TMPR_TYPE
						  FROM F161601 A
							   JOIN F161602 B
								  ON     A.DC_CODE = B.DC_CODE
									 AND A.GUP_CODE = B.GUP_CODE
									 AND A.CUST_CODE = B.CUST_CODE
									 AND A.RTN_APPLY_NO = B.RTN_APPLY_NO
							   LEFT JOIN F1980 D
								  ON D.DC_CODE = B.DC_CODE AND D.WAREHOUSE_ID = B.WAREHOUSE_ID
							   LEFT JOIN F1903 C
								  ON C.GUP_CODE = B.GUP_CODE AND C.ITEM_CODE = B.ITEM_CODE AND C.CUST_CODE = B.CUST_CODE
							   LEFT JOIN F1908 V
								  ON B.GUP_CODE = V.GUP_CODE AND B.VNR_CODE = V.VNR_CODE
						 WHERE     A.DC_CODE = @p0
							   AND A.GUP_CODE = @p1
							   AND A.CUST_CODE = @p2
							   AND A.RTN_APPLY_NO = @p3";

			return SqlQuery<F161601DetailDatas>(sql, parameters.ToArray()).AsQueryable();
		}

		public IQueryable<F161401ReturnWarehouse> GetF161401ReturnWarehouse(string dcCode, string gupCode, string custCode, string returnNo, string locCode, string itemCode, string itemName)
		{
			var parameterList = new List<object>
			{
				dcCode,
				gupCode,
				custCode,
			};
			var filter1 = string.Empty;
			var filter2 = string.Empty;
			var filter3 = string.Empty;
			var filter4 = string.Empty;
			if (!string.IsNullOrWhiteSpace(returnNo))
			{
				filter1 = $@" AND EXISTS(
									SELECT 1 
										FROM F161401 F
										JOIN F161201 G
											ON G.DC_CODE = F.DC_CODE
										AND G.GUP_CODE = F.GUP_CODE
										AND G.CUST_CODE = F.CUST_CODE
										AND G.RETURN_NO = F.RETURN_NO
										JOIN (SELECT DC_CODE,GUP_CODE,CUST_CODE,RETURN_NO,LOC_CODE,ITEM_CODE,RTN_QTY,SUM(MOVED_QTY) MOVED_QTY 
													FROM F161402
													GROUP BY DC_CODE,GUP_CODE,CUST_CODE,RETURN_NO,LOC_CODE,ITEM_CODE,RTN_QTY
													) H
										ON H.DC_CODE = F.DC_CODE
										AND H.GUP_CODE = F.GUP_CODE
										AND H.CUST_CODE = F.CUST_CODE
										AND H.RETURN_NO = F.RETURN_NO
									WHERE F.DC_CODE = A.DC_CODE
										AND F.GUP_CODE = A.GUP_CODE
										AND F.CUST_CODE = A.CUST_CODE
										AND F.RETURN_NO = @p{parameterList.Count}
										AND H.LOC_CODE = A.LOC_CODE
										AND H.ITEM_CODE = A.ITEM_CODE
										AND G.STATUS IN ('1','2')
										AND H.MOVED_QTY >0
							  )";
				parameterList.Add(returnNo);
			}
			if(!string.IsNullOrWhiteSpace(locCode))
			{
				filter2 = @" AND A.LOC_CODE = @p" + parameterList.Count;
				parameterList.Add(locCode);
			}

			if (!string.IsNullOrWhiteSpace(itemCode))
			{
				filter3 = @" AND A.ITEM_CODE = @p" + parameterList.Count;
				parameterList.Add(itemCode);
			}

			if (itemName !="%%")
			{
				filter4 = @" AND D.ITEM_NAME LIKE @p"+parameterList.Count;
				parameterList.Add(itemName);
			}

			var sql = $@" SELECT ROW_NUMBER()OVER(ORDER BY Z.DC_CODE,Z.GUP_CODE,Z.CUST_CODE,Z.LOC_CODE)ROWNUM, Z.* FROM (
      SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.LOC_CODE,A.ITEM_CODE,
             ISNULL(A.VALID_DATE, convert(varchar,'9999/12/31',111)) as VALID_DATE ,
             ISNULL(A.ENTER_DATE, convert(varchar,'9999/12/31',111)) as ENTER_DATE ,
             ISNULL(A.VNR_CODE, '000000') AS VNR_CODE,E.VNR_NAME,
             D.ITEM_NAME,D.ITEM_SIZE,D.ITEM_SPEC,D.ITEM_COLOR,
						 (CASE D.TMPR_TYPE
								 WHEN '02' THEN '01'
								 WHEN '03' THEN '02'
								 WHEN '04' THEN '03'
								 ELSE D.TMPR_TYPE
							END) AS TMPR_TYPE,
							SUM(A.QTY) LOC_QTY,0 MOVED_QTY,
			 A.BOX_CTRL_NO,A.PALLET_CTRL_NO,A.MAKE_NO,A.BOX_CTRL_NO as TAR_BOX_CTRL_NO ,
			 A.PALLET_CTRL_NO as TAR_PALLET_CTRL_NO,A.MAKE_NO as TAR_MAKE_NO, 
			 ISNULL(A.VALID_DATE, convert(varchar,'9999/12/31',111)) as TAR_VALID_DATE
				FROM F1913 A
				JOIN F1912 B
				  ON B.DC_CODE = A.DC_CODE
				 AND B.LOC_CODE = A.LOC_CODE
				JOIN F1980 C
				  ON C.DC_CODE = B.DC_CODE
				 AND C.WAREHOUSE_ID = B.WAREHOUSE_ID
				JOIN F1903 D
				  ON D.GUP_CODE = A.GUP_CODE
				 AND D.ITEM_CODE = A.ITEM_CODE
				 AND D.CUST_CODE = A.CUST_CODE
				LEFT JOIN F1908 E
				  ON E.GUP_CODE = A.GUP_CODE
				 AND E.VNR_CODE = A.VNR_CODE
			 WHERE A.DC_CODE =@p0
				 AND A.GUP_CODE = @p1
				 AND A.CUST_CODE =@p2
				 AND C.WAREHOUSE_TYPE = 'T'
                 AND A.QTY > 0
				 {filter1}
         {filter2}
         {filter3}
         {filter4}
				GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.LOC_CODE,A.ITEM_CODE,A.VALID_DATE,A.ENTER_DATE,A.VNR_CODE,E.VNR_NAME,
				D.ITEM_NAME,D.ITEM_SIZE,D.ITEM_SPEC,D.ITEM_COLOR,D.TMPR_TYPE,A.BOX_CTRL_NO,A.PALLET_CTRL_NO,A.MAKE_NO) Z ";

			return SqlQuery<F161401ReturnWarehouse>(sql, parameterList.ToArray());
		}

		public IQueryable<PrintF161601Data> GetPrintF161601Data(string dcCode, string gupCode, string custCode, string rtnApplyNo)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", rtnApplyNo)
			};
			var sql = @"
								Select ROW_NUMBER()OVER(ORDER BY A.DC_CODE,A.GUP_CODE,O.GUP_NAME,A.CUST_CODE)ROWNUM,A.DC_CODE,A.GUP_CODE,O.GUP_NAME,A.CUST_CODE,N.CUST_NAME,A.RTN_APPLY_NO,A.RTN_APPLY_DATE,A.STATUS,ISNULL(A.MEMO,' ') MEMO,
															B.RTN_APPLY_SEQ,B.ITEM_CODE,B.SRC_LOC,ISNULL(B.TRA_LOC,' ') TRA_LOC,ISNULL(B.LOC_QTY,0) LOC_QTY,B.WAREHOUSE_ID,D.WAREHOUSE_NAME,ISNULL(B.MOVED_QTY,0) MOVED_QTY,ISNULL(B.POST_QTY,0) POST_QTY,
															ISNULL(C.ITEM_NAME,' ') ITEM_NAME,ISNULL(C.ITEM_SIZE,' ') ITEM_SIZE,ISNULL(C.ITEM_SPEC,' ') ITEM_SPEC,ISNULL(C.ITEM_COLOR,' ') ITEM_COLOR,ISNULL(A.APPROVE_STAFF,' ') APPROVE_STAFF,ISNULL(A.APPROVE_NAME,' ') APPROVE_NAME,M.ALLOCATION_NO
									From F161601 A
									LEFT Join F161602 B On A.DC_CODE=B.DC_CODE And A.GUP_CODE=B.GUP_CODE And A.CUST_CODE=B.CUST_CODE And A.RTN_APPLY_NO=B.RTN_APPLY_NO--(+)
									Join F151001 M On A.DC_CODE=M.DC_CODE And A.GUP_CODE=M.GUP_CODE And A.CUST_CODE=M.CUST_CODE And A.RTN_APPLY_NO=M.SOURCE_NO And B.WAREHOUSE_ID=M.SRC_WAREHOUSE_ID
									Left Join F1980 D On D.DC_CODE=B.DC_CODE And D.WAREHOUSE_ID=B.WAREHOUSE_ID
									Left Join F1903 C On C.GUP_CODE=B.GUP_CODE And C.ITEM_CODE=B.ITEM_CODE And C.CUST_CODE=B.CUST_CODE
									Left Join F1929 O On A.GUP_CODE=O.GUP_CODE
									Left Join F1909 N On A.GUP_CODE=N.GUP_CODE And A.CUST_CODE=N.CUST_CODE
								 Where A.DC_CODE=@p0 AND A.GUP_CODE=@p1 AND A.CUST_CODE=@p2 AND A.RTN_APPLY_NO=@p3";

			return SqlQuery<PrintF161601Data>(sql, parameters.ToArray());
		}

	
		public IQueryable<P160102Report> GetP160102Reports(string dcCode, string gupCode, string custCode, string rtnApplyNo)
		{
			var parameters = new SqlParameter[]
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", rtnApplyNo),
            };

			var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY B.ALLOCATION_NO, B.SRC_LOC_CODE, B.SUG_LOC_CODE)ROWNUM, A.ALLOCATION_NO, A.ALLOCATION_DATE, H.WAREHOUSE_NAME SRC_WAREHOUSE_NAME,
												A.CRT_NAME, A.CRT_DATE, F.ITEM_SIZE, F.ITEM_SPEC, F.ITEM_COLOR, 
												C.WAREHOUSE_NAME, D.GUP_NAME, E.SHORT_NAME CUST_NAME, 
                        F.ITEM_NAME, G.RTN_APPLY_NO, G.APPROVE_STAFF, G.APPROVE_NAME, 
                        B.ITEM_CODE, B.TAR_QTY,
                        CASE WHEN LEN(B.SRC_LOC_CODE) = 9 
                            THEN substring(B.SRC_LOC_CODE,1,1) + '-' + substring(B.SRC_LOC_CODE,2,2)  + '-' + substring(B.SRC_LOC_CODE,4,2)  + '-' + substring(B.SRC_LOC_CODE,6,2)  + '-' + substring(B.SRC_LOC_CODE,8,2) 
                            ELSE B.SRC_LOC_CODE 
                        END SRC_LOC_CODE,
                        CASE WHEN len(B.SUG_LOC_CODE) = 9 
                            THEN substring(B.SUG_LOC_CODE,1,1) + '-' + substring(B.SUG_LOC_CODE,2,2)  + '-' + substring(B.SUG_LOC_CODE,4,2)  + '-' + substring(B.SUG_LOC_CODE,6,2)  + '-' + substring(B.SUG_LOC_CODE,8,2) 
                            ELSE B.SUG_LOC_CODE 
                        END SUG_LOC_CODE
                        FROM F151001 A
                        INNER JOIN ( 
                            SELECT B1.DC_CODE, B1.GUP_CODE, B1.CUST_CODE, B1.ALLOCATION_NO, B1.ITEM_CODE, B1.SRC_LOC_CODE, B1.SUG_LOC_CODE, SUM(B1.TAR_QTY) TAR_QTY 
                            FROM F151002 B1
                            GROUP BY B1.DC_CODE, B1.GUP_CODE, B1.CUST_CODE, B1.ALLOCATION_NO, B1.ITEM_CODE, B1.SRC_LOC_CODE, B1.SUG_LOC_CODE
                        ) B
                            ON A.DC_CODE = B.DC_CODE
                            AND A.GUP_CODE = B.GUP_CODE
                            AND A.CUST_CODE = B.CUST_CODE
                            AND A.ALLOCATION_NO = B.ALLOCATION_NO
                        LEFT JOIN F1980 C
                            ON A.DC_CODE = C.DC_CODE
                            AND A.TAR_WAREHOUSE_ID = C.WAREHOUSE_ID
												LEFT JOIN F1980 H
														ON A.DC_CODE = H.DC_CODE AND A.SRC_WAREHOUSE_ID = H.WAREHOUSE_ID
                        LEFT JOIN F1929 D
                            ON A.GUP_CODE = D.GUP_CODE
                        LEFT JOIN F1909 E
                            ON A.GUP_CODE = E.GUP_CODE
                            AND A.CUST_CODE = E.CUST_CODE
                        LEFT JOIN F1903 F
                            ON A.GUP_CODE = F.GUP_CODE
                            AND B.ITEM_CODE = F.ITEM_CODE
                            AND B.CUST_CODE = F.CUST_CODE
                        INNER JOIN F161601 G
                            ON A.DC_CODE = G.DC_CODE
                            AND A.GUP_CODE = G.GUP_CODE
                            AND A.CUST_CODE = G.CUST_CODE
                            AND A.SOURCE_NO = G.RTN_APPLY_NO
                        WHERE G.DC_CODE = @p0
                            AND G.GUP_CODE = @p1
                            AND G.CUST_CODE = @p2
                            AND G.RTN_APPLY_NO = @p3
                        --ORDER BY B.ALLOCATION_NO, B.SRC_LOC_CODE, B.SUG_LOC_CODE    -- 依照調撥單、儲位順序來排序";

			return SqlQuery<P160102Report>(sql, parameters);
		}


		public IQueryable<F161601> GetWaitUpLocDataByDc(string dcCode, DateTime rtnApplyDate)
		{
			var sql = @" SELECT *
										 FROM F161601 A
										INNER JOIN F151001 B
											 ON B.DC_CODE = A.DC_CODE
											AND B.GUP_CODE = A.GUP_CODE
											AND B.CUST_CODE = A.CUST_CODE
											AND B.SOURCE_NO = A.RTN_APPLY_NO
										WHERE A.DC_CODE =@p0 
											AND A.RTN_APPLY_DATE = @p1 
                      AND A.STATUS <> '9' 
											AND B.STATUS <>'5' ";
			var param = new object[] { dcCode, rtnApplyDate.Date };
			return SqlQuery<F161601>(sql, param);

		}

		public IQueryable<DcWmsNoStatusItem> GetReturnWaitUpLocOver30MinByDc(string dcCode, DateTime rtnApplyDate)
		{
			var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY WMS_NO , STAFF_NAME,START_DATE)ROWNUM,A.* 
										 FROM (
										 SELECT DISTINCT A.RTN_APPLY_NO AS WMS_NO,C.SRC_STAFF + C.SRC_NAME AS STAFF_NAME,C.SRC_DATE AS START_DATE
										   FROM F161601 A
									  	INNER JOIN F151001 B
										     ON B.DC_CODE = A.DC_CODE
												AND B.GUP_CODE = A.GUP_CODE
												AND B.CUST_CODE = A.CUST_CODE
												AND B.SOURCE_NO = A.RTN_APPLY_NO
												INNER JOIN F151002 C
													 ON C.DC_CODE = B.DC_CODE
													AND C.GUP_CODE = B.GUP_CODE
													AND C.CUST_CODE = B.CUST_CODE
													AND C.ALLOCATION_NO =B.ALLOCATION_NO
												INNER JOIN 
													(SELECT DC_CODE,GUP_CODE,CUST_CODE,ALLOCATION_NO,MIN(SRC_DATE) SRC_DATE 
													FROM F151002 
													GROUP BY DC_CODE,GUP_CODE,CUST_CODE,ALLOCATION_NO) D
												  ON D.DC_CODE = C.DC_CODE
												 AND D.GUP_CODE = C.GUP_CODE
												 AND D.CUST_CODE = C.CUST_CODE
												 AND D.ALLOCATION_NO = C.ALLOCATION_NO
												 AND ISNULL(D.SRC_DATE,@p0) = ISNULL(C.SRC_DATE,@p0)
											 WHERE A.DC_CODE = @p1
												 AND A.RTN_APPLY_DATE = @p2
												 AND A.STATUS <>'9' 
												 AND B.STATUS <>'5'
												 AND DATEDIFF(MINUTE,ISNULL(C.SRC_DATE,@p0),@p0) >30
												)A  ";

      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", DateTime.Now) {SqlDbType = SqlDbType.DateTime2},
        new SqlParameter("@p1", dcCode) {SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p2", rtnApplyDate) {SqlDbType = SqlDbType.DateTime2}
      };

			return SqlQuery<DcWmsNoStatusItem>(sql, param);

		}
	}

}