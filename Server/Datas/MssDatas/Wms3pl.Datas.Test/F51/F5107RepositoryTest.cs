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
    public class F5107RepositoryTest : BaseRepositoryTest
    {
        private F5107Repository _F5107Repo;
        public F5107RepositoryTest()
        {
            _F5107Repo = new F5107Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void DeleteByDate()
        {
             _F5107Repo.DeleteByDate( DateTime.Today);
            //Trace.Write(JsonSerializer.Serialize(r));
        }

        [TestMethod]
        public void GetSettleMonFee()
        {
            //主表沒有資料
            var r = _F5107Repo.GetSettleMonFee(DateTime.Parse("2020/07/09"), "Q2020070700002");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
