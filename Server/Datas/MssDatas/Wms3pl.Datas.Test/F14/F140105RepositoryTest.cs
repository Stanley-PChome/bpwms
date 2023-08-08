using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F14;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F14
{
    [TestClass]
    public class F140105RepositoryTest: BaseRepositoryTest
    {
        private readonly F140105Repository _f140105Repository;
        public F140105RepositoryTest()
        {
            _f140105Repository = new F140105Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void CountInventoryDetailItems()
        {
            string dcCode = "12";
            string gupCode = "10";
            string custCode = "010001";
            string inventoryNo = "I20230425000004";
            string wareHouseId = "T01";
            string begLocCode = "A02010A01";
            string endLocCode = "A02010A03";
            string itemCode = "KK003";
            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo,
                wareHouseId,
                begLocCode,
                endLocCode,
                itemCode
            })}");
            var result = _f140105Repository.CountInventoryDetailItems(dcCode,
                gupCode,
                custCode,
                inventoryNo,
                wareHouseId,
                begLocCode,
                endLocCode,
                itemCode);
            Console.WriteLine(JsonSerializer.Serialize(result));

      //var result0 = _f140105Repository.CountInventoryDetailItems0(dcCode,
      //          gupCode,
      //          custCode,
      //          inventoryNo,
      //          wareHouseId,
      //          begLocCode,
      //          endLocCode,
      //          itemCode);
      //Assert.AreEqual(JsonSerializer.Serialize(result), JsonSerializer.Serialize(result0));
    }

    [TestMethod]
        public void GetInventoryDetailItems()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010002";
            string inventoryNo = "I2019042500001";
            string wareHouseId = "T01";
            string begLocCode = "10Q010201";
            string endLocCode = "10Q010202";
            string itemCode = "";
			      string checkTool = "0";
            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo,
                wareHouseId,
                begLocCode,
                endLocCode,
                itemCode
            })}");
            var result = _f140105Repository.GetInventoryDetailItems(dcCode,
                gupCode,
                custCode,
                inventoryNo,
                wareHouseId,
                begLocCode,
                endLocCode,
                itemCode,
								checkTool);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        [TestMethod]
        public void GetInventoryDetailItems2()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010002";
            string inventoryNo = "I2019042500001";
            string locCode = "10Q010201";
            string itemCode = "PS17002-C0004";
            DateTime enterDate = new DateTime(2019,4,25);
            DateTime validDate = new DateTime(2020,11,16);
            string makeNo = "0";
            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo,
                locCode,
                itemCode,
                enterDate,
                validDate,
                makeNo
            })}");
            var result = _f140105Repository.GetInventoryDetailItems(dcCode,
                gupCode,
                custCode,
                inventoryNo,
                locCode,
                itemCode,
                enterDate,
                validDate,
                makeNo);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        [TestMethod]
        public void Delete()
        {
//            DC_CODE GUP_CODE    CUST_CODE INVENTORY_NO
//001 01  010002  I2019042500002
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010002";
            string inventoryNo = "I2019042500002";

            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo
            })}");
            _f140105Repository.Delete(dcCode,
                gupCode,
                custCode,
                inventoryNo);
        }

        [TestMethod]
        public void GetF140105FirstQtyNull()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010002";
            string inventoryNo = "I2019042500001";
            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo
            })}");
            var result = _f140105Repository.GetF140105FirstQtyNull(dcCode,
                gupCode,
                custCode,
                inventoryNo);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        [TestMethod]
        public void CheckF140105Exist()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "030002";
            string inventoryNo = "I2018090700001";

            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo
            })}");

            var result = _f140105Repository.CheckF140105Exist(dcCode,
               gupCode,
               custCode,
               inventoryNo);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        [TestMethod]
        public void DeleteF140105()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010002";
            string inventoryNo = "I2019042500001";
            string locCode = "10Q010201";
            string itemCode = "PS17002-C0004";
            DateTime validDate = new DateTime(2020,11,16);
            DateTime enterDate = new DateTime(2019, 3, 17);
            string boxCtrlNo = "SAS0425";
            string palletCtrlNo = "0";
            string makeNo = "SAS0425";

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

            _f140105Repository.DeleteF140105(dcCode,
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
        public void FindF140105()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010002";
            string inventoryNo = "I2019042500001";
            string locCode = "10Q010201";
            string itemCode = "PS17002-C0004";
            DateTime validDate = new DateTime(2020, 11, 16);
            DateTime enterDate = new DateTime(2019, 3, 17);
            string boxCtrlNo = "kk0425";
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

            var result =_f140105Repository.FindF140105(dcCode,
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
