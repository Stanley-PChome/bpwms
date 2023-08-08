using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910206RepositoryTest : BaseRepositoryTest
    {
        private readonly F910206Repository _f910206Repository;
        public F910206RepositoryTest()
        {
            _f910206Repository = new F910206Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetBackListForP9101010500()
        {
            string dcCode = "";
            string gupCode = "";
            string custCode = "";
            string processNo = "";
            Output(new { dcCode, gupCode, custCode, processNo });
            var result = _f910206Repository.GetBackListForP9101010500(dcCode, gupCode, custCode, processNo);
            Output(result);
        }

        [TestMethod]
        public void GetBackList()
        {
            string dcCode = "";
            string gupCode = "";
            string custCode = "";
            string processNo = "";
            bool isBacked = true;
            List<long> excludeBackNos = new List<long>();
            Output(new { dcCode, gupCode, custCode, processNo, isBacked, excludeBackNos });
            var result = _f910206Repository.GetBackList(dcCode, gupCode, custCode, processNo, isBacked, excludeBackNos);
            Output(result);
        }

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
