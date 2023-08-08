using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1981Repository : RepositoryBase<F1981, Wms3plDbContext, F1981Repository>
    {
        public F1981Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
        {
        }
    }
}
