using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F050007Repository : RepositoryBase<F050007, Wms3plDbContext, F050007Repository>
	{
		public F050007Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F050007> GetDatas(string gupCode, string custCode)
		{
            var result = _db.F050007s.Where(x => x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode);

            return result;
		}
	}
}
