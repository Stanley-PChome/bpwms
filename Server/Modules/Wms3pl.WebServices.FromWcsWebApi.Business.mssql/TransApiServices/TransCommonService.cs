using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Services;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.TransApiServices
{
  public class TransCommonService : BaseService
  {
    /// <summary>
    /// 入庫完成結果回傳
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public WcsApiResult<WcsInWarehouseReceiptApiDataResult> InWarehouseReceipt(InWarehouseReceiptReq req)
    {
      #region 變數
      WcsApiResult<WcsInWarehouseReceiptApiDataResult> res = new WcsApiResult<WcsInWarehouseReceiptApiDataResult>();
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();
      #endregion

      #region 主程式
      // 連線檢核是否主機授權碼錯誤
      if (!DbSchemaHelper.CheckSCCode())
        return new WcsApiResult<WcsInWarehouseReceiptApiDataResult> { Code = "20057", Msg = tacService.GetMsg("20057") };

      // 啟動紀錄
      var result = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_IW, req == null ? "00" : req.DcCode, req == null ? "00" : commonService.GetGupCode(req.OwnerCode), req == null ? "00" : req.OwnerCode, "InWarehouseReceipt", req, () =>
      {
        CommonInWarehouseReceiptService cwiService = new CommonInWarehouseReceiptService();
        return cwiService.RecevieApiDatas(req);
      });

      // ApiResult To WcsApiResult
      var apiResponses = ((List<ApiResponse>)result.Data);
      res.Code = result.IsSuccessed ? "200" : result.MsgCode;
      res.Msg = WcsConvertMessage(result.MsgContent);
      res.Data = result.IsSuccessed || apiResponses == null || (apiResponses != null && !apiResponses.Any()) ?
          null :
          apiResponses.Select(x => new WcsInWarehouseReceiptApiDataResult
          {
            ReceiptCode = x.No,
            ErrorColumn = x.ErrorColumn,
            errors = new WcsApiErrorResult { MsgCode = x.MsgCode, MsgContent = x.MsgContent }
          }).ToList();

      return res;
      #endregion
    }

    /// <summary>
    /// 出庫完成結果回傳
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public WcsApiResult<WcsApiOutWarehouseReceiptResultData> OutWarehouseReceipt(OutWarehouseReceiptReq req)
    {
      #region 變數
      WcsApiResult<WcsApiOutWarehouseReceiptResultData> res = new WcsApiResult<WcsApiOutWarehouseReceiptResultData>();
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();
      #endregion

      #region 主程式
      // 連線檢核是否主機授權碼錯誤
      if (!DbSchemaHelper.CheckSCCode())
        return new WcsApiResult<WcsApiOutWarehouseReceiptResultData> { Code = "20057", Msg = tacService.GetMsg("20057") };

      // 啟動紀錄
      var result = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_OW, req == null ? "00" : req.DcCode, req == null ? "00" : commonService.GetGupCode(req.OwnerCode), req == null ? "00" : req.OwnerCode, "OutWarehouseReceipt", req, () =>
      {
        CommonOutWarehouseReceiptService cwiService = new CommonOutWarehouseReceiptService();
        return cwiService.RecevieApiDatas(req);
      });

      // ApiResult To WcsApiResult
      var apiResponses = ((List<ApiResponse>)result.Data);
      res.Code = result.IsSuccessed ? "200" : result.MsgCode;
      res.Msg = WcsConvertMessage(result.MsgContent);
      res.Data = result.IsSuccessed || apiResponses == null || (apiResponses != null && !apiResponses.Any()) ?
          null :
          apiResponses.Select(x => new WcsApiOutWarehouseReceiptResultData
          {
            OrderCode = x.No,
            ErrorColumn = x.ErrorColumn,
            errors = new WcsApiErrorResult { MsgCode = x.MsgCode, MsgContent = x.MsgContent }
          }).ToList();

      return res;
      #endregion
    }

    /// <summary>
    /// 盤點完成結果回傳
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public WcsApiResult<WcsApiInventoryReceiptResultData> InventoryReceipt(InventoryReceiptReq req)
    {
      #region 變數
      WcsApiResult<WcsApiInventoryReceiptResultData> res = new WcsApiResult<WcsApiInventoryReceiptResultData>();
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();
      #endregion

      #region 主程式
      // 連線檢核是否主機授權碼錯誤
      if (!DbSchemaHelper.CheckSCCode())
        return new WcsApiResult<WcsApiInventoryReceiptResultData> { Code = "20057", Msg = tacService.GetMsg("20057") };

      // 啟動紀錄
      var result = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_IT, req == null ? "00" : req.DcCode, req == null ? "00" : commonService.GetGupCode(req.OwnerCode), req == null ? "00" : req.OwnerCode, "InventoryReceipt", req, () =>
      {
        CommonInventoryReceiptService cwiService = new CommonInventoryReceiptService();
        return cwiService.RecevieApiDatas(req);
      });

      // ApiResult To WcsApiResult
      var apiResponses = ((List<ApiResponse>)result.Data);
      res.Code = result.IsSuccessed ? "200" : result.MsgCode;
      res.Msg = WcsConvertMessage(result.MsgContent);
      res.Data = result.IsSuccessed || apiResponses == null || (apiResponses != null && !apiResponses.Any()) ?
          null :
          apiResponses.Select(x => new WcsApiInventoryReceiptResultData
          {
            CheckCode = x.No,
            ErrorColumn = x.ErrorColumn,
            errors = new WcsApiErrorResult { MsgCode = x.MsgCode, MsgContent = x.MsgContent }
          }).ToList();

      return res;
      #endregion
    }

    /// <summary>
    /// 盤點調整完成結果回傳
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public WcsApiResult<WcsApiInventoryAdjustReceiptResultData> InventoryAdjustReceipt(InventoryAdjustReceiptReq req)
    {
      #region 變數
      WcsApiResult<WcsApiInventoryAdjustReceiptResultData> res = new WcsApiResult<WcsApiInventoryAdjustReceiptResultData>();
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();
      #endregion

      #region 主程式
      // 連線檢核是否主機授權碼錯誤
      if (!DbSchemaHelper.CheckSCCode())
        return new WcsApiResult<WcsApiInventoryAdjustReceiptResultData> { Code = "20057", Msg = tacService.GetMsg("20057") };

      // 啟動紀錄
      var result = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_IA, req == null ? "00" : req.DcCode, req == null ? "00" : commonService.GetGupCode(req.OwnerCode), req == null ? "00" : req.OwnerCode, "InventoryAdjustReceipt", req, () =>
      {
        CommonInventoryAdjustReceiptService cwiService = new CommonInventoryAdjustReceiptService();
        return cwiService.RecevieApiDatas(req);
      });

      // ApiResult To WcsApiResult
      var apiResponses = ((List<ApiResponse>)result.Data);
      res.Code = result.IsSuccessed ? "200" : result.MsgCode;
      res.Msg = WcsConvertMessage(result.MsgContent);
      res.Data = result.IsSuccessed || apiResponses == null || (apiResponses != null && !apiResponses.Any()) ?
          null :
          apiResponses.Select(x => new WcsApiInventoryAdjustReceiptResultData
          {
            AdjustCode = x.No,
            ErrorColumn = x.ErrorColumn,
            errors = new WcsApiErrorResult { MsgCode = x.MsgCode, MsgContent = x.MsgContent }
          }).ToList();

      return res;
      #endregion
    }

    /// <summary>
    /// 每日庫存快照回傳
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public WcsApiResult<WcsApiSnapshotStocksReceiptResultData> SnapshotStocksReceipt(SnapshotStocksReceiptReq req)
    {
      #region 變數
      WcsApiResult<WcsApiSnapshotStocksReceiptResultData> res = new WcsApiResult<WcsApiSnapshotStocksReceiptResultData>();
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();
      #endregion

      #region 主程式
      // 連線檢核是否主機授權碼錯誤
      if (!DbSchemaHelper.CheckSCCode())
        return new WcsApiResult<WcsApiSnapshotStocksReceiptResultData> { Code = "20057", Msg = tacService.GetMsg("20057") };

      // 啟動紀錄
      var result = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_SS, req == null ? "00" : req.DcCode, "00", "00", "SnapshotStocksReceipt", req, () =>
      {
        CommonSnapshotStocksReceiptService cwiService = new CommonSnapshotStocksReceiptService();
        return cwiService.RecevieApiDatas(req);
      });

      // ApiResult To WcsApiResult
      var apiResponses = ((List<ApiResponse>)result.Data);
      res.Code = result.IsSuccessed ? "200" : result.MsgCode;
      res.Msg = WcsConvertMessage(result.MsgContent);
      res.Data = result.IsSuccessed || apiResponses == null || (apiResponses != null && !apiResponses.Any()) ?
          null :
          apiResponses.Select(x => new WcsApiSnapshotStocksReceiptResultData
          {
            Index = string.IsNullOrWhiteSpace(x.No) ? default(int?) : Convert.ToInt32(x.No),
            ErrorColumn = x.ErrorColumn,
            errors = new WcsApiErrorResult { MsgCode = x.MsgCode, MsgContent = x.MsgContent }
          }).ToList();

      return res;
      #endregion
    }

    /// <summary>
    /// 分揀出貨資訊回報
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public WcsApiResult<WcsApiShipToDebitReceiptResultData> ShipToDebitReceipt(ShipToDebitReceiptReq req)
    {
      #region 變數
      WcsApiResult<WcsApiShipToDebitReceiptResultData> res = new WcsApiResult<WcsApiShipToDebitReceiptResultData>();
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();
      #endregion

      #region 主程式
      // 連線檢核是否主機授權碼錯誤
      if (!DbSchemaHelper.CheckSCCode())
        return new WcsApiResult<WcsApiShipToDebitReceiptResultData> { Code = "20057", Msg = tacService.GetMsg("20057") };

      // 啟動紀錄
      var result = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_SDB, req == null ? "00" : req.DcCode, "00", "00", "WcsShipToDebit", req, () =>
      {
        CommonShipToDebitReceiptService cwiService = new CommonShipToDebitReceiptService();
        return cwiService.RecevieApiDatas(req);
      });

      // ApiResult To WcsApiResult
      var apiResponses = ((List<ApiResponse>)result.Data);
      res.Code = result.IsSuccessed ? "200" : result.MsgCode;
      res.Msg = WcsConvertMessage(result.MsgContent);
      res.Data = result.IsSuccessed || apiResponses == null || (apiResponses != null && !apiResponses.Any()) ?
          null :
          apiResponses.Select(x => new WcsApiShipToDebitReceiptResultData
          {
            ShipCode = x.No,
            ErrorColumn = x.ErrorColumn,
            errors = new WcsApiErrorResult { MsgCode = x.MsgCode, MsgContent = x.MsgContent }
          }).ToList();

      return res;
      #endregion
    }

    /// <summary>
    /// 容器釋放查詢
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public WcsApiResult<WcsApiSearchUsableContainerReceiptResultData> SearchUsableContainerReceipt(SearchUsableContainerReceiptReq req)
    {
      #region 變數
      WcsApiResult<WcsApiSearchUsableContainerReceiptResultData> res = new WcsApiResult<WcsApiSearchUsableContainerReceiptResultData>();
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();
      #endregion

      #region 主程式
      // 連線檢核是否主機授權碼錯誤
      if (!DbSchemaHelper.CheckSCCode())
        return new WcsApiResult<WcsApiSearchUsableContainerReceiptResultData> { Code = "20057", Msg = tacService.GetMsg("20057") };

      // 啟動紀錄
      var result = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_CRQ, req == null ? "0" : req.DcCode, "0", "0", "SearchUsableContainerReceipt", req, () =>
      {
        CommonSearchUsableContainerReceiptService cwiService = new CommonSearchUsableContainerReceiptService();
        return cwiService.RecevieApiDatas(req);
      });

      // ApiResult To WcsApiResult
      var apiResponses = ((List<ApiResponse>)result.Data);
      res.Code = result.IsSuccessed ? "200" : result.MsgCode;
      res.Msg = WcsConvertMessage(result.MsgContent);
      res.Data = apiResponses == null || (apiResponses != null && !apiResponses.Any()) ?
          null :
          apiResponses.Select(x => new WcsApiSearchUsableContainerReceiptResultData
          {
            ContainerCode = x.No,
            Status = x.Status,
            ErrorColumn = x.ErrorColumn,
            errors = new WcsApiErrorResult { MsgCode = x.MsgCode, MsgContent = x.MsgContent }
          }).ToList();

      return res;
      #endregion
    }

    /// <summary>
    /// 容器位置回報
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public WcsApiResult<WcsApiContainerPositionReceiptResultData> ContainerPositionReceipt(ContainerPositionReceiptReq req)
    {
      #region 變數
      WcsApiResult<WcsApiContainerPositionReceiptResultData> res = new WcsApiResult<WcsApiContainerPositionReceiptResultData>();
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();
      #endregion

      #region 主程式
      // 連線檢核是否主機授權碼錯誤
      if (!DbSchemaHelper.CheckSCCode())
        return new WcsApiResult<WcsApiContainerPositionReceiptResultData> { Code = "20057", Msg = tacService.GetMsg("20057") };

      // 啟動紀錄
      var result = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_F009003, req == null ? "0" : req.DcCode, "0", "0", "ContainerPositionReceipt", req, () =>
      {
        CommonContainerPositionReceiptService cwiService = new CommonContainerPositionReceiptService();
        return cwiService.RecevieApiDatas(req);
      });

      // ApiResult To WcsApiResult
      var apiResponses = ((List<ApiResponse>)result.Data);
      res.Code = result.IsSuccessed ? "200" : result.MsgCode;
      res.Msg = WcsConvertMessage(result.MsgContent);
      res.Data = result.IsSuccessed || apiResponses == null || (apiResponses != null && !apiResponses.Any()) ?
          null :
          apiResponses.Select(x => new WcsApiContainerPositionReceiptResultData
          {
            ContainerCode = x.No,
            ErrorColumn = x.ErrorColumn,
            errors = new WcsApiErrorResult { MsgCode = x.MsgCode, MsgContent = x.MsgContent }
          }).ToList();

      return res;
      #endregion
    }


    /// <summary>
    /// 補貨超揀申請
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public WcsApiResult<WcsApiOverPickApplyReceiptResultData> OverPickApply(OverPickApplyReq req)
    {
      #region 變數
      WcsApiResult<WcsApiOverPickApplyReceiptResultData> res = new WcsApiResult<WcsApiOverPickApplyReceiptResultData>();
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();
      #endregion

      #region 主程式
      // 連線檢核是否主機授權碼錯誤
      if (!DbSchemaHelper.CheckSCCode())
        return new WcsApiResult<WcsApiOverPickApplyReceiptResultData> { Code = "20057", Msg = tacService.GetMsg("20057") };

      // 啟動紀錄
      var result = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_OPA, req == null ? "00" : req.DcCode, req == null ? "00" : commonService.GetGupCode(req.OwnerCode), req == null ? "00" : req.OwnerCode, "WcsOverPickApply", req, () =>
      {
        CommonOverPickApplyReceiptService cwiService = new CommonOverPickApplyReceiptService();
        return cwiService.RecevieApiDatas(req);
      });

      // ApiResult To WcsApiResult
      var apiResponses = ((List<ApiResponse>)result.Data);
      res.Code = result.IsSuccessed ? "200" : result.MsgCode;
      res.Msg = WcsConvertMessage(result.MsgContent);
      res.Data = result.IsSuccessed || apiResponses == null || (apiResponses != null && !apiResponses.Any()) ?
              null :
              apiResponses.Select(x => new WcsApiOverPickApplyReceiptResultData
              {
                ErrorColumn = x.ErrorColumn,
                errors = new WcsApiErrorResult { MsgCode = x.MsgCode, MsgContent = x.MsgContent }
              }).ToList();

      return res;
      #endregion
    }


    /// <summary>
    /// 出庫結果回報(按箱)
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public WcsApiResult<WcsApiOutWarehouseContainerReceiptResultData> OutWarehouseContainerReceipt(OutWarehouseContainerReceiptReq req)
    {
      #region 變數
      WcsApiResult<WcsApiOutWarehouseContainerReceiptResultData> res = new WcsApiResult<WcsApiOutWarehouseContainerReceiptResultData>();
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();
      #endregion

      #region 主程式
      // 連線檢核是否主機授權碼錯誤
      if (!DbSchemaHelper.CheckSCCode())
        return new WcsApiResult<WcsApiOutWarehouseContainerReceiptResultData> { Code = "20057", Msg = tacService.GetMsg("20057") };

      // 啟動紀錄
      var result = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_F009003, req == null ? "00" : req.DcCode, "00", "00", "OutWarehouseContainerReceipt", req, () =>
      {
        CommonOutWarehouseContainerReceiptService cwiService = new CommonOutWarehouseContainerReceiptService();
        return cwiService.RecevieApiDatas(req);
      });

      // ApiResult To WcsApiResult
      var apiResponses = ((List<ApiResponse>)result.Data);
      res.Code = result.IsSuccessed ? "200" : result.MsgCode;
      res.Msg = WcsConvertMessage(result.MsgContent);
      res.Data = result.IsSuccessed || apiResponses == null || (apiResponses != null && !apiResponses.Any()) ?
          null :
          apiResponses.Select(x => new WcsApiOutWarehouseContainerReceiptResultData
          {
            ContainerCode = x.No,
            ErrorColumn = x.ErrorColumn,
            errors = new WcsApiErrorResult { MsgCode = x.MsgCode, MsgContent = x.MsgContent }
          }).ToList();

      return res;
      #endregion
    }

    /// <summary>
    /// 儲位異常回報(A7)
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public WcsApiResult<WcsApiAutoLocAbnormalNotifyResultData> AutoLocAbnormalNotify(AutoLocAbnormalNotifyReq req)
    {
      #region 變數
      WcsApiResult<WcsApiAutoLocAbnormalNotifyResultData> res = new WcsApiResult<WcsApiAutoLocAbnormalNotifyResultData>();
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();
      #endregion 變數

      #region 主程式
      // 連線檢核是否主機授權碼錯誤
      if (!DbSchemaHelper.CheckSCCode())
        return new WcsApiResult<WcsApiAutoLocAbnormalNotifyResultData> { Code = "20057", Msg = tacService.GetMsg("20057") };

      var result = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_F009004, req == null ? "00" : req.DcCode, "00", "00", "WcsAutoLocAbnormalNotify", req, () =>
      {
        CommonAutoLocAbnormalNotifyService cwiService = new CommonAutoLocAbnormalNotifyService();
        return cwiService.AutoLocAbnormalNotify(req);
      });

      // ApiResult To WcsApiResult
      var apiResponses = ((List<ApiResponse>)result.Data);
      res.Code = result.IsSuccessed ? "200" : result.MsgCode;
      res.Msg = WcsConvertMessage(result.MsgContent);
      res.Data = result.IsSuccessed || apiResponses == null || (apiResponses != null && !apiResponses.Any()) ?
              null :
              apiResponses.Select(x => new WcsApiAutoLocAbnormalNotifyResultData
              {
                ErrorColumn = x.ErrorColumn,
                errors = new WcsApiErrorResult { MsgCode = x.MsgCode, MsgContent = x.MsgContent }
              }).ToList();

      return res;

      #endregion 主程式
    }

    /// <summary>
    /// 分揀機異常回報(A7)
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public WcsApiResult<WcsApiSorterAbnormalNotifyResultData> SorterAbnormalNotify(SorterAbnormalNotifyReq req)
    {
      #region 變數
      WcsApiResult<WcsApiSorterAbnormalNotifyResultData> res = new WcsApiResult<WcsApiSorterAbnormalNotifyResultData>();
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();
      #endregion

      #region 主程式
      // 連線檢核是否主機授權碼錯誤
      if (!DbSchemaHelper.CheckSCCode())
        return new WcsApiResult<WcsApiSorterAbnormalNotifyResultData> { Code = "20057", Msg = tacService.GetMsg("20057") };

      var result = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_F009004, req == null ? "00" : req.DcCode, "00", "00", "WcsSorterAbnormalNotify", req, () =>
      {
        CommonSorterAbnormalNotifyService cwiService = new CommonSorterAbnormalNotifyService();
        return cwiService.SorterAbnormalNotify(req);
      });
      // ApiResult To WcsApiResult
      var apiResponses = ((List<ApiResponse>)result.Data);
      res.Code = result.IsSuccessed ? "200" : result.MsgCode;
      res.Msg = WcsConvertMessage(result.MsgContent);
      res.Data = result.IsSuccessed || apiResponses == null || (apiResponses != null && !apiResponses.Any()) ?
          null :
          apiResponses.Select(x => new WcsApiSorterAbnormalNotifyResultData
          {
            ErrorColumn = x.ErrorColumn,
            errors = new WcsApiErrorResult { MsgCode = x.MsgCode, MsgContent = x.MsgContent }
          }).ToList();

      return res;
      #endregion 主程式
    }

    /// <summary>
    /// 包裝完成回報(12)
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public WcsApiResult<WcsApiPackageFinishResultData> PackageFinish(PackageFinishReq req)
    {
      #region 變數
      WcsApiResult<WcsApiPackageFinishResultData> res = new WcsApiResult<WcsApiPackageFinishResultData>();
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();
      #endregion

      #region 主程式
      // 連線檢核是否主機授權碼錯誤
      if (!DbSchemaHelper.CheckSCCode())
        return new WcsApiResult<WcsApiPackageFinishResultData> { Code = "20057", Msg = tacService.GetMsg("20057") };

      var result = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_F009004, req == null ? "00" : req.DcCode, "00", "00", "WcsPackageFinish", req, () =>
      {
        CommonPackageFinishService srv = new CommonPackageFinishService();
        return srv.PackageFinish(req);
      });

      // ApiResult To WcsApiResult
      var apiResponses = (List<ApiResponse>)result.Data;
      res.Code = result.IsSuccessed ? "200" : result.MsgCode;
      res.Msg = WcsConvertMessage(result.MsgContent);
      res.Data = result.IsSuccessed || apiResponses == null || (apiResponses != null && !apiResponses.Any()) ? null :
        apiResponses.Select(x => new WcsApiPackageFinishResultData
        {
          No = x.No,
          ErrorColumn = x.ErrorColumn,
          errors = new WcsApiErrorResult { MsgCode = x.MsgCode, MsgContent = x.MsgContent }
        }).ToList();

      return res;
      #endregion 主程式
    }

  }
}
