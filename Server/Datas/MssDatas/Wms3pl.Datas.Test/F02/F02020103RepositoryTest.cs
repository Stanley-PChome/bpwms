using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F02;
using System;

namespace Wms3pl.Datas.Test
{
    [TestClass]
    public class F02020103RepositoryTest : BaseRepositoryTest
    {
        public F02020103RepositoryTest()
        {
        }

        [TestMethod]
        public void DeleteBeforeDate()
        {
            var repo = new F02020103Repository(Schemas.CoreSchema);
            repo.DeleteBeforeDate("001", "01", "030001", DateTime.Now);
            //Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
