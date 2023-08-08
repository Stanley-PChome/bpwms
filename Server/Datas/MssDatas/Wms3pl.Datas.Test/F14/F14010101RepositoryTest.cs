using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F14;
using Wms3pl.WebServices.DataCommon;
using System.Text.Json;

namespace Wms3pl.Datas.Test.F14
{
    [TestClass]
    public class F14010101RepositoryTest: BaseRepositoryTest
    {
        private F14010101Repository _f14010101Repository;

        public F14010101RepositoryTest()
        {
            _f14010101Repository = new F14010101Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetDatasByHasSerialNo()
        {
            var dcCode= "001";
            var gupCode= "01";
            var custCode= "030002";
            var inventoryNo= "I2018090700001";
            Console.WriteLine($"{JsonSerializer.Serialize(new { dcCode , gupCode, custCode, inventoryNo })}");
            var result = _f14010101Repository.GetDatasByHasSerialNo(dcCode, gupCode, custCode, inventoryNo);
            //Console.WriteLine(JsonSerializer.Serialize(result.ToList()));
        }

        [TestMethod]
        public void DeleteF14010101()
        {           
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030002";
            var inventoryNo = "I2018090700001";
            var locCode = "10A010102";
            var itemCode = "CAPIE009803";
            var validDate = new DateTime(9999,12,31);
            var enterDate = new DateTime(2018, 08, 31);
            var boxCtrlNo = "0";
            var palletCtrlNo = "0";
            var makeNo = "0";
            Console.WriteLine($"{JsonSerializer.Serialize(new { dcCode, gupCode, custCode, inventoryNo, locCode, itemCode, validDate, enterDate, boxCtrlNo, palletCtrlNo, makeNo })}");
            _f14010101Repository.DeleteF14010101(dcCode, gupCode, custCode,
                                                inventoryNo, locCode, itemCode,
                                                validDate, enterDate, boxCtrlNo,
                                                palletCtrlNo, makeNo);
        }
    }
}
