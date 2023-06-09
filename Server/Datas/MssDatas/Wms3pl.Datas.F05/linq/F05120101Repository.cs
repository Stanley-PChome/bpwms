using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F05120101Repository : RepositoryBase<F05120101, Wms3plDbContext, F05120101Repository>
    {
        public F05120101Repository(string connName, WmsTransaction wmsTransaction = null)
             : base(connName, wmsTransaction)
        {
        }
    }
}
