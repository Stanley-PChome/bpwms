using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F14;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F14
{
    [TestClass]
    public class F140110RepositoryTest : BaseRepositoryTest
    {
        private readonly F140110Repository _f140110Repository;
        public F140110RepositoryTest()
        {
            _f140110Repository = new F140110Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void UpdateItemForSql()
        {
            // DC_CODE GUP_CODE    CUST_CODE INVENTORY_NO    ISSECOND LOC_CODE    ITEM_CODE
            // 001 01  030002  I2019073100001  0   10C020404 CAPIE024904
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "030002";
            string inventoryNo = "I2019073100001";
            string isSecond = "0";
            string locCode = "10C020404";
            string itemCode = "CAPIE024904T";
            string itemName = "【A'PIEU】香水信號護手護唇膏/櫻花-A"; //【A'PIEU】香水信號護手護唇膏/櫻花
            string orginalItemCode = "CAPIE024904";


            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo,
                isSecond,
                locCode,
                itemCode,
                itemName,
                orginalItemCode
            })}");
            _f140110Repository.UpdateItemForSql(dcCode,
                gupCode,
                custCode,
                inventoryNo,
                isSecond,
                locCode,
                itemCode,
                itemName,
                orginalItemCode);
        }

        [TestMethod]
        public void DeleteByLocNotInventory()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "030002";
            string inventoryNo = "I2019073100001";
            string isSecond = "0";
            string locCode = "10C020405";
            string crtStaff = "wms";
            string crtStaffName = "WMS";

            Console.WriteLine($@"{JsonSerializer.Serialize(new
            {
                dcCode,
                gupCode,
                custCode,
                inventoryNo,
                isSecond,
                locCode,
                crtStaff,
                crtStaffName
            })}");

           _f140110Repository.DeleteByLocNotInventory(dcCode,
                gupCode,
                custCode,
                inventoryNo,
                isSecond,
                locCode,
                crtStaff,
                crtStaffName);
            
        }
    }
}
