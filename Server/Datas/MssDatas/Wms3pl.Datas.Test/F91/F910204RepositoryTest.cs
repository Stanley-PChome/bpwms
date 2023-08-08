using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910204RepositoryTest : BaseRepositoryTest
    {
        private readonly F910204Repository _f910204Repository;
        public F910204RepositoryTest()
        {
            _f910204Repository = new F910204Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetProcessActions()
        {
            string dcCode = "";
            string gupCode = "";
            string custCode = "";
            string processNo = "";
            Output(new { dcCode, gupCode, custCode, processNo });
            var result = _f910204Repository.GetProcessActions(dcCode, gupCode, custCode, processNo);
            Output(result);
        }

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
