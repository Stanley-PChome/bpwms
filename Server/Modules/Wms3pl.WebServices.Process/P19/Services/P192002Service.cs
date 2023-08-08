using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
  public partial class P192002Service
  {
    private WmsTransaction _wmsTransaction;
    public P192002Service(WmsTransaction wmsTransaction = null)
    {
      _wmsTransaction = wmsTransaction;
    }

    public IQueryable<F91000302SearchData> GetF91000302Data(string itemTypeId, string accUnit, string accUnitName)
    {
      var rep = new F91000302Repository(Schemas.CoreSchema);
      var result = rep.GetF91000302Data(itemTypeId, accUnit, accUnitName);
      return result;
    }
  }
}

