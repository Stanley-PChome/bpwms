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
    public class F500103RepositoryTest : BaseRepositoryTest
    {
        private F500103Repository _F500103Repo;
        public F500103RepositoryTest()
        {
            _F500103Repo = new F500103Repository(Schemas.CoreSchema);
        }
        
        [TestMethod]
        public void GetF500103QueryData()
        {
            var r = _F500103Repo.GetF500103QueryData("001", DateTime.Today.AddYears(-10), DateTime.Today.AddYears(0),"", "2");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetQuoteDatas()
        {
            var r = _F500103Repo.GetQuoteDatas("001", "01","010001", new List<string>() { ""});
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
