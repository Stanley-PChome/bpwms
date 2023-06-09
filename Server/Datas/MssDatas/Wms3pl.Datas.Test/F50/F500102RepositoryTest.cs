using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F50;
using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Test.F50
{
    [TestClass]
    public class F500102RepositoryTest : BaseRepositoryTest
    {
        private F500102Repository _F500102Repo;
        public F500102RepositoryTest()
        {
            _F500102Repo = new F500102Repository(Schemas.CoreSchema);
        }
        
        [TestMethod]
        public void GetF500102QueryData()
        {
            var r = _F500102Repo.GetF500102QueryData("001", DateTime.Today.AddYears(-10), DateTime.Today.AddYears(0),"", "2");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetQuoteDatas()
        {
            //Q2020070700025
            //Q2020070700016
            var r = _F500102Repo.GetQuoteDatas("001", "01","010001", new List<string>() { "Q2020070700025", "Q2020070700016" });
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
