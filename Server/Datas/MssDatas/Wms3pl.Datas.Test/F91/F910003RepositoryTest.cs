using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910003RepositoryTest : BaseRepositoryTest
    {
        private readonly F910003Repository _f910003Repository;
        public F910003RepositoryTest()
        {
            _f910003Repository = new F910003Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetF910003Datas()
        {
            string ITEM_TYPE_ID = "001";
            string ITEM_TYPE = "";
            Output(new { ITEM_TYPE_ID , ITEM_TYPE });
            var result = _f910003Repository.GetF910003Datas(ITEM_TYPE_ID, ITEM_TYPE);
            Output(result);
        }

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
