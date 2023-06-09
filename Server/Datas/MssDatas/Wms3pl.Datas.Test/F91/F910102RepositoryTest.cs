using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910102RepositoryTest : BaseRepositoryTest
    {
        private readonly F910102Repository _f910102Repository;
        public F910102RepositoryTest()
        {
            _f910102Repository = new F910102Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetBomQtyData()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string processNo = "";
            Output(new { dcCode, gupCode , custCode, processNo });
            var result = _f910102Repository.GetBomQtyData(dcCode, gupCode, custCode, processNo);
            Output(result);
        }

        [TestMethod]
        public void GetBomQtyData2()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string processNo = "";
            Output(new { dcCode, gupCode, custCode, processNo });
            var result = _f910102Repository.GetBomQtyData2(dcCode, gupCode, custCode, processNo);
            Output(result);
        }

        [TestMethod]
        public void GetBomItemDetailList()
        {
            string gupCode = "01";
            string custCode = "010001";
            List<string> itemCodes = new List<string>() { "BB010606" };
            Output(new { gupCode, custCode, itemCodes });
            var result = _f910102Repository.GetBomItemDetailList(gupCode, custCode, itemCodes);
            Output(result);
        }

        [TestMethod]
        public void GetF910102Data()
        {
            string gupCode = "01";
            string custCode = "010001";
            string bomNo = "2016120001";
            Output(new { gupCode, custCode, bomNo });
            var result = _f910102Repository.GetF910102Data(gupCode, custCode, bomNo);
            Output(result);
        }

        [TestMethod]
        public void GetF910102ByBomNoDatas()
        {
            string gupCode = "01";
            string custCode = "010001";
            List<string> bomNos = new List<string>() { "2016120001" };
            Output(new { gupCode, custCode, bomNos });
            var result = _f910102Repository.GetF910102ByBomNoDatas(gupCode, custCode, bomNos);
            Output(result);
        }

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
