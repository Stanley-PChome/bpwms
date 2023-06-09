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
    public class F700703RepositoryTest : BaseRepositoryTest
    {
        private F700703Repository _F700703Repo;
        public F700703RepositoryTest()
        {
            _F700703Repo = new F700703Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetBoxNumUsedStatus()
        {
            var a = _F700703Repo.GetBoxNumUsedStatus(DateTime.Parse("2017/03/01"), DateTime.Parse("2017/03/02"));
            Trace.Write(JsonSerializer.Serialize(a));
        }

    }
}