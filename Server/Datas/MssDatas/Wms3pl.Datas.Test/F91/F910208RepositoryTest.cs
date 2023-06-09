using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910208RepositoryTest : BaseRepositoryTest
    {
        private readonly F910208Repository _f910208Repository;
        public F910208RepositoryTest()
        {
            _f910208Repository = new F910208Repository(Schemas.CoreSchema);
        }

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
