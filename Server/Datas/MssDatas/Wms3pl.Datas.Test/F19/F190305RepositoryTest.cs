using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
namespace Wms3pl.Datas.Test
{
    [TestClass]
    public class F190305RepositoryTest : BaseRepositoryTest
    {
        public F190305RepositoryTest()
        {
        }

        [TestMethod]
        public void GetDatasByItems()//指定的轉換無效
        {
            var repo = new F190305Repository(Schemas.CoreSchema);
            var r = repo.GetDatasByItems("01", "030001", new System.Collections.Generic.List<string>() {"BG18112005"});
            Trace.Write(JsonSerializer.Serialize(r));
        }

    }
}
