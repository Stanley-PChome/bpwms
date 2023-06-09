using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F02;
using System;

namespace Wms3pl.Datas.Test
{
    [TestClass]
    public class F02020106RepositoryTest : BaseRepositoryTest
    {
        public F02020106RepositoryTest()
        {
        }
        [TestMethod]
        public void GetFileUploadSetting()
        {
            var repo = new F02020106Repository(Schemas.CoreSchema);
            var r = repo.GetFileUploadSetting("001", "01", "030001", "pchsQ", "rtNo");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
