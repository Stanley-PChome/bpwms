using System;
using System.Collections.Generic;
using System.Data;
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
	public partial class F194501Repository : RepositoryBase<F194501, Wms3plDbContext, F194501Repository>
	{
		/// <summary>
		/// 取得該使用者集貨場儲格資料
		/// </summary>
		/// <param name="empId"></param>
		/// <returns></returns>
		public IQueryable<CellInfo> GetCellType(string empId)
		{
			var parm = new List<SqlParameter>();
			parm.Add(new SqlParameter("@p0", empId));

			string sql = $@" SELECT DISTINCT 
												c.DC_CODE DcCode,
												b.CELL_TYPE CellType,
												b.CELL_NAME CellName
												FROM  F192402 a, F194501 b ,F1945 c
												Where b.DC_CODE = a.DC_CODE AND a.DC_CODE = c.DC_CODE AND a.EMP_ID=@p0 ";
			var result = SqlQuery<CellInfo>(sql, parm.ToArray());
			return result;
		}

		public void DeleteF194501(string dcCode,string cellType)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.VarChar){ Value = dcCode},
				new SqlParameter("@p1",SqlDbType.VarChar){Value = cellType}
			};
			var sql = @"DELETE F194501 FROM F194501 WHERE DC_CODE =@p0 AND CELL_TYPE = @p1";

			ExecuteSqlCommand(sql, param.ToArray());
		}
	}
}
