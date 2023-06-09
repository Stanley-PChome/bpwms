using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F056002Repository : RepositoryBase<F056002, Wms3plDbContext, F056002Repository>
	{
		public F056002Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public string LockF056002()
		{
			var sql = "SELECT TOP 1 UPD_LOCK_TABLE_NAME FROM F0000 WITH(UPDLOCK) WHERE UPD_LOCK_TABLE_NAME ='f056002';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}

		public F056002 GetF056002(string dcCode, string gupCode, string custCode, string workstationCode, string boxCode)
		{
			List<SqlParameter> param = new List<SqlParameter>
			{
				new SqlParameter("@p0",System.Data.SqlDbType.VarChar){ Value = dcCode},
				new SqlParameter("@p1",System.Data.SqlDbType.VarChar){ Value = gupCode},
				new SqlParameter("@p2",System.Data.SqlDbType.VarChar){ Value = custCode},
				new SqlParameter("@p3",System.Data.SqlDbType.VarChar){ Value = workstationCode},
				new SqlParameter("@p4",System.Data.SqlDbType.VarChar){ Value = boxCode}
			};

			var sql = @"SELECT * FROM f056002
									WHERE DC_CODE = @p0
									AND GUP_CODE = @p1
									AND CUST_CODE = @p2
									AND WORKSTATION_CODE = @p3
									AND BOX_CODE = @p4
									AND STATUS IN ('0','1')";

			return SqlQuery<F056002>(sql, param.ToArray()).FirstOrDefault();
		}

		public IQueryable<F056002> GetF056002ById(Int64 id)
		{
			List<SqlParameter> param = new List<SqlParameter>
			{
				new SqlParameter("@p0",System.Data.SqlDbType.BigInt){ Value = id}
			};

			var sql = @"SELECT * FROM F056002
									WHERE ID = @p0";
			return SqlQuery<F056002>(sql, param.ToArray());
		}

		public void UpdateF056002(Int64 id)
		{
			List<SqlParameter> param = new List<SqlParameter>
			{
				new SqlParameter("@p0",System.Data.SqlDbType.BigInt){ Value = id}
			};
			var sql = @"UPDATE F056002 SET STATUS = '0',
								REPLENISH_STAFF = NULL,
								REPLENISH_NAME = NULL,
								REPLENISH_STARTTIME = NULL
								WHERE ID = @p0";

			ExecuteSqlCommand(sql, param.ToArray());
		}

		public IQueryable<GetCartonReplenishRes> GetCartonReplenish(string dcCode, string gupCode, string custCode, string accNo,string floor,string workStationCode,string boxCode)
		{
			List<SqlParameter> param = new List<SqlParameter>
			{
				new SqlParameter("@p0",System.Data.SqlDbType.VarChar){ Value = dcCode},
				new SqlParameter("@p1",System.Data.SqlDbType.VarChar){ Value = gupCode},
				new SqlParameter("@p2",System.Data.SqlDbType.VarChar){ Value = custCode},
				new SqlParameter("@p3",System.Data.SqlDbType.VarChar){ Value = accNo}
			};

			var sql = @"SELECT ID,
												WORKSTATION_CODE WorkStationCode,
												BOX_CODE BoxCode,
												CRT_DATE NoticeTime,
												STATUS Status,
												(SELECT TOP(1) NAME FROM F000904 WHERE TOPIC = 'F056002' AND VALUE = STATUS) StatusName
									FROM F056002 WHERE 
									DC_CODE =@p0
									AND GUP_CODE =@p1
									AND CUST_CODE =@p2
									AND (STATUS  = '0'
									OR (STATUS ='1' AND REPLENISH_STAFF = @p3))";

			if (!string.IsNullOrWhiteSpace(floor))
			{
				sql += $@" AND FLOOR = @p{param.Count}";
				param.Add(new SqlParameter("@p" + param.Count, System.Data.SqlDbType.VarChar) { Value = floor });
			}

			if (!string.IsNullOrWhiteSpace(workStationCode))	
			{
				sql += $@" AND WORKSTATION_CODE = @p{param.Count}";
				param.Add(new SqlParameter("@p" + param.Count, System.Data.SqlDbType.VarChar) { Value = workStationCode });
			}

			if (!string.IsNullOrWhiteSpace(boxCode))
			{
				sql += $@" AND BOX_CODE = @p{param.Count}";
				param.Add(new SqlParameter("@p" + param.Count, System.Data.SqlDbType.VarChar) { Value = boxCode });
			}

			sql += @" ORDER BY CAST(SUBSTRING(BOX_CODE,CHARINDEX('-',BOX_CODE)+1,LEN(BOX_CODE)- CHARINDEX('-',BOX_CODE)) AS INT),
									RIGHT(WORKSTATION_CODE ,LEN( WORKSTATION_CODE )-1)";

			return SqlQuery<GetCartonReplenishRes>(sql, param.ToArray());
		}
	}
}
