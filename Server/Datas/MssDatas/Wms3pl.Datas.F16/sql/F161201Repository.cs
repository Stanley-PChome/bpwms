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
	public partial class F161201Repository : RepositoryBase<F161201, Wms3plDbContext, F161201Repository>
	{
		public IQueryable<F161201DetailDatas> GetF161201DetailDatas(string dcCode, string gupCode, string custCode, string returnNo)
		{
			var sql = @"SELECT A.*,
							   B.AUDIT_QTY AS AUDIT_QTY_SUM,
							   B.MOVED_QTY,
							   B.MEMO,
							   D.ITEM_NAME,
							   D.ITEM_SIZE,
							   D.ITEM_SPEC,
							   D.ITEM_COLOR,
							   E.CAUSE
						  FROM F161202 A
							   LEFT JOIN (  SELECT C.DC_CODE,
												   C.GUP_CODE,
												   C.CUST_CODE,
												   C.RETURN_NO,
												   C.ITEM_CODE,
												   C.CAUSE,
												   C.MEMO,
												   SUM (C.AUDIT_QTY) AS AUDIT_QTY,
												   SUM (C.MOVED_QTY) AS MOVED_QTY
											  FROM F161402 C
										  GROUP BY C.DC_CODE,
												   C.GUP_CODE,
												   C.CUST_CODE,
												   C.RETURN_NO,
												   C.ITEM_CODE,
												   C.CAUSE,
												   C.MEMO) B
								  ON     A.DC_CODE = B.DC_CODE
									 AND A.GUP_CODE = B.GUP_CODE
									 AND A.CUST_CODE = B.CUST_CODE
									 AND A.RETURN_NO = B.RETURN_NO
									 AND A.ITEM_CODE = B.ITEM_CODE
							   LEFT JOIN F1903 D
								  ON A.ITEM_CODE = D.ITEM_CODE AND A.GUP_CODE = D.GUP_CODE AND A.CUST_CODE = D.CUST_CODE
							   LEFT JOIN F1951 E ON B.CAUSE = E.UCC_CODE AND E.UCT_ID = 'RC'
						 WHERE     A.DC_CODE = @p0
							   AND A.GUP_CODE = @p1
							   AND A.CUST_CODE = @p2
							   AND A.RETURN_NO = @p3";

			return SqlQuery<F161201DetailDatas>(sql, new object[] { dcCode, gupCode, custCode, returnNo });
		}

		

		public IQueryable<DcWmsNoOrdPropItem> GetDcWmsNoOrdPropItems(string dcCode, DateTime returnDate)
		{
			var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.CUST_CODE,A.ORD_PROP)ROWNUM,A.*
                     FROM (
										 SELECT A.CUST_CODE,A.ORD_PROP,
														SUM(CASE WHEN A.STATUS = '2' THEN 1 ELSE 0 END) AS CUST_FINISHCOUNT,
														COUNT(*)  AS CUST_TOTALCOUNT 
											 FROM F161201 A
									 		WHERE A.DC_CODE = @p0
												AND A.RETURN_DATE = @p1
									  		AND A.STATUS <>'9'
											GROUP BY A.CUST_CODE,A.ORD_PROP ) A ";
			var param = new object[] { dcCode, returnDate.Date };
			return SqlQuery<DcWmsNoOrdPropItem>(sql, param);
		}

		public IQueryable<DcWmsNoDateItem> GetDcWmsNoDateItems(string dcCode, string gupCode, string custCode,
				DateTime begReturnDate, DateTime endReturnDate)
		{
			var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.WmsDate) ROWNUM,A.* 
										 FROM (
										 SELECT A.RETURN_DATE AS WmsDate,Count(*) AS WmsCount 
										   FROM F161201 A
										  WHERE A.DC_CODE =@p0 
												AND A.GUP_CODE = @p1
												AND A.CUST_CODE = @p2
												AND A.RETURN_DATE BETWEEN @p3 AND @p4
												AND A.STATUS<>'9'
											GROUP BY A.RETURN_DATE
										 ) A";
			var param = new object[] { dcCode, gupCode, custCode, begReturnDate.Date, endReturnDate.Date };
			return SqlQuery<DcWmsNoDateItem>(sql, param);

		}

		public IQueryable<SettleData> GetSettleData(string dcCode, string gupCode, string custCode, DateTime settleDate)
		{
			var parameter = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", settleDate),
								new SqlParameter("@p4", settleDate.AddDays(1))
						};
			var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY WMS_NO,ITEM_CODE)ROWNUM,TB.* FROM (
						SELECT @p3 CAL_DATE,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.RETURN_NO WMS_NO,
										B.ITEM_CODE,SUM (B.AUDIT_QTY) QTY,A.RTN_CUST_CODE RETAIL_CODE,
										CASE WHEN A.RTN_CUST_CODE IS NULL THEN '02' ELSE '04' END DELV_ACC_TYPE
							FROM F161201 A
										JOIN F161402 B
											ON A.DC_CODE = B.DC_CODE
										 AND A.GUP_CODE = B.GUP_CODE
										 AND A.CUST_CODE = B.CUST_CODE
										 AND A.RETURN_NO = B.RETURN_NO
							WHERE (A.DC_CODE = @p0 OR @p0 = '000') AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2
										AND A.POSTING_DATE >= @p3 AND A.POSTING_DATE < @p4 AND A.STATUS = '2'
					 GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.RETURN_NO,B.ITEM_CODE,A.RTN_CUST_CODE) TB
					 ";
			return SqlQuery<SettleData>(sql, parameter.ToArray());
		}




		/// <summary>
		/// 取消客戶退貨單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="returnNo"></param>
		/// <param name="importFlag"></param>
		public void CancelNotProcessReturn(string dcCode, string gupCode, string custCode, string returnNo, string importFlag)
		{
			string sql = @"
				           update F161201 set STATUS = '9', UPD_DATE = @p7, UPD_STAFF = @p0, UPD_NAME = @p1, IMPORT_FLAG = @p6
                           Where DC_CODE =@p2
				             and GUP_CODE =@p3
				             and CUST_CODE =@p4
                             and RETURN_NO=@p5
                             and STATUS <> '9'
			               ";
			var sqlParams = new SqlParameter[]
			{
								 new SqlParameter("@p0", Current.Staff),
								 new SqlParameter("@p1", Current.StaffName),
								 new SqlParameter("@p2", dcCode),
								 new SqlParameter("@p3", gupCode),
								 new SqlParameter("@p4", custCode),
								 new SqlParameter("@p5", returnNo),
								 new SqlParameter("@p6", importFlag),
                 new SqlParameter("@p7", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
      };

			ExecuteSqlCommand(sql, sqlParams);
		}

	}
}