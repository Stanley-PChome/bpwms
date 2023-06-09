using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using System;

namespace Wms3pl.Datas.Test
{
    [TestClass]
    public class F1905RepositoryTest : BaseRepositoryTest
    {
        public F1905RepositoryTest()
        {
        }

        [TestMethod]
        public void GetPackCase()
        {
            // "BG18111005","BG18112005","BOX001"
            var repo = new F1905Repository(Schemas.CoreSchema);
            var r = repo.GetPackCase("01","010001", "BG18111005", "");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetCartonSize()
        {
            // "BG18111005","BG18112005","BOX001"
            var repo = new F1905Repository(Schemas.CoreSchema);
            var r = repo.GetCartonSize("01", "020001", "");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF1905ByItems()
        {
            var repo = new F1905Repository(Schemas.CoreSchema);
            var r = repo.GetF1905ByItems("01","010001",new System.Collections.Generic.List<string>() { "BG18111005" });
            Trace.Write(JsonSerializer.Serialize(r));
        }

    }
}
