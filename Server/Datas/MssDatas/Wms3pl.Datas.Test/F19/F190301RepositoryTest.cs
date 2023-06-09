using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;

namespace Wms3pl.Datas.Test
{
    [TestClass]
    public class F190301RepositoryTest : BaseRepositoryTest
    {
        public F190301RepositoryTest()
        {
            
        }

        [TestMethod]
        public void GetItemPack()//指定的轉換無效
        {
            //CAPIE017403
            //CAPIE017404
            //CAPIE017501
            var repo = new F190301Repository(Schemas.CoreSchema);
            var r = repo.GetItemPack("01", "001", "CAPIE017404", "");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetUnitQtyDatas()//指定的轉換無效
        {
            var repo = new F190301Repository(Schemas.CoreSchema);
            var r = repo.GetUnitQtyDatas("01", new System.Collections.Generic.List<string>() { "PCS" }, new System.Collections.Generic.List<string>() { "EG120206", "EG130101" });
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetItemUnits()//指定的轉換無效
        {
            var repo = new F190301Repository(Schemas.CoreSchema);
            var r = repo.GetItemUnits("01", new System.Collections.Generic.List<string>() { "EG120206", "EG130101" });
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
