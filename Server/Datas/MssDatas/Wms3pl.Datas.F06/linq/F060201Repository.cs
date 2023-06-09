using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
    public partial class F060201Repository : RepositoryBase<F060201, Wms3plDbContext, F060201Repository>
    {
        public F060201Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F060201> GetDatas(string dcCode, string gupCode, string custCode, string cmdType, List<string> statusList, int midApiRelmt)
        {
            return _db.F060201s.Where(x => x.DC_CODE == dcCode && 
            x.GUP_CODE == gupCode && 
            x.CUST_CODE == custCode && 
            x.CMD_TYPE == cmdType &&
            statusList.Contains(x.STATUS) &&
            x.RESENT_CNT < midApiRelmt);
        }

        public IQueryable<F060201> GetDatas(string dcCode, string gupCode, string custCode, string cmdType, List<string> docIds)
        {
            return _db.F060201s.Where(x => x.DC_CODE == dcCode &&
            x.GUP_CODE == gupCode &&
            x.CUST_CODE == custCode &&
            x.CMD_TYPE == cmdType &&
            docIds.Contains(x.DOC_ID));
        }

    }
}
