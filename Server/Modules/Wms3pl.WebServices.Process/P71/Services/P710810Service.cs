
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P71.Services
{
  public partial class P710810Service
  {
    private WmsTransaction _wmsTransaction;
    public P710810Service(WmsTransaction wmsTransaction = null)
    {
      _wmsTransaction = wmsTransaction;
    }

    public IQueryable<InventoryQueryData> GetInventoryQueryDatas(string dcCode, string gupCode, string custCode,
      string postingDateBegin, string postingDateEnd)
    {
      var repF140101 = new F140101Repository(Schemas.CoreSchema);
      return repF140101.GetInventoryQueryDatas(dcCode, gupCode, custCode, postingDateBegin, postingDateEnd);
    }
  }
}

