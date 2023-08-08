using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P71.Services
{
  public partial class P710507Service
  {
    private WmsTransaction _wmsTransaction;

    public P710507Service(WmsTransaction wmsTransaction = null)
    {
      _wmsTransaction = wmsTransaction;
    }

    public IQueryable<F194707Ex> GetP710507SearchData(string dcCode, string allId, string accKind,
      string inTax, string logiType, string custType, string status)
    {
      var repF194707 = new F194707Repository(Schemas.CoreSchema);
      return repF194707.GetP710507SearchData(dcCode, allId, accKind,
        inTax, logiType, custType, status);
    }

  }
}
