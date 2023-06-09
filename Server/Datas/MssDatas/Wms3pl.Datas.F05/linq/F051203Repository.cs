using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F051203Repository : RepositoryBase<F051203, Wms3plDbContext, F051203Repository>
	{
		public F051203Repository(string connName, WmsTransaction wmsTransaction = null)
					: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F051203> GetDatasByPickNos(string dcCode, string gupCode, string custCode, List<string> pickNos)
		{
			return _db.F051203s.AsNoTracking().Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && pickNos.Contains(x.PICK_ORD_NO));
		}


		public IQueryable<F051203> GetDatasByPickNosNotStatus(string dcCode, string gupCode, string custCode, string pickStatus, List<string> pickNos)
		{
			return GetDatasByPickNos(dcCode, gupCode, custCode, pickNos).Where(x => x.PICK_STATUS != pickStatus);
		}
	}
}
