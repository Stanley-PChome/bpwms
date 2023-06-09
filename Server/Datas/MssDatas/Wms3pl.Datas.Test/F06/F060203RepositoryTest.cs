using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F06
{
  [TestClass]
  public class F060203RepositoryTest : BaseRepositoryTest
  {
    private F060203Repository _f060203Repo;
    public F060203RepositoryTest()
    {
      _f060203Repo = new F060203Repository(Schemas.CoreSchema);
    }

    [TestMethod]
    public void GetSerialNosByWmsNos()
    {
      var r = _f060203Repo.GetSerialNosByWmsNos("12", "10", "010001", new[] { "O20221214000007", "T20221214000009", "O20221214000002" }.ToList());
      Trace.Write(JsonConvert.SerializeObject(r));
    }

  }
}
