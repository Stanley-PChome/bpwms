﻿using System.Collections.Generic;
using System.Data.SqlClient;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.DBCore;
using System.Linq;
using System;
using Wms3pl.Datas.F00.Interfaces;
using System.Transactions;
using System.Data;

namespace Wms3pl.Datas.F00
{
	public partial class F0091Repository : RepositoryBase<F0091, Wms3plDbContext, F0091Repository>, IApiLogRepository<F0091>
  {
		public IQueryable<F0091> GetData(string name)
		{
			var param = new object[] { name };
			var sql = @"SELECT * FROM F0091 WHERE NAME = @p0 AND STATUS IS NULL";
			return SqlQuery<F0091>(sql, param);
		}

		public void UpdateLog(int id, string status, string errMsg, string retrunData)
		{
			if (!string.IsNullOrWhiteSpace(errMsg))
			{
				if (errMsg.Length > 200)
					errMsg = errMsg.Substring(0, 200);
			}
			if (!string.IsNullOrWhiteSpace(retrunData))
			{
				if (retrunData.Length > 4000)
					retrunData = retrunData.Substring(0, 4000);
			}
			var parm = new List<SqlParameter>();
			parm.Add(new SqlParameter("@p0", status));
			parm.Add(new SqlParameter("@p1", string.IsNullOrWhiteSpace(errMsg) ? (object)DBNull.Value : errMsg));
			parm.Add(new SqlParameter("@p2", string.IsNullOrWhiteSpace(retrunData) ? (object)DBNull.Value : retrunData));
			parm.Add(new SqlParameter("@p3", Current.Staff));
			parm.Add(new SqlParameter("@p4", Current.StaffName));
			parm.Add(new SqlParameter("@p5", id));
      parm.Add(new SqlParameter("@p6", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });

      var sql = @" UPDATE F0091
                     SET STATUS = @p0,
                         ERRMSG = @p1,
                         RETURN_DATA = @p2,
                         UPD_DATE = @p6,
                         UPD_STAFF = @p3,
                         UPD_NAME = @p4
                   WHERE ID = @p5 ";
			ExecuteSqlCommand(sql, parm.ToArray());
		}

		public void InsertLog(string dcCode, string gupCode, string custCode, string apiName, string sendData, string returnData, string errMsg, string status, DateTime startTime)
		{
			var parm = new List<SqlParameter>();
			parm.Add(new SqlParameter("@p0", dcCode));
			parm.Add(new SqlParameter("@p1", gupCode));
			parm.Add(new SqlParameter("@p2", custCode));
			parm.Add(new SqlParameter("@p3", apiName));
			parm.Add(new SqlParameter("@p4", sendData));
			parm.Add(new SqlParameter("@p5", string.IsNullOrWhiteSpace(returnData) ? (object)DBNull.Value : returnData));
			parm.Add(new SqlParameter("@p6", string.IsNullOrWhiteSpace(errMsg) ? (object)DBNull.Value : errMsg));
			parm.Add(new SqlParameter("@p7", status));
			parm.Add(new SqlParameter("@p8", startTime));
			parm.Add(new SqlParameter("@p9", Current.Staff));
			parm.Add(new SqlParameter("@p10", Current.StaffName));
      parm.Add(new SqlParameter("@p11", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });

      var sql = @" INSERT INTO F0091(DC_CODE,GUP_CODE,CUST_CODE,NAME,SEND_DATA,RETURN_DATA,ERRMSG,STATUS,
																			 CRT_DATE,CRT_STAFF,CRT_NAME,UPD_DATE,UPD_STAFF,UPD_NAME)
											VALUES(@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p9,@p10); ";

			ExecuteSqlCommand(sql, parm.ToArray());
		}

    
  }
}
