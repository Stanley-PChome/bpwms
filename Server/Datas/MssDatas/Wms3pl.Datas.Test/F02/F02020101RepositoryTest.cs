using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F02;
using System;

namespace Wms3pl.Datas.Test
{
    [TestClass]
    public class F02020101RepositoryTest : BaseRepositoryTest
    {
        public F02020101RepositoryTest()
        {

        }

        [TestMethod]
        public void FindEx()
        {
            var repo = new F02020101Repository(Schemas.CoreSchema);
            var r = repo.FindEx("001", "01","030001","pchsNo", "020001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void Delete()
        {
            //參數
            //var r = repo.Delete("001", "01", "030001", "pchsNo", "rtNo");
            var repo = new F02020101Repository(Schemas.CoreSchema);
            //DELETE 先行移除參數避免真的刪除資料
            repo.Delete("", "", "", "", "");
        }
        [TestMethod]
        public void GetReceProcessOver30MinDatasByDc()
        {
            var repo = new F02020101Repository(Schemas.CoreSchema);
            var r = repo.GetReceProcessOver30MinDatasByDc("001", DateTime.Now.AddYears(-9));
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF02020101sByVirtualItem()
        {
            var repo = new F02020101Repository(Schemas.CoreSchema);
            //var r = repo.GetF02020101sByVirtualItem("001", "01","030002","pchNo","reNo);
            var r = repo.GetF02020101sByVirtualItem("001", "01","030002","", "");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void IsRecvQtyNotEqualsSerialTotal()
        {
            var repo = new F02020101Repository(Schemas.CoreSchema);
            var r = repo.IsRecvQtyNotEqualsSerialTotal("001", "01", "030002", "");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetP020203Datas()
        {
            #region
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030002";
            var purchaseNo = "A2018083100001";
            var rtNo = "";
            #endregion
            var repo = new F02020101Repository(Schemas.CoreSchema);
            //var r = repo.GetP020203Datas("001", "01", "030002", "pchNo","rtno");
            var r = repo.GetP020203Datas( dcCode, gupCode, custCode, purchaseNo, rtNo);
            Trace.Write(JsonSerializer.Serialize(r));
        }
        
    }
}
