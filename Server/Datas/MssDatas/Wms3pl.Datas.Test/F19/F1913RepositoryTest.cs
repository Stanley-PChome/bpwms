using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F19;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.Datas.Test.F19
{
    [TestClass]
    public class F1913RepositoryTest : Wms3pl.Datas.Test.BaseRepositoryTest
    {
        private F1913Repository _f1913Repo;
        public F1913RepositoryTest()
        {
            _f1913Repo = new F1913Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetStockDataForP910101()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var processNo = "BG18112005";
            #endregion

            var result = _f1913Repo.GetStockDataForP910101(dcCode, gupCode, custCode, processNo);
        }

        [TestMethod]
        public void GetStockData2ForP910101()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "BG18112005";
            #endregion

            var result = _f1913Repo.GetStockData2ForP910101(dcCode, gupCode, custCode, itemCode);
        }

        // 輸出有問題
        [TestMethod]
        public void GetItemStockWithoutResupply()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "BG18112005";
            var warehouseType = "G";
            #endregion

            var result = _f1913Repo.GetItemStockWithoutResupply(dcCode, gupCode, custCode, itemCode, warehouseType);
        }

        [TestMethod]
        public void GetF1913Datas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var warehouseId = "G07";
            var itemCode = "DB1001";
            var itemName = "狐獴吊飾（飲料提袋配件）";
            #endregion

            var result = _f1913Repo.GetF1913Datas(dcCode, gupCode, custCode, warehouseId,
            itemCode, itemName);
        }

        [TestMethod]
        public void GetItemStockWithVirtual()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "DB1001";
            var warehouseType = "G";
            var isForIn = true;
            #endregion

            var result = _f1913Repo.GetItemStockWithVirtual(dcCode, gupCode, custCode, itemCode, warehouseType, isForIn);
        }

        [TestMethod]
        public void GetItemWarehouseStock()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "DB1001";
            var warehouseType = "G";
            var isForIn = true;
            #endregion

            var result = _f1913Repo.GetItemWarehouseStock(dcCode, gupCode, custCode, itemCode, warehouseType, isForIn);
        }

        [TestMethod]
        public void GetItemLocStock()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "DB1001";
            var warehouseType = "G";
            var aTypeCode = "";
            var warehouseId = "";
            var isForIn = true;
            var isAllowExpiredItem = false;
            #endregion

            var result = _f1913Repo.GetItemLocStock(dcCode, gupCode, custCode, itemCode, warehouseType,
                aTypeCode, warehouseId, isForIn, isAllowExpiredItem);
        }

        [TestMethod]
        public void GetMinEnterDateByItem()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "DB1001";
            var validDate = Convert.ToDateTime("2022-06-24");
            var vnrCode = "000000";
            var serialNo = "0";
            #endregion

            var result = _f1913Repo.GetMinEnterDateByItem(dcCode, gupCode, custCode, itemCode,
            validDate, vnrCode, serialNo);
        }

        [TestMethod]
        public void FindDataByKey()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "DB1001";
            var locCode = "10H010101";
            var validDate = Convert.ToDateTime("2022-06-24");
            var enterDate = Convert.ToDateTime("2019-04-19");
            var vnrCode = "000000";
            var serialNo = "0";
            var boxCtrlNo = "SDSD0419";
            var palletCtrlNo = "0";
            var makeNo = "SDSD0419";
            #endregion

            var result = _f1913Repo.FindDataByKey(dcCode, gupCode, custCode, itemCode, locCode,
                validDate, enterDate, vnrCode, serialNo, boxCtrlNo, palletCtrlNo, makeNo);
        }

        [TestMethod]
        public void DeleteDataByKey()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "TEST";
            var locCode = "10H010101";
            var validDate = Convert.ToDateTime("2022-06-24");
            var enterDate = Convert.ToDateTime("2019-04-19");
            var vnrCode = "000000";
            var serialNo = "0";
            var boxCtrlNo = "SDSD0419";
            var palletCtrlNo = "0";
            var makeNo = "SDSD0419";
            #endregion

            _f1913Repo.DeleteDataByKey(dcCode, gupCode, custCode, itemCode, locCode,
                validDate, enterDate, vnrCode, serialNo, boxCtrlNo, palletCtrlNo, makeNo);
        }

        [TestMethod]
        public void DeleteDataByBulkDelete()
        {
            #region Params
            var f1913s = new List<F1913> {
                new F1913
                {
                    DC_CODE = "001",
                    GUP_CODE = "01",
                    CUST_CODE = "030001",
                    ITEM_CODE = "MAI005",
                    LOC_CODE  = "10A010209",
                    VALID_DATE = Convert.ToDateTime("2020-12-10 00:00:00"),
                    ENTER_DATE = Convert.ToDateTime("2019-04-19 00:00:00"),
                    VNR_CODE = "000000",
                    SERIAL_NO = "0",
                    BOX_CTRL_NO = "AA0419",
                    PALLET_CTRL_NO = "0",
                    MAKE_NO= "AA0419"
                }
            };
            #endregion

            _f1913Repo.DeleteDataByBulkDelete(f1913s);
        }

        [TestMethod]
        public void GetData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "DB1001";
            var locCode = "10H010101";
            var validDate = Convert.ToDateTime("2022-06-24");
            var enterDate = Convert.ToDateTime("2019-04-19");
            var serialNo = "0";
            var vnrCode = "000000";
            var boxCtrlNo = "SDSD0419";
            var palletCtrlNo = "0";
            var makeNo = "SDSD0419";
            #endregion

            var result = _f1913Repo.GetData(dcCode, gupCode, custCode, itemCode, locCode,
                validDate, enterDate, serialNo, vnrCode, boxCtrlNo, palletCtrlNo, makeNo);
        }


        [TestMethod]
        public void UpdateQty()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "DB1001";
            var locCode = "10H010101";
            var validDate = Convert.ToDateTime("2022-06-24");
            var enterDate = Convert.ToDateTime("2019-04-19");
            var vnrCode = "000000";
            var serialNo = "0";
            var qty = 1402;
            var boxCtrlNo = "SDSD0419";
            var palletCtrlNo = "0";
            var makeNo = "SDSD0419";
            #endregion

            _f1913Repo.UpdateQty(dcCode, gupCode, custCode, itemCode, locCode,
                    validDate, enterDate, vnrCode, serialNo, qty, boxCtrlNo, palletCtrlNo,
                    makeNo);
        }

        [TestMethod]
        public void UpdateQtyByBulkUpdate()
        {
            #region Params
            List<F1913> data = new List<F1913>
           {
               new F1913
               {
                   LOC_CODE = "10H010101",
                   ITEM_CODE = "DB1001",
                   QTY = 1400,
                   VALID_DATE = Convert.ToDateTime("2022-06-24 00:00:00"),
                   ENTER_DATE = Convert.ToDateTime("2019-04-19 00:00:00"),
                   MAKE_NO = "SDSD0419",
                   REMARK = null,
                   DC_CODE = "001",
                   GUP_CODE = "01",
                   CUST_CODE = "030001",
                   CRT_STAFF = "wms",
                   CRT_DATE = Convert.ToDateTime("2019-04-19 14:45:49"),
                   UPD_STAFF = "System",
                   UPD_DATE = Convert.ToDateTime("2020-09-16 16:47:07"),
                   CRT_NAME = "WMS",
                   UPD_NAME = "System",
                   SERIAL_NO = "0",
                   VNR_CODE = "000000",
                   BOX_CTRL_NO = "SDSD0419",
                   PALLET_CTRL_NO = "0"
               },
               new F1913
               {
                   LOC_CODE = "10H010101",
                   ITEM_CODE = "DB1001",
                   QTY = 907,
                   VALID_DATE = Convert.ToDateTime("2021-01-19 00:00:00"),
                   ENTER_DATE = Convert.ToDateTime("2019-04-19 00:00:00"),
                   MAKE_NO = "XATYT",
                   REMARK = null,
                   DC_CODE = "001",
                   GUP_CODE = "01",
                   CUST_CODE = "030001",
                   CRT_STAFF = "wms",
                   CRT_DATE = Convert.ToDateTime("2019-05-19 14:45:49"),
                   UPD_STAFF = "wms",
                   UPD_DATE = Convert.ToDateTime("2019-08-19 17:13:59"),
                   CRT_NAME = "WMS",
                   UPD_NAME = "WMS",
                   SERIAL_NO = "0",
                   VNR_CODE = "000000",
                   BOX_CTRL_NO = "XATYT",
                   PALLET_CTRL_NO = "0"
               }
           };
            #endregion

            _f1913Repo.UpdateQtyByBulkUpdate(data);
        }

        [TestMethod]
        public void MinusQty()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "DB1001";
            var locCode = "10H010101";
            var validDate = Convert.ToDateTime("2022-06-24");
            var enterDate = Convert.ToDateTime("2019-04-19");
            var vnrCode = "000000";
            var serialNo = "0";
            var minusQty = 1;
            var userId = "wms";
            var userName = "WMS";
            var boxCtrlNo = "SDSD0419";
            var palletCtrlNo = "0";
            var makeNo = "SDSD0419";
            #endregion

            _f1913Repo.MinusQty(dcCode, gupCode, custCode, itemCode, locCode,
            validDate, enterDate, vnrCode, serialNo, minusQty, userId, userName,
            boxCtrlNo, palletCtrlNo, makeNo);
        }

        [TestMethod]
        public void GetItemPickLocPriorityInfo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            List<string> itemCodes = new List<string> { "DB1001" };
            var isForIn = false;
            var warehouseType = "";
            var targetWarehouseId = "";
            var wareHouseTmpr = "";
            #endregion

            _f1913Repo.GetItemPickLocPriorityInfo(dcCode, gupCode, custCode, itemCodes, isForIn, warehouseType, targetWarehouseId, wareHouseTmpr);
        }

        [TestMethod]
        public void GetItemResupplyLocPriorityInfo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            List<string> itemCodes = new List<string> { "DB1001" };
            var isForIn = false;
            var warehouseType = "";
            var targetWarehouseId = "";
            var wareHouseTmpr = "";
            #endregion

            _f1913Repo.GetItemResupplyLocPriorityInfo(dcCode, gupCode, custCode, itemCodes, isForIn, warehouseType, targetWarehouseId, wareHouseTmpr);
        }

        [TestMethod]
        public void GetDatasByAllocationTarget()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var allocationNo = "T2018042600001";
            #endregion

            _f1913Repo.GetDatasByAllocationTarget(dcCode, gupCode, custCode, allocationNo);
        }

        [TestMethod]
        public void GetItemGoldenLocs()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "BT18111107";
            var warehouseType = "";
            var warehouseId = "";
            #endregion

            _f1913Repo.GetItemGoldenLocs(dcCode, gupCode, custCode, itemCode, warehouseType, warehouseId);
        }

        [TestMethod]
        public void GetStockQueryData1()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var typeBegin = "BT18111107";
            var typeEnd = "";
            var lTypeBegin = "";
            var lTypeEnd = "";
            var mTypeBegin = "";
            var mTypeEnd = "";
            var sTypeBegin = "";
            var sTypeEnd = "";
            var enterDateBegin = Convert.ToDateTime("1900-01-01");
            var enterDateEnd = Convert.ToDateTime("2020-01-01"); ;
            var validDateBegin = Convert.ToDateTime("1900-01-01"); ;
            var validDateEnd = Convert.ToDateTime("2020-01-01"); ;
            var locCodeBegin = "";
            var locCodeEnd = "";
            var itemCodes = new string[] { "PN18100030", "PN18100015" };
            var wareHouseIds = new string[] { "G07" };
            var boundleSerialNo = "";
            var boundleSerialLoc = "";
            var multiFlag = "";
            var packWareW = "";
            var virtualType = "";
            var expend = "1";
            var boxCtrlNoBegin = "";
            var boxCtrlNoEnd = "";
            var palletCtrlNoBegin = "";
            var palletCtrlNoEnd = "";
            var makeNo = new string[] { };
      var vnrCode = "";
            #endregion

            _f1913Repo.GetStockQueryData1(gupCode, custCode, dcCode,
                    typeBegin, typeEnd,
                    lTypeBegin, lTypeEnd, mTypeBegin, mTypeEnd, sTypeBegin, sTypeEnd,
                    enterDateBegin, enterDateEnd, validDateBegin, validDateEnd,
                    locCodeBegin, locCodeEnd, itemCodes, wareHouseIds,
                    boundleSerialNo, boundleSerialLoc, multiFlag, packWareW, virtualType,
                    expend, boxCtrlNoBegin, boxCtrlNoEnd, palletCtrlNoBegin, palletCtrlNoEnd, makeNo, vnrCode);
        }

        [TestMethod]
        public void GetStockQueryData3()
        {
            #region Params
            var gupCode = "01";
            var custCode = "030001";
            var dcCode = "001";
            var typeBegin = "";
            var typeEnd = "";
            var lTypeBegin = "";
            var lTypeEnd = "";
            var mTypeBegin = "";
            var mTypeEnd = "";
            var sTypeBegin = "";
            var sTypeEnd = "";
            var enterDateBegin = "1900-01-01";
            var enterDateEnd = "2020-01-01"; ;
            var validDateBegin = "1900-01-01";
            var validDateEnd = "2020-01-01";
            var closeDateBegin = "1900-01-01";
            var closeDateEnd = "2020-01-01";
            var itemCodes = "PN18100030";
            var wareHouseIds = new string[] { "G07" };
            var boundleSerialNo = "";
            var boundleSerialLoc = "";
            var multiFlag = "";
            var packWareW = "";
            var virtualType = "";
            var boxCtrlNoBegin = "";
            var boxCtrlNoEnd = "";
            var palletCtrlNoBegin = "";
            var palletCtrlNoEnd = "";
            #endregion

            _f1913Repo.GetStockQueryData3(gupCode, custCode, dcCode,
            typeBegin, typeEnd,
            lTypeBegin, lTypeEnd, mTypeBegin, mTypeEnd, sTypeBegin, sTypeEnd,
            enterDateBegin, enterDateEnd, validDateBegin, validDateEnd,
            closeDateBegin, closeDateEnd, itemCodes,
            boundleSerialNo, boundleSerialLoc, multiFlag, packWareW, virtualType,
            boxCtrlNoBegin, boxCtrlNoEnd, palletCtrlNoBegin, palletCtrlNoEnd);
        }


        [TestMethod]
        public void GetItemLocData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "DB1001";
            var locCdoe = "10H010102";
            #endregion

            _f1913Repo.GetItemLocData(dcCode, gupCode, custCode, itemCode, locCdoe);
        }

        [TestMethod]
        public void GetItemMixItemLoc()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "DB1001";
            #endregion

            _f1913Repo.GetItemMixItemLoc(dcCode, gupCode, custCode, itemCode);
        }
		
        [TestMethod]
    public void GetDatasByInventoryWareHouseList()
    {
      #region Params
      var dcCode = "12";
      var gupCode = "10";
      var custCode = "010001";
      var inventoryWareHouses = new List<Shared.Entities.InventoryWareHouse> { new Shared.Entities.InventoryWareHouse { WAREHOUSE_ID = "G86", AREA_CODE = "A49" } };
      var itemCodeList = new List<string> { "KK001" };
      var inventoryType = "1";
      var inventoryDate = DateTime.Now.Date.AddDays(-1);
      #endregion

      var res = _f1913Repo.GetDatasByInventoryWareHouseList(dcCode, gupCode, custCode,
       inventoryWareHouses, itemCodeList, inventoryDate);
      Trace.WriteLine(JsonConvert.SerializeObject(res));
    }


    [TestMethod]
        public void GetP710705BackWarehouseInventory()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010003";
            var vnrCode = "test";
            var account = "test";
            #endregion

            _f1913Repo.GetP710705BackWarehouseInventory(dcCode, gupCode, custCode, vnrCode, account);
        }


        [TestMethod]
        public void GetP710705MergeExecution()
        {
            #region Params
            var dcCode = "001";
            var qty = 50;
            #endregion

            _f1913Repo.GetP710705MergeExecution(dcCode, qty);
        }

        [TestMethod]
        public void GetP710705Availability()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "";
            var custCode = "";
            var inventoryDate = "";
            var account = "wms";
            #endregion

            _f1913Repo.GetP710705Availability(dcCode, gupCode, custCode, inventoryDate, account);
        }

        [TestMethod]
        public void GetP710705ChangeDetail()
        {
            #region Params
            var warehouseId = "G07";
            var startLocCode = "10H140101";
            var endLocCode = "10H140203";
            var itemCodes = "";
            var updateDateBegin = Convert.ToDateTime("2019-01-01");
            var updateDateEnd = Convert.ToDateTime("2019-12-31");
            #endregion

            _f1913Repo.GetP710705ChangeDetail(warehouseId, startLocCode, endLocCode, itemCodes, updateDateBegin, updateDateEnd);
        }

        [TestMethod]
        public void GetP710705WarehouseDetail()
        {
            #region Params
            var gupCode = "01";
            var custCode = "030001";
            var warehouseId = "G07";
            var srcLocCode = "10H310302";
            var tarLocCode = "10H340301";
            var itemCode = "CP18104004";
            var account = "ests";
            #endregion

            _f1913Repo.GetP710705WarehouseDetail(gupCode, custCode, warehouseId, srcLocCode, tarLocCode, itemCode, account);
        }

        [TestMethod]
        public void GetSchOrderData()
        {
            #region Params
            var checkDay1 = 7;
            var checkDay2 = 7;
            #endregion

            _f1913Repo.GetSchOrderData(checkDay1, checkDay2);
        }

        [TestMethod]
        public void GetSchOrderNormalData()
        {
            #region Params
            var baseDay = 90;
            #endregion

            _f1913Repo.GetSchOrderNormalData(baseDay);
        }

        [TestMethod]
        public void GetSchOrderAllData()
        {
            #region Params
            var baseDay = 90;
            #endregion

            _f1913Repo.GetSchOrderAllData(baseDay);
        }

        [TestMethod]
        public void DeleteDataByItemZeroQty()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "BOX0011";
            #endregion

            _f1913Repo.DeleteDataByItemZeroQty(dcCode, gupCode, custCode, itemCode);
        }

        [TestMethod]
        public void GetItemStock()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "BOX001";
            var warehouseType = "S";
            var isCareValid = false;
            #endregion

            _f1913Repo.GetItemStock(dcCode, gupCode, custCode, itemCode, warehouseType, isCareValid);
        }

        [TestMethod]
        public void GetP710705LocMergeExecution()
        {
            #region Params
            var dcCode = "001";
            var qty = 12;
            #endregion

            _f1913Repo.GetP710705LocMergeExecution(dcCode, qty);
        }

        [TestMethod]
        public void UpdateF1913ValidDate()
        {
            #region Params
            var listSerialNo = new List<string> { "1", "2" };
            var validDate = Convert.ToDateTime("2020-09-22");
            var gupCode = "01";
            var custCode = "020003";
            var userId = "S";
            var userName = "wms";
            #endregion

            _f1913Repo.UpdateF1913ValidDate(listSerialNo, validDate, gupCode, custCode,
            userId, userName);
        }

        [TestMethod]
        public void UpdateF1913SerialNo()
        {
            #region Params
            var oldSerialNo = "3";
            var newSerialNo = "4";
            var gupCode = "01";
            var custCode = "020003";
            var userId = "Ss";
            var userName = "wms2";
            #endregion

            _f1913Repo.UpdateF1913SerialNo(oldSerialNo, newSerialNo, gupCode, custCode,
            userId, userName);
        }

        [TestMethod]
        public void GetSettleData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var settleDate = Convert.ToDateTime("2020-09-22");
            #endregion

            _f1913Repo.GetSettleData(dcCode, gupCode, custCode, settleDate);
        }

        [TestMethod]
        public void GetDatas2()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var locCode = "10H150101";
            var itemCode = "BG18112005";
            var validDate = Convert.ToDateTime("9999-12-31");
            var enterDate = Convert.ToDateTime("2018-08-14");
            var boxCtrlNo = "0";
            var palletCtrlNo = "0";
            var makeNo = "0";
            #endregion

            _f1913Repo.GetDatas(dcCode, gupCode, custCode, locCode, itemCode,
             validDate, enterDate, boxCtrlNo, palletCtrlNo, makeNo);
        }

        //[TestMethod]
        //public void GetStocktakingAllotDatas()
        //{
        //    #region Params
        //    var dcCodes = new List<string> { "001"};
        //    var gupCode = "01";
        //    var custCode = "010001";
        //    var itemCode = new List<string> { "SRJUN01", "BB010702", "BB040101" };
        //    var locType = new List<string> { "G","N","W" };
        //    var locCode = new List<string> { "10A010110", "10A010207" };
        //    #endregion

        //    _f1913Repo.GetStocktakingAllotDatas( dcCodes, gupCode, custCode
        //    , itemCode,  locType,  locCode);
        //}

        [TestMethod]
        public void GetF1913ScrapData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            #endregion

            _f1913Repo.GetF1913ScrapData(dcCode, gupCode, custCode);
        }

        [TestMethod]
        public void GetMinEnterDate()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "CP18115004";
            var vnrCode = "000000";
            var validDate = Convert.ToDateTime("9999-12-31");
            #endregion

            _f1913Repo.GetMinEnterDate(dcCode, gupCode, custCode, itemCode,
                vnrCode, validDate);
        }

        [TestMethod]
        public void GetMinValidDate()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "CP18115004";
            var vnrCode = "000000";
            var validDate = Convert.ToDateTime("9999-12-31");
            #endregion

            _f1913Repo.GetMinValidDate(dcCode, gupCode, custCode, itemCode,
                vnrCode);
        }

        [TestMethod]
        public void GetVirtualQtyItems()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var typeId = "G";
            IEnumerable<string> itemCodes = new string[] { "BB070504" };
            #endregion

            _f1913Repo.GetVirtualQtyItems(dcCode, gupCode, custCode, typeId, itemCodes);
        }

        [TestMethod]
        public void GetDatasByItems()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCodes = new List<string> { "BOX001" };
            #endregion

            _f1913Repo.GetDatasByItems(dcCode, gupCode, custCode, itemCodes);
        }

        [TestMethod]
        public void GetDatasByLocs()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var locCodes = new List<string> { "10Q010301", "10H150101" };
            #endregion

            _f1913Repo.GetDatasByLocs(dcCode, gupCode, custCode, locCodes);
        }

        [TestMethod]
        public void GetSuggestLocsByStock()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCodes = new List<string> { "TS18102046" };
            #endregion

            _f1913Repo.GetSuggestLocsByStock(dcCode, gupCode, custCode, itemCodes);
        }

        [TestMethod]
        public void SearchStockData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var locCode = "10H090201";
            var itemCode = "TS18111016";
            var validDate = Convert.ToDateTime("9999-12-31");
            var enterDate = Convert.ToDateTime("2018-08-02");
            var boxCtrlNo = "0";
            var palletCtrlNo = "0";
            var makeNo = "0";
            var vnrCode = "000000";
            var serialNo = "0";
            #endregion

            _f1913Repo.SearchStockData(dcCode, gupCode, custCode, locCode, itemCode,
                validDate, enterDate, boxCtrlNo, palletCtrlNo, makeNo, vnrCode, serialNo);
        }

        [TestMethod]
        public void GetF1913WithF1912Qty()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "TS18113026";
            var dataTable = "F1903";
            #endregion

            _f1913Repo.GetF1913WithF1912Qty(dcCode, gupCode, custCode,
                 itemCode, dataTable);
        }

        [TestMethod]
        public void GetStockInfies()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "JD17100001";
            var locCode = "10H170101";
            var warehouseType = "G";
            var warehouseId = "G07";
            var aTypeCode = "A";
            var isForIn = false;
            var serialNos = new List<string> { "0" };
            var validDates = new List<DateTime> { Convert.ToDateTime("9999-12-31") };
            var enterDates = new List<DateTime> { Convert.ToDateTime("2018-04-26") };
            var vnrCodes = new List<string> { "000000" };
            var boxCtrlNos = new List<string> { "0" };
            var palletCtrlNos = new List<string> { "0" };
            var makeNos = new List<string> { "0" };
            var isAllowExpiredItem = false;
            #endregion

            _f1913Repo.GetStockInfies(dcCode, gupCode, custCode, itemCode,
         locCode, warehouseType, warehouseId, aTypeCode,
         isForIn, serialNos, validDates,
        enterDates, vnrCodes, boxCtrlNos,
         palletCtrlNos, makeNos, isAllowExpiredItem);
        }

        [TestMethod]
        public void UpdateF1913ValidDateAndMakeNo()
        {
            #region Params
            var f1913s = new F1913
            {
                LOC_CODE = "10Q010301",
                ITEM_CODE = "BOX001",
                QTY = 99035,
                VALID_DATE = Convert.ToDateTime("2099-12-31"),
                ENTER_DATE = Convert.ToDateTime("2018-05-03"),
                MAKE_NO = "0",
                REMARK = "0",
                DC_CODE = "001",
                GUP_CODE = "01",
                CUST_CODE = "030001",
                CRT_STAFF = "system",
                CRT_DATE = Convert.ToDateTime("2018-05-03"),
                UPD_STAFF = "wms1",
                UPD_DATE = Convert.ToDateTime("2020-02-12"),
                CRT_NAME = "system",
                UPD_NAME = "048000XY015400",
                SERIAL_NO = "0",
                VNR_CODE = "000000",
                BOX_CTRL_NO = "0",
                PALLET_CTRL_NO = "0"

            };
            var newValidDate = Convert.ToDateTime("2099-12-30");
            var makeNo = "1";
			long newQry = 1;
            #endregion

            _f1913Repo.UpdateF1913ValidDateAndMakeNo(f1913s, newValidDate, makeNo, newQry);
        }

        [TestMethod]
        public void GetStocks()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var locCodes = new List<string> { "10H270101", "10H320201", "10H280201", "10H330301" };
            var itemCodes = new List<string> { "CP18102004" };
            #endregion

            _f1913Repo.GetStocks(dcCode, gupCode, custCode, locCodes, itemCodes);
        }

        [TestMethod]
        public void GetP081301StockSumQties()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var scanItemOrLocCode = "10H010101";
            #endregion

            _f1913Repo.GetP081301StockSumQties(dcCode, gupCode, custCode, scanItemOrLocCode);
        }

        [TestMethod]
        public void GetP08130101Stocks()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var locCode = "10H140101";
            var itemCode = "BG18111005";
            #endregion

            _f1913Repo.GetP08130101Stocks(dcCode, gupCode, custCode, locCode, itemCode);
        }

        [TestMethod]
        public void GetF1913WithF1912Moveds()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var srcLocCodeS = "10A010304";
            var srcLocCodeE = "10A010305";
            var itemCode = "BB010103";
            var itemName = "酒紅";
            var srcWarehouseId = "G01";
            #endregion

            _f1913Repo.GetF1913WithF1912Moveds(dcCode, gupCode, custCode, srcLocCodeS, srcLocCodeE, itemCode, itemName, srcWarehouseId);
        }

        [TestMethod]
        public void GetStockQuerys()
        {
            #region Params
            var gupCode = "01";
            var custCode = "030001";
            var dcCodes = new List<string> { "001", "002" };
            var locCodes = new List<string> { "10H260301", "10H270101" };
            var itemCodes = new List<string> { "TS18113016" };
            #endregion

            _f1913Repo.GetStockQuerys(gupCode, custCode, dcCodes, locCodes, itemCodes);
        }

    [TestMethod]
    public void GetStockQtyByInventory()
    {
      #region Params
      var dcCodes = "12";
      var gupCode = "10";
      var custCode = "010001";
      var warehouseid = "G01";
      var itemCodes = new List<Shared.Entities.StockDataByInventoryParam>()
      {
        new Shared.Entities.StockDataByInventoryParam()
        {
          ITEM_CODE ="NEO001",
          LOC_CODE ="A01030502",
          MAKE_NO ="230118001",
          ENTER_DATE = new DateTime(2023,01,18),
          VALID_DATE = new DateTime(9999,12,31),
          BOX_CTRL_NO = "0",
          PALLET_CTRL_NO = "0"
        },
        new StockDataByInventoryParam()
        {
          ITEM_CODE ="NN007",
          LOC_CODE ="A01010105",
          MAKE_NO ="220412002",
          ENTER_DATE = new DateTime(2022,04,12),
          VALID_DATE = new DateTime(2023,12,15),
          BOX_CTRL_NO = "0",
          PALLET_CTRL_NO = "0"
        }
      };
      #endregion
      var data = new List<StockDataByInventory>();
      foreach (var item in itemCodes)
        data.AddRange(_f1913Repo.GetStockQtyByInventory(dcCodes, gupCode, custCode, warehouseid, item).ToList());
      Trace.WriteLine(JsonConvert.SerializeObject(data));

      var data2 = _f1913Repo.GetStockQtyByInventory0(dcCodes, gupCode, custCode, warehouseid, itemCodes).ToList();
      Trace.WriteLine(JsonConvert.SerializeObject(data2));

      Assert.AreEqual(JsonConvert.SerializeObject(data), JsonConvert.SerializeObject(data2));

    }

  }
}
