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
    public class F160503RepositoryTest : BaseRepositoryTest
    {
        private F160503Repository _F160503Repo;
        public F160503RepositoryTest()
        {
            _F160503Repo = new F160503Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetDestoryNoFile()
        {
            var r = _F160503Repo.GetDestoryNoFile("");//D2020010200002
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetDestoryNoRelation()
        {
            var r = _F160503Repo.GetDestoryNoRelation("D2020010200002");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void DeleteF160503File()
        {
            var r = _F160503Repo.DeleteF160503File("D2020010200002");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
