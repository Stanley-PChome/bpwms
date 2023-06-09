using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using System;

namespace Wms3pl.Datas.Test
{
    [TestClass]
    public class F190701RepositoryTest : BaseRepositoryTest
    {
        public F190701RepositoryTest()
        {
        }
        [TestMethod]
        public void GetQueryListByGroupId()
        {
            // "BG18111005","BG18112005","BOX001"
            var repo = new F190701Repository(Schemas.CoreSchema);
            var r = repo.GetQueryListByGroupId("1");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetQueryListByEmpId()
        {
            // "BG18111005","BG18112005","BOX001"
            var repo = new F190701Repository(Schemas.CoreSchema);
            var r = repo.GetQueryListByEmpId("1", "12345");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
