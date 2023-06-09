using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F02;
using System;

namespace Wms3pl.Datas.Test
{
    [TestClass]
    public class F020302RepositoryTest : BaseRepositoryTest
    {
        public F020302RepositoryTest()
        {
        }
        [TestMethod]
        public void GetJincangNoFileDetail()
        {
            var repo = new F020302Repository(Schemas.CoreSchema);
            var r = repo.GetJincangNoFileDetail("001", "01", "010001", "SYSCHK99_A202007090000101", "A2020070900001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void UpdateSerialCancel()
        {
            var repo = new F020302Repository(Schemas.CoreSchema);
            repo.UpdateSerialCancel("001", "01", "010001", "stockNo", "itemCOde",new System.Collections.Generic.List<string>() { "SerialNos"});
            //Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void BulkDelete()
        {
            var repo = new F020302Repository(Schemas.CoreSchema);
            repo.BulkDelete("001", "01", "010001", new System.Collections.Generic.List<string>() { "SerialNos" });
        }
        [TestMethod]
        public void DeleteWithCancelAcceptance()
        {
            var repo = new F020302Repository(Schemas.CoreSchema);
            repo.DeleteWithCancelAcceptance("001", "01", "010001","pchNo", "rtNo");
        }
    }
}
