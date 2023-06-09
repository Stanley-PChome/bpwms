using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Test
{
    [TestClass]
    public class F1903RepositoryTest : BaseRepositoryTest
    {
        private F1903Repository _f1903Repo;
        public F1903RepositoryTest()
        {
            _f1903Repo = new F1903Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetRatio()
        {
            // "BG18111005","BG18112005","BOX001"
            var repo = new F1903Repository(Schemas.CoreSchema);
            var r = repo.GetRatio("BG18111005", "01", "030001", "");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetItemMixItemLoc()
        {
            //LOCCODE
            //10A010101
            //10A010102
            //10A010103
            //10A010104
            var repo = new F1903Repository(Schemas.CoreSchema);
            var r = repo.GetItemMixItemLoc("001", "01", "030002", "10A010102", "10A010101");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetItemMixBatchLoc()
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            var r = repo.GetItemMixBatchLoc("001", "01", "030002", "TMACAAB00200001", "10A010102", null);
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetItemInfo()
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            var r = repo.GetItemInfo("01", "010001", "BB010140O");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetDatas()
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            var r = repo.GetDatas("01", "TMACAAB00200001", "010002");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetRecvSettleQty()
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            //2019/1/4
            //2019/1/4
            //2018/8/31
            var r = repo.GetRecvSettleQty("001", "01", "030002", DateTime.Parse("2019/1/4"));
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetDeliverySettleQty()
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            //2019/1/4
            //2019/1/4
            //2018/8/31
            var r = repo.GetDeliverySettleQty("001", "01", "030002", DateTime.Parse("2019/1/4"));
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetReturnSettleQty()
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            var r = repo.GetReturnSettleQty("001", "01", "030002", DateTime.Parse("2019/1/4"));
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetMoveOutSettleQty()
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            var r = repo.GetMoveOutSettleQty("001", "01", "030002", DateTime.Parse("2019/1/4"));
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetMoveInSettleQty()
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            var r = repo.GetMoveInSettleQty("001", "01","030002",DateTime.Now);
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void DeleteAllCustData()
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            repo.DeleteAllCustData("01", "030002");
        }
        
        
        [TestMethod]
        public void GetRetailDeliverDetailReport()
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            var r = repo.GetRetailDeliverDetailReport("001", "01", "030002", "", new System.Collections.Generic.List<string>() { "" });
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetRetailDeliverDetailReportByRetail()
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            var r = repo.GetRetailDeliverDetailReportByRetail("001", "01", "030002", "", DateTime.Now);
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetRetailDeliverDetailReportByRetailIntact()
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            var r = repo.GetRetailDeliverDetailReportByRetailIntact("001", "01", "030002",DateTime.Now,"","","");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetItemBarcodes()
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            var r = repo.GetItemBarcodes("01", "030002");
            Trace.Write(JsonSerializer.Serialize(r));
        }

        [TestMethod]
        public void GetTmprTypeData()
        {
            #region Params
            var gupCode = "01";
            var itemCode = "BOX001";
            #endregion

            _f1903Repo.GetTmprTypeData(gupCode, itemCode);
        }

        [TestMethod]
        public void GetDataPprocessing1()
        {
            #region Params
            var cust_code = "010001";
            var gup_code = "01";
            var upd_date = Convert.ToDateTime("2017-05-15 15:36:55");
            var rownum = 3;
            #endregion

            _f1903Repo.GetDataPprocessing1(cust_code, gup_code, upd_date, rownum);
        }

        [TestMethod]
        public void GetDataPprocessing2()
        {
            #region Params
            var cust_code = "030001";
            var gup_code = "01";
            var itemNoList = new string[] { "TS18111036", "TS18111046" };
            #endregion

            _f1903Repo.GetDataPprocessing2(cust_code, gup_code, itemNoList);
        }

        [TestMethod]
        public void GetItemName()
        {
            #region Params
            var gupCode = "01";
            var custCode = "030001";
            var itemCode = "BG18111005";
            #endregion

            _f1903Repo.GetItemName(gupCode, custCode, itemCode);
        }

        [TestMethod]
        public void GetF1903Tmpr()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var itemCode = "BB010101";
            #endregion

            _f1903Repo.GetF1903Tmpr(itemCode, custCode, gupCode);
        }

        #region 原F1902Repository 移到 F1903RRepository
        [TestMethod]
        public void GetF1903WithF1915()
        {
            #region Params
            var gupCode = "01";
            var itemCode = "BOX001";
            var custCode = "010001";
            #endregion

            _f1903Repo.GetF1903WithF1915(gupCode, itemCode, custCode);
        }

        [TestMethod]
        public void GetSameItems()
        {
            #region Params
            var gupCode = "01";
            var itemCode = "HLIN-QY03";
            var custCode = "010001";
            var notInItemCodes = new  List<string>{ "BJCB-A900APDNP", "HLIN-QY02" };
            #endregion

            _f1903Repo.GetSameItems(gupCode, itemCode,custCode, notInItemCodes);
        }

        [TestMethod]
        public void GetItemsByWmsOrdNo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNo = "O2017022000001";
            #endregion

            _f1903Repo.GetItemsByWmsOrdNo(dcCode, gupCode, custCode, wmsOrdNo);
        }

        [TestMethod]
        public void GetF1903sByCarton()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var isCarton = "O2017022000001";
            #endregion

            _f1903Repo.GetF1903sByCarton(gupCode, custCode, isCarton);
        }


        [TestMethod]
        public void GetSameItemWithBom()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var itemCode = "BB010406";
            #endregion

            _f1903Repo.GetSameItemWithBom(gupCode, custCode, itemCode);
        }

        [TestMethod]
        public void GetF1903ByItemCodeOrEanCode()
        {
            #region Params
            var gupCode = "01";
            var itemCodeOrEanCode = "ISMKPPSU0101";
            var custCode = "010001";
            #endregion

            _f1903Repo.GetF1903ByItemCodeOrEanCode(gupCode, itemCodeOrEanCode, custCode);
        }

        [TestMethod]
        public void GetByItemCodeOrEanCode()
        {
            #region Params
            var gupCode = "01";
            var itemCodeOrEanCodes = new List<string> { "ISMKPPSU0101" };
            #endregion

            _f1903Repo.GetByItemCodeOrEanCode(gupCode, itemCodeOrEanCodes);
        }

        [TestMethod]
        public void GetCartonItem()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var searchCode = "BOX001";
            #endregion

            _f1903Repo.GetCartonItem(gupCode, custCode, searchCode);
        }

        [TestMethod]
        public void GetF1912s()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var itemCode = "BOX001";
            var itemName = "紙";
            #endregion

            _f1903Repo.GetF1912s(gupCode, custCode, itemCode, itemName);
        }

       

        [TestMethod]
        public void GetF1903sByItemName()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var itemName = "頂級";
            var itemSearchMaximum = 5;
            #endregion

            _f1903Repo.GetF1903sByItemName(gupCode, custCode, itemName, itemSearchMaximum);
        }

        [TestMethod]
        public void GetF1903sByItemCode()
        {
            #region Params
            var gupCode = "01";
            var itemCode = "BB010101";
            var custCode = "010001";
            var account = "wms";
            #endregion

            _f1903Repo.GetF1903sByItemCode(gupCode, itemCode, custCode, account);
        }

        [TestMethod]
        public void GetDatasByItems()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var itemCodes = new List<string> { "BB010102", "BB010103" };
            #endregion

            _f1903Repo.GetDatasByItems(gupCode, custCode, itemCodes);
        }

        [TestMethod]
        public void GetDatasByEanCodes()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var eanCodes = new List<string> { "8809323132281", "8809323132304" };
            #endregion

            _f1903Repo.GetDatasByEanCodes(gupCode, custCode, eanCodes);
        }

        [TestMethod]
        public void IsExits()
        {
            #region Params
            var gupCode = "01";
            var custCode = "030001";
            var lType = "DN";
            var mType = "DNA";
            var sType = "DNA01";
            #endregion

            _f1903Repo.IsExits(gupCode, custCode, lType, mType, sType);
        }

        [TestMethod]
        public void GetF1912s1()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var itemCodes = new string[] { "ISMKPPSU0102", "ISMKPPSU0103", "ISMKPPSU0201" };
            var itemName = "頂級";
            var itemSpec = "";
            var lType = "SHT02";
            #endregion

            _f1903Repo.GetF1912s1(gupCode, custCode, itemCodes, itemName, itemSpec, lType);
        }
        #endregion
    }
}
