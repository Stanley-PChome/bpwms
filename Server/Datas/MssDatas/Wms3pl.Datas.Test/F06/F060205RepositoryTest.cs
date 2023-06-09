using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Wms3pl.Datas.F06;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F06
{

  [TestClass]
  public class F060205RepositoryTest : BaseRepositoryTest
  {
    private F060205Repository _f060205Repo;

    public F060205RepositoryTest()
    {
      _f060205Repo = new F060205Repository(Schemas.CoreSchema);
    }

    [TestMethod]
    public void GetDatasForDocIds()
    {
      var f060205 = _f060205Repo.GetDatasForDocIds(new[] { "O20230330000011", "O20230330000010", "O20230330000009" }.ToList());
      Trace.Write(JsonSerializer.Serialize(f060205));
    }

  }
}
