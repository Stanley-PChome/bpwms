using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F50
{
    public partial class F500201Repository : RepositoryBase<F500201, Wms3plDbContext, F500201Repository>
    {
        public F500201Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        { }
    }
}
