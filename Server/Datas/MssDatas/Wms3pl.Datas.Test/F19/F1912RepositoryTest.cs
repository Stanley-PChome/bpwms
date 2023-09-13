using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Entities;
using System.Linq;
using Wms3pl.Datas.Shared.Pda.Entitues;

namespace Wms3pl.Datas.Test
{
    [TestClass]
    public class F1912RepositoryTest : BaseRepositoryTest
    {
        private F1912Repository _f1912Repo;
        public F1912RepositoryTest()
        {
            _f1912Repo = new F1912Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetAssignedLoc()
        {
            // "BG18111005","BG18112005","BOX001"
            var repo = new F1912Repository(Schemas.CoreSchema);
            var r = repo.GetAssignedLoc("", "01");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetUnAssignedLoc()
        {
            //LOCCODE
            //10A010101
            //10A010102
            //10A010103
            //10A010104
            var repo = new F1912Repository(Schemas.CoreSchema);
            var r = repo.GetUnAssignedLoc("001", "01","", "", "10A010101","10A010102");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetLocListForLocControl()
        {
            var repo = new F1912Repository(Schemas.CoreSchema);
            var r = repo.GetLocListForLocControl("001", "01", "030002", "", "", "G","","","");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetLocCodesByWarehouseType()
        {
            string[] aa = { "123" };
            var repo = new F1912Repository(Schemas.CoreSchema);
            var r = repo.GetLocCodesByWarehouseType("001", "01", "030002", "G01", new System.Collections.Generic.List<string>() { "1000207" });
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetCustWarehouseDatas()
        {
            var repo = new F1912Repository(Schemas.CoreSchema);
            var r = repo.GetCustWarehouseDatas("001", "01", "010001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetDatas()
        {
            var repo = new F1912Repository(Schemas.CoreSchema);
            var r = repo.GetDatas(new System.Collections.Generic.List<string>() { "01" }, new System.Collections.Generic.List<string>() { "" });
            var r2 = repo.GetDatas("001", "01", "010001");
            Trace.Write(JsonSerializer.Serialize(r));
            Trace.Write(JsonSerializer.Serialize(r2));
        }
        [TestMethod]
        public void DeleteLocByLocCode()
        {
            var repo = new F1912Repository(Schemas.CoreSchema);
            //給予錯誤資料避免蓁刪除
            repo.DeleteLocByLocCode("A001","A01","A010002","AG01",new System.Collections.Generic.List<string>() { ""});
            //Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetDcWmsNoLocTypeItems()
        {
            var repo = new F1912Repository(Schemas.CoreSchema);
            var r = repo.GetDcWmsNoLocTypeItems("001", "01", "030020");
            Trace.Write(JsonSerializer.Serialize(r));
        }

        [TestMethod]
        public void GetF1912Datas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var owCustCode = "030002";
            var locCode = "10C020404";
            #endregion

            _f1912Repo.GetF1912Datas(dcCode, gupCode, owCustCode, locCode);
        }

        [TestMethod]
        public void GetTmprTypeData()
        {
            #region Params
            var dcCode = "001";
            var locCode = "10C020404";
            #endregion

            _f1912Repo.GetTmprTypeData(dcCode, locCode);
        }

    [TestMethod]
    public void GetInventoryInfo()
    {
      #region Params
      var custNo = "010001";
      var dcNo = "12";
      var gupCode = "10";
      var itemNo = "";
      var mkNo = "";
      var sn = "0223042133";
      var whNo = "";
      var begLoc = "";
      var endLoc = "";
      var begPalletNo = "";
      var endPalletNo = "";
      DateTime? begEnterDate = null;
      DateTime? endEnterDate = null;
      DateTime? begValidDate = null;
      DateTime? endValidDate = null;
      #endregion

      var newRes = new List<GetStockRes>();
      var itemCode = new List<string>();
      string BUNDLE_SERIALLOC = null;
      string serialItemCode = null;
      if (!string.IsNullOrWhiteSpace(itemNo))
      {
        var f1903Repo = new F1903Repository(Schemas.CoreSchema);
        itemCode = f1903Repo.GetDatasByBarCode(gupCode, custNo, itemNo).Select(x => x.ITEM_CODE).ToList();
      }
      if (!string.IsNullOrWhiteSpace(sn))
      {
        var f2501Repo = new F2501Repository(Schemas.CoreSchema);
        var f1903 = f2501Repo.GetF1903DataBySerialNo(gupCode, custNo, sn);
        if (f1903 == null)
          return;
        BUNDLE_SERIALLOC = f1903.BUNDLE_SERIALLOC;
        serialItemCode = f1903.ITEM_CODE;
      }
      newRes = _f1912Repo.GetInventoryInfo(custNo, dcNo, gupCode, itemCode, mkNo,
     sn, whNo, begLoc, endLoc, begPalletNo, endPalletNo, begEnterDate,
     endEnterDate, begValidDate, endValidDate, BUNDLE_SERIALLOC, serialItemCode).OrderBy(x => x.Loc).ThenBy(x => x.MkNo).ThenBy(x => x.Sn).ToList();

      //Console.WriteLine(JsonSerializer.Serialize(oldRes));
      Console.WriteLine(JsonSerializer.Serialize(newRes));

    }

    [TestMethod]
        public void GetMoveLocRes()
        {
            #region Params
            var dcCode = "001";
            var loc = "10C020404";
            var custNo = "030002";
            var gupCode = "01";
            var itemNo = "CAPIE024904";

            #endregion

            _f1912Repo.GetMoveLocRes(dcCode, loc, custNo, gupCode, itemNo);
        }

    [TestMethod]
    public void GetMoveItemLocRes()
    {
      #region Params
      var dcCode = "001";
      var loc = "10H020201";
      var custNo = "030001";
      var gupCode = "01";
      var itemNo = "BT18111107";
      var sn = "";

      #endregion


    }

        [TestMethod]
        public void CheckLocExist()
        {
            #region Params
            var dcCode = "001";

            #endregion
            //J2017022300003
            var r = _f1912Repo.CheckLocExist(dcCode);
            Trace.Write(JsonSerializer.Serialize(r));
            
        }

        [TestMethod]
        public void CheckCustCodeLoc()
        {
            #region Params
            var dcCode = "001";
            var locCod = "10C020404";
            #endregion
            //J2017022300003
            var r = _f1912Repo.CheckCustCodeLoc(dcCode, locCod);
            Trace.Write(JsonSerializer.Serialize(r));
        }

        [TestMethod]
        public void GetF1912Tmpr()
        {
            #region Params
            var dcCode = "001";
            var locCod = "10C020404";
            #endregion

            _f1912Repo.GetF1912Tmpr(dcCode, locCod);
        }

        [TestMethod]
        public void GetLocListForLocControlByItemCode()
        {
            #region Params
            var dcCode = "001";
            var locCod = "10C020404";
            #endregion

            var r = _f1912Repo.GetLocListForLocControlByItemCode("001", "01", "030002", "TMACAAB00200012", "");
            System.Diagnostics.Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetLocStatisticForLocControl()
        {
            #region Params
            var dcCode = "001";
            var locCod = "10C020404";
            #endregion

            var r = _f1912Repo.GetLocStatisticForLocControl("001", "01", "010001", "", "","");
            System.Diagnostics.Trace.Write(JsonSerializer.Serialize(r));
        }

    [TestMethod]
    public void GetDatasByLocCodes()
    {
      #region Params
      var dcCode = "12";
      var gupCode = "10";
      var custCode = "010001";
      var locCod =new List<string>() { "A01010101", "A01010102", "A01010103", "A01010104", "A01010105", "A01010106", "A01010107", "A01010108", "A01010109", "A01010110" };
      #endregion

      var r = _f1912Repo.GetDatasByLocCodes(dcCode,gupCode,custCode,locCod);
      System.Diagnostics.Trace.WriteLine(JsonSerializer.Serialize(r));

      //var r1 = _f1912Repo.GetDatasByLocCodes0(dcCode, gupCode, custCode, locCod);
      //System.Diagnostics.Trace.WriteLine(JsonSerializer.Serialize(r1));

      //Assert.AreEqual(JsonSerializer.Serialize(r), JsonSerializer.Serialize(r1));
    }

    [TestMethod]
    public void GetDatasByLocCodes0()
    {
      #region Params
      var dcCode = "12";
      var locCod = new List<string>() { "A01010101", "A01010102", "A01010103", "A01010104", "A01010105", "A01010106", "A01010107", "A01010108", "A01010109", "A01010110" };
      #endregion

      var r = _f1912Repo.GetDatasByLocCodes(dcCode, locCod);
      System.Diagnostics.Trace.WriteLine(JsonSerializer.Serialize(r));

      //var r1 = _f1912Repo.GetDatasByLocCodes0(dcCode, locCod);
      //System.Diagnostics.Trace.WriteLine(JsonSerializer.Serialize(r1));

      //Assert.AreEqual(JsonSerializer.Serialize(r), JsonSerializer.Serialize(r1));
    }

  }
}
