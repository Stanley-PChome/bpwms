using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F051801Repository : RepositoryBase<F051801, Wms3plDbContext, F051801Repository>
	{
		public IQueryable<F051801> GetDataByConvenientCodeWithVnrCode(string dcCode, string gupCode, string custCode, string convenientCode, string vnrCode)
		{
			var sql = @" SELECT * FROM 
									 F051801
									WHERE DC_CODE = @p0  
									 AND GUP_CODE = @p1
									 AND CUST_CODE= @p2
									 AND CONVENIENT_CODE = @p3
									 AND VNR_CODE = @p4
									 AND STATUS IN ('1','2') ";
			var param = new object[] { dcCode, gupCode, custCode, convenientCode, vnrCode };

			return SqlQuery<F051801>(sql, param);
		}

		public string LockF051801()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F051801';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}

		/// <summary>
		/// 釋放儲格
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="convenientCode"></param>
		/// <param name="cellCode"></param>
		/// <param name="status"></param>
		/// <param name="setStatus"></param>
		public void UpdateStatusByScheduled(string dcCode, string convenientCode, string cellCode, string status, string setStatus)
		{
			var parm = new List<SqlParameter>
						{
								new SqlParameter("@p0", setStatus),
								new SqlParameter("@p1", Current.Staff),
								new SqlParameter("@p2", Current.StaffName),
								new SqlParameter("@p3", dcCode),
								new SqlParameter("@p4", convenientCode),
								new SqlParameter("@p5", cellCode),
                new SqlParameter("@p6", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };

			var sql = @" UPDATE F051801 SET STATUS = @p0, VNR_CODE = NULL, GUP_CODE = NULL, CUST_CODE = NULL, UPD_DATE = @p6, UPD_STAFF=@p1, UPD_NAME=@p2 
										WHERE DC_CODE = @p3
										AND CONVENIENT_CODE =@p4
										AND CELL_CODE = @p5
                    ";

			if (!string.IsNullOrWhiteSpace(status))
			{
				sql += $"AND STATUS = @p{parm.Count}";
				parm.Add(new SqlParameter($"@p{parm.Count}", status));
			}

			ExecuteSqlCommand(sql, parm.ToArray());
		}

		/// <summary>
		/// 釋放儲格
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="exclideCellCode"></param>
		public void UpdateStatusByScheduledExcludeCellCode(string dcCode, string exclideCellCode)
		{
			var parm = new List<SqlParameter>
						{
								new SqlParameter("@p0", Current.Staff),
								new SqlParameter("@p1", Current.StaffName),
								new SqlParameter("@p2", dcCode),
								new SqlParameter("@p3", exclideCellCode),
								new SqlParameter("@p4", Current.Staff),
                new SqlParameter("@p5", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };

			var sql = @" UPDATE F051801 
									 SET STATUS='0',
									 VNR_CODE=NULL,
									 GUP_CODE=NULL,
									 CUST_CODE=NULL, 
									 UPD_DATE = @p5, 
									 UPD_STAFF= @p0, 
									 UPD_NAME= @p1 
									 WHERE DC_CODE = @p2
									 AND CELL_CODE <> @p3
									 AND STATUS = '1'
									 AND UPD_STAFF = @p4
                    ";

			ExecuteSqlCommand(sql, parm.ToArray());
		}
	}
}
