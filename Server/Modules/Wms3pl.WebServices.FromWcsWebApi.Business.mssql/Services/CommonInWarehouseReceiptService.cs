using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Checks;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Services
{
  /// <summary>
  /// 入庫完成結果回傳
  /// </summary>
  public class CommonInWarehouseReceiptService
  {
    #region 定義需檢核欄位、必填、型態、長度
    // 入庫單檢核設定
    private List<ApiCkeckColumnModel> receiptCheckColumnList = new List<ApiCkeckColumnModel>
        {
            new ApiCkeckColumnModel{  Name = "ReceiptCode",    Type = typeof(string),   MaxLength = 32, Nullable = false },
            new ApiCkeckColumnModel{  Name = "Status",         Type = typeof(int),      MaxLength = 0,  Nullable = false },
            new ApiCkeckColumnModel{  Name = "StartTime",      Type = typeof(DateTime), MaxLength = 0,  Nullable = false },
            new ApiCkeckColumnModel{  Name = "CompleteTime",   Type = typeof(DateTime), MaxLength = 0,  Nullable = false },
            new ApiCkeckColumnModel{  Name = "Operator",       Type = typeof(string),   MaxLength = 20, Nullable = false },
            new ApiCkeckColumnModel{  Name = "IsException",    Type = typeof(int),      MaxLength = 1 , Nullable = false },
            new ApiCkeckColumnModel{  Name = "PalletCode",     Type = typeof(string),   MaxLength = 32 },
            new ApiCkeckColumnModel{  Name = "SkuTotal",       Type = typeof(int),      MaxLength = 0 , Nullable = false }
        };

    // 入庫明細檢核設定
    private List<ApiCkeckColumnModel> skuCheckColumnList = new List<ApiCkeckColumnModel>
        {
            new ApiCkeckColumnModel{  Name = "RowNum",             Type = typeof(int),      MaxLength = 0,  Nullable = false },
            new ApiCkeckColumnModel{  Name = "SkuCode",            Type = typeof(string),   MaxLength = 20, Nullable = false },
            new ApiCkeckColumnModel{  Name = "SkuPlanQty",         Type = typeof(int),      MaxLength = 0 , Nullable = false },
            new ApiCkeckColumnModel{  Name = "SkuQty",             Type = typeof(int),      MaxLength = 0 , Nullable = false },
            new ApiCkeckColumnModel{  Name = "ReceiptFlag",        Type = typeof(int),      MaxLength = 0 , Nullable = false },
            new ApiCkeckColumnModel{  Name = "SkuLevel",           Type = typeof(int),      MaxLength = 0 , Nullable = false },
            new ApiCkeckColumnModel{  Name = "ExpiryDate",         Type = typeof(string),   MaxLength = 10, IsDate = true },
            new ApiCkeckColumnModel{  Name = "OutBatchCode",       Type = typeof(string),   MaxLength = 20  },
            new ApiCkeckColumnModel{  Name = "ContainerCode",      Type = typeof(string),   MaxLength = 32 },
            new ApiCkeckColumnModel{  Name = "BinCode",        Type = typeof(string),   MaxLength = 20 }
        };

    // 入庫明細檢核設定
    private List<ApiCkeckColumnModel> shelfBinCheckColumnList = new List<ApiCkeckColumnModel>
        {
            new ApiCkeckColumnModel{  Name = "ShelfCode",     Type = typeof(string),   MaxLength = 20 },
            new ApiCkeckColumnModel{  Name = "BinCode",       Type = typeof(string),   MaxLength = 20 },
            new ApiCkeckColumnModel{  Name = "SkuQty",        Type = typeof(int),      MaxLength = 0 , Nullable = false },
            new ApiCkeckColumnModel{  Name = "Operator",      Type = typeof(string),   MaxLength = 20, Nullable = false }
        };
    #endregion

    #region Private Property
    /// <summary>
    /// AGV入庫任務清單
    /// </summary>
    private List<F060101> _f060101List;

    /// <summary>
    /// 調撥單主檔清單
    /// </summary>
    private List<F151001> _f151001List = new List<F151001>();

    /// <summary>
    /// 調撥單明細清單
    /// </summary>
    private List<F151002> _f151002List = new List<F151002>();

    /// <summary>
    /// 失敗入庫單數
    /// </summary>
    private int _failCnt = 0;

    /// <summary>
    /// 紀錄有新增過F075105的DOC_ID，用以若檢核失敗 找出是否有新增，用以刪除
    /// </summary>
    private List<string> _IsAddF075105DocIdList = new List<string>();
    #endregion

    #region Main Method
    /// <summary>
    /// Func1
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult RecevieApiDatas(InWarehouseReceiptReq req)
    {
      CheckTransWcsApiService ctaService = new CheckTransWcsApiService();
      TransApiBaseService tacService = new TransApiBaseService();
      CommonService commonService = new CommonService();
      ApiResult res = new ApiResult { IsSuccessed = true };

      #region 資料檢核
      // 檢核參數
      if (req == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };

      // 檢核物流中心 必填、是否存在
      ctaService.CheckDcCode(ref res, req);
      if (!res.IsSuccessed)
        return res;

      // 檢核貨主編號 必填、是否存在
      ctaService.CheckOwnerCode(ref res, req);
      if (!res.IsSuccessed)
        return res;

      // 檢核倉庫編號 必填、是否存在
      ctaService.CheckZoneCode(ref res, req);
      if (!res.IsSuccessed)
        return res;

      // 檢核入庫單列表
      if (req.ReceiptList == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "20098", MsgContent = string.Format(tacService.GetMsg("20098"), "入庫單") };

      // 檢核資料筆數
      int reqTotal = req.ReceiptTotal != null ? Convert.ToInt32(req.ReceiptTotal) : 0;
      if (req.ReceiptList == null || (req.ReceiptList != null && !tacService.CheckDataCount(reqTotal, req.ReceiptList.Count)))
        return new ApiResult { IsSuccessed = false, MsgCode = "20022", MsgContent = string.Format(tacService.GetMsg("20022"), "入庫單", reqTotal, req.ReceiptList.Count) };

      // 檢核入庫單筆數是否超過[排程每次執行最大單據數]筆
      int midApiMisMax = Convert.ToInt32(commonService.GetSysGlobalValue("MIDAPIMISMAX"));
      if (req.ReceiptList.Count > midApiMisMax)
        return new ApiResult { IsSuccessed = false, MsgCode = "20099", MsgContent = string.Format(tacService.GetMsg("20099"), "入庫單筆數", req.ReceiptList.Count, midApiMisMax) };
      #endregion

      // 取得業主編號
      string gupCode = commonService.GetGupCode(req.OwnerCode);

      // 資料處理1
      return ProcessApiDatas(req.DcCode, gupCode, req.OwnerCode, req.ReceiptList);
    }

    /// <summary>
    /// 資料處理
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="receiptList"></param>
    /// <returns></returns>
    public ApiResult ProcessApiDatas(string dcCode, string gupCode, string custCode, List<InWarehouseReceiptModel> receiptList)
    {
      #region 變數
      WmsTransaction wmsTransation = new WmsTransaction();
      var tacService = new TransApiBaseService();
      var f060101Repo = new F060101Repository(Schemas.CoreSchema, wmsTransation);
      var f151001Repo = new F151001Repository(Schemas.CoreSchema);
      var f151002Repo = new F151002Repository(Schemas.CoreSchema);
      var f1913Repo = new F1913Repository(Schemas.CoreSchema);
      var f1924Repo = new F1924Repository(Schemas.CoreSchema);
      var f075105Repo = new F075105Repository(Schemas.CoreSchema);
      #endregion

      #region 取得資料[以下清單設為目前服務的 private property]
      // 取得AGV入庫任務清單
      _f060101List = f060101Repo.GetDatas(dcCode, gupCode, custCode, receiptList.Select(x => x.ReceiptCode).ToList()).ToList();

      // 取得入庫單所有單號
      var wmsNos = _f060101List.Select(x => x.WMS_NO).ToList();

      // 取得調撥單資料
      _f151001List = f151001Repo.GetDatas(dcCode, gupCode, custCode, wmsNos).ToList();

      // 取得調撥單明細資料
      _f151002List = f151002Repo.GetDatas(dcCode, gupCode, custCode, wmsNos).ToList();

      // 取得上架人名資料
      var f1924s = f1924Repo.GetDatasForEmpIds(receiptList.Where(x => !string.IsNullOrWhiteSpace(x.Operator)).Select(x => x.Operator).ToList());


      #endregion

      #region Foreach [入庫單列表]
      var res = new ApiResult();
      List<ApiResponse> data = new List<ApiResponse>();
      List<string> successedReceiptCodes = new List<string>();

      // Foreach[入庫單列表] in <參數4> 
      receiptList.ForEach(receipt =>
      {
        var currRes = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_IW, dcCode, gupCode, custCode, "InWarehouseReceipt_Record", receipt, () =>
        {
          if (!string.IsNullOrWhiteSpace(receipt.ReceiptCode) && successedReceiptCodes.Contains(receipt.ReceiptCode))
          {
            var sameWmsNoErrRes = new List<ApiResponse> { new ApiResponse { No = receipt.ReceiptCode, MsgCode = "99999", MsgContent = "該單號重複處理" } };
            data.AddRange(sameWmsNoErrRes);
            return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = "該單號重複處理", Data = sameWmsNoErrRes };
          }
          else
          {
            var f060101 = _f060101List.Where(x => x.DOC_ID == receipt.ReceiptCode).FirstOrDefault();

            var wmsNo = f060101 != null ? f060101.WMS_NO : string.Empty;

            // 入庫單檢核
            var res1 = CheckInWarehouseReceipt(dcCode, gupCode, custCode, wmsNo, receipt);
            if (!res1.IsSuccessed)
            {
              var currDatas = (List<ApiResponse>)res1.Data;
              data.AddRange(currDatas);

              if (_IsAddF075105DocIdList.Contains(receipt.ReceiptCode))
              {
                f075105Repo.DelF075105ByKey(dcCode, receipt.ReceiptCode);
                _IsAddF075105DocIdList = _IsAddF075105DocIdList.Where(x => x != receipt.ReceiptCode).ToList();
              }
              wmsTransation.Complete();
              return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = "檢核失敗", Data = currDatas };
            }
            else
            {


              var f060102Repo = new F060102Repository(Schemas.CoreSchema, wmsTransation);
              var sharedService = new SharedService(wmsTransation);
              var details = new List<AllocationConfirmDetail>();

              if (receipt.Status == 0)   // 派發失敗
              {
                // 將原F060101.STATUS更新為4(中介回傳派發失敗)
                f060101.STATUS = "4";
                f060101Repo.Update(f060101);
                // 新增一筆新的F060101但DOC_ID產生新的編號，狀態為0
                sharedService.CreateF060101(f060101.DC_CODE, f060101.GUP_CODE, f060101.CUST_CODE, wmsNo, f060101.WAREHOUSE_ID);
              }
              else if (receipt.Status == 4)   // 上架完成
              {
                #region 更新調撥明細檔 F151002 & 新增調撥缺貨資料 F151003
                var f151002s = _f151002List.Where(x => x.ALLOCATION_NO == wmsNo).ToList();

                receipt.SkuList.ForEach(sku =>
                {
                  #region 更新調撥明細檔 F151002
                  var f151002 = f151002s.Where(x => x.ALLOCATION_SEQ == sku.RowNum).FirstOrDefault();

                  if (f151002 != null)
                  {
                    details.Add(new AllocationConfirmDetail
                    {
                      Seq = f151002.ALLOCATION_SEQ,
                      TarLocCode = f151002.TAR_LOC_CODE,
                      Qty = Convert.ToInt32(sku.SkuQty)
                    });
                  }
                  #endregion

                  #region 新增調撥單明細貨架資料 F060102
                  sku.ShelfBinList.ForEach(shelfBin =>
                  {
                    f060102Repo.Add(new F060102
                    {
                      DC_CODE = dcCode,
                      GUP_CODE = gupCode,
                      CUST_CODE = custCode,
                      ALLOCATION_NO = wmsNo,
                      ALLOCATION_SEQ = Convert.ToInt32(sku.RowNum),
                      ITEM_CODE = sku.SkuCode,
                      SHELF_CODE = shelfBin.ShelfCode,
                      BIN_CODE = shelfBin.BinCode,
                      SKU_QTY = Convert.ToInt32(shelfBin.SkuQty),
                      OPERATOR = shelfBin.Operator
                    });
                  });
                  #endregion
                });

                var param = new AllocationConfirmParam
                {
                  DcCode = dcCode,
                  GupCode = gupCode,
                  CustCode = custCode,
                  AllocNo = f060101.WMS_NO,
                  Operator = receipt.Operator,
                  Details = details
                };
                sharedService.AllocationConfirm(param);
                #endregion
              }

              wmsTransation.Complete();

              // 記錄成功的單號，用以再有同單號的不處理
              successedReceiptCodes.Add(receipt.ReceiptCode);
              return new ApiResult { IsSuccessed = true };
            }
          }
        });

        if (!currRes.IsSuccessed && currRes.MsgCode == "99999") //執行時有例外
          data.Add(new ApiResponse { MsgCode = "99999", MsgContent = currRes.MsgContent, No = receipt.ReceiptCode });

      });
      #endregion

      #region 組回傳資料
      res.SuccessCnt = successedReceiptCodes.Count;
      res.FailureCnt = receiptList.Count - successedReceiptCodes.Count;
      res.TotalCnt = receiptList.Count;

      res.IsSuccessed = !data.Any();
      res.MsgCode = "10005";
      res.MsgContent = string.Format(tacService.GetMsg("10005"),
          "入庫完成結果回傳",
          res.SuccessCnt,
          res.FailureCnt,
          res.TotalCnt);

      res.Data = data.Any() ? data : null;
      #endregion

      return res;
    }


    /// <summary>
    /// 資料處理2
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="wmsNo"></param>
    /// <param name="receipt"></param>
    /// <returns></returns>
    private ApiResult CheckInWarehouseReceipt(string dcCode, string gupCode, string custCode, string wmsNo, InWarehouseReceiptModel receipt)
    {
      ApiResult result = new ApiResult();
      List<ApiResponse> data = new List<ApiResponse>();

      // 預設值設定
      data.AddRange((List<ApiResponse>)CheckDefaultSetting(dcCode, gupCode, custCode, wmsNo, receipt).Data);

      // 共用欄位格式檢核
      data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(dcCode, gupCode, custCode, wmsNo, receipt).Data);

      // 貨主自訂欄位格式檢核
      data.AddRange((List<ApiResponse>)CheckCustomColumnType(dcCode, gupCode, custCode, wmsNo, receipt).Data);

      // 如果以上檢核成功
      if (!data.Any())
      {
        // 共用欄位資料檢核
        data.AddRange((List<ApiResponse>)CheckCommonColumnData(dcCode, gupCode, custCode, wmsNo, receipt).Data);

        // 貨主自訂欄位資料檢核
        data.AddRange((List<ApiResponse>)CheckCustomColumnValue(dcCode, gupCode, custCode, wmsNo, receipt).Data);

        // 如果以上檢核失敗
        if (!data.Any())
          _failCnt++;
      }
      else
        _failCnt++;

      result.IsSuccessed = !data.Any();
      result.Data = data;

      return result;
    }
    #endregion

    #region Protected 檢核
    /// <summary>
    /// 預設值設定
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="wmsNo"></param>
    /// <param name="receipt"></param>
    /// <returns></returns>
    protected ApiResult CheckDefaultSetting(string dcCode, string gupCode, string custCode, string wmsNo, InWarehouseReceiptModel receipt)
    {
      // 請預留方法
      ApiResult res = new ApiResult();
      res.Data = new List<ApiResponse>();
      return res;
    }

    /// <summary>
    /// 共用欄位格式檢核
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="wmsNo"></param>
    /// <param name="receipt"></param>
    /// <returns></returns>
    protected ApiResult CheckColumnNotNullAndMaxLength(string dcCode, string gupCode, string custCode, string wmsNo, InWarehouseReceiptModel receipt)
    {
      TransApiBaseService tacService = new TransApiBaseService();
      ApiResult res = new ApiResult();
      List<ApiResponse> data = new List<ApiResponse>();
      string nullErrorMsg = tacService.GetMsg("20058");
      string formatErrorMsg = tacService.GetMsg("20059");

      #region 檢查入庫單欄位必填、最大長度
      List<string> notDateColumn = new List<string>();

      // 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
      receiptCheckColumnList.ForEach(column =>
      {
        // 必填
        if (!column.Nullable)
          if (!DataCheckHelper.CheckRequireColumn(receipt, column.Name))
            data.Add(new ApiResponse { No = receipt.ReceiptCode, ErrorColumn = column.Name, MsgCode = "20058", MsgContent = string.Format(nullErrorMsg, receipt.ReceiptCode, column.Name) });

        // 最大長度
        if (column.MaxLength > 0)
          if (!DataCheckHelper.CheckDataMaxLength(receipt, column.Name, column.MaxLength))
            data.Add(new ApiResponse { No = receipt.ReceiptCode, ErrorColumn = column.Name, MsgCode = "20059", MsgContent = string.Format(formatErrorMsg, receipt.ReceiptCode, column.Name) });
      });

      if (!receipt.SkuList.Any())
        data.Add(new ApiResponse { No = receipt.ReceiptCode, ErrorColumn = "SkuList", MsgCode = "20058", MsgContent = string.Format(nullErrorMsg, receipt.ReceiptCode, "SkuList") });
      #endregion

      #region 檢查明細欄位必填、最大長度
      if (receipt.SkuList.Any())
      {
        for (int i = 0; i < receipt.SkuList.Count; i++)
        {
          var currSku = receipt.SkuList[i];

          skuCheckColumnList.ForEach(o =>
          {
            // 必填
            if (!o.Nullable)
              if (!DataCheckHelper.CheckRequireColumn(currSku, o.Name))
                data.Add(new ApiResponse { No = receipt.ReceiptCode, ErrorColumn = o.Name, MsgCode = "20058", MsgContent = string.Format(nullErrorMsg, $"{receipt.ReceiptCode}第{i + 1}筆明細", o.Name) });

            // 最大長度
            if (o.MaxLength > 0)
              if (!DataCheckHelper.CheckDataMaxLength(currSku, o.Name, o.MaxLength))
                data.Add(new ApiResponse { No = receipt.ReceiptCode, ErrorColumn = o.Name, MsgCode = "20059", MsgContent = string.Format(formatErrorMsg, $"{receipt.ReceiptCode}第{i + 1}筆明細", o.Name) });
          });

          #region 檢驗儲位
          for (int j = 0; j < currSku.ShelfBinList.Count; j++)
          {
            var currShelfBin = currSku.ShelfBinList[j];

            shelfBinCheckColumnList.ForEach(p =>
            {
              // 必填
              if (!p.Nullable)
                if (!DataCheckHelper.CheckRequireColumn(currShelfBin, p.Name))
                  data.Add(new ApiResponse { No = receipt.ReceiptCode, ErrorColumn = p.Name, MsgCode = "20058", MsgContent = string.Format(nullErrorMsg, $"{receipt.ReceiptCode}第{i + 1}筆明細第{j + 1}筆儲位", p.Name) });

              // 最大長度
              if (p.MaxLength > 0)
                if (!DataCheckHelper.CheckDataMaxLength(currShelfBin, p.Name, p.MaxLength))
                  data.Add(new ApiResponse { No = receipt.ReceiptCode, ErrorColumn = p.Name, MsgCode = "20059", MsgContent = string.Format(formatErrorMsg, $"{receipt.ReceiptCode}第{i + 1}筆明細第{j + 1}筆儲位", p.Name) });
            });
          }
          #endregion
        }
      }
      #endregion

      res.Data = data;

      return res;
    }

    /// <summary>
    /// 貨主自訂欄位格式檢核
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="wmsNo"></param>
    /// <param name="receipt"></param>
    /// <returns></returns>
    protected ApiResult CheckCustomColumnType(string dcCode, string gupCode, string custCode, string wmsNo, InWarehouseReceiptModel receipt)
    {
      // 請預留方法
      ApiResult res = new ApiResult();
      res.Data = new List<ApiResponse>();
      return res;
    }

    /// <summary>
    /// 共用欄位資料檢核
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="wmsNo"></param>
    /// <param name="receipt"></param>
    /// <returns></returns>
    protected ApiResult CheckCommonColumnData(string dcCode, string gupCode, string custCode, string wmsNo, InWarehouseReceiptModel receipt)
    {
      CheckInWarehouseReceiptService ciwService = new CheckInWarehouseReceiptService();
      ApiResult res = new ApiResult();
      List<ApiResponse> data = new List<ApiResponse>();

      #region 入庫單欄位資料檢核

      // 檢查資料庫任務單號是否存在
      var isAdd = ciwService.CheckDocExist(data, receipt, dcCode);
      if (isAdd)
        _IsAddF075105DocIdList.Add(receipt.ReceiptCode);

      // 檢查入庫單狀態
      ciwService.CheckStatus(data, receipt);

      // 檢查是否異常
      ciwService.CheckIsException(data, receipt);

      // 檢查單號狀態
      ciwService.CheckReceiptCodeData(data, wmsNo, receipt, _f151001List);

      // 檢查品項數與明細筆數是否相同
      ciwService.CheckSkuTotalEqualDetailCnt(data, receipt);

      // 檢查ROWNUM 跟F151002.SEQ是否一致
      ciwService.CheckRowNumEqualSeq(data, wmsNo, receipt, _f151002List);

      // 檢查SkuCode 跟F151002.ITEM_CODE是否一致
      ciwService.CheckSkuCodeEqualItemCode(data, wmsNo, receipt, _f151002List);

      #endregion

      #region 明細欄位資料檢核

      for (int index = 0; index < receipt.SkuList.Count; index++)
      {
        var sku = receipt.SkuList[index];

        // 檢查[入庫標示]是否設定錯誤
        ciwService.CheckReceiptFlag(data, sku, receipt.ReceiptCode, index);

        // 檢查[商品等級]是否設定錯誤
        ciwService.CheckSkuLevel(data, sku, receipt.ReceiptCode, index);

        // 檢查[實際入庫數量]是否有大於0
        ciwService.CheckSkuQty(data, sku, receipt.ReceiptCode, index);

        // 檢查[實際入庫數量]是否大於[預計入庫量]
        ciwService.CheckSkuQtyAndSkuPlanQty(data, sku, receipt.ReceiptCode, index);

        // 檢查[實際入庫數量]、[預計入庫數量]是否超過調撥單上架數
        ciwService.CheckSkuQtyAndSkuPlanQtyExceedTarQty(data, wmsNo, sku, receipt.ReceiptCode, index, _f151002List);
      }
      #endregion

      res.Data = data;

      return res;
    }

    /// <summary>
    /// 貨主自訂欄位資料檢核
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="wmsNo"></param>
    /// <param name="receipt"></param>
    /// <returns></returns>
    protected ApiResult CheckCustomColumnValue(string dcCode, string gupCode, string custCode, string wmsNo, InWarehouseReceiptModel receipt)
    {
      // 請預留方法
      ApiResult res = new ApiResult();
      res.Data = new List<ApiResponse>();
      return res;
    }
    #endregion
  }
}
