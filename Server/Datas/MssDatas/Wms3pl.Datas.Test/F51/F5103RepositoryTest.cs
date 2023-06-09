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
    public class F5103RepositoryTest : BaseRepositoryTest
    {
        private F5103Repository _F5103Repo;
        public F5103RepositoryTest()
        {
            _F5103Repo = new F5103Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void DeleteByDate()
        {
             _F5103Repo.DeleteByDate( DateTime.Today);
            //Trace.Write(JsonSerializer.Serialize(r));
        }

        [TestMethod]
        public void GetSettleMonFee()
        {
            //WHERE 拿掉也沒有資料
            var r = _F5103Repo.GetSettleMonFee(DateTime.Today,"");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
