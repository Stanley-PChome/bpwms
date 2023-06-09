using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910207RepositoryTest : BaseRepositoryTest
    {
        private readonly F910207Repository _f910207Repository;
        public F910207RepositoryTest()
        {
            _f910207Repository = new F910207Repository(Schemas.CoreSchema);
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
