using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F16;
using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Test.F16
{
    [TestClass]
    public class F160402RepositoryTest : BaseRepositoryTest
    {
        private F160402Repository _F160402Repo;
        public F160402RepositoryTest()
        {
            _F160402Repo = new F160402Repository(Schemas.CoreSchema);
        }
        
        [TestMethod]
        public void GetF160402ScrapDetails()
        {
            var r = _F160402Repo.GetF160402ScrapDetails("001", "01", "010002", "X2019032700001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF160402AddScrapDetails()
        {
            var r = _F160402Repo.GetF160402AddScrapDetails("001", "01", "010001", "G01", "", "", "", null, null);
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF160402StockSum()
        {
            var r = _F160402Repo.GetF160402StockSum("001", "01", "010002", "PS14122-12520", "10A010105", "G01");

            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF160402()
        {
            var r = _F160402Repo.GetF160402("001", "01", "010002", "X2019032700001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetMaxSeq()
        {
            var r = _F160402Repo.GetMaxSeq("001", "01", "010002", "X2019032700001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF160402ScrapSum()
        {
            var r = _F160402Repo.GetF160402ScrapSum("001", "01", "010002", "", "PS17002-C0003", "10A010107", "G01");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF160402ScrapData()
        {
            var r = _F160402Repo.GetF160402ScrapData("001", "01", "010001", "", "PS17002-C0003", "10A010107", "G01");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
