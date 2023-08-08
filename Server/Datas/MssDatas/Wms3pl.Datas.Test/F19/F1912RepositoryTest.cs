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
            var custNo = "020001";
            var dcNo = "001";
            var gupCode = "01";
            var itemNo = "1B6C0021E";
            var mkNo = "0";
            var sn = "0";
            var whNo = "G09";
            var begLoc = "20A030402";
            var endLoc = "20A050401";
            var begPalletNo = "0";
            var endPalletNo = "2";
            var begEnterDate = Convert.ToDateTime("2018-07-01");
            var endEnterDate = Convert.ToDateTime("2018-07-20");
            var begValidDate = Convert.ToDateTime("1990-10-02");
            var endValidDate = Convert.ToDateTime("9999-12-31");
            #endregion

            _f1912Repo.GetInventoryInfo(custNo, dcNo, gupCode, itemNo, mkNo,
            sn, whNo, begLoc, endLoc, begPalletNo, endPalletNo, begEnterDate,
            endEnterDate, begValidDate, endValidDate);
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

            _f1912Repo.GetMoveItemLocRes(dcCode, loc, custNo, gupCode, itemNo, sn);
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
