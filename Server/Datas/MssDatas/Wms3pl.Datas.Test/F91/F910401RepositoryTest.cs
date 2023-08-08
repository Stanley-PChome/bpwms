using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910401RepositoryTest :BaseRepositoryTest
    {
        private readonly F910401Repository _f910401Repository;
        public F910401RepositoryTest()
        {
            _f910401Repository = new F910401Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetF910401Report()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string quoteNo = "Q2020070700020";
            Output(new { dcCode, gupCode, custCode, quoteNo});
            var result = _f910401Repository.GetF910401Report(dcCode, gupCode, custCode, quoteNo);
            Output(result);
        }

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
