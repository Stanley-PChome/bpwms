using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
    public partial class F060209Repository : RepositoryBase<F060209, Wms3plDbContext, F060209Repository>
    {
        public F060209Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

    public F060209 GetDataByDocId(string dcCode,string gupCode,string custCode,string docId)
    {
      return _db.F060209s.AsNoTracking().Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.DOC_ID == docId).SingleOrDefault();
    }
  }
}
