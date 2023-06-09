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
    public class F500104RepositoryTest : BaseRepositoryTest
    {
        private F500104Repository _F500104Repo;
        public F500104RepositoryTest()
        {
            _F500104Repo = new F500104Repository(Schemas.CoreSchema);
        }
        
        [TestMethod]
        public void GetF500104QueryData()
        {
            var r = _F500104Repo.GetF500104QueryData("001", DateTime.Today.AddYears(-10), DateTime.Today.AddYears(0),"", "2");
            Trace.Write(JsonSerializer.Serialize(r));
        }

        [TestMethod]
        public void GetQuoteDatas()
        {
            var r = _F500104Repo.GetQuoteDatas("001", "01","010001", new List<string>() { "Q2020070700011" });
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
