using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
    public class P810105Service
    {
        private WmsTransaction _wmsTransation;
        public P810105Service(WmsTransaction wmsTransation)
        {
            _wmsTransation = wmsTransation;

        }
    /// <summary>
    /// 庫存作業-庫存資料查詢
    /// </summary>
    /// <param name="getStockReq"></param>
    /// <returns></returns>
    public ApiResult GetStock(GetStockReq getStockReq)
    {
      var p81Service = new P81Service();
      var apiResult = new ApiResult();
      var f1912Repo = new F1912Repository(Schemas.CoreSchema);

      // 帳號檢核
      var checkAcc = p81Service.CheckAcc(getStockReq.AccNo);

      // 檢核人員功能權限
      var checkAccFunction = p81Service.CheckAccFunction(getStockReq.FuncNo, getStockReq.AccNo);

      // 檢核人員貨主權限
      var checkAccCustCode = p81Service.CheckAccCustCode(getStockReq.CustNo, getStockReq.AccNo);

      // 檢核人員物流中心權限
      var checkAccDc = p81Service.CheckAccDc(getStockReq.DcNo, getStockReq.AccNo);

      if (string.IsNullOrWhiteSpace(getStockReq.FuncNo) ||
          string.IsNullOrWhiteSpace(getStockReq.AccNo) ||
          string.IsNullOrWhiteSpace(getStockReq.DcNo) ||
          string.IsNullOrWhiteSpace(getStockReq.CustNo) ||
          checkAcc == null ||
          checkAccFunction == 0 ||
          checkAccCustCode == 0 ||
          checkAccDc == 0
          )
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

      // 取得業主編號
      var gupCode = p81Service.GetGupCode(getStockReq.CustNo);

      // 資料處裡
      //var getInventoryInfo = f1912Repo.GetInventoryInfo(getStockReq.CustNo, getStockReq.DcNo, gupCode, getStockReq.ItemNo,
      //    getStockReq.MkNo, getStockReq.Sn, getStockReq.WhNo, getStockReq.BegLoc, getStockReq.EndLoc,
      //    getStockReq.BegPalletNo, getStockReq.EndPalletNo, getStockReq.BegEnterDate, getStockReq.EndEnterDate,
      //    getStockReq.BegValidDate, getStockReq.EndValidDate);

      var getInventoryInfo = new List<GetStockRes>();

      var itemCode = new List<string>();
      string BUNDLE_SERIALLOC = null;
      string serialItemCode = null;
      if (!string.IsNullOrWhiteSpace(getStockReq.ItemNo))
      {
        var f1903Repo = new F1903Repository(Schemas.CoreSchema);
        itemCode = f1903Repo.GetDatasByBarCode(gupCode, getStockReq.CustNo, getStockReq.ItemNo).Select(x => x.ITEM_CODE).ToList();
        if (itemCode == null || !itemCode.Any())
          return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001"), Data = getInventoryInfo };

      }
      if (!string.IsNullOrWhiteSpace(getStockReq.Sn))
      {
        var f2501Repo = new F2501Repository(Schemas.CoreSchema);
        var f1903 = f2501Repo.GetF1903DataBySerialNo(gupCode, getStockReq.CustNo, getStockReq.Sn);
        //找不到這序號就直接回傳
        if (f1903 == null)
          return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001"), Data = getInventoryInfo };

        BUNDLE_SERIALLOC = f1903.BUNDLE_SERIALLOC;
        serialItemCode = f1903.ITEM_CODE;

      }
      getInventoryInfo = f1912Repo.GetInventoryInfo(getStockReq.CustNo, getStockReq.DcNo, gupCode, itemCode, getStockReq.MkNo,
     getStockReq.Sn, getStockReq.WhNo, getStockReq.BegLoc, getStockReq.EndLoc, getStockReq.BegPalletNo, getStockReq.EndPalletNo, getStockReq.BegEnterDate,
     getStockReq.EndEnterDate, getStockReq.BegValidDate, getStockReq.EndValidDate, BUNDLE_SERIALLOC, serialItemCode).ToList();


      return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001"), Data = getInventoryInfo.Where(x => x.StockQty > 0 || x.BPickQty > 0).ToList() };
    }
  }
}
