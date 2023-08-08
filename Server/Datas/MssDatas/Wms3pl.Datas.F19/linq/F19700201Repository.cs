using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F19700201Repository : RepositoryBase<F19700201, Wms3plDbContext, F19700201Repository>
    {
        public F19700201Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
        {
        }
    }
}
