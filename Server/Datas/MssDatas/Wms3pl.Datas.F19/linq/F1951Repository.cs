using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1951Repository : RepositoryBase<F1951, Wms3plDbContext, F1951Repository>
    {
        public F1951Repository(string connName, WmsTransaction wmsTransaction = null)
         : base(connName, wmsTransaction)
        {
        }
    }
}
