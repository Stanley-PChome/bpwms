using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F194801Repository : RepositoryBase<F194801, Wms3plDbContext, F194801Repository>
    {
        public F194801Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
        {
        }
    }
}
