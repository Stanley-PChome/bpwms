using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.Wcs.WcsApiConnectSetting;
using Wms3pl.WebServices.Shared.WcsService;
using Wms3pl.WebServices.Shared.WcsServices;

namespace Wms3pl.WebServices.ToWcsWebApi.Business.mssql.Services
{
  public class WcsInboundServices : WcsBaseService
  {

    private CommonService _commonService;
    public CommonService CommonService
    {
      get { return _commonService == null ? _commonService = new CommonService() : _commonService; }
      set { _commonService = value; }
    }
    #region 入庫任務
    /// <summary>
    /// 入庫任務
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult Inbound(WcsExportReq req)
    {
      var res = new ApiResult { IsSuccessed = true };
      var data = new List<ApiResponse>();

      // 新增API Log
      res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ExportInboundResults", req, () =>
      {
        // 取得物流中心服務貨主檔
        var dcCustList = CommonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);
        dcCustList.ForEach(item =>
        {
          var result = ExportInboundResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);
          data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
        });
        res.Data = JsonConvert.SerializeObject(data);
        return res;
      }, true);

      return res;
    }

		

		private ApiResult ExportInboundResults(string dcCode, string gupCode, string custCode)
		{
			#region 初始化設定
			var maxResendCnt = GetMidApiRelmt();
			var maxDataCnt = GetMidApiMisMax();
			int successCnt = 0;
			int failureCnt = 0;
			int totalCnt = 0;
			var wmsTransaction = new WmsTransaction();
			var f060101Repo = new F060101Repository(Schemas.CoreSchema);
			var f1903AsyncRepo = new F1903_ASYNCRepository(Schemas.CoreSchema);
			F151001Repository f151001Repo = null;
			F151002Repository f151002Repo = null;
			WcsInboundReq req = null;
			var orderService = new OrderService(wmsTransaction);
			var f0003Repo = new F0003Repository(Schemas.CoreSchema);
			#endregion

			#region 取得入庫任務資料 與相關資料
			// 更新可派發任務的狀態為處理中
			f060101Repo.UpdateCanSendToProcess(dcCode, gupCode, custCode, maxDataCnt, "1");
			// 取得處理中的任務資料
			var executeDatas = f060101Repo.GetCanSendCurrentProcessData(dcCode, gupCode, custCode, "1").ToList();
			totalCnt = executeDatas.Count;
			if (!executeDatas.Any())
			{
				return new ApiResult
				{
					MsgCode = "10005",
					MsgContent = string.Format(_tacService.GetMsg("10005"), "入庫任務", successCnt, failureCnt, totalCnt),
					TotalCnt = totalCnt,
					SuccessCnt = successCnt,
					FailureCnt = failureCnt
				};
			}
			// [蘋果廠商編號清單] = &取得F0003設定檔[DC_CODE, GUP_CODE, CUST_CODE]
			var _f0003AppleVendorList = f0003Repo.GetAppleVendor(dcCode, gupCode, custCode);
			#endregion

			foreach (var f060101 in executeDatas)
			{
					#region 調撥任務 檢核
					if (f151001Repo == null)
						f151001Repo = new F151001Repository(Schemas.CoreSchema, wmsTransaction);

					var allocData = f151001Repo.GetF151001(dcCode, gupCode, custCode, f060101.WMS_NO);
					if (allocData == null)
					{
						f060101.STATUS = "9";
						f060101.MESSAGE = "找不到此單據，不派發入庫任務";
						f060101Repo.UpdateToCancel(f060101.CMD_TYPE, f060101.DOC_ID, f060101.STATUS, f060101.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					// 檢查是否取消調撥單
					if (allocData.STATUS == "9")
					{
						f060101.STATUS = "9";
						f060101.MESSAGE = "單據已取消，不派發入庫任務";
						f060101Repo.UpdateToCancel(f060101.CMD_TYPE, f060101.DOC_ID, f060101.STATUS, f060101.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					// 檢查是否取消調撥單
					if (allocData.STATUS == "5")
					{
						f060101.STATUS = "9";
						f060101.MESSAGE = "單據已結案，不派發入庫任務";
						f060101Repo.UpdateToCancel(f060101.CMD_TYPE, f060101.DOC_ID, f060101.STATUS, f060101.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					// 檢查是否可上架
					if (allocData.STATUS == "0" || allocData.STATUS == "1" || allocData.STATUS=="2")
					{
						f060101.STATUS = "0";
						f060101.MESSAGE = "單據未下架完成，不派發入庫任務";
						f060101Repo.UpdateToCancel(f060101.CMD_TYPE, f060101.DOC_ID, f060101.STATUS, f060101.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}


					#endregion

					#region 調撥任務傳入參數
					if (f151002Repo == null)
						f151002Repo = new F151002Repository(Schemas.CoreSchema);
					var details = f151002Repo.GetDatasWithNoCancel(dcCode, gupCode, custCode, f060101.WMS_NO).ToList();
					// 檢查是否還有明細資料
					if (!details.Any())
					{
						f060101.STATUS = "9";
						f060101.MESSAGE = "單據已無明細資料，不派發出入庫任務";
						f060101Repo.UpdateToCancel(f060101.CMD_TYPE, f060101.DOC_ID, f060101.STATUS, f060101.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
				  var itemCodes = details.Select(x => x.ITEM_CODE).Distinct().ToList();
				if (!f1903AsyncRepo.CheckItemAsync(f060101.GUP_CODE, f060101.CUST_CODE, itemCodes))
				{
					// 商品未同步，等待下次派發，不須累計
					f060101.STATUS = "0";
					f060101.MESSAGE = "等待商品同步";
					f060101Repo.Update(f060101);
					continue;
				}
				var skuList = details.Select(x => new WcsInboundSkuModel
					{
						RowNum = x.ALLOCATION_SEQ,
						SkuCode = x.ITEM_CODE,
						SkuQty = Convert.ToInt32(x.TAR_QTY),
						SkuLevel = 1,
						OutLevel = 0,
						ExpiryDate = x.VALID_DATE.ToString("yyyy/MM/dd"),
						OutBatchCode = x.MAKE_NO,
						ContainerCode = x.CONTAINER_CODE,
						BinCode = x.BIN_CODE
					}).ToList();

					req = new WcsInboundReq
					{
						OwnerCode = custCode,
						ReceiptCode = f060101.DOC_ID,
						ReceiptType = 1,
						PalletCode = allocData.CONTAINER_CODE,
						SkuTotal = skuList.Count,
						PutawayType = 0,
						SkuList = skuList
					};
					var productList = CommonService.GetProductList(gupCode, custCode, details.Select(x => x.ITEM_CODE).Distinct().ToList());
					if (productList.Any(a => _f0003AppleVendorList.Contains(a.VNR_CODE)))
					{
						req.PutawayType = 1;
					}


				#endregion


				#region 執行入庫任務派發API

				var result = new ApiResult { IsSuccessed = false };

				// 入庫任務Url
				string url = $"v1/{f060101.DC_CODE}/{f060101.WAREHOUSE_ID}/Inbound";
				ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "WcsInboundResult", new { WcsApiUrl = $"{WcsSetting.ApiUrl}{url}", WcsToken = WcsSetting.ApiAuthToken, WcsData = isSaveWcsData ? req : null, F060101 = f060101 }, () =>
				{
					if (req != null)
					{
						f060101.RESENT_CNT++;
						f060101.PROC_DATE = DateTime.Now;
#if (DEBUG)
						result = WcsApiFuncTest(req, "Inbound");
#else
							  result = WcsApiFunc(req, url);
#endif

						f060101.MESSAGE = result.MsgContent;

						if (result.IsSuccessed)
						{
							f060101.STATUS = "2";// 成功狀態改為2
							successCnt++;

							// 更新單據狀態
							f151001Repo.UpdateStatus(dcCode, gupCode, custCode, f060101.WMS_NO, "4");

							wmsTransaction.Complete();
						}
						else
						{
							var errDataString = JsonConvert.SerializeObject(result.Data);
							if (errDataString.Contains("記錄已存在") || errDataString.Contains("记录已存在"))
							{
								f060101.STATUS = "2";
								f060101.MESSAGE += " 回傳內容含有記錄已存在，不再重新派發入庫任務";
								// 更新單據狀態
								f151001Repo.UpdateStatus(dcCode, gupCode, custCode, f060101.WMS_NO, "4");

								wmsTransaction.Complete();
							}
							else
								f060101.STATUS = "T";
						}
					}
					else
					{
						result = new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = "傳入參數資料為null" };
						f060101.STATUS = "F";
						f060101.MESSAGE += "傳入參數資料為null，不派發入庫任務";
					}
					return result;
				}, false,
					(fResult) =>
					{
						if (!fResult.IsSuccessed)
						{
							if (f060101.RESENT_CNT >= maxResendCnt)
								f060101.STATUS = "F"; //超過重派次數將狀態改為F
							else
								f060101.STATUS = "T";
						}

						#region 更新 F060201 完成、錯誤、逾時狀態
						f060101Repo = new F060101Repository(Schemas.CoreSchema);
						f060101Repo.UpdateExecResult(f060101.CMD_TYPE, f060101.DOC_ID, f060101.STATUS, f060101.MESSAGE, f060101.PROC_DATE.Value, f060101.RESENT_CNT);
						#endregion

						return new ApiResult();
					});
				#endregion

			}

			failureCnt = totalCnt - successCnt;

			return new ApiResult
			{
				MsgCode = "10005",
				MsgContent = string.Format(_tacService.GetMsg("10005"), "入庫任務", successCnt, failureCnt, totalCnt),
				TotalCnt = totalCnt,
				SuccessCnt = successCnt,
				FailureCnt = failureCnt
			};
		}

    #endregion

    #region 入庫任務取消
    /// <summary>
    /// 入庫任務取消
    /// </summary>
    /// <returns></returns>
    public ApiResult InboundCancel(WcsExportReq req)
    {
      ApiResult res = new ApiResult { IsSuccessed = true };
      List<ApiResponse> data = new List<ApiResponse>();

      // 新增API Log
      res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ExportInboundCancelResults", req, () =>
      {
        // 取得物流中心服務貨主檔
        var dcCustList = CommonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);

        dcCustList.ForEach(item =>
        {
          var result = ExportInboundCancelResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);
          data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
        });

        res.Data = JsonConvert.SerializeObject(data);

        return res;

      }, true);

      return res;
    }

		private ApiResult ExportInboundCancelResults(string dcCode, string gupCode, string custCode)
		{
			#region 初始化設定
			var maxResendCnt = GetMidApiRelmt();
			var maxDataCnt = GetMidApiMisMax();

			int successCnt = 0;
			int failureCnt = 0;
			int totalCnt = 0;
			var wmsTransaction = new WmsTransaction();
			var f060101Repo = new F060101Repository(Schemas.CoreSchema);
			F151001Repository f151001Repo = null;
			WcsInboundCancelReq req = null;
			var inboundCancelService = new WcsInboundCancelService();

			#endregion

			#region 取得入庫取消任務資料 與相關資料
			// 更新可派發任務的狀態為處理中
			f060101Repo.UpdateCanSendToProcess(dcCode, gupCode, custCode, maxDataCnt, "2");
			// 取得處理中的任務資料
			var executeDatas = f060101Repo.GetCanSendCurrentProcessData(dcCode, gupCode, custCode, "2").ToList();
			totalCnt = executeDatas.Count;
			if (!executeDatas.Any())
			{
				return new ApiResult
				{
					MsgCode = "10005",
					MsgContent = string.Format(_tacService.GetMsg("10005"), "入庫取消任務", successCnt, failureCnt, totalCnt),
					TotalCnt = totalCnt,
					SuccessCnt = successCnt,
					FailureCnt = failureCnt
				};
			}
			#endregion

			foreach (var f060101 in executeDatas)
			{
					#region 調撥取消任務 檢核
					if (f151001Repo == null)
						f151001Repo = new F151001Repository(Schemas.CoreSchema, wmsTransaction);

					var allocData = f151001Repo.GetF151001(dcCode, gupCode, custCode, f060101.WMS_NO);
					if (allocData == null)
					{
						f060101.STATUS = "9";
						f060101.MESSAGE = "找不到此單據，不派發入庫取消任務";
						f060101Repo.UpdateToCancel(f060101.CMD_TYPE, f060101.DOC_ID, f060101.STATUS, f060101.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					// 檢查是否取消調撥單
					if (allocData.STATUS == "9")
					{
						f060101.STATUS = "9";
						f060101.MESSAGE = "單據已取消，不派發入庫取消任務";
						f060101Repo.UpdateToCancel(f060101.CMD_TYPE, f060101.DOC_ID, f060101.STATUS, f060101.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					// 檢查是否取消調撥單
					if (allocData.STATUS == "5")
					{
						f060101.STATUS = "9";
						f060101.MESSAGE = "單據已結案，不派發入庫取消任務";
						f060101Repo.UpdateToCancel(f060101.CMD_TYPE, f060101.DOC_ID, f060101.STATUS, f060101.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					// 檢查是否已下架完成
					if (allocData.STATUS == "0" || allocData.STATUS == "1" || allocData.STATUS=="2")
					{
						f060101.STATUS = "0";
						f060101.MESSAGE = "單據未下架完成，不派發入庫取消任務";
						f060101Repo.UpdateToCancel(f060101.CMD_TYPE, f060101.DOC_ID, f060101.STATUS, f060101.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					#endregion

				

				#region 入庫任務派發檢查

				var sendF060101 = f060101Repo.GetData("1", f060101.DOC_ID);
				if (sendF060101 == null)
				{
					f060101.STATUS = "9";
					f060101.MESSAGE = "找不到派發入庫任務，不派發入庫取消任務";
					f060101Repo.UpdateToCancel(f060101.CMD_TYPE, f060101.DOC_ID, f060101.STATUS, f060101.MESSAGE);
					continue;
				}
				else if (sendF060101.STATUS == "9")
				{
					f060101.STATUS = "9";
					f060101.MESSAGE = "出庫任務派發已取消，不派發出庫取消任務";
					f060101Repo.UpdateToCancel(f060101.CMD_TYPE, f060101.DOC_ID, f060101.STATUS, f060101.MESSAGE);
					continue;
				}
				else if (sendF060101.STATUS == "0" || sendF060101.STATUS == "1" || sendF060101.STATUS == "3")
				{
					f060101.STATUS = "0"; // 1要轉回0
					f060101.MESSAGE = "入庫任務尚未派發或派發執行中，跳過，暫不派發入庫取消任務";
					f060101Repo.UpdateToCancel(f060101.CMD_TYPE, f060101.DOC_ID, f060101.STATUS, f060101.MESSAGE);
					continue;
				}
				else
				{
					//sendF060101.STATUS=2(已派發) => 執行入庫取消任務
				}

				#endregion

				#region 執行入庫取消任務派發API


				var result = new ApiResult { IsSuccessed = false };

				#region 入庫取消任務傳入參數
				req = new WcsInboundCancelReq
				{
					OwnerCode = custCode,
					ReceiptCode = f060101.DOC_ID
				};
				#endregion

				// 入庫任務取消Url
				string url = $"v1/{f060101.DC_CODE}/{f060101.WAREHOUSE_ID}/Inbound/Cancel";
				ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "WcsInboundCancelResult", new { WcsApiUrl = $"{WcsSetting.ApiUrl}{url}", WcsToken = WcsSetting.ApiAuthToken, WcsData = isSaveWcsData ? req : null, F060101 = f060101 }, () =>
				{
					if (req != null)
					{
						f060101.RESENT_CNT++;
						f060101.PROC_DATE = DateTime.Now;
						result = inboundCancelService.InboundCancel(url, req);

						f060101.MESSAGE = result.MsgContent;

						if (result.IsSuccessed)
						{
							f060101.STATUS = "2";// 成功狀態改為2
							successCnt++;

							wmsTransaction.Complete();
						}
					}
					else
					{
						result = new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = "傳入參數資料為null" };
						f060101.STATUS = "F";
						f060101.MESSAGE += "傳入參數資料為null，不派發出入庫取消任務";
					}
					return result;
				}, false,
					(fResult) =>
					{
						// 回傳代碼是失敗，但第二層內容有回傳取消成功或取消失敗處理
						if (!fResult.IsSuccessed)
						{
							if (f060101.RESENT_CNT >= maxResendCnt)
								f060101.STATUS = "F"; //超過重派次數，狀態改為F
							else
								f060101.STATUS = "T";

							var strSerializeJson = string.Empty;
							var errMesg = string.Empty;



							if (result.Data == null)
								errMesg = $"Msg:{result.MsgContent} Status:null ErrorMsg:Wcs Api 回傳的Data is null";

							if (string.IsNullOrWhiteSpace(errMesg))
							{
								strSerializeJson = JsonConvert.SerializeObject(result.Data);
								if (string.IsNullOrEmpty(strSerializeJson) || strSerializeJson?.ToUpper() == "NULL")
									errMesg = $"Msg:{result.MsgContent} Status:null ErrorMsg:Wcs Api 回傳的Data 為空物件";
							}

							if (string.IsNullOrWhiteSpace(errMesg))
							{
								var rtnData = JsonConvert.DeserializeObject<List<WcsInboundCancelResData>>(strSerializeJson).FirstOrDefault();
								if (rtnData == null)
									errMesg = $"Msg:{result.MsgContent} Status:null ErrorMsg:Wcs Api 回傳的Data 資料異常，無法轉換";

								if (string.IsNullOrWhiteSpace(errMesg) && string.IsNullOrEmpty(rtnData.Status))
									errMesg = $"Msg:{result.MsgContent} Status:{rtnData.Status} ErrorMsg:Wcs Api 回傳的Data Status為空白，無法識別";

								if (string.IsNullOrWhiteSpace(errMesg) && !new[] { "0", "1" }.Contains(rtnData.Status))
									errMesg = $"Msg:{result.MsgContent} Status:{rtnData.Status} ErrorMsg:Wcs Api 回傳的Data Status為{rtnData.Status}，無法識別";

								if (string.IsNullOrWhiteSpace(errMesg) && rtnData.Status == "1")
									errMesg = $"Msg:{result.MsgContent} Status:{rtnData.Status} ErrorMsg:{rtnData.ErrorMsg}";
								if (string.IsNullOrWhiteSpace(errMesg) && rtnData.Status == "0") // 回傳取消成功
									errMesg = string.Empty;
							}

							if (!string.IsNullOrWhiteSpace(errMesg))
							{
								f060101.STATUS = "F";
								f060101.MESSAGE += (f060101.MESSAGE?.Length??0 +errMesg.Length) > 4000 ? errMesg.Substring(0, 4000 - f060101.MESSAGE?.Length??0) : errMesg;
							}
							else
							{
								f060101.STATUS = "2";
								successCnt++;
							}
						}

						#region 更新 F060201 完成、錯誤、逾時狀態
						f060101Repo = new F060101Repository(Schemas.CoreSchema);
						f060101Repo.UpdateExecResult(f060101.CMD_TYPE, f060101.DOC_ID, f060101.STATUS, f060101.MESSAGE, f060101.PROC_DATE.Value, f060101.RESENT_CNT);
						#endregion

						return new ApiResult();
					});
				#endregion

			}

			failureCnt = totalCnt - successCnt;

			return new ApiResult
			{
				MsgCode = "10005",
				MsgContent = string.Format(_tacService.GetMsg("10005"), "入庫取消任務", successCnt, failureCnt, totalCnt),
				TotalCnt = totalCnt,
				SuccessCnt = successCnt,
				FailureCnt = failureCnt
			};
		}
    #endregion
  }
}
