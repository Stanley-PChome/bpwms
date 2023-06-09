using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F050104Repository : RepositoryBase<F050104, Wms3plDbContext, F050104Repository>
	{
		public F050104Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

        public IQueryable<F050104> GetDatas(string dcCode, string gupCode, string custCode, string ordNo)
        {
            var result = _db.F050104s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ORD_NO == ordNo);
            return result;
        }

        public IQueryable<F050104> GetDatas(string dcCode, string gupCode, string custCode, List<string> ordNo)
        {
            var result = _db.F050104s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && ordNo.Contains(x.ORD_NO));
            return result;
        }
    }
}