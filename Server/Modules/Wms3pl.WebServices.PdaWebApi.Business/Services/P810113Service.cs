using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Lms.Services;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
  public class P810113Service
  {
    private P81Service __p81Service;
    public P81Service _p81Service
    {
      get { return __p81Service == null ? __p81Service = new P81Service() : __p81Service; }
      set { __p81Service = value; }
    }

    private CommonService _commonService;
    public CommonService CommonService
    {
      get { return _commonService == null ? _commonService = new CommonService() : _commonService; }
      set { _commonService = value; }
    }

    private WmsTransaction _wmsTransaction;
    public P810113Service(WmsTransaction wmsTransation)
    {
      _wmsTransaction = wmsTransation;
      _p81Service = new P81Service();
    }

    /// <summary>
    /// 商品複驗-容器查詢
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult GetRcvData(GetRcvDataReq req, string gupCode)
    {
      var containerService = new ContainerService();
      var f0701Repo = new F0701Repository(Schemas.CoreSchema);
      var f020501Repo = new F020501Repository(Schemas.CoreSchema);
      var f020502Repo = new F020502Repository(Schemas.CoreSchema);
      var warehouseInService = new WarehouseInService(new WmsTransaction());

      #region 資料檢核

      // 帳號檢核
      var accData = _p81Service.CheckAcc(req.AccNo);

      // 檢核人員功能權限
      var accFunctionCount = _p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

      // 檢核人員貨主權限
      var accCustCount = _p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

      // 檢核人員物流中心權限
      var accDcCount = _p81Service.CheckAccDc(req.DcNo, req.AccNo);

      // 傳入的參數驗證
      if (string.IsNullOrWhiteSpace(req.FuncNo) ||
              string.IsNullOrWhiteSpace(req.AccNo) ||
              string.IsNullOrWhiteSpace(req.DcNo) ||
              string.IsNullOrWhiteSpace(req.CustNo) ||
              !accData.Any() ||
              accFunctionCount == 0 ||
              accCustCount == 0 ||
              accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      // 檢核傳入容器條碼不得為空
      if (string.IsNullOrWhiteSpace(req.ContainerCode))
        return new ApiResult { IsSuccessed = false, MsgCode = "21001", MsgContent = _p81Service.GetMsg("21001") };

      // 傳入容器條碼轉大寫
      req.ContainerCode = req.ContainerCode.ToUpper();

      // 檢查容器狀態
      var chkCtnRes = containerService.CheckContainer(req.ContainerCode);
      if (!chkCtnRes.IsSuccessed)
        return new ApiResult { IsSuccessed = false, MsgCode = "21300", MsgContent = chkCtnRes.Message };

      // 檢核F0701是否存在
      var f0701 = f0701Repo.GetDatasByTrueAndCondition(o => o.CONTAINER_TYPE == "0" && o.CONTAINER_CODE == chkCtnRes.ContainerCode).FirstOrDefault();
      if (f0701 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21301", MsgContent = _p81Service.GetMsg("21301") };

      // 取得驗收容器上架檔資料，檢核是否有無驗收容器上架檔資料
      var f020501 = f020501Repo.GetDataByF0701Id(req.DcNo, gupCode, req.CustNo, f0701.ID);
      if (f020501 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21301", MsgContent = _p81Service.GetMsg("21301") };

      // 檢核驗收容器上架檔狀態
      var chkF020501Status = warehouseInService.CheckF020501Status(f020501);
      if (!chkF020501Status.IsSuccessed)
        return chkF020501Status;

      var f020502s = f020502Repo.GetDataByF020501Id(req.DcNo, gupCode, req.CustNo, f020501.ID, chkCtnRes.BinCode).ToList();
      if (!string.IsNullOrWhiteSpace(chkCtnRes.BinCode)) // 若傳入的是容器分格條碼([A].BinCode不是null)
      {
        // 取得驗收容器上架明細檔資料，檢核是否有無驗收容器上架明細檔資料
        var f020502 = f020502s.FirstOrDefault();
        if (f020502 == null)
          return new ApiResult { IsSuccessed = false, MsgCode = "21301", MsgContent = _p81Service.GetMsg("21301") };
        // 檢查F020502.status = 1(不須複驗) 或2(複驗完成) 則回傳錯誤訊息”容器已複驗 / 不須複驗”
        if (f020502.STATUS == "1" || f020502.STATUS == "2")
          return new ApiResult { IsSuccessed = false, MsgCode = "21302", MsgContent = _p81Service.GetMsg("21302") };
        // 檢查F020502.status = 3(複驗失敗) 則回傳錯誤訊息”此容器複驗失敗，請至[複驗異常處理功能]，進行後續的作業”
        else if (f020502.STATUS == "3")
          return new ApiResult { IsSuccessed = false, MsgCode = "21305", MsgContent = _p81Service.GetMsg("21305") };
      }
      else // 若傳入的是容器條碼([A].BinCode是null)
      {
        f020502s = f020502s.Where(x => x.STATUS == "0").ToList();
        if (!f020502s.Any())
          return new ApiResult { IsSuccessed = false, MsgCode = "21323", MsgContent = _p81Service.GetMsg("21323") };
      }
      #endregion

      #region 組回傳資料
      var res = new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = _p81Service.GetMsg("10005") };

      // 找出待複驗的資料回傳
      var rcvData = f020502s.ToList();

      // 找出帶複驗的明細商品資料
      var f1903s = CommonService.GetProductList(gupCode, req.CustNo, rcvData.Select(x => x.ITEM_CODE).Distinct().ToList());

      res.Data = (from A in rcvData
                  join B in f1903s
                  on A.ITEM_CODE equals B.ITEM_CODE
                  select new GetRcvDataRes
                  {
                    StockNo = A.STOCK_NO,
                    RtNo = A.RT_NO,
                    F020502_ID = A.ID,
                    ItemCode = A.ITEM_CODE,
                    CustItemCode = B.CUST_ITEM_CODE,
                    ItemName = B.ITEM_NAME,
                    TarQty = A.QTY
                  }).ToList();

      return res;
      #endregion
    }

    /// <summary>
    /// 商品複驗-複驗查詢
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult GetRecheckData(GetRecheckDataReq req, string gupCode)
    {
      var commonService = new CommonService();
      var doubleCheckService = new DoubleCheckService();
      var warehouseInService = new WarehouseInService(_wmsTransaction);
      var f020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020502Repo = new F020502Repository(Schemas.CoreSchema, _wmsTransaction);
      var f010201Repo = new F010201Repository(Schemas.CoreSchema);
      var f1905Repo = new F1905Repository(Schemas.CoreSchema);
      var f020201Repo = new F020201Repository(Schemas.CoreSchema);

      #region 資料檢核

      // 帳號檢核
      var accData = _p81Service.CheckAcc(req.AccNo);

      // 檢核人員功能權限
      var accFunctionCount = _p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

      // 檢核人員貨主權限
      var accCustCount = _p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

      // 檢核人員物流中心權限
      var accDcCount = _p81Service.CheckAccDc(req.DcNo, req.AccNo);

      // 傳入的參數驗證
      if (string.IsNullOrWhiteSpace(req.FuncNo) ||
              string.IsNullOrWhiteSpace(req.AccNo) ||
              string.IsNullOrWhiteSpace(req.DcNo) ||
              string.IsNullOrWhiteSpace(req.CustNo) ||
              !accData.Any() ||
              accFunctionCount == 0 ||
              accCustCount == 0 ||
              accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      // 檢核傳入F020502_ID不得為空
      if (req.F020502_ID == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21306", MsgContent = _p81Service.GetMsg("21306") };

      // 檢核傳入F020502_ID是否找得到F020502
      var f020502Id = Convert.ToInt64(req.F020502_ID);
      var f020502 = f020502Repo.AsForUpdate().Find(o => o.ID == f020502Id);
      if (f020502 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21306", MsgContent = _p81Service.GetMsg("21306") };

      var f020501 = f020501Repo.AsForUpdate().Find(o => o.ID == f020502.F020501_ID);
      if (f020501 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21306", MsgContent = _p81Service.GetMsg("21306") };

      // 檢查F020502.status = 1(不須複驗) 或2(複驗完成) 則回傳錯誤訊息”容器已複驗 / 不須複驗”
      if (f020502.STATUS == "1" || f020502.STATUS == "2")
        return new ApiResult { IsSuccessed = false, MsgCode = "21302", MsgContent = _p81Service.GetMsg("21302") };
      // 檢查F020502.status = 3(複驗失敗) 則回傳錯誤訊息”此容器複驗失敗，請至[複驗異常處理功能]，進行後續的作業”
      else if (f020502.STATUS == "3")
        return new ApiResult { IsSuccessed = false, MsgCode = "21305", MsgContent = _p81Service.GetMsg("21305") };

      // 取得進倉單資料
      var f010201 = f010201Repo.Find(o => o.DC_CODE == f020502.DC_CODE && o.GUP_CODE == f020502.GUP_CODE && o.CUST_CODE == f020502.CUST_CODE && o.STOCK_NO == f020502.STOCK_NO);
      if (f010201 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21309", MsgContent = _p81Service.GetMsg("21309") };
      #endregion

      #region 呼叫LmsApi複驗比例確認
      try
      {
        var lmsRes = doubleCheckService.DoubleCheckConfirm(gupCode, new Datas.Shared.ApiEntities.DoubleCheckConfirmReq
        {
          DcCode = req.DcNo,
          CustCode = req.CustNo,
          CustInNo = f010201.CUST_ORD_NO,
          ItemList = new List<Datas.Shared.ApiEntities.DoubleCheckConfirmItem>
          {
            new Datas.Shared.ApiEntities.DoubleCheckConfirmItem
            {
              ItemCode = f020502.ITEM_CODE,
              Qty = f020502.QTY
            }
          }
        });

        int? checkQty = null;
        if (lmsRes.IsSuccessed)
        {
          var lmsResData = JsonConvert.DeserializeObject<List<Datas.Shared.ApiEntities.DoubleCheckConfirmData>>(JsonConvert.SerializeObject(lmsRes.Data));
          if (lmsResData != null && lmsResData.Any())
          {
            var currItemLmsResData = lmsResData.Where(x => x.ItemCode == f020502.ITEM_CODE).FirstOrDefault();
            if (currItemLmsResData == null)
              return new ApiResult { IsSuccessed = false, MsgCode = "21308", MsgContent = _p81Service.GetMsg("21308") };

            if (currItemLmsResData.IsNeedDoubleCheck == "0")
            {
              // 更新F020502.STATUS=1 不須複驗
              f020502.STATUS = "1";
              f020502Repo.Update(f020502);

              var lockRes = warehouseInService.LockContainerProcess(f020501.CONTAINER_CODE);
              if (!lockRes.IsSuccessed)
                return new ApiResult { IsSuccessed = false, MsgCode = "21329", MsgContent = string.Format(_p81Service.GetMsg("21329"), f020501.CONTAINER_CODE) };

              #region F020501容器頭檔狀態檢查
              var chkF020501Status = warehouseInService.CheckF020501Status(f020501);
              if (!chkF020501Status.IsSuccessed)
                return chkF020501Status;
                #endregion F020501容器頭檔狀態檢查

              // 呼叫[容器上架共用函數]
              warehouseInService.ContainerTarget(f020501, f020502, null);

              _wmsTransaction.Complete();

              var msgCode = "10014";
              if (f020501.STATUS == "3")// 不可上架
                msgCode = "10016";
              else if (f020501.STATUS == "2")// 可上架
                msgCode = "10017";

              return new ApiResult
              {
                IsSuccessed = true,
                MsgCode = "10005",
                MsgContent = _p81Service.GetMsg("10005"),
                Data = new GetRecheckDataRes
                {
                  ApiInfo = _p81Service.GetMsg(msgCode)
                }
              };
            }
            else
            {
              checkQty = currItemLmsResData.Qty;
            }
          }
        }
        else
        {
          return new ApiResult { IsSuccessed = false, MsgCode = "21307", MsgContent = string.Format(_p81Service.GetMsg("21307"), lmsRes.MsgContent) };
        }
        #endregion

        #region 組回傳資料

        var f1903 = CommonService.GetProduct(f020502.GUP_CODE, f020502.CUST_CODE, f020502.ITEM_CODE);

        var f1905 = f1905Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == f020502.CUST_CODE && o.ITEM_CODE == f020502.ITEM_CODE);

        var f020201 = f020201Repo.GetDatasByF020502(req.DcNo, gupCode, req.CustNo, new[] { f020502 }.ToList()).FirstOrDefault();
        if (f020201 == null)
          return new ApiResult { IsSuccessed = false, MsgCode = "21331", MsgContent = _p81Service.GetMsg("21331") };

        var itemFlag = new List<string>();
        if (f1903.IS_PRECIOUS == "1")
          itemFlag.Add("貴重品");
        if (f1903.IS_EASY_LOSE == "1")
          itemFlag.Add("易遺失品");
        if (f1903.IS_MAGNETIC == "1")
          itemFlag.Add("強磁標示");
        if (f1903.IS_TEMP_CONTROL == "1")
          itemFlag.Add("需溫控");
        if (f1903.FRAGILE == "1")
          itemFlag.Add("易碎品");
        if (f1903.SPILL == "1")
          itemFlag.Add("液體");
        if (f1903.IS_PERISHABLE == "1")
          itemFlag.Add("易變質品");

        var res = new ApiResult
        {
          IsSuccessed = true,
          MsgCode = "10005",
          MsgContent = _p81Service.GetMsg("10005"),
          Data = new GetRecheckDataRes
          {
            CustCode = f020502.CUST_CODE,
            StockNo = f020502.STOCK_NO,
            RtNo = f020502.RT_NO,
            CustOrdNo = f010201.CUST_ORD_NO,
            ItemCode = f020502.ITEM_CODE,
            CustItemCode = f1903.CUST_ITEM_CODE,
            EanCode1 = f1903.EAN_CODE1,
            EanCode2 = f1903.EAN_CODE2,
            EanCode3 = f1903.EAN_CODE3,
            ItemName = f1903.ITEM_NAME,
            ItemSpec = f1903.ITEM_SPEC,
            ItemSizeText = $"{f1905.PACK_LENGTH}*{f1905.PACK_WIDTH}*{f1905.PACK_HIGHT}",
            ItemWeight = f1905.PACK_WEIGHT.ToString(),
            Qty = f020502.QTY,
            CheckQty = Convert.ToInt32(checkQty),
            F020502_ID = f020502.ID,
            SaveDay = f1903.SAVE_DAY?.ToString() ?? "",
            ValidDate = f020201.VALI_DATE?.ToString("yyyy/MM/dd") ?? "",
            ItemFlagText = string.Join("、", itemFlag),
            VnrItemCode = f1903.VNR_ITEM_CODE,
            RcvMemo = f1903.RCV_MEMO
          }
        };
        return res;
      }
      catch (Exception ex)
      { return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = ex.Message }; }
      finally
      { warehouseInService.UnlockContainerProcess(new[] { f020501.CONTAINER_CODE }.ToList()); }
      #endregion
    }

    /// <summary>
    /// 商品複驗-商品條碼檢驗
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult ConfirmInputItemData(ConfirmInputItemDataReq req, string gupCode)
    {
      var commonService = new CommonService();
      var f1903Repo = new F1903Repository(Schemas.CoreSchema);
      var f02020109Repo = new F02020109Repository(Schemas.CoreSchema);
      var f02020104Repo = new F02020104Repository(Schemas.CoreSchema);

      #region 資料檢核

      // 帳號檢核
      var accData = _p81Service.CheckAcc(req.AccNo);

      // 檢核人員功能權限
      var accFunctionCount = _p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

      // 檢核人員貨主權限
      var accCustCount = _p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

      // 檢核人員物流中心權限
      var accDcCount = _p81Service.CheckAccDc(req.DcNo, req.AccNo);

      // 傳入的參數驗證
      if (string.IsNullOrWhiteSpace(req.FuncNo) ||
              string.IsNullOrWhiteSpace(req.AccNo) ||
              string.IsNullOrWhiteSpace(req.DcNo) ||
              string.IsNullOrWhiteSpace(req.CustNo) ||
              string.IsNullOrWhiteSpace(req.RtNo) ||
              string.IsNullOrWhiteSpace(req.ItemCode) ||
              !accData.Any() ||
              accFunctionCount == 0 ||
              accCustCount == 0 ||
              accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      // 檢核刷入的條碼是否為空
      if (string.IsNullOrWhiteSpace(req.InputItemCode))
        return new ApiResult { IsSuccessed = false, MsgCode = "20050", MsgContent = string.Format(_p81Service.GetMsg("20050"), "刷入的條碼") };
      #endregion

      #region 資料處理
      req.InputItemCode = req.InputItemCode.ToUpper();

      var f1903 = f1903Repo.GetDatasByBarCode(gupCode, req.CustNo, req.ItemCode, req.InputItemCode);
      if (f1903 != null)
        return new ApiResult { IsSuccessed = true, MsgCode = "10010", MsgContent = _p81Service.GetMsg("10010") };

      // 此處只要是該驗收單的不良品，不管是否屬於該品號的，只要存在，都不能算
      var f02020109s = f02020109Repo.GetDatasByBarCode(req.DcNo, gupCode, req.CustNo, req.RtNo, req.InputItemCode);
      if (f02020109s.Any())
        return new ApiResult { IsSuccessed = false, MsgCode = "21304", MsgContent = _p81Service.GetMsg("21304") };

      // 找序號資料
      var f02020104s = f02020104Repo.GetDatasByBarCode(req.DcNo, gupCode, req.CustNo, req.RtNo, req.ItemCode, req.InputItemCode);
      if (f02020104s.Any())
        return new ApiResult { IsSuccessed = true, MsgCode = "10010", MsgContent = _p81Service.GetMsg("10010") };
      else
        return new ApiResult { IsSuccessed = false, MsgCode = "21310", MsgContent = _p81Service.GetMsg("21310") };
      #endregion
    }

    /// <summary>
    /// 商品複驗-複驗開始
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult StartToReCheck(StartToReCheckReq req, string gupCode)
    {
      var commonService = new CommonService();
      var f1951Repo = new F1951Repository(Schemas.CoreSchema);
      var f020502Repo = new F020502Repository(Schemas.CoreSchema);
      var f077101Repo = new F077101Repository(Schemas.CoreSchema);

      #region 資料檢核

      // 帳號檢核
      var accData = _p81Service.CheckAcc(req.AccNo);

      // 檢核人員功能權限
      var accFunctionCount = _p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

      // 檢核人員貨主權限
      var accCustCount = _p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

      // 檢核人員物流中心權限
      var accDcCount = _p81Service.CheckAccDc(req.DcNo, req.AccNo);

      // 傳入的參數驗證
      if (string.IsNullOrWhiteSpace(req.FuncNo) ||
              string.IsNullOrWhiteSpace(req.AccNo) ||
              string.IsNullOrWhiteSpace(req.DcNo) ||
              string.IsNullOrWhiteSpace(req.CustNo) ||
              !accData.Any() ||
              accFunctionCount == 0 ||
              accCustCount == 0 ||
              accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      // 檢核傳入F020502_ID不得為空
      if (req.F020502_ID == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21306", MsgContent = _p81Service.GetMsg("21306") };

      // 檢核傳入F020502_ID是否找得到F020502
      var f020502Id = Convert.ToInt64(req.F020502_ID);
      var f020502 = f020502Repo.Find(o => o.ID == f020502Id);
      if (f020502 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21306", MsgContent = _p81Service.GetMsg("21306") };
      #endregion

      #region 資料處理
      var f077101 = f077101Repo.GetData(req.DcNo, "1", f020502Id, "0");

      if (f077101 == null)
      {
        // 新增F077101進倉人員作業紀錄
        f077101Repo.Add(new F077101
        {
          DC_CODE = req.DcNo,
          EMP_ID = req.AccNo,
          WORK_TYPE = "1",
          REF_ID = f020502.ID,
          WORKING_TIME = DateTime.Now,
          STATUS = "0"
        });
      }
      else
      {
        // 修改F077101進倉人員作業紀錄
        f077101.WORKING_TIME = DateTime.Now; // 更新作業時間
        f077101Repo.Update(f077101);
      }
      #endregion

      #region 組回傳資料
      var res = new ApiResult
      {
        IsSuccessed = true,
        MsgCode = "10005",
        MsgContent = _p81Service.GetMsg("10005"),
        Data = f1951Repo.GetDataByUctId("IQ").Select(x => new ReCheckCauseData
        {
          UccCode = x.UCC_CODE,
          Cause = x.CAUSE
        }).ToList()
      };

      return res;
      #endregion
    }

    /// <summary>
    /// 商品複驗-複驗通過
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult ConfirmRecheckData(ConfirmRecheckDataReq req, string gupCode)
    {
      var commonService = new CommonService();
      var warehouseInService = new WarehouseInService(_wmsTransaction);
      var f020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020502Repo = new F020502Repository(Schemas.CoreSchema, _wmsTransaction);
      var f077101Repo = new F077101Repository(Schemas.CoreSchema, _wmsTransaction);
      #region 資料檢核

      // 帳號檢核
      var accData = _p81Service.CheckAcc(req.AccNo);

      // 檢核人員功能權限
      var accFunctionCount = _p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

      // 檢核人員貨主權限
      var accCustCount = _p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

      // 檢核人員物流中心權限
      var accDcCount = _p81Service.CheckAccDc(req.DcNo, req.AccNo);

      // 傳入的參數驗證
      if (string.IsNullOrWhiteSpace(req.FuncNo) ||
              string.IsNullOrWhiteSpace(req.AccNo) ||
              string.IsNullOrWhiteSpace(req.DcNo) ||
              string.IsNullOrWhiteSpace(req.CustNo) ||
              req.InputType != 0 ||
              !accData.Any() ||
              accFunctionCount == 0 ||
              accCustCount == 0 ||
              accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      // 檢核傳入F020502_ID不得為空
      if (req.F020502_ID == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21306", MsgContent = _p81Service.GetMsg("21306") };

      // 檢核傳入F020502_ID是否找得到F020502
      var f020502Id = Convert.ToInt64(req.F020502_ID);
      var f020502 = f020502Repo.AsForUpdate().Find(o => o.ID == f020502Id);
      if (f020502 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21306", MsgContent = _p81Service.GetMsg("21306") };

      var f020501 = f020501Repo.AsForUpdate().Find(o => o.ID == f020502.F020501_ID);

      #endregion

      #region 資料處理
      try
      {
        #region 上架容器鎖定
        var lockRes = warehouseInService.LockContainerProcess(f020501.CONTAINER_CODE);
        if (!lockRes.IsSuccessed)
          return new ApiResult { IsSuccessed = false, MsgCode = "21329", MsgContent = string.Format(_p81Service.GetMsg("21329"), f020501.CONTAINER_CODE) };
        #endregion

        #region F020501容器頭檔狀態檢查
        var chkF020501Status = warehouseInService.CheckF020501Status(f020501);
        if (!chkF020501Status.IsSuccessed)
        {
          //如果回傳false，會導致PDA就卡在當前畫面造成User操作上的疑惑，要手動回到查詢頁面，因此改為回傳成功
          chkF020501Status.IsSuccessed = true;
          chkF020501Status.Data = chkF020501Status.MsgContent;
          return chkF020501Status;
        }
        #endregion

        #region 更新F020502&F1903 RCV_MEMO驗貨輔助註記欄位
        var updRcvMemoRes = UpdateF1903RcvMemo(ref f020502, req.RcvMemo);
        //基本上不可能出現這錯誤
        if (!updRcvMemoRes.IsSuccessed)
          return updRcvMemoRes;
        #endregion

        #region 更新 F020502
        f020502.STATUS = "2";// 複驗完成
        f020502Repo.Update(f020502);
        #endregion

        #region 新增 F077101
        f077101Repo.Add(new F077101
        {
          DC_CODE = req.DcNo,
          EMP_ID = req.AccNo,
          WORK_TYPE = "1",
          REF_ID = f020502.ID,
          WORKING_TIME = DateTime.Now,
          STATUS = "1"
        });
        #endregion

        #region 檢查該容器內的所有商品是否還要檢驗
        var wisResult = warehouseInService.ContainerTarget(f020501, f020502, null);
        #endregion

        _wmsTransaction.Complete();

        return new ApiResult
        {
          IsSuccessed = true,
          MsgCode = "10005",
          MsgContent = _p81Service.GetMsg("10005"),
          Data = f020501.STATUS == "3" ? _p81Service.GetMsg("10015") : _p81Service.GetMsg("10011")
        };
      }
      catch (Exception ex)
      { return new ApiResult { IsSuccessed = false, MsgCode = "21330", MsgContent = ex.Message }; }
      finally
      { warehouseInService.UnlockContainerProcess(new[] { f020501.CONTAINER_CODE }.ToList()); }

      #endregion
    }

    /// <summary>
    /// 商品複驗-不通過原因登錄與容器移轉
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult ChangeContainerData(ChangeContainerDataReq req, string gupCode)
    {
      var result = new ApiResult() { IsSuccessed = false };
      var containerService = new ContainerService();
      var commonService = new CommonService();
      var warehouseInService = new WarehouseInService(_wmsTransaction);
      var f020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020502Repo = new F020502Repository(Schemas.CoreSchema, _wmsTransaction);
      var f0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransaction);
      var f077101Repo = new F077101Repository(Schemas.CoreSchema, _wmsTransaction);
      var f070101Repo = new F070101Repository(Schemas.CoreSchema, _wmsTransaction);
      var f070102Repo = new F070102Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020201Repo = new F020201Repository(Schemas.CoreSchema, _wmsTransaction);
      var lockF020501s = new List<F020501>();
      #region 資料檢核

      // 帳號檢核
      var accData = _p81Service.CheckAcc(req.AccNo);

      // 檢核人員功能權限
      var accFunctionCount = _p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

      // 檢核人員貨主權限
      var accCustCount = _p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

      // 檢核人員物流中心權限
      var accDcCount = _p81Service.CheckAccDc(req.DcNo, req.AccNo);

      // 傳入的參數驗證
      if (string.IsNullOrWhiteSpace(req.FuncNo) ||
              string.IsNullOrWhiteSpace(req.AccNo) ||
              string.IsNullOrWhiteSpace(req.DcNo) ||
              string.IsNullOrWhiteSpace(req.CustNo) ||
              (req.InputType != 0 && req.InputType != 1) ||
              !accData.Any() ||
              accFunctionCount == 0 ||
              accCustCount == 0 ||
              accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      // 檢核傳入F020502_ID不得為空
      if (req.F020502_ID == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21306", MsgContent = _p81Service.GetMsg("21306") };

      // 檢核傳入F020502_ID是否找得到F020502
      var f020502Id = Convert.ToInt64(req.F020502_ID);
      var f020502 = f020502Repo.AsForUpdate().Find(o => o.ID == f020502Id);
      if (f020502 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21306", MsgContent = _p81Service.GetMsg("21306") };

      // 檢核傳入複驗不通過原因是否為空
      if (string.IsNullOrWhiteSpace(req.CauseCode))
        return new ApiResult { IsSuccessed = false, MsgCode = "21311", MsgContent = _p81Service.GetMsg("21311") };

      // 取得F020501
      var oriF020501 = f020501Repo.AsForUpdate().Find(o => o.ID == f020502.F020501_ID);
      if (oriF020501 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21306", MsgContent = _p81Service.GetMsg("21306") };

      long f0701Id = 0;
      string newContainerCode = string.Empty;
      F070102 f070102 = null;
      F070101 f070101ByOldCon = null;
      if (req.InputType == 0)// 表示要處理更換容器的作業
      {
        // 新容器條碼若有值轉大寫
        newContainerCode = req.NewContainerCode.ToUpper();

        // 檢核傳入新容器條碼是否為空
        if (string.IsNullOrWhiteSpace(newContainerCode))
          return new ApiResult { IsSuccessed = false, MsgCode = "21312", MsgContent = _p81Service.GetMsg("21312") };

        // 檢查容器狀態
        var chkCtnRes = containerService.CheckContainer(newContainerCode);
        if (!chkCtnRes.IsSuccessed)
          return new ApiResult { IsSuccessed = false, MsgCode = "21300", MsgContent = chkCtnRes.Message };

        // 檢核是否輸入容器條碼(9碼)
        if (!string.IsNullOrWhiteSpace(chkCtnRes.BinCode))
          return new ApiResult { IsSuccessed = false, MsgCode = "21320", MsgContent = _p81Service.GetMsg("21320") };

				// 檢查原本容器的資料
				var f0701ByOldCon = f0701Repo.GetDatasByTrueAndCondition(x => x.ID == oriF020501.F0701_ID).FirstOrDefault();

        List<F070102> f070102sByOldCon = null;

        if (f0701ByOldCon == null)
          return new ApiResult { IsSuccessed = false, MsgCode = "21319", MsgContent = _p81Service.GetMsg("21319") };

        if (f0701ByOldCon.CONTAINER_TYPE == "0") // 單一型容器
        {
          f070101ByOldCon = f070101Repo.GetDatasByTrueAndCondition(o => o.F0701_ID == f0701ByOldCon.ID).FirstOrDefault();
          if (f070101ByOldCon == null)
            return new ApiResult { IsSuccessed = false, MsgCode = "21315", MsgContent = _p81Service.GetMsg("21315") };

          f070102sByOldCon = f070102Repo.AsForUpdate().GetDatasByTrueAndCondition(o => o.F070101_ID == f070101ByOldCon.ID).ToList();
          if (!f070102sByOldCon.Any())
            return new ApiResult { IsSuccessed = false, MsgCode = "21316", MsgContent = _p81Service.GetMsg("21316") };
        }
        else if (f0701ByOldCon.CONTAINER_TYPE == "2") // 混和型容器
          return new ApiResult { IsSuccessed = false, MsgCode = "21314", MsgContent = _p81Service.GetMsg("21314") };

        // 確認原本容器的商品與驗收容器紀錄相同
        var f020201 = f020201Repo.Find(o =>
        o.DC_CODE == f020502.DC_CODE &&
        o.GUP_CODE == f020502.GUP_CODE &&
        o.CUST_CODE == f020502.CUST_CODE &&
        o.RT_NO == f020502.RT_NO &&
        o.RT_SEQ == f020502.RT_SEQ);
        if (f020201 == null)
          return new ApiResult { IsSuccessed = false, MsgCode = "21317", MsgContent = _p81Service.GetMsg("21317") };

        f070102 = f070102sByOldCon.Where(o =>
        o.GUP_CODE == f020201.GUP_CODE &&
        o.CUST_CODE == f020201.CUST_CODE &&
        o.ITEM_CODE == f020201.ITEM_CODE &&
        o.MAKE_NO == f020201.MAKE_NO &&
        o.VALID_DATE == f020201.VALI_DATE &&
				o.BIN_CODE == f020502.BIN_CODE).FirstOrDefault();

        if (f070102 == null)
          return new ApiResult { IsSuccessed = false, MsgCode = "21318", MsgContent = _p81Service.GetMsg("21318") };
      }
      #endregion

      #region 資料處理
      try
      {
        var msg = string.Empty;

        #region 更新F020502&F1903 RCV_MEMO驗貨輔助註記欄位
        var updRcvMemoRes = UpdateF1903RcvMemo(ref f020502, req.RcvMemo);
        //基本上不可能出現這錯誤
        if (!updRcvMemoRes.IsSuccessed)
          return updRcvMemoRes;
        #endregion

        if (req.InputType == 0) // 表示要處理更換容器的作業
        {
          // 檢查容器是否在使用中
          var f0701 = f0701Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
                      new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
                      () =>
                      {
                        var lockF0701 = f0701Repo.LockF0701();
                        var f0701ByNewCon = f0701Repo.GetDataByContainerCode("0", newContainerCode);

                        if (f0701ByNewCon == null)
                        {
                          f0701Id = f0701Repo.GetF0701NextId();
                          f0701Repo.InsertF0701(f0701Id, req.DcNo, req.CustNo, "NA" ,newContainerCode, "0");
                        }

                        return f0701ByNewCon;
                      });

          if (f0701 != null)// 若資料存在 回傳錯誤訊息”新容器已被使用，請更換其他容器”
          {
            result = new ApiResult { IsSuccessed = false, MsgCode = "21313", MsgContent = _p81Service.GetMsg("21313") };
            return result;
          }

          //檢查容器否有其他人正在處理上架中
          lockF020501s.Add(oriF020501);
          var lockRes = warehouseInService.LockContainerProcess(oriF020501.CONTAINER_CODE);
          if (!lockRes.IsSuccessed)
          {
            result = new ApiResult { IsSuccessed = false, MsgCode = "21329", MsgContent = string.Format(_p81Service.GetMsg("21329"), oriF020501.CONTAINER_CODE) };
            return result;
          }

          #region F020501容器頭檔狀態檢查
          var chkF020501Status = warehouseInService.CheckF020501Status(oriF020501);
          if (!chkF020501Status.IsSuccessed)
          {
            //如果回傳false，會導致PDA就卡在當前畫面造成User操作上的疑惑，要手動回到查詢頁面，因此改為回傳成功
            chkF020501Status.IsSuccessed = true;
            chkF020501Status.Data = chkF020501Status.MsgContent;
            return chkF020501Status;
          }
          #endregion

          #region 針對已存在的容器，從舊容器移到新容器中
          var f070101Id = containerService.GetF070101NextId();

          var addF070101 = new F070101
          {
            ID = f070101Id,
            F0701_ID = f0701Id,
            DC_CODE = f070101ByOldCon.DC_CODE,
            CONTAINER_CODE = newContainerCode,
            GUP_CODE = f070101ByOldCon.GUP_CODE,
            CUST_CODE = f070101ByOldCon.CUST_CODE,
            WMS_NO = f070101ByOldCon.WMS_NO,
            WMS_TYPE = f070101ByOldCon.WMS_TYPE
          };
          f070101Repo.Add(addF070101);

          // 更新F070102
          f070102.F070101_ID = f070101Id;
					f070102.BIN_CODE = null;
					f070102Repo.Update(f070102);
          #endregion

          #region 更新驗收容器紀錄
          var f020501Id = warehouseInService.GetF020501NextId();

          // 新增F020501
          var addF020501 = new F020501
          {
            ID = f020501Id,
            DC_CODE = f020502.DC_CODE,
            GUP_CODE = f020502.GUP_CODE,
            CUST_CODE = f020502.CUST_CODE,
            CONTAINER_CODE = newContainerCode,
            F0701_ID = f0701Id,
            PICK_WARE_ID = oriF020501.PICK_WARE_ID,
            STATUS = "3",
            ALLOCATION_NO = oriF020501.ALLOCATION_NO,
            TYPE_CODE = oriF020501.TYPE_CODE
          };
          f020501Repo.Add(addF020501);

          lockF020501s.Add(addF020501);
          var lockRes2 = warehouseInService.LockContainerProcess(addF020501.CONTAINER_CODE);
          if (!lockRes.IsSuccessed)
          {
            result = new ApiResult { IsSuccessed = false, MsgCode = "21329", MsgContent = string.Format(_p81Service.GetMsg("21329"), addF020501.CONTAINER_CODE) };
            return result;
          }

          // 更新F020502
          f020502.CONTAINER_CODE = newContainerCode;
          f020502.F020501_ID = f020501Id;
          f020502.STATUS = "3";//複驗失敗
          f020502.RECHECK_CAUSE = req.CauseCode;
          f020502.BIN_CODE = null;
          if (!string.IsNullOrWhiteSpace(req.Note))
            f020502.RECHECK_MEMO = req.Note;
          f020502Repo.Update(f020502);
          #endregion

          //撈出屬於此F020501的明細(原始F020502更換容器已不屬於此F020501)
          var f020502s = f020502Repo.GetDatasByTrueAndCondition(o => o.F020501_ID == oriF020501.ID)
              .Where(x => x.ID != f020502.ID).ToList();

          if (f020502s.Any())
            // 呼叫容器上架共用函數
            ContainerTarget(oriF020501, f020502s, warehouseInService);

          // 容器釋放
          // 檢查原本的容器中是否還有其他的商品，若沒有，表示此容器中沒有其他東西(自己那筆將會換到新的容器)，所以原本容器應該要被釋放
          if (!f020502s.Any())
          {
            f0701Repo.DeleteF0701(oriF020501.F0701_ID);
            oriF020501.STATUS = "9";
            f020501Repo.Update(oriF020501);
          }
          if (oriF020501.STATUS == "1" || oriF020501.STATUS == "2") // 已關箱待複驗、可上架
            msg = _p81Service.GetMsg("10018");
          else if (oriF020501.STATUS == "3") // 不可上架
            msg = _p81Service.GetMsg("10012");
          else if (oriF020501.STATUS == "9")
            msg = _p81Service.GetMsg("10025");
        }
        else // 表示處理不更換容器的作業
        {
          #region 更新 F020502
          f020502.STATUS = "3";// 複驗失敗
          f020502.RECHECK_CAUSE = req.CauseCode;
          if (!string.IsNullOrWhiteSpace(req.Note))
            f020502.RECHECK_MEMO = req.Note;
          f020502Repo.Update(f020502);
          #endregion

          warehouseInService.ContainerTarget(oriF020501, f020502, f070101ByOldCon);

          if (oriF020501.STATUS == "1")   // 已關箱待複驗
            msg = _p81Service.GetMsg("10013");
          else if (oriF020501.STATUS == "3")   // 不可上架
            msg = _p81Service.GetMsg("10019");
        }

        #region 新增 F077101 進倉人員作業紀錄
        f077101Repo.Add(new F077101
        {
          DC_CODE = f020502.DC_CODE,
          EMP_ID = req.AccNo,
          WORK_TYPE = "1",
          REF_ID = f020502.ID,
          WORKING_TIME = DateTime.Now,
          STATUS = "1"
        });
        #endregion

        _wmsTransaction.Complete();

        result = new ApiResult
        {
          IsSuccessed = true,
          MsgCode = "10005",
          MsgContent = _p81Service.GetMsg("10005"),
          Data = msg
        };
        return result;
      }
      catch (Exception ex)
      {
        result = new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = ex.Message };
        return result;
      }
      finally
      {
        //發生錯誤時直接刪除容器資料
        if (!result.IsSuccessed && f0701Id != 0)
        {
          f0701Repo = new F0701Repository(Schemas.CoreSchema);
          f0701Repo.DeleteF0701(f0701Id);
        }

        warehouseInService.UnlockContainerProcess(lockF020501s.Select(x => x.CONTAINER_CODE).Distinct().ToList());
      }
      #endregion
    }

    /// <summary>
    /// 不良品更換容器，剩餘品項上架
    /// </summary>
    /// <param name="f020501">原始F020501</param>
    /// <param name="f020502s">更換容器後的F020502內容</param>
    /// <param name="f070101">原始F070101</param>
    /// <param name="warehouseInService"></param>
    /// <returns></returns>
    public ExecuteResult ContainerTarget(F020501 f020501, List<F020502> f020502s, WarehouseInService warehouseInService)
    {
      var res = new ExecuteResult { IsSuccessed = true };
      var f020201Repo = new F020201Repository(Schemas.CoreSchema);
      var f020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020502Repo = new F020502Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020107Repo = new F02020107Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020108Repo = new F02020108Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020202Repo = new F020202Repository(Schemas.CoreSchema, _wmsTransaction);
      var f070101Repo = new F070101Repository(Schemas.CoreSchema, _wmsTransaction);

      // 從[B]資料檢核容器明細篩選是否有未完成複驗
      // 條件:[B].STATUS =0 (0=待複驗) 如果存在，回傳IsSuccessed = false,Message = 此容器尚有商品未完成複驗，不可上架
      if (f020502s.Any(x => x.STATUS == "0"))// 待複驗
        return new ExecuteResult { IsSuccessed = false, Message = "此容器尚有商品未完成複驗，不可上架" };

      // 如果[B]資料中，有一筆狀態=3(複驗失敗) 或 <參數2>不是NULL且<參數2>的狀態=3(複驗失敗) 更新F020501.STATUS=3(不可上架)
      if (f020502s.Any(x => x.STATUS == "3"))
        f020501.STATUS = "3";// 不可上架
      else // 否則更新F020501.STATUS=2(可上架)
        f020501.STATUS = "2";// 可上架

      // 如果F020501.STAUTS = 2(可上架)
      if (f020501.STATUS == "2")
      {
        res = warehouseInService.ContainerTargetProcess(f020501, f020502s);
        if (!res.IsSuccessed)
          return res;
        var rtNoList = f020502s.Select(x => x.RT_NO).Distinct().ToList();
        var finishedRtContainerStatusList = f020502s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.STOCK_NO, x.RT_NO })
          .Select(x => new RtNoContainerStatus
          {
            DC_CODE = x.Key.DC_CODE,
            GUP_CODE = x.Key.GUP_CODE,
            CUST_CODE = x.Key.CUST_CODE,
            STOCK_NO = x.Key.STOCK_NO,
            RT_NO = x.Key.RT_NO,
            F020501_ID = f020501.ID,
            F020501_STATUS = f020501.STATUS,
            ALLOCATION_NO = f020501.ALLOCATION_NO
          }).ToList();
        res = warehouseInService.AfterConatinerTargetFinishedProcess(f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, rtNoList, finishedRtContainerStatusList);
        if (!res.IsSuccessed)
          return res;
      }
      else
        f020501Repo.Update(f020501);

      return res;
    }

    /// <summary>
    /// 更新F020502&F1903 RCV_MEMO驗貨輔助註記欄位
    /// </summary>
    /// <param name="f020502"></param>
    /// <param name="rcvMemo"></param>
    /// <returns></returns>
    private ApiResult UpdateF1903RcvMemo(ref F020502 f020502, string RcvMemo)
    {
      var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);

      var f1903 = CommonService.GetProduct(f020502.GUP_CODE, f020502.CUST_CODE, f020502.ITEM_CODE);
      if (f1903 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "20251", MsgContent = _p81Service.GetMsg("20251") };

      if (f1903.RCV_MEMO != RcvMemo)
      {
        f020502.RCV_MEMO = RcvMemo;
        f1903.RCV_MEMO = RcvMemo;
        f1903Repo.UpdateFields(new { RCV_MEMO = RcvMemo }, x => x.GUP_CODE == f1903.GUP_CODE && x.CUST_CODE == f1903.CUST_CODE && x.ITEM_CODE == f1903.ITEM_CODE);
      }
      return new ApiResult() { IsSuccessed = true };
    }

  }
}
