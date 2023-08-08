using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910503RepositoryTest :BaseRepositoryTest
    {
        private readonly F910503Repository _f910503Repository;
        public F910503RepositoryTest()
        {
            _f910503Repository = new F910503Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF910503ScanLog()
        {
            
            string dcCode = "";
            string gupCode = "";
            string custCode = "";
            string processNo = "";
            string clientIp = "";
            Output(new { dcCode, gupCode, custCode, processNo, clientIp });
            var result = _f910503Repository.GetF910503ScanLog(dcCode, gupCode, custCode, processNo, clientIp);
            Output(result);
        }

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
