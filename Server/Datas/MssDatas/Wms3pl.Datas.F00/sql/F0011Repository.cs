using Microsoft.EntityFrameworkCore;
using System;
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
	public partial class F0011Repository : RepositoryBase<F0011, Wms3plDbContext, F0011Repository>
	{
		public F0011 GetDatasForNotClosed(string dcCode, string gupCode, string custCode, string orderNo)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",dcCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",gupCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p2",custCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p3",orderNo){SqlDbType = SqlDbType.VarChar},
			};
			var sql = @" SELECT TOP(1) *
                        FROM F0011 
                       WHERE DC_CODE = @p0
                         AND GUP_CODE = @p1
                         AND CUST_CODE = @p2
                         AND ORDER_NO = @p3
                         AND CLOSE_DATE  IS NULL ";
			return SqlQuery<F0011>(sql, parms.ToArray()).FirstOrDefault();
		}

		public IQueryable<F0011> GetDatasForNotClosed(string dcCode, string gupCode, string custCode, List<string> orderNos)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",dcCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",gupCode){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p2",custCode){SqlDbType = SqlDbType.VarChar},
			};
			var sql = @" SELECT *
                        FROM F0011 
                       WHERE DC_CODE = @p0
                         AND GUP_CODE = @p1
                         AND CUST_CODE = @p2
                         AND CLOSE_DATE  IS NULL ";
			sql += parms.CombineSqlInParameters(" AND ORDER_NO", orderNos, SqlDbType.VarChar);
			return SqlQuery<F0011>(sql, parms.ToArray());
		}

		public F0011 GetDataById(Int64 id)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",id){SqlDbType = SqlDbType.BigInt},
			};
			var sql = @" SELECT TOP(1) *
                        FROM F0011 
                       WHERE ID = @p0 ";
			return SqlQuery<F0011>(sql, parms.ToArray()).FirstOrDefault();
		}
	}
}



