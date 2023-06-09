using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F075102Repository : RepositoryBase<F075102, Wms3plDbContext, F075102Repository>
	{
		public string LockF075102()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F075102';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}

		public void DelF075102ByKey(string custCode, string custOrdNo)
		{
			var sql = @" DELETE F075102 
                           WHERE CUST_CODE =@p0 
                             AND CUST_ORD_NO =@p1 ";
			var param = new object[] { custCode, custOrdNo };
			ExecuteSqlCommand(sql, param);
		}

		public void DelF075102ByCustOrdNos(string custCode, List<string> custOrdNos)
		{
			var parameters = new List<object>
						{
								custCode
						};

			var sql = @"DELETE F075102 
									WHERE CUST_CODE = @p0
								 ";

			sql += parameters.CombineSqlInParameters(" AND CUST_ORD_NO ", custOrdNos);
			ExecuteSqlCommand(sql, parameters.ToArray());
		}
	}
}
