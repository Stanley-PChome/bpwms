using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910001RepositoryTest : BaseRepositoryTest
    {
        private readonly F910001Repository _f910001Repository;
        public F910001RepositoryTest()
        {
            _f910001Repository = new F910001Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void TestMethod1()
        {
        }

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
