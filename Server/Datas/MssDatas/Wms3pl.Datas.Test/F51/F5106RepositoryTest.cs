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
    public class F5106RepositoryTest : BaseRepositoryTest
    {
        private F5106Repository _F5106Repo;
        public F5106RepositoryTest()
        {
            _F5106Repo = new F5106Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void DeleteByDate()
        {
             _F5106Repo.DeleteByDate( DateTime.Today);
            //Trace.Write(JsonSerializer.Serialize(r));
        }

        [TestMethod]
        public void GetSettleMonFee()
        {
            //沒有測試資料
            var r = _F5106Repo.GetSettleMonFee(DateTime.Parse("2020/07/08"), "Q2020070700002");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
