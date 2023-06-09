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
    public class F500105RepositoryTest : BaseRepositoryTest
    {
        private F500105Repository _F500105Repo;
        public F500105RepositoryTest()
        {
            _F500105Repo = new F500105Repository(Schemas.CoreSchema);
        }
        
        [TestMethod]
        public void GetF500105QueryData()
        {
            var r = _F500105Repo.GetF500105QueryData("001", DateTime.Today.AddYears(-10), DateTime.Today.AddYears(0),"", "2");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetQuoteDatas()
        {
            //Q2020070700002
            //Q2020070700003
            //Q2020070700017
            var r = _F500105Repo.GetQuoteDatas("001", "01", "010001", new List<string>() { "Q2020070700002" });
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
