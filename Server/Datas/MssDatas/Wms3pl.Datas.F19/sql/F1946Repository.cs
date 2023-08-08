using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F1946Repository : RepositoryBase<F1946, Wms3plDbContext, F1946Repository>
	{
		public IQueryable<F1946> GetWorkstationList(string dcCode, string workstationGroup, string workstationType, string workstationCode, string status)
		{

			var sql = @"SELECT * FROM F1946 WHERE 1=1";
			var param = new List<object> { };
			if (!string.IsNullOrWhiteSpace(dcCode))
			{
				sql += " AND DC_CODE = @p" + param.Count;
				param.Add(dcCode);
			}
			if (!string.IsNullOrWhiteSpace(workstationGroup))
			{
				sql += " AND WORKSTATION_GROUP = @p" + param.Count;
				param.Add(workstationGroup);
			}
			if (!string.IsNullOrWhiteSpace(workstationType))
			{
				sql += " AND WORKSTATION_TYPE = @p" + param.Count;
				param.Add(workstationType);
			}
			if (!string.IsNullOrWhiteSpace(workstationCode))
			{
				sql += " AND WORKSTATION_CODE = @p" + param.Count;
				param.Add(workstationCode);
			}
			if (!string.IsNullOrWhiteSpace(status))
			{
				sql += " AND STATUS = @p" + param.Count;
				param.Add(status);
			}


			return SqlQuery<F1946>(sql, param.ToArray());
		}

		public void Update(string status,string dcCode,string workstationCode)
		{
			var param = new object[] { status, DateTime.Now, Current.Staff, Current.StaffName, dcCode, workstationCode };
			var sql = @"UPDATE F1946
						SET STATUS = @p0,
						UPD_DATE = @p1,
						UPD_STAFF = @p2,
						UPD_NAME = @p3
						WHERE DC_CODE = @p4
						AND WORKSTATION_CODE = @P5";

			ExecuteSqlCommand(sql, param.ToArray());
		}

		public void Update(WorkstationData workstationData)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",System.Data.SqlDbType.Char){Value = workstationData.WORKSTATION_GROUP},
				new SqlParameter("@p1",System.Data.SqlDbType.VarChar){Value = workstationData.WORKSTATION_TYPE},
				new SqlParameter("@p2",System.Data.SqlDbType.DateTime2){Value = DateTime.Now},
				new SqlParameter("@p3",System.Data.SqlDbType.VarChar){Value = Current.Staff},
				new SqlParameter("@p4",System.Data.SqlDbType.NVarChar){Value = Current.StaffName},
				new SqlParameter("@p5",System.Data.SqlDbType.VarChar){Value = workstationData.DC_CODE},
				new SqlParameter("@p6",System.Data.SqlDbType.VarChar){Value = workstationData.WORKSTATION_CODE},
			};
			var sql = @"UPDATE F1946
						SET WORKSTATION_GROUP = @p0,
						WORKSTATION_TYPE = @p1,
						UPD_DATE = @p2,
						UPD_STAFF = @p3,
						UPD_NAME = @p4
						WHERE DC_CODE = @p5
						AND WORKSTATION_CODE = @p6";

			ExecuteSqlCommandWithSqlParameterSetDbType(sql, param.ToArray());
		}
	}
}
