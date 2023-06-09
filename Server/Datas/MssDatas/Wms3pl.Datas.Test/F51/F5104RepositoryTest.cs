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
    public class F5104RepositoryTest : BaseRepositoryTest
    {
        private F5104Repository _F5104Repo;
        public F5104RepositoryTest()
        {
            _F5104Repo = new F5104Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void DeleteByDate()
        {
             _F5104Repo.DeleteByDate( DateTime.Today);
            //Trace.Write(JsonSerializer.Serialize(r));
        }

        [TestMethod]
        public void GetSettleMonFee()
        {
            //2020-07-07 00:00:00
            var r = _F5104Repo.GetSettleMonFee(DateTime.Parse("2020/07/09"), "Q2020070700012");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
