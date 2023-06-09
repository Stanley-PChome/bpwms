using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
  public partial class F076104Repository : RepositoryBase<F076104, Wms3plDbContext, F076104Repository>
  {
    public F076104Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
    {
    }
  }
}
