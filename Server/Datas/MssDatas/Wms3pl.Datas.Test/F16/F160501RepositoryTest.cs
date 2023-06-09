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
    public class F160501RepositoryTest : BaseRepositoryTest
    {
        private F160501Repository _F160501Repo;
        public F160501RepositoryTest()
        {
            _F160501Repo = new F160501Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void Get160501QueryData()
        {
            var r = _F160501Repo.Get160501QueryData("01", "010001", "001", "",DateTime.Now.AddYears(-9),DateTime.Now,"","2","",null,null);
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void UpdateF160501s()
        {
            var r = _F160501Repo.UpdateF160501s(null);
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void UpdateF160501Status()
        {
            var r = _F160501Repo.UpdateF160501Status("","","001","01","0100001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF160501Status()
        {
            //var r = _F160501Repo.GetF160501Status("001", "01","010001","desNo");
            var r = _F160501Repo.GetF160501Status("001", "01","010001", "R2017060200001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetSettleData()
        {
            //var r = _F160501Repo.GetF160501Status("001", "01","010001","desNo");
            var r = _F160501Repo.GetSettleData("001", "01", "010001", DateTime.Parse("2020/01/03"));
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF160501ItemType()
        {
            //var r = _F160501Repo.GetF160501Status("001", "01","010001","desNo");
            var r = _F160501Repo.GetF160501ItemType("001", "01", "010001", "");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
