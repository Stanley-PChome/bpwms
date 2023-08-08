using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F0537Repository : RepositoryBase<F0537, Wms3plDbContext, F0537Repository>
	{
		public IQueryable<F0537> GetDatasByPickOrdNo(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", pickOrdNo) { SqlDbType = SqlDbType.VarChar });

			var sql = @" SELECT *
						   FROM F0537
						  WHERE DC_CODE = @p0
						    AND GUP_CODE = @p1
						    AND CUST_CODE = @p2
						    AND PICK_ORD_NO = @p3
						";
			return SqlQuery<F0537>(sql, sqlParameter.ToArray());
		}

		public F0537 GetCancelAllotDetail(string dcCode, string gupCode, string custCode, string pickOrdNo, List<string> itemCodes, string status)
		{
			var sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameters.Add(new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameters.Add(new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameters.Add(new SqlParameter("@p3", pickOrdNo) { SqlDbType = SqlDbType.VarChar });
			sqlParameters.Add(new SqlParameter("@p4", status) { SqlDbType = SqlDbType.Char });

			var sql = @"
                        SELECT TOP(1) *
                          FROM F0537 
                         WHERE DC_CODE = @p0
                           AND GUP_CODE = @p1
                           AND CUST_CODE = @p2
                           AND PICK_ORD_NO = @p3
                           AND STATUS = @p4
                       ";
			if (itemCodes.Any())
				sql += sqlParameters.CombineSqlInParameters(" AND ITEM_CODE", itemCodes, SqlDbType.VarChar);

			return SqlQuery<F0537>(sql, sqlParameters.ToArray()).FirstOrDefault();
		}

		public void UpdateStatusByPickOrdNo(string dcCode, string gupCode, string custCode, string pickOrdNo, string status)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", status) { SqlDbType = SqlDbType.Char });
			sqlParameter.Add(new SqlParameter("@p1", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p2", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });
			sqlParameter.Add(new SqlParameter("@p4", dcCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p5", gupCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p6", custCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p7", pickOrdNo) { SqlDbType = SqlDbType.VarChar });

			var sql = $@" UPDATE F0537
							 SET STATUS = @p0, UPD_DATE = @p1, UPD_STAFF = @p2, UPD_NAME = @p3
						   WHERE DC_CODE = @p4 
							 AND GUP_CODE = @p5 
							 AND CUST_CODE = @p6 
							 AND PICK_ORD_NO = @p7 
							 AND STATUS = '0' ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public IQueryable<F0537_LackData> GetLackDatasByPickOrdNo(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", pickOrdNo) { SqlDbType = SqlDbType.VarChar });

			var sql = @" SELECT ITEM_CODE, PICK_ORD_SEQ, B_SET_QTY - A_SET_QTY AS B_LACK_QTY, 0 A_LACK_QTY
						   FROM F0537
						  WHERE DC_CODE = @p0
						    AND GUP_CODE = @p1
						    AND CUST_CODE = @p2
						    AND PICK_ORD_NO = @p3
						    AND STATUS = '2'
						";
			return SqlQuery<F0537_LackData>(sql, sqlParameter.ToArray());
		}
	}
}
