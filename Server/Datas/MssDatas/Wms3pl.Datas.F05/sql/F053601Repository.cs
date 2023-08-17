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
	public partial class F053601Repository : RepositoryBase<F053601, Wms3plDbContext, F053601Repository>
	{
		public IQueryable<BindingPickContainerDetail> GetBindingPickContainerDetails(long f0701_Id)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", f0701_Id) { SqlDbType = SqlDbType.BigInt });

			var sql = @"
                        SELECT F053601.ITEM_CODE, 
                               F1903.ITEM_NAME, 
                               F1903.EAN_CODE1, 
                               F1903.EAN_CODE2, 
                               F1903.EAN_CODE3, 
                               F1903.BUNDLE_SERIALNO, 
                               F053601.B_SET_QTY, 
                               F053601.A_SET_QTY
                          FROM F053601 
                          JOIN F1903 ON F1903.GUP_CODE = F053601.GUP_CODE 
                                    AND F1903.CUST_CODE = F053601.CUST_CODE 
                                    AND F1903.ITEM_CODE = F053601.ITEM_CODE 
                         WHERE F053601.F0701_ID = @p0
                       ";

			return SqlQuery<BindingPickContainerDetail>(sql, sqlParameter.ToArray());
		}

		public IQueryable<F053601> GetLackDatas(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", pickOrdNo) { SqlDbType = SqlDbType.VarChar });

			var sql = @"
                        SELECT F053601.*
                          FROM F053601 
                          JOIN F0536 ON F0536.F0701_ID = F053601.F0701_ID 
                         WHERE F053601.STATUS = '2' 
                           AND F0536.DC_CODE = @p0 
                           AND F0536.GUP_CODE = @p1 
                           AND F0536.CUST_CODE = @p2 
                           AND F0536.PICK_ORD_NO = @p3 
                       ";

			return SqlQuery<F053601>(sql, sqlParameter.ToArray());
		}

		public F053601 GetAllotDetail(long f0701_Id, List<string> itemCodes, string status)
		{
			var sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(new SqlParameter("@p0", f0701_Id) { SqlDbType = SqlDbType.BigInt });
			sqlParameters.Add(new SqlParameter("@p1", status) { SqlDbType = SqlDbType.Char });

			var sql = @"
                        SELECT TOP(1) *
                          FROM F053601 
                         WHERE F0701_ID = @p0
                           AND STATUS = @p1
                       ";
			if (itemCodes.Any())
				sql += sqlParameters.CombineSqlInParameters(" AND ITEM_CODE", itemCodes, SqlDbType.VarChar);

			return SqlQuery<F053601>(sql, sqlParameters.ToArray()).FirstOrDefault();
		}

		public IQueryable<F053601_NotAllotData> GetNotAllotDataInPickContainer(long f0701_Id)
		{
			var sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(new SqlParameter("@p0", f0701_Id) { SqlDbType = SqlDbType.BigInt });

			var sql = @"
                        SELECT F1903.ITEM_CODE, F1903.ITEM_NAME, F053601.B_SET_QTY - F053601.A_SET_QTY AS NOALLOT_QTY
                          FROM F053601 
                          LEFT JOIN F1903 WITH(NOLOCK)
                            ON F053601.ITEM_CODE = F1903.ITEM_CODE 
                           AND F053601.GUP_CODE = F1903.GUP_CODE
                           AND F053601.CUST_CODE = F1903.CUST_CODE
                         WHERE F053601.F0701_ID = @p0
                           AND F053601.STATUS = '0'
                       ";

			return SqlQuery<F053601_NotAllotData>(sql, sqlParameters.ToArray());
		}

		public void UpdateStatusByF0701Id(long f0701_Id, string status)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", status) { SqlDbType = SqlDbType.Char });
			sqlParameter.Add(new SqlParameter("@p1", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p2", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });
			sqlParameter.Add(new SqlParameter("@p4", f0701_Id) { SqlDbType = SqlDbType.BigInt });

			var sql = @" UPDATE F053601
							SET STATUS = @p0, UPD_DATE = @p1, UPD_STAFF = @p2, UPD_NAME = @p3
					  WHERE F0701_ID = @p4 AND STATUS='0' ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public IQueryable<F053601> GetDatasByF0701Id(long f0701_Id)
		{
			var sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(new SqlParameter("@p0", f0701_Id) { SqlDbType = SqlDbType.BigInt });

			var sql = @"
                        SELECT *
                          FROM F053601 
                         WHERE F0701_ID = @p0
                       ";

			return SqlQuery<F053601>(sql, sqlParameters.ToArray());
		}
	}
}
