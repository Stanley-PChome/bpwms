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
    public class F700708RepositoryTest : BaseRepositoryTest
    {
        private F700708Repository _F700708Repo;
        public F700708RepositoryTest()
        {
            _F700708Repo = new F700708Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetDcPerformanceStatistics()
        {
            var a = _F700708Repo.GetDcPerformanceStatistics(DateTime.Parse("2017/03/01"), DateTime.Parse("2017/03/04"));
            Trace.Write(JsonSerializer.Serialize(a));
        }
    }
}
