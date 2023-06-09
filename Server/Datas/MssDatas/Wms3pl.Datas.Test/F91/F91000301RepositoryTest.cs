using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F91000301RepositoryTest : BaseRepositoryTest
    {
        private readonly F91000301Repository _f91000301Repository;
        public F91000301RepositoryTest()
        {
            _f91000301Repository = new F91000301Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetAccItemKinds()
        {
            string itemTypeId = "001";
            Output(new { itemTypeId });
            var result = _f91000301Repository.GetAccItemKinds(itemTypeId);
            Output(result);
        }

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
