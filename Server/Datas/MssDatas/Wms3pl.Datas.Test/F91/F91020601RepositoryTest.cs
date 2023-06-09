using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F91020601RepositoryTest : BaseRepositoryTest
    {
        private readonly F91020601Repository _f91020601Repository;
        public F91020601RepositoryTest()
        {
            _f91020601Repository = new F91020601Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetBackedAllocationNos()
        {
            string dcCode = "";
            string gupCode = "";
            string custCode = "";
            string processNo = "";
            Output(new { dcCode, gupCode, custCode, processNo });
            var result = _f91020601Repository.GetBackedAllocationNos(dcCode, gupCode, custCode, processNo);
            Output(result);
        }

        [TestMethod]
        public void GetItemSumQtys()
        {
            string dcCode = "";
            string gupCode = "";
            string custCode = "";
            string processNo = "";
            string backItemType = "";
            bool isBacked = false;
            Output(new { dcCode, gupCode, custCode, processNo, backItemType, isBacked });
            var result = _f91020601Repository.GetItemSumQtys(dcCode, gupCode, custCode, processNo, backItemType, isBacked);
            Output(result);
        }

        [TestMethod]
        public void GetBackItemSumQtys()
        {
            string dcCode = "";
            string gupCode = "";
            string custCode = "";
            string processNo = "";
            bool isBacked = false;
            Output(new { dcCode, gupCode, custCode, processNo , isBacked });
            var result = _f91020601Repository.GetBackItemSumQtys(dcCode, gupCode, custCode, processNo, isBacked);
            Output(result);
        }

        [TestMethod]
        public void GetP91010105Reports()
        {
            string dcCode = "";
            string gupCode = "";
            string custCode = "";
            string processNo = "";
            Output(new { dcCode, gupCode, custCode, processNo });
            var result = _f91020601Repository.GetP91010105Reports(dcCode, gupCode, custCode, processNo);
            Output(result);
        }

        [TestMethod]
        public void GetBackedSerialNos()
        {
            string dcCode = "";
            string gupCode = "";
            string custCode = "";
            string processNo = "";
            Output(new { dcCode, gupCode, custCode, processNo });
            var result = _f91020601Repository.GetBackedSerialNos(dcCode, gupCode, custCode, processNo);
            Output(result);
        }

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
