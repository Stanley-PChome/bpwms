using Microsoft.EntityFrameworkCore;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F05080505Repository : RepositoryBase<F05080505, Wms3plDbContext, F05080505Repository>
    {
        public F05080505Repository(string connName, WmsTransaction wmsTransaction = null) : base(connName, wmsTransaction)
        { }
    }
}
