using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910203RepositoryTest : BaseRepositoryTest
    {
        private readonly F910203Repository _f910203Repository;
        public F910203RepositoryTest()
        {
            _f910203Repository = new F910203Repository(Schemas.CoreSchema);
        }
      

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
