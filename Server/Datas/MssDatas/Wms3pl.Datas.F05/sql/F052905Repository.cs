using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F052905Repository : RepositoryBase<F052905, Wms3plDbContext, F052905Repository>
	{
		public BoxInfo GetBoxInfo(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string moveOutTarget, string sowType)
		{
			var parms = new object[] { dcCode, gupCode, custCode, delvDate, pickTime, moveOutTarget, sowType };
			var sql = @" SELECT A.SOW_TYPE SowType,A.CONTAINER_CODE BoxNo,SUM(B.A_SET_QTY) SowQty
										 FROM F052905 A
										 LEFT JOIN F05290501 B
										   ON B.REF_ID = A.ID
										WHERE A.DC_CODE = @p0
										  AND A.GUP_CODE = @p1
										  AND A.CUST_CODE = @p2
										  AND A.DELV_DATE = @p3
										  AND A.PICK_TIME = @p4
										  AND A.MOVE_OUT_TARGET= @p5
										  AND A.SOW_TYPE = @p6
										  AND A.STATUS ='0'
										GROUP BY  A.SOW_TYPE,A.CONTAINER_CODE";
			return SqlQuery<BoxInfo>(sql, parms).FirstOrDefault();
		}

		public F052905 GetData(string dcCode,string gupCode,string custCode,DateTime delvDate,string pickTime,string moveOutTarget,string sowType,string containerCode)
		{
			var parms = new object[] { dcCode, gupCode, custCode, delvDate, pickTime, moveOutTarget, sowType, containerCode };
			var sql = @" SELECT *
                     FROM F052905
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND DELV_DATE = @p3
                      AND PICK_TIME = @p4
                      AND MOVE_OUT_TARGET = @p5
                      AND SOW_TYPE = @p6
                      AND CONTAINER_CODE =@p7
                      AND STATUS <> '2'";
			return SqlQuery<F052905>(sql, parms).FirstOrDefault();
		}
		public F052905 GetData(string dcCode, string containerCode)
		{
			var parms = new object[] { dcCode,  containerCode };
			var sql = @" SELECT *
                     FROM F052905
                    WHERE DC_CODE = @p0
                      AND CONTAINER_CODE =@p1
                      AND STATUS <> '2'";
			return SqlQuery<F052905>(sql, parms).FirstOrDefault();
		}

		public void UpdateToDebit(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime)
		{
			var parms = new object[] { DateTime.Now,Current.Staff,Current.StaffName, dcCode, gupCode, custCode, delvDate, pickTime };
			var sql = @" UPDATE F052905 SET STATUS= '2',UPD_DATE = @p0,UPD_STAFF=@p1,UPD_NAME = @p2
                   WHERE DC_CODE = @p3
                     AND GUP_CODE = @p4
                     AND CUST_CODE = @p5
                     AND DELV_DATE = @p6
                     AND PICK_TIME = @p7 ";

			ExecuteSqlCommand(sql, parms);
		}

		public IQueryable<P0808040100_BoxData> GetBoxData(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string sowType, string status)
		{
			var parm = new List<SqlParameter>();
			parm.Add(new SqlParameter("@p0", dcCode));
			parm.Add(new SqlParameter("@p1", gupCode));
			parm.Add(new SqlParameter("@p2", custCode));
			parm.Add(new SqlParameter("@p3", delvDate));
			parm.Add(new SqlParameter("@p4", pickTime));

			var sql = @" SELECT 
									 A.ID ID,
									 A.DC_CODE,
									 A.GUP_CODE,
									 A.CUST_CODE,
									 A.CONTAINER_CODE BOX_NUM,
									 A.MOVE_OUT_TARGET,
									 B.CROSS_NAME MOVE_OUT_TARGET_NAME,
									 A.SOW_TYPE, 
									 C.NAME SOW_TYPE_NAME,
									 A.STATUS STATUS,
									 D.NAME STATUS_NAME,
									 A.CONTAINER_CODE,
									 A.DELV_DATE DELV_DATE,
									 A.PICK_TIME PICK_TIME
									 FROM F052905 A
									 JOIN F0001 B
									 ON A.MOVE_OUT_TARGET = B.CROSS_CODE
									 AND B.CROSS_TYPE = '01'
									 JOIN F000904 C
									 ON A.SOW_TYPE = C.VALUE
									 AND C.TOPIC = 'F052905'
									 AND C.SUBTOPIC = 'SOW_TYPE'
									 JOIN F000904 D
									 ON A.STATUS = D.VALUE
									 AND D.TOPIC = 'F052905'
									 AND D.SUBTOPIC = 'STATUS'
									 WHERE A.DC_CODE = @p0
									   AND A.GUP_CODE = @p1
									   AND A.CUST_CODE = @p2
									   AND A.DELV_DATE = @p3
									   AND A.PICK_TIME = @p4
									 ";

			string addCondition = string.Empty;
			if (!string.IsNullOrWhiteSpace(sowType))
			{
				addCondition += $" AND A.SOW_TYPE = @p{parm.Count}";
				parm.Add(new SqlParameter($"@p{parm.Count}", sowType));
			}

			if (!string.IsNullOrWhiteSpace(status))
			{
				addCondition += $" AND A.STATUS = @p{parm.Count}";
				parm.Add(new SqlParameter($"@p{parm.Count}", status));
			}

			if (!string.IsNullOrWhiteSpace(addCondition))
				sql += addCondition;

			return SqlQuery<P0808040100_BoxData>(sql, parm.ToArray());
		}
	}
}
