using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F05080503Repository : RepositoryBase<F05080503, Wms3plDbContext, F05080503Repository>
    {
        public F05080503Repository(string connName, WmsTransaction wmsTransaction = null)
             : base(connName, wmsTransaction)
        {
        }
    }
}
