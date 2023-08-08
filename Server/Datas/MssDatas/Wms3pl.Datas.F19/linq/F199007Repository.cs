using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F199007Repository : RepositoryBase<F199007, Wms3plDbContext, F199007Repository>
    {
        public F199007Repository(string connName, WmsTransaction wmsTransaction = null)
         : base(connName, wmsTransaction)
        {
        }
    }
}
