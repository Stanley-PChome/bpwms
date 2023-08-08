using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910302RepositoryTest : BaseRepositoryTest
    {
        private readonly F910302Repository _f910302Repository;
        public F910302RepositoryTest()
        {
            _f910302Repository = new F910302Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetF910302ByProcessIds()
        {
            string dcCode = "001";
            string gupCode = "01";
            DateTime enableDate = new DateTime(2020,7,1);
            DateTime disableDate = new DateTime(2020, 7, 31);
            var processIds = new List<string>() { "A001" };
            Output(new { dcCode , gupCode, enableDate, disableDate , processIds });
            var result = _f910302Repository.GetF910302ByProcessIds(dcCode, gupCode, enableDate, disableDate, processIds);
            Output(result);
        }
        [TestMethod]
        public void GetContractDetails()
        {
            string dcCode = "001";
            string gupCode = "01";
            string contractNo = "BP202007070AZ";
            Output(new { dcCode , gupCode, contractNo });
            var result = _f910302Repository.GetContractDetails(dcCode, gupCode, contractNo);
            Output(result);
        }
        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
