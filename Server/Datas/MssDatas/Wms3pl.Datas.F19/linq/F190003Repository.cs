using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F190003Repository : RepositoryBase<F190003, Wms3plDbContext, F190003Repository>
	{
		public F190003Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public IQueryable<decimal> GetGrpId1s(string dcCode, string gupCode, string custCode, string workNo)
		{
            var query = _db.F190003s.Where(x => 
                                x.DC_CODE == dcCode
                            && x.GUP_CODE == gupCode
                            && x.CUST_CODE == custCode
                            && x.WORK_NO == workNo
                            );
            return query.Select(x => x.GRP_ID_1.Value);
		}
	}
}
