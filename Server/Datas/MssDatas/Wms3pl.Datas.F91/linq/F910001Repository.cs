using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
    public partial class F910001Repository : RepositoryBase<F910001, Wms3plDbContext, F910001Repository>
    {
        public F910001Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {

        }
    }
}
