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
    public class F700501RepositoryTest : BaseRepositoryTest
    {
        private F700501Repository _F700501Repo;
        public F700501RepositoryTest()
        {
            _F700501Repo = new F700501Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetF700501Ex()
        {
            var a = _F700501Repo.GetF700501Ex("001",DateTime.Parse("2020-07-08"), DateTime.Parse("2020-07-08"),"S");
            Trace.Write(JsonSerializer.Serialize(a));
        }
        [TestMethod]
        public void GetF700501ForMessageData()
        {
            var a = _F700501Repo.GetF700501ForMessageData();
            Trace.Write(JsonSerializer.Serialize(a));
        }
        [TestMethod]
        public void UpdateF700501MessageId()
        {
            _F700501Repo.UpdateF700501MessageId("001", "ZS2017022000001", decimal.Parse("9"));
        }
    }
}