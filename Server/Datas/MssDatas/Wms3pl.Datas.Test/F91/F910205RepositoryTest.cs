using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910205RepositoryTest : BaseRepositoryTest
    {
        private readonly F910205Repository _f910205Repository;
        public F910205RepositoryTest()
        {
            _f910205Repository = new F910205Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetCanBackMaterialItem()
        {
            string dcCode = "";
            string gupCode = "";
            string custCode = "";
            string processNo = "";
            Output(new { dcCode, gupCode, custCode, processNo });
            var result = _f910205Repository.GetCanBackMaterialItem(dcCode, gupCode, custCode, processNo);
            Output(result);
        }
        [TestMethod]
        public void GetPickTicketReport()
        {
            string dcCode = "";
            string gupCode = "";
            string custCode = "";
            string processNo = "";
            Output(new { dcCode, gupCode, custCode, processNo });
            var result = _f910205Repository.GetPickTicketReport(dcCode, gupCode, custCode, processNo);
            Output(result);
        }
        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
