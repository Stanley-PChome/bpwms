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
    public class F5102RepositoryTest : BaseRepositoryTest
    {
        private F5102Repository _F5102Repo;
        public F5102RepositoryTest()
        {
            _F5102Repo = new F5102Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetSettleLocQty()
        {
            var r = _F5102Repo.GetSettleLocQty("001", "01", "030001", DateTime.Parse("2018/05/08"),"");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF51ComplexReportData()
        {
            var r = _F5102Repo.GetF51ComplexReportData("001", DateTime.Today,DateTime.Today, "","","");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetLocSettleMonFee()
        {
            //where 拿掉也沒有測試資料
            var r = _F5102Repo.GetLocSettleMonFee(DateTime.Today, "");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        

    }
}
