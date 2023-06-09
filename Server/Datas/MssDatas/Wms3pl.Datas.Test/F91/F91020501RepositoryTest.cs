using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F91020501RepositoryTest : BaseRepositoryTest
    {
        private readonly F91020501Repository _f91020501Repository;
        public F91020501RepositoryTest()
        {
            _f91020501Repository = new F91020501Repository(Schemas.CoreSchema);
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
