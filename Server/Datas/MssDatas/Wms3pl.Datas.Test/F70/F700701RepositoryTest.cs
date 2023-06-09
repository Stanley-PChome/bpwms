using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F70;
using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Test.F70
{
    [TestClass]
    public class F700701RepositoryTest : BaseRepositoryTest
    {
        private F700701Repository _F700701Repo;
        public F700701RepositoryTest()
        {
            _F700701Repo = new F700701Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetF700701QueryData()
        {
            var a = _F700701Repo.GetF700701QueryData("001", null, null);
            Trace.Write(JsonSerializer.Serialize(a));
        }
    }
}