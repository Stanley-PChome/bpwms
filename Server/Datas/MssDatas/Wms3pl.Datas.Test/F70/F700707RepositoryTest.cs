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
    public class F700707RepositoryTest : BaseRepositoryTest
    {
        private F700707Repository _F700707Repo;
        public F700707RepositoryTest()
        {
            _F700707Repo = new F700707Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetDcPurchaseQty()
        {
            var a = _F700707Repo.GetDcPurchaseQty(DateTime.Parse("2017/03/01"));
            Trace.Write(JsonSerializer.Serialize(a));
        }
    }
}
