using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F01
{
    public partial class F010203Repository : RepositoryBase<F010203, Wms3plDbContext, F010203Repository>
    {
        public F010203Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }
    }
}
