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
    public class F700709RepositoryTest : BaseRepositoryTest
    {
        private F700709Repository _F700709Repo;
        public F700709RepositoryTest()
        {
            _F700709Repo = new F700709Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetOrderPickTimeStatistics()
        {
            var a = _F700709Repo.GetOrderPickTimeStatistics(DateTime.Parse("2017/03/01"), DateTime.Parse("2017/03/04"));
            Trace.Write(JsonSerializer.Serialize(a));
        }
    }
}