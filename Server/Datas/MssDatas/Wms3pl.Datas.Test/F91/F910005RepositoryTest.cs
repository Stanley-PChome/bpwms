using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910005RepositoryTest : BaseRepositoryTest
    {
        private readonly F910005Repository _f910005Repository;
        public F910005RepositoryTest()
        {
            _f910005Repository = new F910005Repository(Schemas.CoreSchema);
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
