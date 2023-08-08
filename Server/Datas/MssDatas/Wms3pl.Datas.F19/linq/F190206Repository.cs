using System.Collections.Generic;
using System.Linq;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.DBCore;
using Wms3pl.Datas.F19;
namespace Wms3pl.Datas.F19
{
	public partial class F190206Repository : RepositoryBase<F190206, Wms3plDbContext, F190206Repository>
	{
		public F190206Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

        public IQueryable<F190206> GetDatas(string gupCode, string custCode, string checkType, List<string> itemCodes)
        {
            var query = _db.F190206s
                .Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.CHECK_TYPE == checkType)
                .Where(x => itemCodes.Contains(x.ITEM_CODE));
            return query;
        }
    }
}
