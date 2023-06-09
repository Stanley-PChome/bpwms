using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
  public partial class F02050402Repository : RepositoryBase<F02050402, Wms3plDbContext, F02050402Repository>
  {
    public F02050402Repository(string connName, WmsTransaction wmsTransaction = null)
    : base(connName, wmsTransaction)
    {
    }

    public IQueryable<F02050402> GetDatasByWcsExecute(string dcCode, string gupCode, string custCode, string wmsNo, int midApiRelmt)
    {
      return _db.F02050402s.Where(x =>
        x.DC_CODE == dcCode
        && x.GUP_CODE == gupCode
        && x.CUST_CODE == custCode
        && new[] { "0", "T" }.Contains(x.STATUS)
        && x.STOCK_NO == wmsNo
        && x.RESENT_CNT < midApiRelmt);
    }

  }
}
