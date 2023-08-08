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
    public class F160504RepositoryTest : BaseRepositoryTest
    {
        private F160504Repository _F160504Repo;
        public F160504RepositoryTest()
        {
            _F160504Repo = new F160504Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void Get160504SerialData()
        {
            var r = _F160504Repo.Get160504SerialData("001","01","010001", "D2020010200002");//D2020010200002
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void DeleteF160504s()
        {
            var r = _F160504Repo.DeleteF160504s("");//D2020010200002
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
