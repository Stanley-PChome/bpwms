using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
	public partial class F060601Repository : RepositoryBase<F060601, Wms3plDbContext, F060601Repository>
	{
		public F060601Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public F060601 GetData(string dcCode, string srcSystem, string calDate, int currentPage)
		{
			return _db.F060601s.Where(x => x.DC_CODE == dcCode &&
			x.SRC_SYSTEM == srcSystem &&
			x.CAL_DATE == calDate &&
			x.CURRENT_PAGE == currentPage &&
			x.PROC_FLAG != "9").OrderByDescending(x => x.CRT_DATE).FirstOrDefault();
		}

		public IQueryable<F060601> GetDatasByDcWithCalDate(string dcCode, string calDate)
		{
			return _db.F060601s.AsNoTracking().Where(x => x.DC_CODE == dcCode && x.CAL_DATE == calDate && x.PROC_FLAG == "0");
		}
	}
}
