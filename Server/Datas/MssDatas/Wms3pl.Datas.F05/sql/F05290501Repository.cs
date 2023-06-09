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

namespace Wms3pl.Datas.F05
{
	public partial class F05290501Repository : RepositoryBase<F05290501, Wms3plDbContext, F05290501Repository>
	{
		public IQueryable<F05290501> GetDatasByRefId(Int64 refId)
		{
			var parms = new object[] { refId };
			var sql = @" SELECT *
                     FROM F05290501
                    WHERE REF_ID = @p0";
			return SqlQuery<F05290501>(sql, parms);
		}
		public IQueryable<WmsShipBoxDetail> GetWmsShipBoxDetails(string dcCode,string gupCode,string custCode,List<string> wmsOrdNos)
		{
			var parms = new List<object> { dcCode, gupCode, custCode };
			var sql = @" SELECT ROW_NUMBER() OVER(ORDER BY B.WMS_ORD_NO,B.ITEM_CODE,B.SERIAL_NO) ROWNUM,
													ROW_NUMBER() OVER(PARTITION BY A.CONTAINER_CODE,B.WMS_ORD_NO ORDER BY B.ITEM_CODE,B.SERIAL_NO) PACKAGE_BOX_SEQ,
                          A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.DELV_DATE,A.PICK_TIME,B.WMS_ORD_NO,
													A.CONTAINER_CODE BOX_NUM,A.CONTAINER_SEQ PACKAGE_BOX_NO,
													A.CRT_STAFF PACKAGE_STAFF,A.CRT_NAME PACKAGE_NAME,B.ITEM_CODE,B.SERIAL_NO,
													B.A_SET_QTY PACKAGE_QTY,B.ID,B.WMS_ORD_SEQ
									 	 FROM F052905 A
										 JOIN F05290501 B
										   ON B.REF_ID = A.ID
                    WHERE B.DC_CODE = @p0
                      AND B.GUP_CODE = @p1
                      AND B.CUST_CODE = @p2 ";

			if (wmsOrdNos.Any())
				sql += parms.CombineSqlInParameters("AND B.WMS_ORD_NO", wmsOrdNos);

			return SqlQuery<WmsShipBoxDetail>(sql, parms.ToArray());
		}

		public IQueryable<P0808040100_BoxDetailData> GetBoxDetailData(Int64 refId)
		{
			var parm = new List<SqlParameter>();
			parm.Add(new SqlParameter("@p0", refId));

			var sql = @" SELECT 
										ROW_NUMBER ()OVER(ORDER BY A.ITEM_CODE) ROWNUM,
										A.ITEM_CODE ITEM_CODE,
										B.ITEM_NAME ITEM_NAME,
										SUM(A.A_SET_QTY) A_SET_QTY
										FROM F05290501 A
										JOIN F1903 B
										ON A.GUP_CODE = B.GUP_CODE
										AND A.CUST_CODE = B.CUST_CODE
										AND A.ITEM_CODE = B.ITEM_CODE
										WHERE A.REF_ID = @p0
										GROUP BY A.ITEM_CODE, B.ITEM_NAME
									 ";

			return SqlQuery<P0808040100_BoxDetailData>(sql, parm.ToArray());
		}

		public IQueryable<P0808040100_PrintData> GetPrintBoxData(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string moveOutTarget, string containerCode, string sowType)
		{
			var parm = new List<SqlParameter>();
			parm.Add(new SqlParameter("@p0", dcCode));
			parm.Add(new SqlParameter("@p1", gupCode));
			parm.Add(new SqlParameter("@p2", custCode));
			parm.Add(new SqlParameter("@p3", delvDate));
			parm.Add(new SqlParameter("@p4", pickTime));
			parm.Add(new SqlParameter("@p5", moveOutTarget));
			parm.Add(new SqlParameter("@p6", containerCode));
			parm.Add(new SqlParameter("@p7", sowType));

			var sql = @" SELECT 
										ROW_NUMBER ()OVER(ORDER BY B.CONTAINER_CODE,
										B.MOVE_OUT_TARGET,
										B.CONTAINER_SEQ,
										B.DELV_DATE,
										B.PICK_TIME,
										C.CUST_ITEM_CODE,
										C.ITEM_NAME,
										D.CROSS_NAME) ROWNUM,
										B.CONTAINER_CODE PACKAGE_BOX,
										D.CROSS_NAME MOVE_OUT_TARGET,
										RIGHT('000'+CAST(B.CONTAINER_SEQ AS nvarchar(50)),3) CONTAINER_SEQ ,
										CONVERT(char(10), B.DELV_DATE,111) DELV_DATE,
										B.PICK_TIME,
										C.CUST_ITEM_CODE ITEM_CODE,
										C.ITEM_NAME,
										SUM(A.A_SET_QTY) A_SET_QTY,
										C.EAN_CODE1,
										C.EAN_CODE2,
										C.EAN_CODE3 
										--E.ORD_NO WMS_ORD_NO
										FROM F05290501 A
										JOIN F052905 B
										ON A.REF_ID = B.ID
										JOIN F0001 D
										ON B.MOVE_OUT_TARGET = D.CROSS_CODE
										AND D.CROSS_TYPE = '01'
										JOIN F1903 C
										ON A.GUP_CODE = C.GUP_CODE
										AND A.CUST_CODE = C.CUST_CODE
										AND A.ITEM_CODE = C.ITEM_CODE
										--JOIN F05030101 E
										--ON A.DC_CODE =E.DC_CODE
										--AND A.GUP_CODE = E.GUP_CODE 
										--AND A.CUST_CODE =E.CUST_CODE
										--AND A.WMS_ORD_NO =E.WMS_ORD_NO 
										WHERE A.DC_CODE = @p0
										AND B.GUP_CODE = @p1
										AND B.CUST_CODE = @p2
										AND B.DELV_DATE = @p3
										AND B.PICK_TIME = @p4
										AND B.MOVE_OUT_TARGET = @p5
										AND B.CONTAINER_CODE = @p6
										AND B.SOW_TYPE = @p7
										GROUP BY    
										B.CONTAINER_CODE,
										B.MOVE_OUT_TARGET,
										B.CONTAINER_SEQ,
										B.DELV_DATE,
										B.PICK_TIME,
										C.CUST_ITEM_CODE,
										C.ITEM_NAME,
										D.CROSS_NAME,
										C.EAN_CODE1,
										C.EAN_CODE2,
										C.EAN_CODE3 
										--E.ORD_NO
									 ";

			return SqlQuery<P0808040100_PrintData>(sql, parm.ToArray());
		}

		public IQueryable<F05290501> GetF05290501ByWmsNos(string dcCode,string gupCode,string custCode,List<string> wmsNos)
		{
			var para = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode)   { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p1", gupCode)  { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
			};
			var sql = @" SELECT *
                     FROM F05290501 A
                     JOIN F051201 B
                       ON B.DC_CODE = A.DC_CODE
                      AND B.GUP_CODE = A.GUP_CODE
                      AND B.CUST_CODE = A.CUST_CODE
                      AND B.PICK_ORD_NO = A.PICK_ORD_NO
                    WHERE A.DC_CODE = @p0
                      AND A.GUP_CODE = @p1
                      AND A.CUST_CODE = @p2
                      AND B.DISP_SYSTEM = '1' ";
			sql += para.CombineSqlInParameters(" AND A.WMS_ORD_NO", wmsNos, SqlDbType.VarChar);
			return SqlQuery<F05290501>(sql, para.ToArray());
		}
	}
}
