using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.Wcs.WcsApiConnectSetting;
using Wms3pl.WebServices.Shared.WcsService;

namespace Wms3pl.WebServices.ToWcsWebApi.Business.mssql.Services
{
  public class WcsContainerServices : WcsBaseService
  {
    /// <summary>
    /// 容器資訊
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult Container(WcsExportReq req)
    {
      var res = new ApiResult { IsSuccessed = true };
      var data = new List<ApiResponse>();

      // 新增API Log
      res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ExportContainerResults", req, () =>
      {
        // 取得物流中心服務貨主檔
        CommonService commonService = new CommonService();
        var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);
        dcCustList.ForEach(item =>
        {
          var result = ExportContainerResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);
          data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
        });
        res.Data = JsonConvert.SerializeObject(data);
        return res;
      }, true);

      return res;
    }

    /// <summary>
    /// 容器資訊
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <returns></returns>
    private ApiResult ExportContainerResults(string dcCode, string gupCode, string custCode)
    {
      #region 變數設定
      var res = new ApiResult { IsSuccessed = true };
      var f060302Repo = new F060302Repository(Schemas.CoreSchema);
      var successCnt = 0;
      #endregion

      #region 主要邏輯
      var SettingsResendCount = GetMidApiRelmt();

      // 取得要執行的容器清單
      var executeDatas = f060302Repo.GetWcsExecuteDatas(dcCode, custCode, new List<string> { "0", "T" }, SettingsResendCount);

      int totalCnt = executeDatas.Count();

      if (executeDatas.Any())
      {
        // Group倉庫編號、新增/修改刪除 用以分批發送Params
        var sendDatas = executeDatas.GroupBy(x => x.WAREHOUSE_ID).Select(x => new
        {
          WarehouseId = x.Key,
          F060302s = x.ToList()
        }).OrderBy(x => x.WarehouseId).ToList();

        // 依照倉庫編號分批發送
        sendDatas.ForEach(obj =>
        {
          #region 更新 F060301 處理中狀態
          // 更新人員資料表處理中狀態
          f060302Repo = new F060302Repository(Schemas.CoreSchema);
          obj.F060302s.ForEach(f060302 => { f060302.STATUS = "1"; });
          f060302Repo.BulkUpdate(obj.F060302s);
          #endregion

          #region 執行容器資訊
          DateTime now = DateTime.Now;
          // 執行次數累加
          obj.F060302s.ForEach(f060302 =>
          {
            f060302.RESENT_CNT++;
            f060302.PROC_DATE = now;
          });

          // 以倉庫編號分批送出
          ApiResult result = new ApiResult { IsSuccessed = true };

          string url = $"v1/{dcCode}/{obj.WarehouseId}/Container/Release";// 容器資訊Url

          var currReq = new WcsContainerReq
          {
            OwnerCode = custCode,
            ContainerTotal = obj.F060302s.Count,
            ContainerList = obj.F060302s.Select(x => new WcsContainerModel { ContainerCode = x.CONTAINER_CODE }).ToList()
          };

          // 呼叫WcsApi-容器資訊
          ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "WcsContainerResult", new { WcsApiUrl = $"{WcsSetting.ApiUrl}{url}", WcsToken = WcsSetting.ApiAuthToken, WcsData = isSaveWcsData ? currReq : null, F060302s = obj.F060302s }, () =>
          {
#if (DEBUG)
            result = WcsApiFuncTest(currReq, "Container");
#else
            result = WcsApiFunc(currReq, url);
#endif

            if (result.IsSuccessed)
            {
              obj.F060302s.ForEach(f060302 =>
              {
                f060302.STATUS = "2";//成功狀態改為2
                f060302.MESSAGE = result.MsgContent;
              });
              successCnt += obj.F060302s.Count;
            }
            else
            {
              var errorDatas = JsonConvert.DeserializeObject<List<WcsContainerResData>>(
                  JsonConvert.SerializeObject(result.Data)
                  );

              if (errorDatas != null && errorDatas.Any())
              {
                // 找出失敗的做處理
                var errorContainerCodes = errorDatas.Where(x => !string.IsNullOrWhiteSpace(x.ContainerCode)).Select(z => z.ContainerCode);
                obj.F060302s.Where(x => errorContainerCodes.Contains(x.CONTAINER_CODE)).ToList().ForEach(f060302 =>
                {
                  f060302.STATUS = "F";//失敗狀態改為F
                  f060302.MESSAGE = result.MsgContent;
                });

                // 找出成功的做處理
                var successDatas = obj.F060302s.Where(x => !errorContainerCodes.Contains(x.CONTAINER_CODE)).ToList();
                successDatas.ForEach(f060302 =>
                {
                  f060302.STATUS = "2";//成功狀態改為2
                  f060302.MESSAGE = "Success";
                });
                successCnt += successDatas.Count;
              }
              else
              {
                obj.F060302s.ForEach(f060302 =>
                {
                  f060302.STATUS = "2";//成功狀態改為2
                  f060302.MESSAGE = "Success";
                });
                successCnt += obj.F060302s.Count;
              }
            }

            return result;
          }, false,
          (fResult) =>
          {
            if (!fResult.IsSuccessed)
            {
              obj.F060302s.ForEach(f060302 =>
              {
                //重試次數超過設定值後才改失敗
                if (f060302.RESENT_CNT >= SettingsResendCount)
                  f060302.STATUS = "F";//錯誤將狀態改為F
                else
                  f060302.STATUS = "T";
              });
            }
            return new ApiResult();
          });
          #endregion

          #region 更新 F060301 完成、錯誤、逾時狀態、執行次數
          f060302Repo = new F060302Repository(Schemas.CoreSchema);
          f060302Repo.BulkUpdate(obj.F060302s);
          #endregion
        });
      }
      int failCnt = totalCnt - successCnt;
      res.MsgCode = "10005";
      res.MsgContent = string.Format(_tacService.GetMsg("10005"), "容器資訊", successCnt, failCnt, totalCnt);
      res.TotalCnt = totalCnt;
      res.SuccessCnt = successCnt;
      res.FailureCnt = failCnt;
      #endregion

      return res;
    }
  }
}
