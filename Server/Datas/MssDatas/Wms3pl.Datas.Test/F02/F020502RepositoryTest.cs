using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Text.Json;
using Wms3pl.Datas.F02;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test
{
  [TestClass]
  public class F020502RepositoryTest : BaseRepositoryTest
  {
    public F020502RepositoryTest()
    {
    }
    [TestMethod]
    public void GetContainerRecheckFaildItem()
    {
      var repo = new F020502Repository(Schemas.CoreSchema);
      var r = repo.GetContainerRecheckFaildItem("12", "10", "010001", "A20220316000006");
      Trace.Write(JsonSerializer.Serialize(r));
    }

  }
}
