using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
  public partial class F076103Repository : RepositoryBase<F076103, Wms3plDbContext, F076103Repository>
  {
    public F076103Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
    {
    }
  }
}