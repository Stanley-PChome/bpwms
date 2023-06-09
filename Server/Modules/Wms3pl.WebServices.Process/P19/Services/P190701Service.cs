
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;


namespace Wms3pl.WebServices.Process.P19.Services
{
  public partial class P190701Service
  {
    private WmsTransaction _wmsTransaction;
    public P190701Service(WmsTransaction wmsTransaction = null)
    {
      _wmsTransaction = wmsTransaction;
    }

    public ExecuteResult SaveData(string gid, string staff, string name, List<string> listQid)
    {
      //刪除F190704
      var rep = new F190704Repository(Schemas.CoreSchema);
      rep.DeleteF190704ByGroupId(gid);

      //新增F190704
      foreach (var s in listQid)
        rep.InsertF190704(gid, s, staff, name);

      return new ExecuteResult(true);
    }

  }
}
