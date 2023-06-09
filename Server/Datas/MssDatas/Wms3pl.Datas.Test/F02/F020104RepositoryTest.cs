using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using System;
using Wms3pl.Datas.F02;

namespace Wms3pl.Datas.Test
{
    [TestClass]
    public class F020104RepositoryTest : BaseRepositoryTest
    {
        public F020104RepositoryTest()
        {
        }

        [TestMethod]
        public void GetF020104Detail()
        {
            // "BG18111005","BG18112005","BOX001"
            var repo = new F020104Repository(Schemas.CoreSchema);
            var r = repo.GetF020104Detail("001", DateTime.Now.AddDays(-666), DateTime.Now, "", "", "", "");
            Trace.Write(JsonSerializer.Serialize(r));
        }        //[TestMethod]
        //public void GetQueryListByEmpId()
        //{
        //    // "BG18111005","BG18112005","BOX001"
        //    var repo = new F020104Repository(Schemas.CoreSchema);
        //    var r = repo.GetQueryListByEmpId("1", "12345");
        //    Trace.Write(JsonSerializer.Serialize(r));
        //}

    }
}
