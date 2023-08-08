using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
  public partial class F020601Repository : RepositoryBase<F020601, Wms3plDbContext, F020601Repository>
  {
    public F020601Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
    { }
  }
}
