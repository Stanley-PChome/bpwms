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
    public class F160502RepositoryTest : BaseRepositoryTest
    {
        private F160502Repository _F160502Repo;
        public F160502RepositoryTest()
        {
            _F160502Repo = new F160502Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void Get160502DetailData()
        {
            var r = _F160502Repo.Get160502DetailData("001", "01", "010001", "");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void DeleteF160502s()
        {
            var r = _F160502Repo.DeleteF160502s("");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        
    }
}
