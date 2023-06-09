using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;
using Wms3pl.WebServices.Shared.TransApiServices.Common;

namespace Wms3pl.WebServices.WebApi.Business.TransApiServices
{
  public class TransCommonService
  {
    private WmsTransaction _wmsTransation;
    public TransCommonService(WmsTransaction wmsTransation)
    {
      _wmsTransation = wmsTransation;
    }

    /// <summary>
    /// 批次商品進倉資料
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult PostCreateWarehouses(PostCreateWarehousesReq req)
    {
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();
      ApiResult res = new ApiResult();

      // 連線檢核
      if (DbSchemaHelper.CheckSCCode())
      {
        // 啟動紀錄
        res = ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSAPI_WI, req.DcCode, commonService.GetGupCode(req.CustCode), req.CustCode, "PostCreateWarehouses", req, () =>
        {
          CommonWarehouseInsService cwiService = new CommonWarehouseInsService(_wmsTransation);
          return cwiService.RecevieApiDatas(req);
        });
      }
      else
      {
        // 主機授權碼錯誤
        res = new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = tacService.GetMsg("20057") };
      }

      return res;
    }

    /// <summary>
    /// 批次新增訂單資料
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult PostCreateOrders(PostCreateOrdersReq req)
    {
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();
      ApiResult res = new ApiResult { IsSuccessed = true };

      // 連線檢核
      if (DbSchemaHelper.CheckSCCode())
      {
        // 啟動紀錄
        res = ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSAPI_OD, req.DcCode, commonService.GetGupCode(req.CustCode), req.CustCode, "PostCreateOrders", req, () =>
        {
          CommonOrderService cwiService = new CommonOrderService(_wmsTransation);
          return cwiService.RecevieApiDatas(req);
        });
      }
      else
      {
        // 主機授權碼錯誤
        res = new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = tacService.GetMsg("20057") };
      }

      return res;
    }

    /// <summary>
    /// 批次客戶退貨單資料
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult PostCreateReturns(PostCreateReturnsReq req)
    {
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();

      // 檢核主機授權碼
      if (!DbSchemaHelper.CheckSCCode())
      {
        return new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = tacService.GetMsg("20057") };
      }
    

      // 啟動紀錄
      return ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSAPI_CR, req.DcCode, commonService.GetGupCode(req.CustCode), req.CustCode, "PostCreateReturns", req, () =>
      {
        CommonReturnService crService = new CommonReturnService(_wmsTransation);
        return crService.RecevieApiDatas(req);
      });
    }

    /// <summary>
    /// 批次廠商退貨單資料
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult PostCreateVendorReturns(PostCreateVendorReturnsReq req)
    {
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();

      // 檢核主機授權碼
      if (!DbSchemaHelper.CheckSCCode())
        return new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = tacService.GetMsg("20057") };


      // 啟動紀錄
      return ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSAPI_VR, req.DcCode, commonService.GetGupCode(req.CustCode), req.CustCode, "PostCreateVendorReturns", req, () =>
      {
        CommonVendorReturnService crService = new CommonVendorReturnService(_wmsTransation);
        return crService.RecevieApiDatas(req);
      });
    }

    /// <summary>
    /// 批次新增門市主檔
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult PostRetailData(PostRetailDataReq req)
    {
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();
      ApiResult res = new ApiResult();

      // 連線檢核
      if (DbSchemaHelper.CheckSCCode())
      {
        // 啟動紀錄
        res = ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSAPI_RT, null, commonService.GetGupCode(req.CustCode), req.CustCode, "PostRetailData", req, () =>
        {
          CommonRetailService crService = new CommonRetailService(_wmsTransation);
          return crService.RecevieApiDatas(req);
        });
      }
      else
      {
        // 主機授權碼錯誤
        res = new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = tacService.GetMsg("20057") };
      }

      return res;
    }

    /// <summary>
    /// 批次新增商品階層檔
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult PostItemLevel(PostItemLevelReq req)
    {
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();
      ApiResult res = new ApiResult();

      // 連線檢核
      if (DbSchemaHelper.CheckSCCode())
      {
        // 啟動紀錄
        res = ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSAPI_PL, null, commonService.GetGupCode(req.CustCode), req.CustCode, "PostItemLevel", req, () =>
        {
          CommonItemLevelService cilService = new CommonItemLevelService(_wmsTransation);
          return cilService.RecevieApiDatas(req);
        });
      }
      else
      {
        // 主機授權碼錯誤
        res = new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = tacService.GetMsg("20057") };
      }

      return res;
    }

    /// <summary>
    /// 批次新增供應商主檔
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult PostVendorData(PostVendorDataReq req)
    {
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();
      ApiResult res = new ApiResult();

      // 連線檢核
      if (DbSchemaHelper.CheckSCCode())
      {
        // 啟動紀錄
        res = ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSAPI_VD, null, commonService.GetGupCode(req.CustCode), req.CustCode, "PostVendorData", req, () =>
        {
          CommonVendorService cvService = new CommonVendorService(_wmsTransation);
          return cvService.RecevieApiDatas(req);
        });
      }
      else
      {
        // 主機授權碼錯誤
        res = new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = tacService.GetMsg("20057") };
      }

      return res;
    }

    /// <summary>
    /// 批次新增商品主檔
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult PostItemData(PostItemDataReq req)
    {
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();

      // 連線檢核
      if (!DbSchemaHelper.CheckSCCode())
      {
        // 主機授權碼錯誤
        return new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = tacService.GetMsg("20057") };
      }

      // 啟動紀錄
      return ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSAPI_PD, null, commonService.GetGupCode(req.CustCode), req.CustCode, "PostItemData", req, () =>
      {
        CommonItemService ciService = new CommonItemService(_wmsTransation);
        return ciService.RecevieApiDatas(req);
      });
    }

    /// <summary>
    /// 批次新增商品分類主檔
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult PostItemCategory(PostItemCategoryReq req)
    {
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();

      // 連線檢核
      if (!DbSchemaHelper.CheckSCCode())
      {
        // 主機授權碼錯誤
        return new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = tacService.GetMsg("20057") };
      }


      // 啟動紀錄
      return ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSAPI_PC, null, commonService.GetGupCode(req.CustCode), req.CustCode, "PostItemCategory", req, () =>
       {
         CommonItemCategoryService cicService = new CommonItemCategoryService(_wmsTransation);
         return cicService.RecevieApiDatas(req);
       });
    }

    /// <summary>
    /// 批次新增商品組合主檔
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult PostItemBom(PostItemBomReq req)
    {
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();

      // 連線檢核
      if (!DbSchemaHelper.CheckSCCode())
      {
        // 主機授權碼錯誤
        return new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = tacService.GetMsg("20057") };
      }

      // 啟動紀錄
      return ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSAPI_PB, null, commonService.GetGupCode(req.CustCode), req.CustCode, "PostItemBom", req, () =>
      {
        CommonItemBomService cibService = new CommonItemBomService(_wmsTransation);
        return cibService.RecevieApiDatas(req);
      });
    }

    /// <summary>
    /// 倉別總庫存資料
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult GetItemStocks(GetItemStocksReq req)
    {
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();

      // 連線檢核
      if (!DbSchemaHelper.CheckSCCode())
        // 主機授權碼錯誤
        return new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = tacService.GetMsg("20057") };

      if (req == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };

      // 啟動紀錄
      return ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSAPI_TS, null, commonService.GetGupCode(req.CustCode), req.CustCode, "GetItemStocks", req, () =>
      {
        CommonStockService csService = new CommonStockService();
        return csService.GetItemStocks(req);
      });
    }

    /// <summary>
    /// 庫存明細資料
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult GetItemStockDetails(GetItemStockDetailsReq req)
    {
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();

      // 連線檢核
      if (!DbSchemaHelper.CheckSCCode())
        // 主機授權碼錯誤
        return new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = tacService.GetMsg("20057") };

      if (req == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };

      // 啟動紀錄
      return ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSAPI_SD, null, commonService.GetGupCode(req.CustCode), req.CustCode, "GetItemStockDetails", req, () =>
      {
        CommonStockService csService = new CommonStockService();
        return csService.GetItemStockDetails(req);
      });
    }

    /// <summary>
    /// 快速移轉庫存調整單
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult FlashStockTransferData(FlashStockTransferDataReq req)
    {
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();

      // 連線檢核
      if (!DbSchemaHelper.CheckSCCode())
        // 主機授權碼錯誤
        return new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = tacService.GetMsg("20057") };

      if (req == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };

      // 啟動紀錄
      return ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSAPI_FT, null, commonService.GetGupCode(req.CustCode), req.CustCode, "FlashStockTransferData", req, () =>
      {
        CommonFlashStockTransferService csService = new CommonFlashStockTransferService(_wmsTransation);
        return csService.RecevieApiDatas(req);
      });
    }

    /// <summary>
    /// 商品序號查詢
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult GetItemSerials(GetItemSerialsReq req)
    {
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();

      // 連線檢核
      if (!DbSchemaHelper.CheckSCCode())
        // 主機授權碼錯誤
        return new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = tacService.GetMsg("20057") };

      if (req == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };

      // 啟動紀錄
      return ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSAPI_SN, null, commonService.GetGupCode(req.CustCode), req.CustCode, "GetItemSerials", req, () =>
      {
        CommonSerialNoService csService = new CommonSerialNoService();
        return csService.GetItemSerials(req);
      });
    }

    public ApiResult ChangeTransportProvider(ChangeTransportProviderReq req)
    {
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();

      // 連線檢核
      if (!DbSchemaHelper.CheckSCCode())
      {
        // 主機授權碼錯誤
        return new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = tacService.GetMsg("20057") };
      }

      if (req == null)
      {
        return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };
      }

      // 啟動紀錄
      return ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSAPI_F009002, req.DcCode, commonService.GetGupCode(req.CustCode), req.CustCode, "ChangeTransportProvider", req, () =>
       {
         var wmsTraansation = new WmsTransaction();
         var comService = new CommonChangeTransportProviderService(wmsTraansation);
         var result = comService.RecevieApiDatas(req);
         if (result.IsSuccessed)
           wmsTraansation.Complete();
         return result;
       });

    }

    public ApiResult PostUserData(PostUserDataReq req)
    {
      CommonService commonService = new CommonService();
      TransApiBaseService tacService = new TransApiBaseService();

      // 連線檢核
      if (!DbSchemaHelper.CheckSCCode())
      {
        // 主機授權碼錯誤
        return new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = tacService.GetMsg("20057") };
      }

      if (req == null)
      {
        return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };
      }

      // 啟動紀錄
      return ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSAPI_F009002, req.DcCode, commonService.GetGupCode(req.OwnerCode), req.OwnerCode, "LmsUserAsync", req, () =>
      {
        var wmsTraansation = new WmsTransaction();
        var comService = new CommonPostUserDataService(_wmsTransation);
        return comService.RecevieApiDatas(req);
      });

    }
  }
}
