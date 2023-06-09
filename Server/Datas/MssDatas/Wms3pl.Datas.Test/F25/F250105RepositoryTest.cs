using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F25;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F25
{
    [TestClass]
    public class F250105RepositoryTest : BaseRepositoryTest
    {
        private readonly F250105Repository _f250105Repository;
        public F250105RepositoryTest()
        {
            _f250105Repository = new F250105Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetF250105Data()
        {

            string gupCode = "";
            string custCode = "";
            string clientIp = "";
            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                gupCode,
                custCode,
                clientIp
            })}");
            var result = _f250105Repository.GetF250105Data(gupCode,
                custCode,
                clientIp);
            Console.WriteLine($@"{JsonSerializer.Serialize(result)}");
        }

        [TestMethod]
        public void UpdateF250105ExtendData()
        {
            List<string> listSerialNo = new List<string>();
            string gupCode = "";
            string custCode = "";
            string userId = "";
            string userName = "";
            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                listSerialNo,
                gupCode,
                custCode,
                userId,
                userName
            })}");
            _f250105Repository.UpdateF250105ExtendData(listSerialNo,
                gupCode,
                custCode,
                userId,
                userName);
        }

        [TestMethod]
        public void DeleteF250105()
        {
            string gupCode = "";
            string custCode = "";
            string clientIp = "";
            string userId = "";
            string userName = "";
            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                gupCode,
                custCode,
                clientIp,
                userId,
                userName
            })}");
            _f250105Repository.DeleteF250105(gupCode,
                custCode,
                clientIp,
                userId,
                userName);
        }
    }
}
