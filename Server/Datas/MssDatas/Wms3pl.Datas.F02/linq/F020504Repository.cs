using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
  public partial class F020504Repository : RepositoryBase<F020504, Wms3plDbContext, F020504Repository>
  {
    public F020504Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
    {

    }

  }
}
