using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F91020502RepositoryTest : BaseRepositoryTest
    {
        private readonly F91020502Repository _f91020502Repository;
        public F91020502RepositoryTest()
        {
            _f91020502Repository = new F91020502Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetPickAllocationNos()
        {
            string dcCode = "";
            string gupCode = "";
            string custCode = "";
            string processNo = "";
            Output(new { dcCode, gupCode, custCode, processNo });
            var result = _f91020502Repository.GetPickAllocationNos(dcCode, gupCode, custCode, processNo);
            Output(result);
        }

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
