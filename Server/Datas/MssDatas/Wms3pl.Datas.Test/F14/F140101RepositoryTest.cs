using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F14;
using Wms3pl.WebServices.DataCommon;
using System.Text.Json;
using System.Linq;

namespace Wms3pl.Datas.Test.F14
{
    [TestClass]
    public class F140101RepositoryTest : BaseRepositoryTest
    {
        private F140101Repository _f140101Repository;

        public F140101RepositoryTest()
        {
            _f140101Repository = new F140101Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetDatas()
        {
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var inventoryNo = "I2017030800001";
            var inventoryType = "2";
            var inventorySDate = new DateTime(2017, 3, 8);
            var inventoryEDate = new DateTime(2017, 3, 8);
            var inventoryCycle = "";
            var inventoryYear = "";
            var inventoryMonth = "";
            var status = "";

            Console.WriteLine($"{JsonSerializer.Serialize(new { dcCode, gupCode, custCode, inventoryNo, inventoryType, inventorySDate, inventoryEDate, inventoryCycle, inventoryYear, inventoryMonth, status })}");
            var result = _f140101Repository.GetDatas(dcCode, gupCode, custCode, inventoryNo, inventoryType, inventorySDate, inventoryEDate, inventoryCycle, inventoryYear, inventoryMonth, status);
            Console.WriteLine(JsonSerializer.Serialize(result.ToList()));
        }

        [TestMethod]
        public void GetDatasExpansion()
        {
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var inventoryNo = "I2017030800001";
            var inventoryType = "2";
            var inventorySDate = new DateTime(2017, 3, 8);
            var inventoryEDate = new DateTime(2017, 3, 8);
            var inventoryCycle = "";
            var inventoryYear = "";
            var inventoryMonth = "";
            var status = "";

            Console.WriteLine($"{JsonSerializer.Serialize(new { dcCode, gupCode, custCode, inventoryNo, inventoryType, inventorySDate, inventoryEDate, inventoryCycle, inventoryYear, inventoryMonth, status })}");
            var result = _f140101Repository.GetDatasExpansion(dcCode, gupCode, custCode, inventoryNo, inventoryType, inventorySDate, inventoryEDate, inventoryCycle, inventoryYear, inventoryMonth, status);
            Console.WriteLine(JsonSerializer.Serialize(result.ToList()));
        }

        [TestMethod]
        public void GetInventoryQueryDatas()
        {
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var postingDateBegin = "2020/07/07";
            var postingDateEnd = "2020/07/07";
            Console.WriteLine($"{JsonSerializer.Serialize(new { dcCode, gupCode, custCode, postingDateBegin, postingDateEnd })}");
            var result = _f140101Repository.GetInventoryQueryDatas(dcCode, gupCode, custCode, postingDateBegin, postingDateEnd);
            Console.WriteLine(JsonSerializer.Serialize(result.ToList()));
        }

        [TestMethod]
        public void GetInventoryQueryDatasForDc()
        {
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var inventoryNo = "I2020070700001";
            var sortByCount = "1";
            var warehouseId = "G01";
            var itemCodes = "BB010103";

            Console.WriteLine($"{JsonSerializer.Serialize(new { dcCode, gupCode, custCode, inventoryNo, sortByCount, warehouseId, itemCodes })}");
            var result = _f140101Repository.GetInventoryQueryDatasForDc(dcCode, gupCode, custCode, inventoryNo, sortByCount, warehouseId, itemCodes);
            Console.WriteLine(JsonSerializer.Serialize(result.ToList()));
        }

        [TestMethod]
        public void GetF140106QueryDetailData()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string inventoryNo = "I2020070700001";
            Console.WriteLine($"{JsonSerializer.Serialize(new { dcCode, gupCode, custCode, inventoryNo })}");
            var result = _f140101Repository.GetF140106QueryDetailData(dcCode, gupCode, custCode, inventoryNo);
            Console.WriteLine(JsonSerializer.Serialize(result.ToList()));
        }

        [TestMethod]
        public void IsExistDatasByTheSameCycleTimes()
        {
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var inventoryYear = (short)0;
            var inventoryMon = (short)0;
            var cycleTimes = (short)0;
            Console.WriteLine($"{JsonSerializer.Serialize(new { dcCode, gupCode, custCode, inventoryYear, inventoryMon, cycleTimes })}");
            var result = _f140101Repository.IsExistDatasByTheSameCycleTimes(dcCode, gupCode, custCode, inventoryYear, inventoryMon, cycleTimes);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        [TestMethod]
        public void UpdateItemCntAndQty()
        {
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030002";
            var inventoryNo = "I2019073100002";
            Console.WriteLine($"{JsonSerializer.Serialize(new { dcCode, gupCode, custCode, inventoryNo })}");
            _f140101Repository.UpdateItemCntAndQty(dcCode, gupCode, custCode, inventoryNo);
        }

        [TestMethod]
        public void GetDataByUserCanInventory()
        {
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030002";
            var inventoryNo = "I2019073100001";
            // 須變更登入使用者才有資料
            Console.WriteLine($"{JsonSerializer.Serialize(new { dcCode, gupCode, custCode, inventoryNo })}");
            var result = _f140101Repository.GetDataByUserCanInventory(dcCode, gupCode, custCode, inventoryNo);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        [TestMethod]
        public void GetSettleData()
        {
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var settleDate = new DateTime(2020, 7, 7);
            Console.WriteLine($"{JsonSerializer.Serialize(new { dcCode, gupCode, custCode, settleDate })}");
            var result = _f140101Repository.GetSettleData(dcCode, gupCode, custCode, settleDate);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        [TestMethod]
        public void GetEnabledData()
        {
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030002";
            var inventoryNo = "I2019073100002";
            Console.WriteLine($"{JsonSerializer.Serialize(new { dcCode, gupCode, custCode, inventoryNo })}");
            var result = _f140101Repository.GetEnabledData(dcCode, gupCode, custCode, inventoryNo);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        [TestMethod]
        public void GetInventoryDetailData()
        {
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010002";
            var inventoryNo = "I2019042500001";
            var isSec = false;
            Console.WriteLine($"{JsonSerializer.Serialize(new { dcCode, gupCode, custCode, inventoryNo, isSec })}");
            var result = _f140101Repository.GetInventoryDetailData(dcCode, gupCode, custCode, inventoryNo, isSec);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

 


        [TestMethod]
        public void GetInventoryDetailList()
        {
            #region Params
            var dcNo = "001";
            var custNo = "030001";
            var gupCode = "01";
            var invNo = "I2019050700001";
            #endregion

            _f140101Repository.GetInventoryDetailList(dcNo, custNo, gupCode, invNo);
        }

        [TestMethod]
        public void GetInventoryList()
        {
            #region Params
            var dcNo = "001";
            var custNo = "030001";
            var gupCode = "01";
            var invNo = "I2019050700001";
            var invDate = "2019-05-07";
            #endregion

            _f140101Repository.GetInventoryList(dcNo, custNo, gupCode, invNo, invDate);
        }
    }
}
