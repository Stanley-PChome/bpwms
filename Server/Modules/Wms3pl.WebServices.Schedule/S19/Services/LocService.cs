using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Schedule.S19.Services
{
  public class LocService
  {
    private WmsTransaction _wmsTransation;
    private CommonService _commonService;
    public CommonService CommonService
    {
      get
      {
        if (_commonService == null)
          _commonService = new CommonService();
        return _commonService;
      }
      set { _commonService = value; }
    }
    public LocService(WmsTransaction wmsTransation)
    {
      _wmsTransation = wmsTransation;
    }

        /// <summary>
        /// 執行更新儲位容積
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ApiResult ExecUpdateLocVolumn(WmsScheduleParam req)
        {
          CommonService commonService = new CommonService();
          List<ApiResponse> data = new List<ApiResponse>();
          ApiResult res = new ApiResult { IsSuccessed = true };
          var f191205Repo = new F191205Repository(Schemas.CoreSchema, _wmsTransation);
          var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransation);
          var f0003Repo = new F0003Repository(Schemas.CoreSchema);
          var f191205RepoNoTran = new F191205Repository(Schemas.CoreSchema);

            res = ApiLogHelper.CreateApiLogInfo("0", "0", "0", "ExecUpdateLocVolumn", req, () =>
            {
            // [A]=取得物流中心服務的貨主資料
              var dcList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode).Select(o => o.DC_CODE).Distinct().ToList();

              // 每次執行只抓X個儲位計算
              var f0003 = f0003Repo.Find(o => o.AP_NAME == "CountMaxLocVolumn" && o.DC_CODE == "00" && o.GUP_CODE == "00" && o.CUST_CODE == "00");
              if (f0003 == null)
              {
                return new ApiResult { IsSuccessed = false, MsgContent = "未設定儲位容積計算筆數" };
              }

              foreach (var dcCode in dcList)
              { 
                // 刪除不計算的儲位 例如: 進貨暫存倉、退貨暫存倉...F198001.CALVOLUMN=0
                f191205RepoNoTran.RemoveByCalvolumn(dcCode);

                // 取得需計算的儲位
                var locs = f191205Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode)
                    .OrderBy(x => x.CRT_DATE)
                    .Skip(0).Take(Convert.ToInt32(f0003.SYS_PATH)) // 依照參數針對幾筆儲位做處理
                    .ToList();

                if (locs.Any())
                {
                  #region 取得資料
                  var locCodes = locs.Select(x => x.LOC_CODE).ToList();
             
                  var usedVolumnDatas = f1912Repo.GetUsedVolumnByLocCodes(dcCode, locCodes);

                  var f1912Datas = f1912Repo.GetDatasByLocCodesNoTracking(dcCode, locCodes);

                  var datas = (from A in usedVolumnDatas
                               join B in f1912Datas
                               on new { A.DC_CODE, A.LOC_CODE } equals new { B.DC_CODE, B.LOC_CODE }
                               select new { UsedVolumnData = A, F1912 = B }).ToList();
                  #endregion

                  List<string> updLocCodes = new List<string>();

          datas.ForEach(currData =>
          {
            #region 更新 F1912 
            if (currData.UsedVolumnData?.VOLUMN > 999999999999999999m) //避免超過DB設定大小decimal(18,0)
              currData.UsedVolumnData.VOLUMN = 999999999999999999m;
            currData.F1912.USED_VOLUMN = currData.UsedVolumnData.VOLUMN;
            f1912Repo.Update(currData.F1912);
            #endregion

                    #region 刪除 F191205
                    f191205Repo.AsForUpdate().RemoveByLocCode(dcCode, currData.F1912.LOC_CODE);
                    #endregion

                    _wmsTransation.Complete();

                    // 成功新增的儲位紀錄下來，用以回傳顯示
                    updLocCodes.Add(currData.F1912.LOC_CODE);
                  });

                  if (updLocCodes.Any())
                  {
                    data.Add(new ApiResponse
                    {
                      MsgContent = $"物流中心 {dcCode} 已完成更新儲位容積儲位數共 {updLocCodes.Count} 筆。",
                      No = string.Join("、", updLocCodes)
                    });
                  }
                  else
                  {
                    data.Add(new ApiResponse { MsgContent = $"物流中心 {dcCode} 無儲位需要更新儲位容積" });
                  }
                }
                else
                {
                  data.Add(new ApiResponse { MsgContent = $"物流中心 {dcCode} 無儲位需要更新儲位容積" });
                }
              }

              res.Data = JsonConvert.SerializeObject(data);
              return res;
            }, true);

      return res;
    }

    /// <summary>
    /// 使用上次計算儲位容積時間更新儲位容積
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public ApiResult ExecUpdateLocVolumnByCalvolumnTime(ExecUpdateLocVolumnParam param)
    {
      List<ApiResponse> resData = new List<ApiResponse>();
      ApiResult res = new ApiResult { IsSuccessed = true };
      res = ApiLogHelper.CreateApiLogInfo(param.DcCode, "0", "0", "ExecUpdateLocVolumnByCalvolumnTime", param, () =>
      {
        var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransation);
        var f1913Repo = new F1913Repository(Schemas.CoreSchema);

        var dcList = CommonService.GetDcCustList(param.DcCode, null, null).Select(o => o.DC_CODE).Distinct().ToList();

        List<string> updLocCodes = new List<string>();

        // 每次執行只抓X個儲位計算
        var strCountMaxLocVolumn = CommonService.GetSysGlobalValue("CountMaxLocVolumn");
        if (string.IsNullOrWhiteSpace(strCountMaxLocVolumn))
          return new ApiResult { IsSuccessed = false, MsgContent = "尚未設定儲位容積計算筆數" };
        int CountMaxLocVolumn = 0;
        if (!int.TryParse(strCountMaxLocVolumn, out CountMaxLocVolumn))
          return new ApiResult { IsSuccessed = false, MsgContent = "儲位容積計算筆數設定錯誤，無法轉換為數值" };

        foreach (var dcCode in dcList)
        {
          updLocCodes = new List<string>();
          var locCodes = f1912Repo.GetUpdateLocVolumnByCalvolumnTime(dcCode, CountMaxLocVolumn).ToList();
          if (locCodes == null || !locCodes.Any())
          {
            resData.Add(new ApiResponse { MsgContent = $"物流中心 {dcCode} 無儲位需要更新儲位容積" });
            continue;
          }


          var usedVolumnDatas = f1912Repo.GetUsedVolumnByLocCodes(dcCode, locCodes).ToList();
          var f1912Datas = f1912Repo.GetF1912DataSQL(dcCode, locCodes).ToList();
          var datas = (from A in usedVolumnDatas
                       join B in f1912Datas
                             on new { A.DC_CODE, A.LOC_CODE } equals new { B.DC_CODE, B.LOC_CODE }
                       select new { UsedVolumnData = A, F1912 = B,}).ToList();

          datas.ForEach(currData =>
          {
            #region 更新 F1912 
            if (currData.UsedVolumnData?.VOLUMN > 999999999999999999m) //避免超過DB設定大小decimal(18,0)
              currData.UsedVolumnData.VOLUMN = 999999999999999999m;
            f1912Repo.UpdateUsedVolumn(currData.F1912.DC_CODE, currData.F1912.LOC_CODE, currData.UsedVolumnData.VOLUMN ?? 0);
            #endregion

            _wmsTransation.Complete();

            // 成功新增的儲位紀錄下來，用以回傳顯示
            updLocCodes.Add(currData.F1912.LOC_CODE);
          });

          if (updLocCodes.Any())
          {
            resData.Add(new ApiResponse
            {
              MsgContent = $"物流中心 {dcCode} 已完成更新儲位容積儲位數共 {updLocCodes.Count} 筆。",
              No = string.Join("、", updLocCodes)
            });
          }
          else
          {
            resData.Add(new ApiResponse { MsgContent = $"物流中心 {dcCode} 無儲位需要更新儲位容積" });
          }

          f1913Repo.RemoveStockZeroData(dcCode);
        }

        return new ApiResult
        {
          IsSuccessed = true,
          Data = JsonConvert.SerializeObject(resData)
        };

      }, true);

      return res;
    }

  }
}
