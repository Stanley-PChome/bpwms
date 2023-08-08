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
    public partial class F161201RepositoryTest : BaseRepositoryTest
    {
        private F161201Repository _F161201Repo;
        public F161201RepositoryTest()
        {
            _F161201Repo = new F161201Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetF161201DetailDatas()
        {
            //var r = _F161201Repo.GetF161201DetailDatas("01", "010001", "001", "",DateTime.Now.AddYears(-9),DateTime.Now,"","1","",null,null);
            var r = _F161201Repo.GetF161201DetailDatas("001", "01", "010001", "R2017060200001");
            Trace.Write(JsonSerializer.Serialize(r));
        }

        [TestMethod]
        public void GetDcWmsNoOrdPropItems()
        {
            var r = _F161201Repo.GetDcWmsNoOrdPropItems("001", DateTime.Parse("2017/06/02"));
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetDcWmsNoDateItems()
        {
            var r = _F161201Repo.GetDcWmsNoDateItems("001", "01", "010001", DateTime.Parse("2017/06/02"), DateTime.Parse("2017/06/03"));
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetSettleData()
        {
            var r = _F161201Repo.GetSettleData("001", "01", "010001", DateTime.Now);
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetItem()
        {
            var r = _F161201Repo.GetItem("001", "01", "010001", "");
            Trace.Write(JsonSerializer.Serialize(r));
        }
      
        [TestMethod]
        public void GetDatasByCustOrdNoAndStoreNoNotCancel()
        {
            var r = _F161201Repo.GetDatasByCustOrdNoAndStoreNoNotCancel("001", "01", "010001", "","");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
