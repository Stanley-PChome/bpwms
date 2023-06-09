using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F191902Repository : RepositoryBase<F191902, Wms3plDbContext, F191902Repository>
    {
        public F191902Repository(string connName, WmsTransaction wmsTransaction = null)
          : base(connName, wmsTransaction)
        {
        }
    }
}
