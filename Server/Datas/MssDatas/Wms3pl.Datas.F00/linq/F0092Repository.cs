using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
    public partial class F0092Repository : RepositoryBase<F0092, Wms3plDbContext, F0092Repository>
    {
        public F0092Repository(string connName, WmsTransaction wmsTransaction = null) 
            : base(connName, wmsTransaction)
        {

        }
    }
}
