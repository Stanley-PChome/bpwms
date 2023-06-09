using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F05010103Repository : RepositoryBase<F05010103, Wms3plDbContext, F05010103Repository>
	{
		public F05010103Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F05010103> GetDatasByOrdNo(string dcCode,string gupCode,string custCode,string ordNo,string type)
		{
            var result = _db.F05010103s.Where(x => x.DC_CODE == dcCode &&
                                                   x.GUP_CODE == gupCode &&
                                                   x.CUST_CODE == custCode &&
                                                   x.ORD_NO == ordNo &&
                                                   x.TYPE == type)
                                        .OrderBy(x => x.CRT_DATE);

            return result;
		}
	}
}
