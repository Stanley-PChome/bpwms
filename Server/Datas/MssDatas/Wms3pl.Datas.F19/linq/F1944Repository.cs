using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1944Repository : RepositoryBase<F1944, Wms3plDbContext, F1944Repository>
    {
        public F1944Repository(string connName, WmsTransaction wmsTransaction = null)
      : base(connName, wmsTransaction)
        {
        }
    }
}
