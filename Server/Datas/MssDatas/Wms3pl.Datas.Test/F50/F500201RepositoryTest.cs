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
    public class F500201RepositoryTest : BaseRepositoryTest
    {
        private F500201Repository _F500201Repo;
        public F500201RepositoryTest()
        {
            _F500201Repo = new F500201Repository(Schemas.CoreSchema);
        }
        
        [TestMethod]
        public void GetF500201ClearingData()
        {
            var r = _F500201Repo.GetF500201ClearingData("01", "010001", "", "2020/07/08");

            Trace.Write(JsonSerializer.Serialize(r));
        }

        [TestMethod]
        public void GetRp7105100001Data()
        {
            var r = _F500201Repo.GetRp7105100001Data("01", "010001", "", DateTime.Parse("2020/07/08"));

            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetRp7105100004Data()
        {
            var r = _F500201Repo.GetRp7105100004Data(DateTime.Parse("2020/07/08"),"");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetRp7105100005Data()
        {
            var r = _F500201Repo.GetRp7105100005Data(DateTime.Parse("2020/07/08"), "");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void SettlementPrint()
        {
            _F500201Repo.SettlementPrint("","",DateTime.Parse("2020/07/08"), "","");
        }
        [TestMethod]
        public void SettlementClosing()
        {
            _F500201Repo.SettlementClosing("", "", DateTime.Parse("2020/07/08"), "", "");
        }
        [TestMethod]
        public void GetBaseDay()
        {
            var r = _F500201Repo.GetBaseDay();
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void DeleteByDate()
        {
             _F500201Repo.DeleteByDate(DateTime.Parse("2020/07/08"));
        }
        [TestMethod]
        public void GetRp7105100002Data()
        {
            var r =_F500201Repo.GetRp7105100002Data(DateTime.Parse("2020/07/08"),"");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetRp7105100003Data()
        {
            var r = _F500201Repo.GetRp7105100003Data(DateTime.Parse("2020/07/08"), "");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        
    }
    
}
