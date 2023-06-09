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
    public class F5108RepositoryTest : BaseRepositoryTest
    {
        private F5108Repository _F5108Repo;
        public F5108RepositoryTest()
        {
            _F5108Repo = new F5108Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void DeleteByDate()
        {
             _F5108Repo.DeleteByDate( DateTime.Today);
            //Trace.Write(JsonSerializer.Serialize(r));
        }

        [TestMethod]
        public void GetSettleMonFee()
        {
            //主表沒有資料
            var r = _F5108Repo.GetSettleMonFee(DateTime.Parse("2020/07/09"), "Q2020070700002");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
