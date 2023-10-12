using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P08.Services;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
    public class P810108Service
    {
        private WmsTransaction _wmsTransation;
        public P810108Service(WmsTransaction wmsTransation)
        {
            _wmsTransation = wmsTransation;
        }

        /// <summary>
        /// 搬移作業-查詢
        /// </summary>
        /// <param name="req"></param>
        /// <param name="gupCode"></param>
        /// <returns></returns>
        public ApiResult GetMove(GetMoveReq req, string gupCode)
        {
            var f1903Repo = new F1903Repository(Schemas.CoreSchema);
            var f1903s = f1903Repo.GetDatasByBarCode(gupCode, req.CustNo, req.Barcode);
            if (f1903s.Any() || !string.IsNullOrWhiteSpace(req.Sn))
            {
                var moveItemLocRes = GetMoveItemLoc(new GetMoveItemLocReq { AccNo = req.AccNo, CustNo = req.CustNo, DcNo = req.DcNo, FuncNo = req.FuncNo, ItemNo = req.Barcode, Sn = req.Sn }, gupCode);
                return new ApiResult
                {
                    IsSuccessed = moveItemLocRes.IsSuccessed,
                    MsgCode = moveItemLocRes.MsgCode,
                    MsgContent = moveItemLocRes.MsgContent,
                    Data = new
                    {
                        Type = "01",
                        Data = moveItemLocRes.Data
                    }
                };
            }
            else
            {
                var moveLocRes = GetMoveLoc(new GetMoveLocReq { AccNo = req.AccNo, CustNo = req.CustNo, DcNo = req.DcNo, FuncNo = req.FuncNo, Loc = req.Barcode }, gupCode);
                return new ApiResult
                {
                    IsSuccessed = moveLocRes.IsSuccessed,
                    MsgCode = moveLocRes.MsgCode,
                    MsgContent = moveLocRes.MsgContent,
                    Data = new
                    {
                        Type = "02",
                        Data = moveLocRes.Data
                    }
                };
            }
        }

        /// <summary>
        /// 搬移作業-儲位查詢
        /// </summary>
        /// <param name="getMoveLoc"></param>
        /// <returns></returns>
        public ApiResult GetMoveLoc(GetMoveLocReq getMoveLoc, string gupCode)
        {
            var p81Service = new P81Service();
            var f1912Repo = new F1912Repository(Schemas.CoreSchema);
            var f1980Repo = new F1980Repository(Schemas.CoreSchema);

            // 傳入參數轉大寫
            if (!string.IsNullOrWhiteSpace(getMoveLoc.ItemNo))
                getMoveLoc.ItemNo = getMoveLoc.ItemNo.ToUpper();
            if (!string.IsNullOrWhiteSpace(getMoveLoc.Loc))
                getMoveLoc.Loc = getMoveLoc.Loc.ToUpper();

            // 帳號檢核
            var checkAcc = p81Service.CheckAcc(getMoveLoc.AccNo).Count();

            // 檢核人員功能權限
            var checkAccFunction = p81Service.CheckAccFunction(getMoveLoc.FuncNo, getMoveLoc.AccNo);

            // 檢核人員貨主權限
            var checkAccCustCode = p81Service.CheckAccCustCode(getMoveLoc.CustNo, getMoveLoc.AccNo);

            // 檢核人員物流中心權限
            var checkAccDc = p81Service.CheckAccDc(getMoveLoc.DcNo, getMoveLoc.AccNo);

            // 查儲位是否存在於F1912
            var f1912 = f1912Repo.Find(x => x.DC_CODE == getMoveLoc.DcNo && x.LOC_CODE == getMoveLoc.Loc);

            // 以上有錯誤，則回傳失敗訊息代碼 &取得訊息內容[20069](傳入的參數驗證失敗)
            if (string.IsNullOrEmpty(getMoveLoc.FuncNo) ||
                    string.IsNullOrWhiteSpace(getMoveLoc.AccNo) ||
                    string.IsNullOrWhiteSpace(getMoveLoc.DcNo) ||
                    string.IsNullOrWhiteSpace(getMoveLoc.CustNo) ||
                    string.IsNullOrWhiteSpace(getMoveLoc.Loc) ||
                    checkAcc == 0 ||
                    checkAccFunction == 0 ||
                    checkAccCustCode == 0 ||
                    f1912 == null)
                return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

            if (f1912.AREA_CODE.Trim() == "-1")
                return new ApiResult { IsSuccessed = false, MsgCode = "20759", MsgContent = string.Format(p81Service.GetMsg("20759"), getMoveLoc.Loc) };

            if (f1912.NOW_STATUS_ID == "03" || f1912.NOW_STATUS_ID == "04")
                return new ApiResult { IsSuccessed = false, MsgCode = "20760", MsgContent = string.Format(p81Service.GetMsg("20760"), getMoveLoc.Loc) };

            // 若此為自動倉的儲位，請提示人員"此為自動倉儲位，請刷入品號"
            if (string.IsNullOrWhiteSpace(getMoveLoc.ItemNo))
            {
                var f1980 = f1980Repo.GetF1980ByLocCode(getMoveLoc.DcNo, getMoveLoc.Loc);
                if (f1980 != null && !string.IsNullOrEmpty(f1980.DEVICE_TYPE) && f1980.DEVICE_TYPE != "0")
                    return new ApiResult { IsSuccessed = false, MsgCode = "20757", MsgContent = p81Service.GetMsg("20757") };
            }

            return new ApiResult
            {
                IsSuccessed = true,
                MsgCode = "10001",
                MsgContent = p81Service.GetMsg("10001"),
                Data = f1912Repo.GetMoveLocRes(getMoveLoc.DcNo, getMoveLoc.Loc, getMoveLoc.CustNo, gupCode, getMoveLoc.ItemNo)
            };
        }

    /// <summary>
    /// 搬移作業-商品儲位查詢
    /// </summary>
    /// <param name="getMoveItemLoc"></param>
    /// <returns></returns>
    private ApiResult GetMoveItemLoc(GetMoveItemLocReq getMoveItemLoc, string gupCode)
    {
      var p81Service = new P81Service();
      var f1903Repo = new F1903Repository(Schemas.CoreSchema);
      var f1912Repo = new F1912Repository(Schemas.CoreSchema);
      var f2501Repo = new F2501Repository(Schemas.CoreSchema);
      var commonService = new CommonService();
      // 傳入參數轉大寫
      if (!string.IsNullOrWhiteSpace(getMoveItemLoc.ItemNo))
        getMoveItemLoc.ItemNo = getMoveItemLoc.ItemNo.ToUpper();
      if (!string.IsNullOrWhiteSpace(getMoveItemLoc.Loc))
        getMoveItemLoc.Loc = getMoveItemLoc.Loc.ToUpper();
      if (!string.IsNullOrWhiteSpace(getMoveItemLoc.Sn))
        getMoveItemLoc.Sn = getMoveItemLoc.Sn.ToUpper();

      // 帳號檢核
      var checkAcc = p81Service.CheckAcc(getMoveItemLoc.AccNo).Count();

      // 檢核人員功能權限
      var checkAccFunction = p81Service.CheckAccFunction(getMoveItemLoc.FuncNo, getMoveItemLoc.AccNo);

      // 檢核人員貨主權限
      var checkAccCustCode = p81Service.CheckAccCustCode(getMoveItemLoc.CustNo, getMoveItemLoc.AccNo);

      // 檢核人員物流中心權限
      var checkAccDc = p81Service.CheckAccDc(getMoveItemLoc.DcNo, getMoveItemLoc.AccNo);

      // 取得商品品號
      var f1903 = p81Service.GetItemCode(getMoveItemLoc.CustNo, getMoveItemLoc.ItemNo);

      // 確認品號與儲位至少有一個欄位有值
      // todo neo:這個F1912的查詢是不是可以拉掉？ 看起來不可能查到資料
      var f1912 = f1912Repo.Find(x => x.DC_CODE == getMoveItemLoc.DcNo && x.LOC_CODE == getMoveItemLoc.Loc);
      if (string.IsNullOrWhiteSpace(getMoveItemLoc.FuncNo) ||
              string.IsNullOrWhiteSpace(getMoveItemLoc.AccNo) ||
              string.IsNullOrWhiteSpace(getMoveItemLoc.DcNo) ||
              string.IsNullOrWhiteSpace(getMoveItemLoc.CustNo) ||
              checkAcc == 0 ||
              checkAccFunction == 0 ||
              checkAccCustCode == 0 ||
              checkAccDc == 0 ||
              !(f1903 != null || f1912 != null))
        return new ApiResult { IsSuccessed = true, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

      CheckSerialTypeEn serialType;
      if (!string.IsNullOrWhiteSpace(getMoveItemLoc.Sn))
      {
        serialType = f2501Repo.CheckSerialType(gupCode, getMoveItemLoc.CustNo, getMoveItemLoc.Sn);
        serialType = serialType == null ? serialType = new CheckSerialTypeEn() : serialType;
      }
      else
        serialType = new CheckSerialTypeEn() { SerialNoType = "2" };

      var itemCodes = new List<string>();
      if (!string.IsNullOrWhiteSpace(getMoveItemLoc.ItemNo))
        itemCodes = f1903Repo.GetItemByCondition(gupCode, getMoveItemLoc.CustNo, getMoveItemLoc.ItemNo).Select(x => x.ITEM_CODE).ToList();

      var data = f1912Repo.GetMoveItemLocRes(getMoveItemLoc.DcNo, getMoveItemLoc.Loc, getMoveItemLoc.CustNo, gupCode, itemCodes, getMoveItemLoc.Sn, serialType);

      //var data = f1912Repo.GetMoveItemLocRes(getMoveItemLoc.DcNo, getMoveItemLoc.Loc, getMoveItemLoc.CustNo, gupCode, getMoveItemLoc.ItemNo, getMoveItemLoc.Sn);
      return new ApiResult
      {
        IsSuccessed = true,
        MsgCode = "10001",
        MsgContent = p81Service.GetMsg("10001"),
        Data = data
      };
    }

    public ApiResult PostMoveConfirm(PostMoveConfirmReq postMoveConfirmReq)
        {
            var p81Service = new P81Service();
            var gupCode = p81Service.GetGupCode(postMoveConfirmReq.CustNo);
            var f1912Repo = new F1912Repository(Schemas.CoreSchema);
            var f1913Repo = new F1913Repository(Schemas.CoreSchema);
            var f1980Repo = new F1980Repository(Schemas.CoreSchema);
            var sharedService = new SharedService();
            var p08130101StockList = new List<P08130101Stock>();
            var p081301Service = new P081301Service(_wmsTransation);
            var checkItemTarLocMixLocList = new List<CheckItemTarLocMixLoc>();
            DateTime dateTime;
            var apiResult = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001") };

            // 傳入參數轉大寫
            if (!string.IsNullOrWhiteSpace(postMoveConfirmReq.SrcLoc))
                postMoveConfirmReq.SrcLoc = postMoveConfirmReq.SrcLoc.ToUpper();
            if (!string.IsNullOrWhiteSpace(postMoveConfirmReq.TarLoc))
                postMoveConfirmReq.TarLoc = postMoveConfirmReq.TarLoc.ToUpper();

            // 帳號檢核
            var checkAcc = p81Service.CheckAcc(postMoveConfirmReq.AccNo).Count();

            // 檢核人員功能權限
            var checkAccFunction = p81Service.CheckAccFunction(postMoveConfirmReq.FuncNo, postMoveConfirmReq.AccNo);

            // 檢核人員貨主權限
            var checkAccCustCode = p81Service.CheckAccCustCode(postMoveConfirmReq.CustNo, postMoveConfirmReq.AccNo);

            // 檢核人員物流中心權限
            var checkAccDc = p81Service.CheckAccDc(postMoveConfirmReq.DcNo, postMoveConfirmReq.AccNo);

            if (string.IsNullOrWhiteSpace(postMoveConfirmReq.FuncNo) ||
                    string.IsNullOrWhiteSpace(postMoveConfirmReq.AccNo) ||
                    string.IsNullOrWhiteSpace(postMoveConfirmReq.DcNo) ||
                    string.IsNullOrWhiteSpace(postMoveConfirmReq.CustNo) ||
                    string.IsNullOrWhiteSpace(postMoveConfirmReq.SrcLoc) ||
                    string.IsNullOrWhiteSpace(postMoveConfirmReq.TarLoc) ||
                    postMoveConfirmReq.Items.TrueForAll(x => string.IsNullOrWhiteSpace(x.ItemNo)) ||
                    postMoveConfirmReq.Items.TrueForAll(x => string.IsNullOrWhiteSpace(x.ValidDate)) ||
                    postMoveConfirmReq.Items.TrueForAll(x => string.IsNullOrWhiteSpace(x.EnterDate)) ||
                    checkAcc == 0 ||
                    checkAccFunction == 0 ||
                    checkAccCustCode == 0 ||
                    checkAccDc == 0)
                apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

            // 檢核儲位權限
            var checkLocCode = p81Service.CheckLocCode(postMoveConfirmReq.DcNo, postMoveConfirmReq.AccNo, postMoveConfirmReq.CustNo, postMoveConfirmReq.TarLoc, "2");
            if (apiResult.IsSuccessed && !checkLocCode.IsSuccessed)
                apiResult = new ApiResult { IsSuccessed = false, MsgCode = checkLocCode.MsgCode, MsgContent = checkLocCode.MsgContent };

            //檢核是否為人工倉
            if (apiResult.IsSuccessed)
            {
                var isArtificialWarehouse = f1980Repo.GetF1980ByLocCode(postMoveConfirmReq.DcNo, postMoveConfirmReq.TarLoc);
                if (isArtificialWarehouse != null)
                {
                    if (!string.IsNullOrEmpty(isArtificialWarehouse.DEVICE_TYPE) && isArtificialWarehouse.DEVICE_TYPE != "0")
                        apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20756", MsgContent = string.Format(p81Service.GetMsg("20756"), "目的倉不可為自動倉") };
                }
                else
                {
                    apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20755", MsgContent = string.Format(p81Service.GetMsg("20755"), postMoveConfirmReq.TarLoc) };
                }
            }

            // 檢核來源儲位是否無儲區
            var f1912BySrcLoc = f1912Repo.Find(x => x.DC_CODE == postMoveConfirmReq.DcNo && x.LOC_CODE == postMoveConfirmReq.SrcLoc);
            if (apiResult.IsSuccessed && f1912BySrcLoc.AREA_CODE.Trim() == "-1")
                apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20759", MsgContent = string.Format(p81Service.GetMsg("20759"), postMoveConfirmReq.SrcLoc) };

            // 檢核來源儲位是否凍結出
            if (apiResult.IsSuccessed && f1912BySrcLoc.NOW_STATUS_ID == "03" || f1912BySrcLoc.NOW_STATUS_ID == "04")
                apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20760", MsgContent = string.Format(p81Service.GetMsg("20760"), postMoveConfirmReq.SrcLoc) };
            
            // 上架儲位若沒有設定儲區(F1912.area_code=-1)，不可以上架
            if (apiResult.IsSuccessed)
            {
                var f1912 = f1912Repo.Find(o => o.DC_CODE == postMoveConfirmReq.DcNo && o.LOC_CODE == postMoveConfirmReq.TarLoc);
                if (f1912 != null && f1912.AREA_CODE.Trim() == "-1")
                    apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20758", MsgContent = string.Format(p81Service.GetMsg("20758"), postMoveConfirmReq.TarLoc) };
            }

            if (apiResult.IsSuccessed)
            {
                foreach (var item in postMoveConfirmReq.Items)
                {
                    item.Sn = string.IsNullOrWhiteSpace(item.Sn) ? "0" : item.Sn;

                    DateTime validate = Convert.ToDateTime(item.ValidDate);
                    DateTime enterDate = Convert.ToDateTime(item.EnterDate);
                    var f1913 = f1913Repo.Find(x => x.DC_CODE == postMoveConfirmReq.DcNo && x.CUST_CODE == postMoveConfirmReq.CustNo
                    && x.ITEM_CODE == item.ItemNo
                    && x.LOC_CODE == postMoveConfirmReq.SrcLoc
                    && x.VALID_DATE == validate
                    && x.BOX_CTRL_NO == item.BoxNo
                    && x.PALLET_CTRL_NO == item.PalletNo
                    && x.MAKE_NO == item.MkNo
                    && x.SERIAL_NO == item.Sn
                    && x.ENTER_DATE == enterDate);

                    if (apiResult.IsSuccessed && f1913 == null)
                    {
                        // 查無庫存資料
                        apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20754", MsgContent = string.Format(p81Service.GetMsg("20754"), "0", item.MoveQty.ToString()) };
                    }
                    else
                    {
                        // 若 MoveQty > qty
                        if (apiResult.IsSuccessed && (f1913.QTY < item.MoveQty))
                            apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20754", MsgContent = p81Service.GetMsg("20754") };
                    }

                    // 檢核儲位的溫層
                    var checkLocTmpr = p81Service.CheckLocTmpr(postMoveConfirmReq.DcNo, item.ItemNo, postMoveConfirmReq.CustNo, postMoveConfirmReq.TarLoc, gupCode);
                    if (apiResult.IsSuccessed && !checkLocTmpr.IsSuccessed)
                        apiResult = new ApiResult { IsSuccessed = false, MsgCode = checkLocTmpr.MsgCode, MsgContent = checkLocTmpr.MsgContent };

                    if (apiResult.IsSuccessed)
                    {
                        // 建立資料 List<P08130101Stock> moveStocks
                        p08130101StockList.Add(
                                new P08130101Stock
                                {
                                    LOC_CODE = postMoveConfirmReq.SrcLoc,
                                    ITEM_CODE = item.ItemNo,
                                    ITEM_NAME = null,
                                    VALID_DATE = DateTime.TryParse(item.ValidDate, out dateTime) ? Convert.ToDateTime(item.ValidDate) : default(DateTime),
                                    ENTER_DATE = DateTime.TryParse(item.EnterDate, out dateTime) ? Convert.ToDateTime(item.EnterDate) : default(DateTime),
                                    MAKE_NO = item.MkNo,
                                    BOX_CTRL_NO = item.BoxNo,
                                    PALLET_CTRL_NO = item.PalletNo,
                                    SERIAL_NO = item.Sn,
                                    QTY = f1913.QTY,
                                    MOVE_QTY = item.MoveQty,
                                });
                    }

                    // 檢查商品混批混品
                    if (apiResult.IsSuccessed)
                    {
                        var checkLocHasItem = p81Service.CheckLocHasItem(postMoveConfirmReq.DcNo, gupCode, postMoveConfirmReq.CustNo, item.ItemNo, postMoveConfirmReq.TarLoc, item.ValidDate);
                        if (!checkLocHasItem.IsSuccessed)
                            apiResult = checkLocHasItem;
                    }
                }
            }

            // 建立調撥單資料
            if (p08130101StockList.Count() > 0)
            {
                var createAllocation = p081301Service.CreateAllocation(postMoveConfirmReq.DcNo, gupCode, postMoveConfirmReq.CustNo, postMoveConfirmReq.TarLoc, p08130101StockList);
                if (createAllocation.IsSuccessed)
                {
                    _wmsTransation.Complete();
                    apiResult = new ApiResult { IsSuccessed = true, MsgCode = "20701", MsgContent = p81Service.GetMsg("20701") };
                }
                else
                    apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20756", MsgContent = string.Format(p81Service.GetMsg("20756"), createAllocation.Message.Replace("\n", "")) };
            }

            return apiResult;
        }
    }
}
