using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.Wcs.WcsApiConnectSetting;
using Wms3pl.WebServices.Shared.WcsService;
using Wms3pl.WebServices.Shared.WcsServices;

namespace Wms3pl.WebServices.ToWcsWebApi.Business.mssql.Services
{
  public class WcsOutboundServices : WcsBaseService
  {
    #region 出庫任務
    /// <summary>
    /// 出庫任務
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult Outbound(WcsExportReq req)
    {
      ApiResult res = new ApiResult { IsSuccessed = true };
      List<ApiResponse> data = new List<ApiResponse>();

      // 新增API Log
      res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ExportOutboundResults", req, () =>
      {
        // 取得物流中心服務貨主檔
        CommonService commonService = new CommonService();
        var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);
        dcCustList.ForEach(item =>
        {
          var result = ExportOutboundResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);
          data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
        });
        res.Data = JsonConvert.SerializeObject(data);
        return res;
      }, true);

      return res;
    }

		private ApiResult ExportOutboundResults(string dcCode,string gupCode,string custCode)
		{
			#region 初始化設定
			var maxResendCnt = GetMidApiRelmt();
			var maxDataCnt = GetMidApiMisMax();
			int successCnt = 0;
			int failureCnt = 0;
			int totalCnt = 0;
			var wmsTransaction = new WmsTransaction();
			var collectionList = new List<WcsOutboundCollectionData>();
			var f190105Repo = new F190105Repository(Schemas.CoreSchema);
			var f190105 = f190105Repo.GetF190105Data(dcCode).FirstOrDefault();
			var f060201Repo = new F060201Repository(Schemas.CoreSchema);
			F051201Repository f051201Repo = null ;
			F051202Repository f051202Repo = null;
			F051203Repository f051203Repo = null;
			F151001Repository f151001Repo = null;
			F151002Repository f151002Repo = null;
			WcsOutboundReq req = null;
			var orderService = new OrderService(wmsTransaction);
			#endregion

			#region 取得出庫任務資料 與相關資料
			// 更新可派發任務的狀態為處理中
			f060201Repo.UpdateCanSendToProcess(dcCode, gupCode, custCode, maxDataCnt, "1");
			// 取得處理中的任務資料
			var executeDatas = f060201Repo.GetCanSendCurrentProcessData(dcCode, gupCode, custCode,"1").ToList();
			totalCnt = executeDatas.Count;
			if (!executeDatas.Any())
			{
				return new ApiResult
				{
					MsgCode = "10005",
					MsgContent = string.Format(_tacService.GetMsg("10005"), "出庫任務", successCnt, failureCnt, totalCnt),
					TotalCnt = totalCnt,
					SuccessCnt = successCnt,
					FailureCnt = failureCnt
				};
			}

			var allotNoList = executeDatas.Where(x => x.PICK_NO.StartsWith("T")).Select(x=> x.PICK_NO).ToList();
			var wmsNoList = executeDatas.Where(x => x.PICK_NO.StartsWith("P")).Select(x => x.WMS_NO).ToList();
			// 如果有出貨任務，取得集貨資訊
			if (wmsNoList.Any())
			{
				var f051301Repo = new F051301Repository(Schemas.CoreSchema);
				collectionList = f051301Repo.GetWcsOutboundCollectionData(dcCode, gupCode, custCode, wmsNoList).ToList();
			}
			#endregion

			foreach (var f060201 in executeDatas)
			{
				// 出貨任務
				if(f060201.PICK_NO.StartsWith("P"))
				{
					#region 出貨任務 檢核
					if (f051201Repo == null)
						f051201Repo = new F051201Repository(Schemas.CoreSchema,wmsTransaction);
					var pickData = f051201Repo.GetWcsOutboundPickOrdData(f060201.DC_CODE, f060201.GUP_CODE, f060201.CUST_CODE, f060201.PICK_NO);
					if(pickData == null)
					{
						f060201.STATUS = "9";
						f060201.MESSAGE = "找不到此單據，不派發出庫任務";
						f060201Repo.UpdateToCancel(f060201.CMD_TYPE,f060201.DOC_ID,f060201.STATUS,f060201.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					// 檢查是否取消揀貨單
					if (pickData.PickStatus == 9)
					{
						f060201.STATUS = "9";
						f060201.MESSAGE = "單據已取消，不派發出庫任務";
						f060201Repo.UpdateToCancel(f060201.CMD_TYPE, f060201.DOC_ID, f060201.STATUS, f060201.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					// 檢查是否已揀貨完成
					if (pickData.PickStatus == 2)
					{
						f060201.STATUS = "9";
						f060201.MESSAGE = "單據已揀貨完成，不派發出庫任務";
						f060201Repo.UpdateToCancel(f060201.CMD_TYPE, f060201.DOC_ID, f060201.STATUS, f060201.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}

					var skuList = new List<WcsOutboundSkuModel>();
					if (f060201.WMS_NO.StartsWith("O"))
					{
						if(f051202Repo == null)
						 f051202Repo = new F051202Repository(Schemas.CoreSchema);
						skuList = f051202Repo.GetWcsDetail(dcCode, gupCode, custCode, f060201.PICK_NO, f060201.WMS_NO, f060201.WAREHOUSE_ID).ToList();
					}
					else
					{
						if (f051203Repo == null)
						 f051203Repo = new F051203Repository(Schemas.CoreSchema);
						skuList = f051203Repo.GetWcsDetail(dcCode, gupCode, custCode, f060201.WMS_NO, f060201.WAREHOUSE_ID).ToList();
					}
					// 檢查是否還有明細資料
					if(!skuList.Any())
					{
						f060201.STATUS = "9";
						f060201.MESSAGE = "單據已無明細資料，不派發出庫任務";
						f060201Repo.UpdateToCancel(f060201.CMD_TYPE, f060201.DOC_ID, f060201.STATUS, f060201.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					#endregion

					#region 出貨任務傳入參數

					req = new WcsOutboundReq
					{
						OwnerCode = custCode,
						OrderCode = f060201.DOC_ID,
						OrderType = 0,
						IsAllowLack = 1,
						ContainerType = pickData.ContainerType,
						OriOrderCode = f060201.WMS_NO,
						SkuTotal = skuList.Count,
						SkuList = skuList
					};

					// 出庫類型(0=訂單出、1=跨庫調撥出、2=庫內移動出、3=廠退出)
					if (pickData.SourceType == "13")
						req.OrderType = 3;
					else
						req.OrderType = pickData.CustCost == "MoveOut" ? 1 : (pickData.NpFlag == "1" ? 5 : 0);

					// 優先處理 (0: 普通、1: 緊急、2: 加急、99999=特級) 預設為0
					if (pickData.FastDealType == "1")
						req.Priority = 0;
					if (pickData.FastDealType == "2")
						req.Priority = 1;
					if (pickData.FastDealType == "3")
						req.Priority = 2;
					if (pickData.PickType == "5")
						req.Priority = 99999;

					// 如果配庫有指定優先順序，先以配庫的優先順序為主

					if (pickData.PriorityValue.HasValue)
						req.Priority = pickData.PriorityValue.Value;

					var collectionData = collectionList.FirstOrDefault(x => x.WmsNo == f060201.WMS_NO);

					// 下一步操作提示(ex:集貨、包裝、跨庫調撥、廠退)
					if (pickData.NextStep == "2" && collectionData != null)// 若下一步為集貨場，則需要找出集貨場名稱
							req.NextStep = collectionData.CollectionName;
					
					else
						req.NextStep = pickData.NextStepName;

					// 目的地 (跨庫調撥出=>跨庫目的地,廠退=> 廠退廠商編號, 一般出貨=> 包裝線/包裝站)
					if (pickData.NextStep == "6")
						req.TargetCode = pickData.MoveOutTarget;
					else if (pickData.SourceType == "13")
						req.TargetCode = pickData.RtnVnrCode;
					else
						req.TargetCode = pickData.PackingType;

					// 單據是否自我滿足(0=否，1=是)
					req.IsSelfSatisfy = pickData.PickType == "6" ? 1 : 0;

					if (req.OrderType == 1)
						req.ContainerType = f190105.DF_CSHIP_CONTANER_TYPE;
					else if (req.OrderType == 3)
						req.ContainerType = f190105.DF_VNRSHIP_CONTAINER_TYPE;
					else if (req.OrderType == 0)
						req.ContainerType = pickData.ContainerType;

					#endregion
				}
				// 調撥任務
				else
				{

					#region 調撥任務 檢核
					if (f151001Repo == null)
						f151001Repo = new F151001Repository(Schemas.CoreSchema, wmsTransaction);

					var allocData = f151001Repo.GetF151001(dcCode, gupCode, custCode, f060201.PICK_NO);
					if (allocData == null)
					{
						f060201.STATUS = "9";
						f060201.MESSAGE = "找不到此單據，不派發出庫任務";
						f060201Repo.UpdateToCancel(f060201.CMD_TYPE, f060201.DOC_ID, f060201.STATUS, f060201.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					// 檢查是否取消調撥單
					if (allocData.STATUS == "9")
					{
						f060201.STATUS = "9";
						f060201.MESSAGE = "單據已取消，不派發出庫任務";
						f060201Repo.UpdateToCancel(f060201.CMD_TYPE, f060201.DOC_ID, f060201.STATUS, f060201.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					// 檢查是否取消調撥單
					if (allocData.STATUS == "5")
					{
						f060201.STATUS = "9";
						f060201.MESSAGE = "單據已結案，不派發出庫任務";
						f060201Repo.UpdateToCancel(f060201.CMD_TYPE, f060201.DOC_ID, f060201.STATUS, f060201.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					// 檢查是否已下架完成
					if (allocData.STATUS == "3" || allocData.STATUS == "4")
					{
						f060201.STATUS = "9";
						f060201.MESSAGE = "單據已下架完成，不派發出庫任務";
						f060201Repo.UpdateToCancel(f060201.CMD_TYPE, f060201.DOC_ID, f060201.STATUS, f060201.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}


					#endregion

					#region 調撥任務傳入參數
					if (f151002Repo == null)
						f151002Repo = new F151002Repository(Schemas.CoreSchema);
					var details = f151002Repo.GetWcsDetail(dcCode, gupCode, custCode, f060201.PICK_NO).ToList();
					req = new WcsOutboundReq
					{
						OwnerCode = custCode,
						OrderCode = f060201.DOC_ID,
						OrderType = 2,
						IsAllowLack = 1,
						Priority = 0,
						ContainerType = f190105.DF_MOVE_CONTAINER_TYPE,
						OriOrderCode = f060201.WMS_NO,
						IsSelfSatisfy = 1,
						SkuTotal = details.Count,
						SkuList = details
					};

					// 下一步操作提示(ex:集貨、包裝、跨庫調撥、廠退)
					req.NextStep = !string.IsNullOrWhiteSpace(allocData.TAR_WAREHOUSE_ID) ? allocData.TAR_WAREHOUSE_ID : allocData.PRE_TAR_WAREHOUSE_ID;

					// 目的地
					req.TargetCode = !string.IsNullOrWhiteSpace(allocData.TAR_WAREHOUSE_ID) ? allocData.TAR_WAREHOUSE_ID : allocData.PRE_TAR_WAREHOUSE_ID;

					if (new string[] { "2", "3", "5" }.Contains(allocData.ALLOCATION_TYPE))
						req.OrderType = 4;

					#endregion
				}

				#region 執行出庫任務派發API

				
				var result = new ApiResult { IsSuccessed = false };

				// 出庫任務Url
				string url = $"v1/{f060201.DC_CODE}/{f060201.WAREHOUSE_ID}/Outbound";
				ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "WcsOutboundResult", new { WcsApiUrl = $"{WcsSetting.ApiUrl}{url}", WcsToken = WcsSetting.ApiAuthToken, WcsData = isSaveWcsData ? req : null, F060201 = f060201 }, () =>
				{
					if (req != null)
					{
						f060201.RESENT_CNT++;
						f060201.PROC_DATE = DateTime.Now;
#if (DEBUG)
						result = WcsApiFuncTest(req, "Outbound");
#else
              result = WcsApiFunc(req, url);
#endif

						f060201.MESSAGE = result.MsgContent;

						if (result.IsSuccessed)
						{
							f060201.STATUS = "2";// 成功狀態改為2
							successCnt++;

							// 修改揀貨單狀態
							if (f060201.PICK_NO.StartsWith("P"))
							{
								orderService.AddF050305(dcCode, gupCode, custCode, new List<string> { f060201.WMS_NO }, "1");
								f051201Repo.UpdateStatus(dcCode, gupCode, custCode, f060201.PICK_NO, "1");
							}
							// 修改調撥單狀態
							else
								f151001Repo.UpdateStatus(dcCode, gupCode, custCode, f060201.PICK_NO, "2");

							wmsTransaction.Complete();
						}
						else
						{
							if(result.Data!=null)
							{
								var errDataString = JsonConvert.SerializeObject(result.Data);
								if (errDataString.Contains("記錄已存在") || errDataString.Contains("记录已存在"))
								{
									f060201.STATUS = "2";
									f060201.MESSAGE += " 回傳內容含有記錄已存在，不再重新派發出庫任務";
									successCnt++;

									// 修改揀貨單狀態
									if (f060201.PICK_NO.StartsWith("P"))
									{
										orderService.AddF050305(dcCode, gupCode, custCode, new List<string> { f060201.WMS_NO }, "1");
										f051201Repo.UpdateStatus(dcCode, gupCode, custCode, f060201.PICK_NO, "1");
									}
									// 修改調撥單狀態
									else
										f151001Repo.UpdateStatus(dcCode, gupCode, custCode, f060201.PICK_NO, "2");

									wmsTransaction.Complete();
								}
								else
									f060201.STATUS = "T";
							}
							else
								f060201.STATUS = "T";
						}
					}
					else
					{
						result = new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = "傳入參數資料為null" };
						f060201.STATUS = "F";
						f060201.MESSAGE += "傳入參數資料為null，不派發出庫任務";
					}
					return result;
				}, false,
					(fResult) =>
					{
						if (!fResult.IsSuccessed)
						{
							if (f060201.RESENT_CNT >= maxResendCnt)
								f060201.STATUS = "F"; //超過重派次數將狀態改為F
							else
								f060201.STATUS = "T";
						}

						#region 更新 F060201 完成、錯誤、逾時狀態
						f060201Repo = new F060201Repository(Schemas.CoreSchema);
						f060201Repo.UpdateExecResult(f060201.CMD_TYPE, f060201.DOC_ID, f060201.STATUS, f060201.MESSAGE,f060201.PROC_DATE.Value,f060201.RESENT_CNT);
						#endregion

						return new ApiResult();
					});
				#endregion

			}

			failureCnt = totalCnt - successCnt;

			return new ApiResult
			{
				MsgCode = "10005",
				MsgContent = string.Format(_tacService.GetMsg("10005"), "出庫任務", successCnt, failureCnt, totalCnt),
				TotalCnt = totalCnt,
				SuccessCnt = successCnt,
				FailureCnt = failureCnt
			};
		}

    #endregion

    #region 出庫任務取消
    /// <summary>
    /// 出庫任務取消
    /// </summary>
    /// <returns></returns>
    public ApiResult OutboundCancel(WcsExportReq req)
    {
      ApiResult res = new ApiResult { IsSuccessed = true };
      List<ApiResponse> data = new List<ApiResponse>();

      // 新增API Log
      res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ExportOutboundCancelResults", req, () =>
      {
        // 取得物流中心服務貨主檔
        CommonService commonService = new CommonService();
        var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);

        dcCustList.ForEach(item =>
        {
          var result = ExportOutboundCancelResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);
          data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
        });

        res.Data = JsonConvert.SerializeObject(data);

        return res;

      }, true);

      return res;
    }

		private ApiResult ExportOutboundCancelResults(string dcCode, string gupCode, string custCode)
		{
			#region 初始化設定
			var maxResendCnt = GetMidApiRelmt();
			var maxDataCnt = GetMidApiMisMax();
			int successCnt = 0;
			int failureCnt = 0;
			int totalCnt = 0;
			var wmsTransaction = new WmsTransaction();
			var f060201Repo = new F060201Repository(Schemas.CoreSchema);
			F051201Repository f051201Repo = null;
			F151001Repository f151001Repo = null;
			WcsOutboundCancelReq req = null;
			var outboundCancelService = new WcsOutboundCancelService();

			#endregion

			#region 取得出庫取消任務資料 與相關資料
			// 更新可派發任務的狀態為處理中
			f060201Repo.UpdateCanSendToProcess(dcCode, gupCode, custCode, maxDataCnt, "2");
			// 取得處理中的任務資料
			var executeDatas = f060201Repo.GetCanSendCurrentProcessData(dcCode, gupCode, custCode, "2").ToList();
			totalCnt = executeDatas.Count;
			if (!executeDatas.Any())
			{
				return new ApiResult
				{
					MsgCode = "10005",
					MsgContent = string.Format(_tacService.GetMsg("10005"), "出庫取消任務", successCnt, failureCnt, totalCnt),
					TotalCnt = totalCnt,
					SuccessCnt = successCnt,
					FailureCnt = failureCnt
				};
			}

			var allotNoList = executeDatas.Where(x => x.PICK_NO.StartsWith("T")).Select(x => x.PICK_NO).ToList();
			var wmsNoList = executeDatas.Where(x => x.PICK_NO.StartsWith("P")).Select(x => x.WMS_NO).ToList();
			
			#endregion

			foreach (var f060201 in executeDatas)
			{
				// 出貨取消任務
				if (f060201.PICK_NO.StartsWith("P"))
				{
					#region 出貨出庫取消任務 檢核
					if (f051201Repo == null)
						f051201Repo = new F051201Repository(Schemas.CoreSchema, wmsTransaction);
					var pickData = f051201Repo.GetF051201(f060201.DC_CODE, f060201.GUP_CODE, f060201.CUST_CODE, f060201.PICK_NO);
					if (pickData == null)
					{
						f060201.STATUS = "9";
						f060201.MESSAGE = "找不到此單據，不派發出庫取消任務";
						f060201Repo.UpdateToCancel(f060201.CMD_TYPE,f060201.DOC_ID,f060201.STATUS,f060201.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					// 檢查是否取消揀貨單
					if (pickData.PICK_STATUS == 9)
					{
						f060201.STATUS = "9";
						f060201.MESSAGE = "單據已取消，不派發出庫取消任務";
						f060201Repo.UpdateToCancel(f060201.CMD_TYPE, f060201.DOC_ID, f060201.STATUS, f060201.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					// 檢查是否已揀貨完成
					if (pickData.PICK_STATUS == 2)
					{
						f060201.STATUS = "9";
						f060201.MESSAGE = "單據已揀貨完成，不派發出庫取消任務";
						f060201Repo.UpdateToCancel(f060201.CMD_TYPE, f060201.DOC_ID, f060201.STATUS, f060201.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}					
					#endregion

					
				}
				// 調撥取消任務
				else
				{

					#region 調撥取消任務 檢核
					if (f151001Repo == null)
						f151001Repo = new F151001Repository(Schemas.CoreSchema, wmsTransaction);

					var allocData = f151001Repo.GetF151001(dcCode, gupCode, custCode, f060201.PICK_NO);
					if (allocData == null)
					{
						f060201.STATUS = "9";
						f060201.MESSAGE = "找不到此單據，不派發出庫取消任務";
						f060201Repo.UpdateToCancel(f060201.CMD_TYPE, f060201.DOC_ID, f060201.STATUS, f060201.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					// 檢查是否取消調撥單
					if (allocData.STATUS == "9")
					{
						f060201.STATUS = "9";
						f060201.MESSAGE = "單據已取消，不派發出庫取消任務";
						f060201Repo.UpdateToCancel(f060201.CMD_TYPE, f060201.DOC_ID, f060201.STATUS, f060201.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					// 檢查是否取消調撥單
					if (allocData.STATUS == "5")
					{
						f060201.STATUS = "9";
						f060201.MESSAGE = "單據已結案，不派發出庫取消任務";
						f060201Repo.UpdateToCancel(f060201.CMD_TYPE, f060201.DOC_ID, f060201.STATUS, f060201.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					// 檢查是否已下架完成
					if (allocData.STATUS == "3" || allocData.STATUS == "4")
					{
						f060201.STATUS = "9";
						f060201.MESSAGE = "單據已下架完成，不派發出庫取消任務";
						f060201Repo.UpdateToCancel(f060201.CMD_TYPE, f060201.DOC_ID, f060201.STATUS, f060201.MESSAGE);
						continue; //這個是executeDatas.ForEach的Continue
					}
					#endregion

				}

				#region 出庫任務派發檢查

				var sendF060201 = f060201Repo.GetData("1", f060201.DOC_ID);
				if (sendF060201 == null)
				{
					f060201.STATUS = "9";
					f060201.MESSAGE = "找不到派發出庫任務，不派發出庫取消任務";
					f060201Repo.UpdateToCancel(f060201.CMD_TYPE, f060201.DOC_ID, f060201.STATUS, f060201.MESSAGE);
					continue;
				}
				else if(sendF060201.STATUS == "9")
				{
					f060201.STATUS = "9";
					f060201.MESSAGE = "出庫任務派發已取消，不派發出庫取消任務";
					f060201Repo.UpdateToCancel(f060201.CMD_TYPE, f060201.DOC_ID, f060201.STATUS, f060201.MESSAGE);
					continue;
				}
				else if (sendF060201.STATUS == "0" || sendF060201.STATUS == "1" || sendF060201.STATUS == "3")
				{
					f060201.STATUS = "0"; // 1要轉回0
					f060201.MESSAGE = "出庫任務尚未派發或派發執行中，跳過，暫不派發出庫取消任務";
					f060201Repo.UpdateToCancel(f060201.CMD_TYPE, f060201.DOC_ID, f060201.STATUS, f060201.MESSAGE);
					continue;
				}
				else
				{
					//sendF060201.STATUS=2(已派發) => 執行出庫取消任務
				}

				#endregion

				#region 執行出庫取消任務派發API


				var result = new ApiResult { IsSuccessed = false };

				#region 出庫取消任務傳入參數
				req = new WcsOutboundCancelReq
				{
					OwnerCode = custCode,
					OrderCode = f060201.DOC_ID
				};
				#endregion

				// 出庫任務取消Url
				string url = $"v1/{f060201.DC_CODE}/{f060201.WAREHOUSE_ID}/Outbound/Cancel";
				ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "WcsOutboundCancelResult", new { WcsApiUrl = $"{WcsSetting.ApiUrl}{url}", WcsToken = WcsSetting.ApiAuthToken, WcsData = isSaveWcsData ? req : null, F060201 = f060201 }, () =>
				{
					if (req != null)
					{
						f060201.RESENT_CNT++;
						f060201.PROC_DATE = DateTime.Now;
						result = outboundCancelService.OutboundCancel(url, req);

						f060201.MESSAGE = result.MsgContent;

						if (result.IsSuccessed)
						{
							f060201.STATUS = "2";// 成功狀態改為2
							successCnt++;

							#region 虛擬儲位回復
							OutboundCancelVirtualRestore(dcCode, gupCode, custCode, f060201.PICK_NO);
							#endregion

							wmsTransaction.Complete();
						}
					}
					else
					{
						result = new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = "傳入參數資料為null" };
						f060201.STATUS = "F";
						f060201.MESSAGE += "傳入參數資料為null，不派發出庫取消任務";
					}
					return result;
				}, false,
					(fResult) =>
					{
						// 回傳代碼是失敗，但第二層內容有回傳取消成功或取消失敗處理
						if (!fResult.IsSuccessed)
						{
							if (f060201.RESENT_CNT >= maxResendCnt)
								f060201.STATUS = "F"; //超過重派次數，狀態改為F
							else
								f060201.STATUS = "T";

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
								var rtnData = JsonConvert.DeserializeObject<List<WcsOutboundCancelResData>>(strSerializeJson).FirstOrDefault();
								if (rtnData == null)
									errMesg = $"Msg:{result.MsgContent} Status:null ErrorMsg:Wcs Api 回傳的Data 資料異常，無法轉換";

								if (string.IsNullOrWhiteSpace(errMesg) && string.IsNullOrEmpty(rtnData.Status))
									errMesg = $"Msg:{result.MsgContent} Status:{rtnData.Status} ErrorMsg:Wcs Api 回傳的Data Status為空白，無法識別";

								if (string.IsNullOrWhiteSpace(errMesg) && !new[] { "0", "1" }.Contains(rtnData.Status))
									errMesg = $"Msg:{result.MsgContent} Status:{rtnData.Status} ErrorMsg:Wcs Api 回傳的Data Status為{rtnData.Status}，無法識別";

								if (string.IsNullOrWhiteSpace(errMesg) && rtnData.Status == "1") // 回傳取消失敗
									errMesg = $"Msg:{result.MsgContent} Status:{rtnData.Status} ErrorMsg:{rtnData.ErrorMsg}";
								if (string.IsNullOrWhiteSpace(errMesg) && rtnData.Status == "0") // 回傳取消成功
									errMesg = string.Empty;

							}

							if (!string.IsNullOrWhiteSpace(errMesg))
							{
								f060201.STATUS = "F";
								f060201.MESSAGE = errMesg.Length > 4000 ? errMesg.Substring(0, 4000) : errMesg;
							}
							else
							{
								f060201.STATUS = "2";
								successCnt++;

								#region 虛擬儲位回復
								OutboundCancelVirtualRestore(dcCode, gupCode, custCode, f060201.PICK_NO);
								#endregion
							}
						}

						#region 更新 F060201 完成、錯誤、逾時狀態
						f060201Repo = new F060201Repository(Schemas.CoreSchema);
						f060201Repo.UpdateExecResult(f060201.CMD_TYPE,f060201.DOC_ID,f060201.STATUS,f060201.MESSAGE,f060201.PROC_DATE.Value,f060201.RESENT_CNT);
						#endregion

						return new ApiResult();
					});
				#endregion

			}

			failureCnt = totalCnt - successCnt;

			return new ApiResult
			{
				MsgCode = "10005",
				MsgContent = string.Format(_tacService.GetMsg("10005"), "出庫取消任務", successCnt, failureCnt, totalCnt),
				TotalCnt = totalCnt,
				SuccessCnt = successCnt,
				FailureCnt = failureCnt
			};
		}

    /// <summary>
    /// 虛擬儲位回復
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="pickNo"></param>
    private void OutboundCancelVirtualRestore(string dcCode,string gupCode,string custCode,string pickNo)
    {
      var wmsTransaction = new WmsTransaction();
      var sharedService = new SharedService(wmsTransaction);
      var stockService = new StockService(wmsTransaction);
      
      sharedService.StockService = stockService;
      var virLocRes = sharedService.PickVirtualLocRecovery(dcCode, gupCode, custCode, pickNo);
      if (virLocRes.IsSuccessed)
      {
        stockService.SaveChange();
        wmsTransaction.Complete();
      }

    }
    #endregion
  }
}
