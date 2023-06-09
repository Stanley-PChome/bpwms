using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F51;
using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Test.F51
{
    [TestClass]
    public class F5105RepositoryTest : BaseRepositoryTest
    {
        private F5105Repository _F5105Repo;
        public F5105RepositoryTest()
        {
            _F5105Repo = new F5105Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void DeleteByDate()
        {
             _F5105Repo.DeleteByDate( DateTime.Today);
            //Trace.Write(JsonSerializer.Serialize(r));
        }

        [TestMethod]
        public void GetSettleMonFee()
        {
            var r = _F5105Repo.GetSettleMonFee(DateTime.Parse("2020/07/08"), "Q2020070700002");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
