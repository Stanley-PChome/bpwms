using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F199003Repository : RepositoryBase<F199003, Wms3plDbContext, F199003Repository>
    {
        public F199003Repository(string connName, WmsTransaction wmsTransaction = null)
       : base(connName, wmsTransaction)
        {
        }
    }
}
