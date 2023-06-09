using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
    public partial class F060101Repository : RepositoryBase<F060101, Wms3plDbContext, F060101Repository>
    {
        public F060101Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F060101> GetDatas(string dcCode, string gupCode, string custCode, string cmdType, List<string> statusList, int midApiRelmt)
        {
            return _db.F060101s.Where(x => x.DC_CODE == dcCode && 
            x.GUP_CODE == gupCode && 
            x.CUST_CODE == custCode && 
            x.CMD_TYPE == cmdType &&
            statusList.Contains(x.STATUS) &&
            x.RESENT_CNT < midApiRelmt);
        }

        public IQueryable<F060101> GetDatas(string dcCode, string gupCode, string custCode, List<string> docId)
        {
            return _db.F060101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
            x.GUP_CODE == gupCode &&
            x.CUST_CODE == custCode &&
            docId.Contains(x.DOC_ID) &&
            x.CMD_TYPE == "1");
        }
	}
}
