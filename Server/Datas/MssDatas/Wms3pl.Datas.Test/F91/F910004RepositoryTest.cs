using System;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910004RepositoryTest : BaseRepositoryTest
    {
        private readonly F910004Repository _f910004Repository;
        public F910004RepositoryTest()
        {
            _f910004Repository = new F910004Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF910004Data()
        {
            string dcCode = "";
            Output(new { dcCode});
            var result = _f910004Repository.GetF910004Data(dcCode);
            Output(result);
        }

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
