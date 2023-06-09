using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F02;
using System;

namespace Wms3pl.Datas.Test
{
    [TestClass]
    public class F020301RepositoryTest : BaseRepositoryTest
    {
        public F020301RepositoryTest()
        {
        }

        [TestMethod]
        public void GetF020301FileSeq()
        {
            var repo = new F020301Repository(Schemas.CoreSchema);
            //var r = repo.GetF020301FileSeq("001","01","030002","pchNo");
            //var r = repo.GetF020301FileSeq("001","01","030002", "pchNo");
            //Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF020302Data()
        {
            var repo = new F020301Repository(Schemas.CoreSchema);
            var r = repo.GetF020302Data("001", "01", "010001", "A2020070900001", "SN001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF020302s()
        {
            var repo = new F020301Repository(Schemas.CoreSchema);
            var r = repo.GetF020302s("001", "01", "030002", "pchNo");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetJincangNoFileMain()
        {
            var repo = new F020301Repository(Schemas.CoreSchema);
            var r = repo.GetJincangNoFileMain("001", "01", "010001", DateTime.Now.AddYears(-10), DateTime.Now, "A2020070900001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void BulkDelete()
        {
            var repo = new F020301Repository(Schemas.CoreSchema);
            repo.BulkDelete("001","01","010001",new System.Collections.Generic.List<string>() { "" });
        }
        
    }
    
}
