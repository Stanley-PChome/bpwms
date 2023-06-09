using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.Wcs.WcsApiConnectSetting;
using Wms3pl.WebServices.Shared.WcsService;

namespace Wms3pl.WebServices.ToWcsWebApi.Business.mssql.Services
{
	public class WcsInventoryConfirmService : WcsBaseService
	{
		#region 盤點任務
		/// <summary>
		/// 盤點任務
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult StockCheck(WcsExportReq req)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();

			// 新增API Log
			res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ExportInventoryResults", req, () =>
			{
				// 取得物流中心服務貨主檔
				CommonService commonService = new CommonService();
				var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);
				dcCustList.ForEach(item =>
				{
					var result = ExportInventoryResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);
					data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
					var resData = (WcsItemSnData)result.Data;
				});
				res.Data = JsonConvert.SerializeObject(data);
				return res;
			}, true);

			return res;
		}

		/// <summary>
		/// 盤點任務
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		private ApiResult ExportInventoryResults(string dcCode, string gupCode, string custCode)
		{
			#region 變數設定
			ApiResult res = new ApiResult { IsSuccessed = true };
			var f060401Repo = new F060401Repository(Schemas.CoreSchema);
			var f140101Repo = new F140101Repository(Schemas.CoreSchema);
			var f140103Repo = new F140103Repository(Schemas.CoreSchema);
			var f140105Repo = new F140105Repository(Schemas.CoreSchema);
			int successCnt = 0;
			#endregion

			#region 主要邏輯
			var SettingsResendCount = GetMidApiRelmt();
			// 取得要執行出庫庫任務單據
			var f060401s = f060401Repo.GetDatas(dcCode, gupCode, custCode, "1", new List<string> { "0", "T" }, SettingsResendCount);

      // 最大單據數
      f060401s = f060401s.Take(GetMidApiMisMax());

      var inventoryList = f060401s.ToList();

      if (inventoryList.Any())
      {
        // 取得初盤明細
        var f140103s = f140103Repo.GetDataInventoryDetail(dcCode, gupCode, custCode, f060401s.Where(x => x.ISSECOND == "0").Select(x => x.WMS_NO).ToList());

        // 取得複盤明細
        var f140105s = f140105Repo.GetDatasByWcsInventoryNos(dcCode, gupCode, custCode, f060401s.Where(x => x.ISSECOND == "1").Select(x => x.WMS_NO).ToList());

				// 取得盤點方式資料
				var f140101s = f140101Repo.GetDatas(dcCode, gupCode, custCode, f060401s.Select(x => x.WMS_NO).ToList());

				// Foreach #取得要執行盤點任務單據
				inventoryList.ForEach(f060401 =>
				{
					#region 更新 F060401 處理中狀態
					// 更新任務資料表處理中狀態
					f060401.STATUS = "1";
					f060401Repo.Update(f060401);
					#endregion

          #region 主邏輯
          var f140101 = f140101s.Where(x => x.INVENTORY_NO == f060401.WMS_NO).FirstOrDefault();
          if (f140101 == null)
          {
            f060401.STATUS = "F";
            f060401.MESSAGE = "無盤點單據";
          }
          else
          {
            #region 參數處理
            var skuList = new List<WcsInventorySkuModel>();

            if (f060401.ISSECOND == "0") // 初盤
            {
              skuList = f140103s.Where(x => x.INVENTORY_NO == f060401.WMS_NO).Select(x => new WcsInventorySkuModel
              {
                SkuCode = x.ITEM_CODE,
                SkuLevel = 1
              }).Distinct().ToList();

							f140101.STATUS = "1";
						}
						else if (f060401.ISSECOND == "1") // 複盤
						{
							skuList = f140105s.Where(x => x.INVENTORY_NO == f060401.WMS_NO).GroupBy(x => new { x.ITEM_CODE, x.MAKE_NO, x.VALID_DATE }).Select(x => new WcsInventorySkuModel
							{
								SkuCode = x.Key.ITEM_CODE,
								SkuLevel = 1,
								OutBatchCode = x.Key.MAKE_NO,
								ExpiryDate = x.Key.VALID_DATE.ToString("yyyy/MM/dd")
							}).ToList();

              f140101.STATUS = "2";
            }

            var checkMode = 0;
            if (f140101.SHOW_CNT == "1")
              checkMode = 0;
            else if (f140101.SHOW_CNT == "0")
              checkMode = 1;

            var req = new WcsInventoryReq
            {
              OwnerCode = custCode,
              CheckCode = f060401.DOC_ID,
              OriCheckCode = f060401.WMS_NO,
              CheckType = 2,
              CheckMode = checkMode,
              SkuTotal = skuList.Count,
              SkuList = skuList
            };
            #endregion

            #region 執行盤點任務

            f060401.RESENT_CNT++;

            ApiResult result = new ApiResult { IsSuccessed = true };

						#region 盤點任務
						// 盤點任務Url
						string url = $"v1/{f060401.DC_CODE}/{f060401.WAREHOUSE_ID}/StockCheck";
						ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "WcsStockCheckResult", new { WcsApiUrl = $"{WcsSetting.ApiUrl}{url}", WcsToken = WcsSetting.ApiAuthToken, WcsData = isSaveWcsData ? req : null, F060401 = f060401 }, () =>
						{
							f060401.PROC_DATE = DateTime.Now;
#if (DEBUG)
							result = WcsApiFuncTest(req, "StockCheck");
#else
              result = WcsApiFunc(req, url);
#endif

							f060401.MESSAGE = result.MsgContent;

							if (result.IsSuccessed)
							{
								f060401.STATUS = "2";// 成功狀態改為2
								successCnt++;

                // 更新盤點單據狀態
                f140101Repo.Update(f140101);
              }

              return result;
            }, false,
            (fResult) =>
            {
              if (!fResult.IsSuccessed)
              {
                //重試次數超過設定值後才改失敗
                if (f060401.RESENT_CNT >= SettingsResendCount)
                  f060401.STATUS = "F";//錯誤將狀態改為F
                else
                  f060401.STATUS = "T";
              }
              return new ApiResult();
            });
            #endregion

            #endregion
          }
          #endregion

          #region 更新 F060401 完成、錯誤、逾時狀態
          f060401Repo = new F060401Repository(Schemas.CoreSchema);
          f060401Repo.Update(f060401);
          #endregion
        });
      }

      int failCnt = inventoryList.Count - successCnt;
      res.MsgCode = "10005";
      res.MsgContent = string.Format(_tacService.GetMsg("10005"), "盤點任務", successCnt, failCnt, inventoryList.Count);
      res.TotalCnt = inventoryList.Count;
      res.SuccessCnt = successCnt;
      res.FailureCnt = failCnt;
      #endregion

      return res;
    }

    #endregion

    #region 盤點調整任務
    /// <summary>
    /// 盤點調整任務
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult InventoryAdjust(WcsExportReq req)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();

			// 新增API Log
			res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ExportInventoryAdjustResults", req, () =>
			{
				// 取得物流中心服務貨主檔
				CommonService commonService = new CommonService();
				var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);
				dcCustList.ForEach(item =>
				{
					var result = ExportInventoryAdjustResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);
					data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
				});
				res.Data = JsonConvert.SerializeObject(data);
				return res;
			}, true);

			return res;
		}

		/// <summary>
		/// 盤點調整任務
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		private ApiResult ExportInventoryAdjustResults(string dcCode, string gupCode, string custCode)
		{
			#region 變數設定
			ApiResult res = new ApiResult { IsSuccessed = true };
			var wmsTransaction = new WmsTransaction();
			var f060404Repo = new F060404Repository(Schemas.CoreSchema);
			var f060402Repo = new F060402Repository(Schemas.CoreSchema, wmsTransaction);
			var f060403Repo = new F060403Repository(Schemas.CoreSchema, wmsTransaction);
			var f140106Repo = new F140106Repository(Schemas.CoreSchema, wmsTransaction);
			int successCnt = 0;
			#endregion

			#region 主要邏輯
			var SettingsResendCount = GetMidApiRelmt();
			// 取得要執行盤點調整任務清單
			var f060404s = f060404Repo.GetDatas(dcCode, gupCode, custCode, "1", new List<string> { "0", "T" }, SettingsResendCount);

      // 最大單據數
      f060404s = f060404s.Take(GetMidApiMisMax());

      var inventoryList = f060404s.ToList();

      if (inventoryList.Any())
      {
        // 取得盤點完成結果回傳清單資料
        var f060402s = f060402Repo.GetDatasForAdjustExecute(dcCode, gupCode, custCode, f060404s.Select(x => x.WMS_NO).ToList());

        // 取得盤點完成結果回傳明細清單資料
        var f060403s = f060403Repo.GetDatasForAdjustExecute(dcCode, gupCode, custCode, f060402s).ToList();

        // 盤點完成結果回傳明細
        var f140106s = f140106Repo.GetDatasForAdjustExecute(dcCode, gupCode, custCode, f060403s.AsQueryable()).ToList();

        // Foreach #取得要執行盤點任務單據
        inventoryList.ForEach(f060404 =>
        {
          var currF140106 = f140106s.Where(x =>
          x.DC_CODE == f060404.DC_CODE &&
          x.GUP_CODE == f060404.GUP_CODE &&
          x.CUST_CODE == f060404.CUST_CODE &&
          x.INVENTORY_NO == f060404.WMS_NO).ToList();

          var f060402 = f060402s.Where(x => x.WMS_NO == f060404.WMS_NO).OrderByDescending(x => x.CRT_DATE).FirstOrDefault();

          #region 更新 F060404 處理中狀態
          // 更新任務資料表處理中狀態
          f060404.STATUS = "1";
          f060404Repo.Update(f060404);
          #endregion

					#region 主邏輯
					if (f060402 == null)
					{
						f060404.STATUS = "F";
						f060404.MESSAGE = "無盤點完成結果回傳資料";
					}
					else
					{
						#region 參數處理
						var skuList = f060403s.Where(x => x.DOC_ID == f060402.DOC_ID).GroupBy(x => new
						{
							x.SKUCODE,
							x.EXPIRYDATE,
							x.OUTBATCHCODE,
							x.SHELFCODE,
							x.BINCODE
						}).Select(x => new WcsStockCheckAdjustmentSkuModel
						{
							SkuCode = x.Key.SKUCODE,
							ExpiryDate = x.Key.EXPIRYDATE,
							OutBatchCode = x.Key.OUTBATCHCODE,
							ShelfCode = x.Key.SHELFCODE,
							BinCode = x.Key.BINCODE,
							SkuQty = x.Sum(z => z.SKUQTY) - x.Sum(z => z.SKUSYSQTY),
							SkuLevel = 1,
							AdjustType = currF140106.Where(z =>
							z.ITEM_CODE == x.Key.SKUCODE &&
							z.VALID_DATE == Convert.ToDateTime(x.Key.EXPIRYDATE) &&
							z.MAKE_NO == x.Key.OUTBATCHCODE).All(z => z.STATUS == "0") ? 1 : 2
						}).Where(x => x.SkuQty != 0).ToList();

            var req = new WcsStockCheckAdjustmentReq
            {
              OwnerCode = f060404.CUST_CODE,
              AdjustCode = f060404.DOC_ID,
              CheckCode = f060404.CHECK_CODE,
              SkuTotal = skuList.Count,
              SkuList = skuList
            };
            #endregion

            #region 執行盤點任務

            f060404.RESENT_CNT++;

            ApiResult result = new ApiResult { IsSuccessed = true };

						#region 盤點調整任務
						// 盤點調整任務Url
						string url = $"v1/{f060404.DC_CODE}/{f060404.WAREHOUSE_ID}/StockCheck/Adjustment";
						ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "WcsStockCheckAdjustmentResult", new { WcsApiUrl = $"{WcsSetting.ApiUrl}{url}", WcsToken = WcsSetting.ApiAuthToken, WcsData = isSaveWcsData ? req : null, F060404 = f060404 }, () =>
						{
							f060404.PROC_DATE = DateTime.Now;
#if (DEBUG)
							result = WcsApiFuncTest(req, "StockCheckAdjustment");
#else
              result = WcsApiFunc(req, url);
#endif

							f060404.MESSAGE = result.MsgContent;

              if (result.IsSuccessed)
              {
                f060404.STATUS = "2";// 成功狀態改為2
                successCnt++;
              }

              return result;
            }, false,
            (fResult) =>
            {
              if (!fResult.IsSuccessed)
              {
                //重試次數超過設定值後才改失敗
                if (f060404.RESENT_CNT >= SettingsResendCount)
                  f060404.STATUS = "F";//錯誤將狀態改為F
                else
                  f060404.STATUS = "T";
              }
              return new ApiResult();
            });
            #endregion


            #endregion
          }
          #endregion

          #region 更新 F060404 完成、錯誤、逾時狀態
          f060404Repo = new F060404Repository(Schemas.CoreSchema);
          f060404Repo.Update(f060404);
          #endregion
        });
      }

      int failCnt = inventoryList.Count - successCnt;
      res.MsgCode = "10005";
      res.MsgContent = string.Format(_tacService.GetMsg("10005"), "盤點調整任務", successCnt, failCnt, inventoryList.Count);
      res.TotalCnt = inventoryList.Count;
      res.SuccessCnt = successCnt;
      res.FailureCnt = failCnt;
      #endregion

      return res;
    }

    #endregion
  }
}
