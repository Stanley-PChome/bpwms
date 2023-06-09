using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F050001Repository : RepositoryBase<F050001, Wms3plDbContext, F050001Repository>
	{
		public F050001Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F050001> GetAllDatas()
		{
			return _db.F050001s;
		}

		public IQueryable<F050001> GetNotAllotedDatas()
		{
			var data = _db.F050001s.Where(x => x.PROC_FLAG == "1");

			return data;
		}


		public IQueryable<F050001> GetNotAllotedDatas(List<string> ordNos)
		{
			var data = _db.F050001s.Where(x =>  ordNos.Contains(x.ORD_NO));

			return data;
		}

		public F050001 GetF050001sByNoProcFlag9(string gupCode, string custCode, string ordNo)
		{
			var result = _db.F050001s.Where(x => x.GUP_CODE == gupCode &&
																					 x.CUST_CODE == custCode &&
																					 x.ORD_NO == ordNo &&
																					 x.PROC_FLAG != "9").FirstOrDefault();

			return result;

		}
	}
}
