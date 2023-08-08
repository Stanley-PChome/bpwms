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
    public partial class F161601RepositoryTest : BaseRepositoryTest
    {
        private F161601Repository _F161601Repo;
        public F161601RepositoryTest()
        {
            _F161601Repo = new F161601Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetF161601DetailDatas()
        {
            var r =_F161601Repo.GetF161601DetailDatas("001", "01", "020003","");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetUpLocDataByDc()
        {
            var r = _F161601Repo.GetUpLocDataByDc("001",DateTime.Parse("2017/06/02"));
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetWaitUpLocDataByDc()
        {
            var r = _F161601Repo.GetWaitUpLocDataByDc("001", DateTime.Parse("2017/06/02"));
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetReturnWaitUpLocOver30MinByDc()
        {
            var r = _F161601Repo.GetReturnWaitUpLocOver30MinByDc("001", DateTime.Parse("2017/06/02"));
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF161401ReturnWarehouse()
        {
            var r = _F161601Repo.GetF161401ReturnWarehouse("001", "01", "010001", "R2017060200001", "","","");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetPrintF161601Data()
        {
            var r = _F161601Repo.GetPrintF161601Data("001", "01", "010001", "");// R2017060200001
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetP160102Reports()
        {
            var r = _F161601Repo.GetP160102Reports("001", "01", "010001", "");// R2017060200001
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
