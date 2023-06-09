using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F02;
using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Test
{
    [TestClass]
    public class F020201RepositoryTest : BaseRepositoryTest
    {
        public F020201RepositoryTest()
        {
        }
        [TestMethod]
        public void GetAcceptancePurchaseReport()
        {
            var repo = new F020201Repository(Schemas.CoreSchema);
            var r = repo.GetAcceptancePurchaseReport("001","01","030002", "A2019010400001", "201901040001", "030002",false);
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetDatas()
        {
            var repo = new F020201Repository(Schemas.CoreSchema);
            //var r = repo.GetDatas("001", "01", "010002", "A2017053100002 \ A2017053100002");
            var r = repo.GetDatas("001", "01", "010002", "A2017053100002");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetDatasByWaitOrUpLoc()
        {
            var repo = new F020201Repository(Schemas.CoreSchema);
            var r = repo.GetDatasByWaitOrUpLoc("001", DateTime.Parse("2017-05-31"));
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetReceUnUpLocOver30MinDatasByDc()
        {
            var repo = new F020201Repository(Schemas.CoreSchema);
            var r = repo.GetReceUnUpLocOver30MinDatasByDc("001", DateTime.Parse("2017-05-04"));
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF020201WithF02020101s()
        {
            var repo = new F020201Repository(Schemas.CoreSchema);
            var r = repo.GetF020201WithF02020101s("001", "01", "030001", "pchNo", "rtNo");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetAllocationData()
        {
            var repo = new F020201Repository(Schemas.CoreSchema);
            //var r = repo.GetAllocationData("001","01","030002","alloNo");
            var r = repo.GetAllocationData("001","01","030002", "");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetSettleData()
        {
            var repo = new F020201Repository(Schemas.CoreSchema);
            //var r = repo.GetAllocationData("001","01","030002","alloNo");
            var r = repo.GetSettleData("001", "01", "030002", DateTime.Now);
            Trace.Write(JsonSerializer.Serialize(r));
        }

    [TestMethod]
    public void GetDatasByF020502()
    {
      var repo = new F020201Repository(Schemas.CoreSchema);
      var f020502s = new List<F020502>
      {
        new F020502(){ RT_NO = "202303210001", RT_SEQ = "1" },
        new F020502(){ RT_NO = "202303170004", RT_SEQ = "1" },
      };
      var r = repo.GetDatasByF020502("12", "10", "010001", f020502s);

      Trace.Write(JsonSerializer.Serialize(r));
    }

  }
}
