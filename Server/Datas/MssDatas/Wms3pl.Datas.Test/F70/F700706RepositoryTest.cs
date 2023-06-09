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
    public class F700706RepositoryTest : BaseRepositoryTest
    {
        private F700706Repository _F700706Repo;
        public F700706RepositoryTest()
        {
            _F700706Repo = new F700706Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetScheduleRefineStatistics()
        {
            var a = _F700706Repo.GetScheduleRefineStatistics(DateTime.Parse("2017/03/01"));
            Trace.Write(JsonSerializer.Serialize(a));
        }
    }
}
