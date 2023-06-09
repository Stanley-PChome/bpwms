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
    public class F700201RepositoryTest : BaseRepositoryTest
    {
        private F700201Repository _F700201Repo;
        public F700201RepositoryTest()
        {
            _F700201Repo = new F700201Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void BulkInsert()
        {
            //2018-05-08 00:00:00
            //2018-05-09 00:00:00
            //2018-05-10 00:00:00
            _F700201Repo.BulkInsert(new List<F700201>() { }, new string[] { "" });
            //Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
