using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F00.Interfaces;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
  public partial class F009004Repository : RepositoryBase<F009004, Wms3plDbContext, F009004Repository>, IApiLogRepository<F009004>
  {

    public IQueryable<F009004> GetData(string name)
    {
      var param = new object[] { name };
      var sql = @"SELECT * FROM F009004 WHERE NAME = @p0 AND STATUS IS NULL";
      return SqlQuery<F009004>(sql, param);
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
      var sql = @" UPDATE F009004
                     SET STATUS = @p0,
                         ERRMSG = @p1,
                         RETURN_DATA = @p2,
                         UPD_DATE = dbo.GetSysDate(),
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

      var sql = @" INSERT INTO F009004(DC_CODE,GUP_CODE,CUST_CODE,NAME,SEND_DATA,RETURN_DATA,ERRMSG,STATUS,
																			 CRT_DATE,CRT_STAFF,CRT_NAME,UPD_DATE,UPD_STAFF,UPD_NAME)
											VALUES(@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,dbo.GetSysDate(),@p9,@p10); ";

      ExecuteSqlCommand(sql, parm.ToArray());
    }

  }
}

