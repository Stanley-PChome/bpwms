using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
  [TestClass]
  public class F051202RepositoryTest : BaseRepositoryTest
  {
    private F051202Repository _f051202Repo;
    public F051202RepositoryTest()
    {
      _f051202Repo = new F051202Repository(Schemas.CoreSchema);
    }

    [TestMethod]
    public void GetF051202Datas()
    {
      #region Params
      var dcCode = "001";
      var gupCode = "01";
      var custCode = "010001";
      var delvDate = "2017-02-21";
      var pickTime = "17:14";
      var ordType = "0";
      #endregion

      _f051202Repo.GetF051202Datas(dcCode, gupCode, custCode, delvDate, pickTime, ordType);
    }

    [TestMethod]
    public void GetF051202ByOrderNonCancel()
    {
      #region Params
      var dcCode = "001";
      var gupCode = "01";
      var custCode = "010001";
      var pickOrdNo = "P2020070700001";
      var itemCode = "BB010103";
      var validDate = new DateTime(2020, 12, 8);
      var pickLoc = "10A010305";
      var serialNo = "";
      #endregion

      _f051202Repo.GetF051202ByOrderNonCancel(dcCode, gupCode, custCode, pickOrdNo, itemCode, validDate, pickLoc, serialNo);
    }

    [TestMethod]
    public void GetF051202ByOrderNonCancelAllWmsOrdNo()
    {
      #region Params
      var dcCode = "001";
      var gupCode = "01";
      var custCode = "010001";
      var pickOrdNo = "P2020070700001";
      var itemCode = "BB010103";
      #endregion

      _f051202Repo.GetF051202ByOrderNonCancelAllWmsOrdNo(dcCode, gupCode, custCode, pickOrdNo, itemCode);
    }

    [TestMethod]
    public void GetDatas()
    {
      #region Params
      var dcNo = "001";
      var gupCode = "01";
      var custNo = "030001";
      var pickOrdNo = "P2018072700006";
      #endregion

      _f051202Repo.GetDataByPickNo(dcNo, gupCode, custNo, pickOrdNo);
    }

    [TestMethod]
    public void GetNotFinishCnt()
    {
      #region Params
      var dcNo = "001";
      var gupCode = "01";
      var custNo = "030001";
      var wmsNo = "O2018050200026";
      #endregion

      _f051202Repo.GetNotFinishCnt(dcNo, gupCode, custNo, wmsNo);
    }

    [TestMethod]
    public void GetSinglePickDetail()
    {
      #region Params
      var dcNo = "001";
      var gupCode = "01";
      var custNo = "030001";
      var pickOrdNo = "P2018072700006";
      #endregion

      _f051202Repo.GetSinglePickDetail(dcNo, gupCode, custNo, pickOrdNo);
    }


    [TestMethod]
    public void GetTop1FinishData()
    {
      #region Params
      var dcNo = "001";
      var gupCode = "01";
      var custNo = "010001";
      var wmsNo = "O2017022000001";
      #endregion

      _f051202Repo.GetTop1FinishData(dcNo, gupCode, custNo, wmsNo);
    }

    [TestMethod]
    public void GetDatasByWmsOrdNosAndPickStatus1()
    {
      #region Params
      var dcNo = "001";
      var gupCode = "01";
      var custNo = "010001";
      var wmsNo = new[] { "O20230612000012", "O20230612000011", "O20230612000010", "O20230612000009", "O20230612000008", "O20230612000007", "O20230612000006", "O20230612000005", "O20230612000004", "O20230612000003" }.ToList();
      #endregion

      var res = _f051202Repo.GetDatasByWmsOrdNosAndPickStatus1(dcNo, gupCode, custNo, wmsNo).ToList();
      Trace.WriteLine(JsonSerializer.Serialize(res));
      //var res0 = _f051202Repo.GetDatasByWmsOrdNosAndPickStatus10(dcNo, gupCode, custNo, wmsNo).ToList();
      //Assert.AreEqual(JsonSerializer.Serialize(res), JsonSerializer.Serialize(res0));

    }

  }
}
