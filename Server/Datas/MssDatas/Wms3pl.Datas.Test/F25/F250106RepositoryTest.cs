using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F25;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F25
{
    [TestClass]
    public class F250106RepositoryTest : BaseRepositoryTest
    {
        private readonly F250106Repository _f250106Repository;
        public F250106RepositoryTest()
        {
            _f250106Repository = new F250106Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF250106Data()
        {
            string gupCode = "";
            string custCode = "";
            string clientIp = "";
            string onlyPass = "";
            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                gupCode,
                custCode,
                clientIp,
                onlyPass
            })}");
            var result = _f250106Repository.GetF250106Data(gupCode,
                custCode,
                clientIp,
                onlyPass);
            Console.WriteLine($@"{JsonSerializer.Serialize(result)}");
        }
    }
}
