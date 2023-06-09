using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1919Repository : RepositoryBase<F1919, Wms3plDbContext, F1919Repository>
    {
        public F1919Repository(string connName, WmsTransaction wmsTransaction = null)
          : base(connName, wmsTransaction)
        {
        }
    }
}
