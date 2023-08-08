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
    public class F700705RepositoryTest : BaseRepositoryTest
    {
        private F700705Repository _F700705Repo;
        public F700705RepositoryTest()
        {
            _F700705Repo = new F700705Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetEmpLackPicks()
        {
            var a = _F700705Repo.GetEmpLackPicks(DateTime.Parse("2017/03/01"), DateTime.Parse("2017/03/04"));
            Trace.Write(JsonSerializer.Serialize(a));
        }

    }
}