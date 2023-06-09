using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F050004Repository : RepositoryBase<F050004, Wms3plDbContext, F050004Repository>
	{
		public F050004Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public F050004 GetData(string dcCode, string gupCode, string custCode, decimal ticketId)
		{
            var result = _db.F050004s.Where(x => x.DC_CODE == dcCode &&
                                                 x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode &&
                                                 x.TICKET_ID == ticketId).SingleOrDefault();

            return result;
		}
	}
}
