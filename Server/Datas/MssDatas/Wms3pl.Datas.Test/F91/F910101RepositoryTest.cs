using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910101RepositoryTest : BaseRepositoryTest
    {
        private readonly F910101Repository _f910101Repository;
        public F910101RepositoryTest()
        {
            _f910101Repository = new F910101Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetF910101Datas()
        {
            string gupCode = "01";
            string custCode = "010001";
            string bomNo = "";
            string itemCode = "";
            string status = "0";
            string bomType = "0";
            Output(new { gupCode, custCode, bomNo, itemCode, status, bomType });
            var result = _f910101Repository.GetF910101Datas(gupCode, custCode, bomNo, itemCode, status, bomType);
            Output(result);
        }

        [TestMethod]
        public void GetF910101ByBomNoDatas()
        {
            string gupCode = "01";
            string custCode = "010001";
            List<string> bomNo = new List<string>() {"2017010008", "2017010009" };
            Output(new { gupCode, custCode, bomNo });
            var result = _f910101Repository.GetF910101ByBomNoDatas(gupCode, custCode, bomNo);
            Output(result);
        }

        [TestMethod]
        public void GetF910102Datas()
        {
            string gupCode = "01";
            string custCode = "010001";
            string bomNo = "2017010008";
            Output(new { gupCode, custCode, bomNo });
            var reuslt = _f910101Repository.GetF910102Datas(gupCode, custCode, bomNo);
            Output(reuslt);
        }

        [TestMethod]
        public void GetF910101Ex2()
        {
            string gupCode = "01";
            string custCode = "010001";
            string status = "0";
            Output(new { gupCode, custCode, status });
            var reuslt = _f910101Repository.GetF910101Ex2(gupCode, custCode, status);
            Output(reuslt);
        }

      

      

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
