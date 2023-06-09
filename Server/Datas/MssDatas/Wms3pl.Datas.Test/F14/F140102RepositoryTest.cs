using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F14;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F14
{
    [TestClass]
    public class F140102RepositoryTest : BaseRepositoryTest
    {
        private F140102Repository _f140102Repository;
        public F140102RepositoryTest()
        {
            _f140102Repository = new F140102Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void IsExistDatasByTheSameWareHouseChannel()
        {
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030002";
            var inventoryYear = (short)0;
            var inventoryMon = (short)0;
            var warehouseId = "G03";
            var begFloor = "1";
            var endFloor = "1";
            var begChannel = "0C";
            var endChannel = "0C";
            var begPlain = "01";
            var endPlain = "05";

            Console.WriteLine($"{JsonSerializer.Serialize(new { dcCode, gupCode, custCode, inventoryYear, inventoryMon, warehouseId, begFloor, endFloor, begChannel, endChannel, begPlain, endPlain })}");
            var result = _f140102Repository.IsExistDatasByTheSameWareHouseChannel(dcCode, gupCode, custCode, inventoryYear, inventoryMon, warehouseId, begFloor, endFloor, begChannel, endChannel, begPlain, endPlain);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }
    }
}
