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
    public class F16050301RepositoryTest : BaseRepositoryTest
    {
        private F16050301Repository _F16050301Repo;
        public F16050301RepositoryTest()
        {
            _F16050301Repo = new F16050301Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void DeleteF16050301Serial()
        {
            var r = _F16050301Repo.DeleteF16050301Serial("");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
