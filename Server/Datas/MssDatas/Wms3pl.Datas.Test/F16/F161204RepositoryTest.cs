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
    public partial class F161204RepositoryTest : BaseRepositoryTest
    {
        private F161204Repository _F161204Repo;
        public F161204RepositoryTest()
        {
            _F161204Repo = new F161204Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetReurnItem()
        {
            var r = _F161204Repo.GetReurnItem("001","","","","","");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
