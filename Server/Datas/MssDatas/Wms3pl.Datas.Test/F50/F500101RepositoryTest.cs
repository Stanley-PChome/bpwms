using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F50;
using System;

namespace Wms3pl.Datas.Test.F50
{
    [TestClass]
    public class F500101RepositoryTest : BaseRepositoryTest
    {
        private F500101Repository _F500101Repo;
        public F500101RepositoryTest()
        {
            _F500101Repo = new F500101Repository(Schemas.CoreSchema);
        }
        
        [TestMethod]
        public void GetF500101QueryData()
        {
            var r = _F500101Repo.GetF500101QueryData("001", "01", "010001", DateTime.Parse("2019-09-05"), DateTime.Parse("2020-01-06"), "", "2");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
