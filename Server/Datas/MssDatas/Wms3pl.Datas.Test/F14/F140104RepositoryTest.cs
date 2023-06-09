using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F14;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F14
{
    [TestClass]
    public class F140104RepositoryTest : BaseRepositoryTest
    {
        private readonly F140104Repository _f140104Repository;
        public F140104RepositoryTest()
        {
            _f140104Repository = new F140104Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void CountInventoryDetailItems()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string inventoryNo = "I2017031400001";
            string wareHouseId = "G01";
            string begLocCode = "10A010301";
            string endLocCode = "10A010302";
            string itemCode = "BB010402";

            Console.WriteLine($"{JsonSerializer.Serialize(new { dcCode, gupCode, custCode, inventoryNo, wareHouseId, begLocCode, endLocCode, itemCode})}");
            var result = _f140104Repository.CountInventoryDetailItems(dcCode, gupCode, custCode, inventoryNo, wareHouseId, begLocCode, endLocCode, itemCode);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        [TestMethod]
        public void GetInventoryDetailItems()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "030002";
            string inventoryNo = "I2018090500003";
            string wareHouseId = "";
            string begLocCode = "10C020201";
            string endLocCode = "10C020202";
            string itemCode = "";
			      string checkTool = "0";
            Console.WriteLine($"{JsonSerializer.Serialize(new { dcCode, gupCode, custCode, inventoryNo, wareHouseId, begLocCode, endLocCode, itemCode })}");
            var result = _f140104Repository.GetInventoryDetailItems(dcCode, gupCode, custCode, inventoryNo, wareHouseId, begLocCode, endLocCode, itemCode, checkTool);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        [TestMethod]
        public void GetInventoryDetailItems2()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string inventoryNo = "I2020070700001";
            string locCode = "10A010303";
            string itemCode = "BB010102";
            DateTime enterDate = new DateTime(2020,7,7);
            DateTime validDate = new DateTime(2022, 12, 1);
            string makeNo = "S01";
            Console.WriteLine($"{JsonSerializer.Serialize(new { dcCode, gupCode, custCode, inventoryNo, locCode, itemCode, enterDate, validDate, makeNo })}");
            var result = _f140104Repository.GetInventoryDetailItems(dcCode, gupCode, custCode, inventoryNo, locCode, itemCode, enterDate, validDate, makeNo);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        [TestMethod]
        public void CheckInventoryDetailHasEnterQty()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010002";
            string inventoryNo = "I2019042500001";
            string isSecond = "0";
            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo,
                isSecond
            })}");
            var result = _f140104Repository.CheckInventoryDetailHasEnterQty(dcCode,
                gupCode,
                custCode,
                inventoryNo,
                isSecond);
            Console.WriteLine(JsonSerializer.Serialize(result));
            dcCode = "001";
            gupCode = "01";
            custCode = "010002";
            inventoryNo = "I2019042500001";
            isSecond = "1";
            Console.WriteLine("========復盤=========");
            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo,
                isSecond
            })}");
            result = _f140104Repository.CheckInventoryDetailHasEnterQty(dcCode,
                gupCode,
                custCode,
                inventoryNo,
                isSecond);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        [TestMethod]
        public void GetF140104FirstQtyNull()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010002";
            string inventoryNo = "I2018100200001";
            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo                
            })}");
            var result = _f140104Repository.GetF140104FirstQtyNull(dcCode,
                gupCode,
                custCode,
                inventoryNo);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        [TestMethod]
        public void GetInventoryLoc()
        {

            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string inventoryNo = "I2017030800001";
            string locCode = "10A020101";
            string isSecond = "0";
            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo,
                locCode,
                isSecond
            })}");
            var result = _f140104Repository.GetInventoryLoc(dcCode,
                gupCode,
                custCode,
                inventoryNo,
                locCode,
                isSecond);
            Console.WriteLine(JsonSerializer.Serialize(result));
            // DC_CODE GUP_CODE    CUST_CODE INVENTORY_NO    LOC_CODE
            // 001 01  010002  I2019042500001  10Q010201
        }

        [TestMethod]
        public void GetInventoryScanItem()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string inventoryNo = "I2017030800001";
            string isSecond = "0";
            string locCode = "10A020302";
            string itemCode = "BB010505";
            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo,
                isSecond,
                locCode,
                itemCode
            })}");
            var result = _f140104Repository.GetInventoryScanItem(dcCode,
                gupCode,
                custCode,
                inventoryNo,
                isSecond,
                locCode,
                itemCode);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        [TestMethod]
        public void GetInventoryDiffLocItemQties()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string inventoryNo = "I2017030800001";
            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo
            })}");
            var result = _f140104Repository.GetInventoryDiffLocItemQties(dcCode,
                gupCode,
                custCode,
                inventoryNo);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        [TestMethod]
        public void GetInventoryLocItems()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string inventoryNo = "I2017030800001";
            string isSecond = "0";

            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo,
                isSecond
            })}");
            var result = _f140104Repository.GetInventoryLocItems(dcCode,
                gupCode,
                custCode,
                inventoryNo,
                isSecond);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        [TestMethod]
        public void GetInventoryByLocDetails()
        {
            // DC_CODE GUP_CODE    CUST_CODE INVENTORY_NO
            // 001 01  010002  I2019042500001
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010002";
            string inventoryNo = "I2019042500001";
            string isSecond = "1";

            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo,
                isSecond
            })}");
            var result = _f140104Repository.GetInventoryByLocDetails(dcCode,
                gupCode,
                custCode,
                inventoryNo,
                isSecond);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        [TestMethod]
        public void GetInventoryDetailItems3()
        {
            //            DC_CODE GUP_CODE    CUST_CODE INVENTORY_NO
            //001 01  010001  I2020070700001
            // BB010102 BB010101
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string inventoryNo = "I2020070700001";
            bool isSecond = false;
            List<string> itemCodes = new List<string>() { "BB010101", "BB010102" };
            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo,
                isSecond,
                itemCodes
            })}");
            var result = _f140104Repository.GetInventoryDetailItems(dcCode,
                gupCode,
                custCode,
                inventoryNo,
                isSecond,
                itemCodes);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        [TestMethod]
        public void DeleteF140104()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string inventoryNo = "I2017030800001";
            string locCode = "10A010103";
            string itemCode = "BB010120";
            DateTime validDate = new DateTime(2018,11,1);
            DateTime enterDate = new DateTime(2017, 2, 18);
            string boxCtrlNo = "0";
            string palletCtrlNo = "0";
            string makeNo = "";
            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo,
                locCode,
                itemCode,
                validDate,
                enterDate,
                boxCtrlNo,
                palletCtrlNo,
                makeNo
            })}");
            _f140104Repository.DeleteF140104(dcCode,
                gupCode,
                custCode,
                inventoryNo,
                locCode,
                itemCode,
                validDate,
                enterDate,
                boxCtrlNo,
                palletCtrlNo,
                makeNo);
        }

        [TestMethod]
        public void FindF140104()
        {

            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string inventoryNo = "I2017030800001";
            string locCode = "10A040404";
            string itemCode = "BB020309";
            DateTime validDate = new DateTime(2019, 11, 1);
            DateTime enterDate = new DateTime(2017, 2, 18);
            string boxCtrlNo = "0";
            string palletCtrlNo = "0";
            string makeNo = "";
            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo,
                locCode,
                itemCode,
                validDate,
                enterDate,
                boxCtrlNo,
                palletCtrlNo,
                makeNo
            })}");
           var result =  _f140104Repository.FindF140104(dcCode,
                gupCode,
                custCode,
                inventoryNo,
                locCode,
                itemCode,
                validDate,
                enterDate,
                boxCtrlNo,
                palletCtrlNo,
                makeNo);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }
    }
}
