using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F070101Repository : RepositoryBase<F070101, Wms3plDbContext, F070101Repository>
	{
		public F070101Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public F070101 GetData(string dcCode, string gupCode,string custCode,long? id)
		{
			return _db.F070101s.Where(x => x.DC_CODE == dcCode 
										&& x.GUP_CODE == gupCode 
										&& x.CUST_CODE == custCode
										&& x.F0701_ID == id).FirstOrDefault();
		}

		public IQueryable<F070101> GetDatasByF0701Ids(List<long> f0701Ids)
		{
			return _db.F070101s.AsNoTracking().Where(x => f0701Ids.Contains(x.F0701_ID));
		}

		public F070101 GetDataByF0701Id(long f0701Id)
		{
			return _db.F070101s.AsNoTracking().Where(x => x.F0701_ID == f0701Id).FirstOrDefault();
		}
	}
}
