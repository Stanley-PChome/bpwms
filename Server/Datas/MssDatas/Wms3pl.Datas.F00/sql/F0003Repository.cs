﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
    public partial class F0003Repository : RepositoryBase<F0003, Wms3plDbContext, F0003Repository>
    {
		/// <summary>
		/// 取得F0003蘋果廠商編號清單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		public List<string> GetAppleVendor(string dcCode, string gupCode, string custCode)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode) {SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1", gupCode) {SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p2", custCode) {SqlDbType = SqlDbType.VarChar},
			};

			var sql = @"SELECT SYS_PATH FROM F0003
                         WHERE AP_NAME = 'TheVendorOfApple'
                           AND DC_CODE = @p0
                           AND GUP_CODE = @p1
                           AND CUST_CODE = @p2";

			var value = SqlQuery<string>(sql, param.ToArray()).SingleOrDefault() ?? string.Empty;
			return value.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
		}

    public bool IsVideoCombinIn(string dcCode)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) {SqlDbType = SqlDbType.VarChar},
      };

      var sql = @"SELECT TOP(1) SYS_PATH FROM F0003
                         WHERE AP_NAME = 'VideoCombinIn'
                           AND DC_CODE = @p0";

      var value = SqlQuery<string>(sql, param.ToArray()).SingleOrDefault() ?? string.Empty;
      return value == "1";
    }

    public bool IsRecvItemNeedContainer(string dcCode, string gupCode, string custCode)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) {SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p1", gupCode) {SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p2", custCode) {SqlDbType = SqlDbType.VarChar},
      };

      var sql = @"SELECT SYS_PATH FROM F0003
                         WHERE AP_NAME = 'RecvItemNeedContainer'
                           AND DC_CODE = @p0
                           AND GUP_CODE = @p1
                           AND CUST_CODE = @p2";

      var value = SqlQuery<string>(sql, param.ToArray()).SingleOrDefault() ?? string.Empty;
      return value == "0";
    }

    public bool IsPrintRecvnote(string dcCode, string gupCode, string custCode)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) {SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p1", gupCode) {SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p2", custCode) {SqlDbType = SqlDbType.VarChar},
      };

      var sql = @"SELECT SYS_PATH FROM F0003
                         WHERE AP_NAME = 'AutoPrintReports'
                           AND DC_CODE = @p0
                           AND GUP_CODE = @p1
                           AND CUST_CODE = @p2";

      var value = SqlQuery<string>(sql, param.ToArray()).SingleOrDefault() ?? string.Empty;
      return value != "0";
    }

    public IQueryable<F0003> GetImmediateItemScheduleDatas(List<string> custCodes)
    {
      if (custCodes == null || !custCodes.Any())
        return null;

      var para = new List<SqlParameter>();

      var sql = @"SELECT * FROM F0003 WHERE AP_NAME IN('ImmediateItemWhId','PickLossWHId','InventoryLossWHId')";

      sql += para.CombineSqlInParameters("AND CUST_CODE", custCodes, SqlDbType.VarChar);

      return SqlQuery<F0003>(sql, para.ToArray());
    }

  }
}

