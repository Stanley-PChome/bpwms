using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F1955Repository : RepositoryBase<F1955, Wms3plDbContext, F1955Repository>
	{
		/// <summary>
		/// 取得該使用者便利倉資料
		/// </summary>
		/// <param name="empId"></param>
		/// <returns></returns>
		public IQueryable<ConvenientInfo> GetConvenient(string empId)
		{
			var parm = new List<SqlParameter>();
			parm.Add(new SqlParameter("@p0", empId));

			string sql = $@" SELECT DISTINCT 
											b.DC_CODE DcCode,
											b.CONVENIENT_CODE ConvenientCode,
											b.CONVENIENT_NAME ConvenientName
											FROM  F192402 a, F1955 b 
											Where b.DC_CODE = A.DC_CODE AND a.EMP_ID=@p0 ";
			var result = SqlQuery<ConvenientInfo>(sql, parm.ToArray());
			return result;
		}
	}
}
