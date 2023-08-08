using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Lms.Services;
using Wms3pl.WebServices.Shared.Lms.WebApiConnectSetting;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;

namespace Wms3pl.WebServices.ForeignWebApi.Business.LmsServices
{
    public class ExportWarehouseInService : LmsBaseService
	{
		private WmsTransaction _wmsTransaction;
		

		public ExportWarehouseInService(WmsTransaction wmsTransation)
		{
			_wmsTransaction = wmsTransation;
		}

		/// <summary>
		/// 匯出商品進倉結果
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		public ApiResult ExportWarehouseInResults(string dcCode, string gupCode, string custCode)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();
			var f010205Repo = new F010205Repository(Schemas.CoreSchema);
			TransApiBaseService tacService = new TransApiBaseService();

			var commonService = new CommonService();
			var outputJsonInLog = commonService.GetSysGlobalValue("OutputJsonInLog");
			bool isSaveWmsData = string.IsNullOrWhiteSpace(outputJsonInLog) ? false : outputJsonInLog == "1";

			#region 取得 商品進倉未回檔資料

			var f010205s = f010205Repo.AsForUpdate().GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode &&
																																 o.GUP_CODE == gupCode &&
																																 o.CUST_CODE == custCode &&
																																 o.PROC_FLAG == "0").OrderBy(x => x.ID).ToList();
			#endregion

			f010205s.ForEach(f010205 =>
			{
				PurchaseOrderReplyReq req = new PurchaseOrderReplyReq { DcCode = dcCode, CustCode = custCode };

				#region 取得 WarehouseIns資料
				req.WarehouseIns = GetWarehouseIns(f010205);
				#endregion

				#region 取得 WarehouseInDetails資料
				// 取得WarehouseInDetails資料(STATUS = 1 OR 4 才撈取資料)
				if (f010205.STATUS == "1" || f010205.STATUS == "4")
				{
					if (req.WarehouseIns.Any())
						req.WarehouseIns[0].WarehouseInDetails = GetWarehouseInDetails(f010205, req.WarehouseIns[0].Status);
				}
				#endregion

				#region 取得 RcvDatas資料
				// 取得 RcvDatas資料(STATUS= 2 才撈取資料)
				if (f010205.STATUS == "2" && req.WarehouseIns.Any())
					req.WarehouseIns[0].RcvDatas = GetRcvDatas(f010205);
				#endregion

				#region 取得 RcvSnDatas資料
				// 取得 RcvSnDatas資料(STATUS= 2 才撈取資料)
				if (f010205.STATUS == "2" && req.WarehouseIns.Any())
					req.WarehouseIns[0].RcvSnDatas = GetRcvSnDatas(f010205);
				#endregion

				#region 取得 AllocDatas資料
				// 取得 AllocDatas資料(STATUS= 3 才撈取資料)
				if (f010205.STATUS == "3" && req.WarehouseIns.Any())
					req.WarehouseIns[0].AllocDatas = GetAllocDatas(f010205);
				#endregion

				#region 商品進倉結果回傳
				// 新增API Log[< 參數1 >,< 參數2 >,< 參數3 >, LmsApi_ ExportWIResult, 傳入F010205, &LmsApiFunc, false]
				ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "ExportWIResult", new { LmsApiUrl = LmsSetting.ApiUrl + "InboundOrder/Reply", LmsToken = LmsSetting.ApiAuthToken, WmsCondition = f010205, LmsData = isSaveWmsData ? req : null }, () =>

				{
					ApiResult result = new ApiResult();
#if (DEBUG)
					result = LmsApiFuncTest(req, "InboundOrder/Reply");
#else
                        result = LmsApiFunc(req, "InboundOrder/Reply");
#endif
					if (result != null)
					{
						if (result.IsSuccessed)
						{
							#region 更新處理狀態
							f010205.PROC_FLAG = "1";
							f010205.TRANS_DATE = DateTime.Now;
							f010205Repo.Update(f010205);
							#endregion
						}
						else
						{
							data.Add(new ApiResponse
							{
								No = $"STOCK_NO：{f010205.STOCK_NO} {(string.IsNullOrWhiteSpace(f010205.RT_NO) ? string.Empty : $"RT_NO：{f010205.RT_NO}")} {(string.IsNullOrWhiteSpace(f010205.ALLOCATION_NO) ? string.Empty : $"ALLOC_NO：{f010205.ALLOCATION_NO}")}",
								MsgCode = result.MsgCode,
								MsgContent = result.MsgContent
							});

							res.FailureCnt++;
						}
					}
					return result;
				}, false);
				#endregion
			});

			if (data.Any())
				res.Data = data;

			res.IsSuccessed = res.FailureCnt == 0;
			res.TotalCnt = f010205s.Count;
			res.MsgCode = "10005";
			res.MsgContent = string.Format(tacService.GetMsg("10005"),
					"商品進倉結果回傳", res.TotalCnt - res.FailureCnt, res.FailureCnt, res.TotalCnt);

			return res;
		}

		private List<PurchaseOrderReplyWarehouseIn> GetWarehouseIns(F010205 f010205)
		{
			var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);

			var res = f010201Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == f010205.DC_CODE &&
																														o.GUP_CODE == f010205.GUP_CODE &&
																														o.CUST_CODE == f010205.CUST_CODE &&
																														o.STOCK_NO == f010205.STOCK_NO).Select(x => new PurchaseOrderReplyWarehouseIn
																														{
																															CustInNo = x.CUST_ORD_NO,
																															OrderType = x.CUST_COST == "In" ? "PO" : "RTO",
																															Status = x.STATUS == "3" ? "1" : f010205.STATUS,
																															WorkTime = f010205.CRT_DATE.ToString("yyyy/MM/dd HH:mm:ss")
																														}).ToList();

			return res;
		}

		private List<PurchaseOrderReplyWarehouseInDetail> GetWarehouseInDetails(F010205 f010205, string status)
		{
			var f010202Repo = new F010202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f010204Repo = new F010204Repository(Schemas.CoreSchema, _wmsTransaction);

			var f010202s = f010202Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == f010205.DC_CODE &&
																																 o.GUP_CODE == f010205.GUP_CODE &&
																																 o.CUST_CODE == f010205.CUST_CODE &&
																																 o.STOCK_NO == f010205.STOCK_NO);

			var f010204s = f010204Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == f010205.DC_CODE &&
																																 o.GUP_CODE == f010205.GUP_CODE &&
																																 o.CUST_CODE == f010205.CUST_CODE &&
																																 o.STOCK_NO == f010205.STOCK_NO);

			var res = (from A in f010202s
								 join B in f010204s
								 on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.STOCK_NO, A.STOCK_SEQ } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.STOCK_NO, B.STOCK_SEQ } into subB
								 from B in subB.DefaultIfEmpty()
								 select new PurchaseOrderReplyWarehouseInDetail
								 {
									 ItemSeq = A.STOCK_SEQ.ToString().PadLeft(2, '0'),
									 ItemCode = A.ITEM_CODE,
									 InQty = A.STOCK_QTY,
									 TotalRcvQty = status == "1" ? 0 : (B == null ? 0 : B.TOTAL_REC_QTY)
								 }).ToList();

			return res;
		}

		private List<PurchaseOrderReplyRcvData> GetRcvDatas(F010205 f010205)
		{
			var f020201Repo = new F020201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f02020109Repo = new F02020109Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1905Repo = new F1905Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);

			var f020201s = f020201Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == f010205.DC_CODE &&
																	o.GUP_CODE == f010205.GUP_CODE &&
																	o.CUST_CODE == f010205.CUST_CODE &&
																	o.PURCHASE_NO == f010205.STOCK_NO &&
																	o.RT_NO == f010205.RT_NO).ToList() ;
			var f02020109s = f02020109Repo.GetDatasByRtNo(f010205.DC_CODE, f010205.GUP_CODE, f010205.CUST_CODE, f010205.RT_NO).ToList();

			var itmCodes = f020201s.Select(x => x.ITEM_CODE).Distinct().ToList();

			var f1905s = f1905Repo.GetF1905ByItems(f010205.GUP_CODE, f010205.CUST_CODE, itmCodes).ToList();

			var f1903s = f1903Repo.GetDatasByItems(f010205.GUP_CODE, f010205.CUST_CODE, itmCodes).ToList();

			var res = (from A in f020201s
								 join B in f1905s
								 on new { A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE } equals new { B.GUP_CODE, B.CUST_CODE, B.ITEM_CODE }
								 join C in f02020109s
								 on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, STOCK_NO = A.PURCHASE_NO, STOCK_SEQ = A.PURCHASE_SEQ } equals new { C.DC_CODE, C.GUP_CODE, C.CUST_CODE, C.STOCK_NO, STOCK_SEQ = C.STOCK_SEQ.ToString() } into subC
								 from C in subC.DefaultIfEmpty()
								 join D in f1903s
								 on new { A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE } equals new { D.GUP_CODE, D.CUST_CODE, D.ITEM_CODE }
								 select new
								 {
									 F020201 = A,
									 F1905 = B,
									 F02020109 = C,
									 F1903 = D
								 }).GroupBy(x => new { x.F020201, x.F1905, x.F1903 }).Select(x => new PurchaseOrderReplyRcvData
								 {
									 ItemSeq = x.Key.F020201.PURCHASE_SEQ.PadLeft(2, '0'),
									 RcvNo = x.Key.F020201.RT_NO,
									 RcvSeq = x.Key.F020201.RT_SEQ,
									 ItemCode = x.Key.F020201.ITEM_CODE,
									 RcvQty = x.Key.F020201.RECV_QTY == null ? 0 : Convert.ToInt32(x.Key.F020201.RECV_QTY),
									 DefectQty = Convert.ToInt32(x.Where(z => z.F02020109 != null).Sum(z => z.F02020109.DEFECT_QTY)),
									 ValidDate = x.Key.F020201.VALI_DATE.HasValue ? x.Key.F020201.VALI_DATE.Value.ToString("yyyy/MM/dd") : "",
                                     PackLength = x.Key.F1905.PACK_LENGTH,
									 PackWidth = x.Key.F1905.PACK_WIDTH,
									 PackHeight = x.Key.F1905.PACK_HIGHT,
									 PackWeight = x.Key.F1905.PACK_WEIGHT,
									 ItemBarCode1 = x.Key.F1903.EAN_CODE1,
									 ItemBarCode2 = x.Key.F1903.EAN_CODE2,
									 ItemBarCode3 = x.Key.F1903.EAN_CODE3,
									 SaveDay = x.Key.F1903.SAVE_DAY,
									 NeedExpired = x.Key.F1903.NEED_EXPIRED,
									 FirstInDate = x.Key.F1903.FIRST_IN_DATE.HasValue ? x.Key.F1903.FIRST_IN_DATE.Value.ToString("yyyy/MM/dd") : "",
									 AllDln = x.Key.F1903.ALL_DLN,
									 AllShp = x.Key.F1903.ALL_SHP,
									 MakeNo = x.Key.F020201.MAKE_NO,
									 IsFragile = Convert.ToBoolean(Convert.ToInt32(x.Key.F1903.FRAGILE)),
									 IsSpill = Convert.ToBoolean(Convert.ToInt32(x.Key.F1903.SPILL)),
									 IsEasyLose = x.Key.F1903.IS_EASY_LOSE,
									 IsPrecious = x.Key.F1903.IS_PRECIOUS,
									 IsMagnetic = x.Key.F1903.IS_MAGNETIC,
                                     IsPerishable = x.Key.F1903.IS_PERISHABLE,
                                     TmprType = x.Key.F1903.TMPR_TYPE,
									 IsTempControl = x.Key.F1903.IS_TEMP_CONTROL,
                                     DefectDatas = GetDefectDatas(x.Key.F020201.DC_CODE, x.Key.F020201.GUP_CODE, x.Key.F020201.CUST_CODE, x.Key.F020201.PURCHASE_NO, Convert.ToInt32(x.Key.F020201.PURCHASE_SEQ), x.Key.F020201.RT_NO, x.Key.F020201.RT_SEQ)
								 }).ToList();

			return res;
		}

		private List<PurchaseOrderReplyDefectDatas> GetDefectDatas(string dcCode,string gupCode, string custCode,string stockNo ,int stockSeq, string rtNo, string rtSeq)
		{
			var f02020109Repo = new F02020109Repository(Schemas.CoreSchema, _wmsTransaction);
			var f02020109s = f02020109Repo.GetDatasByTrueAndCondition(x => x.STOCK_NO == stockNo &&
														x.STOCK_SEQ == stockSeq &&
														x.RT_NO == rtNo &&
														x.RT_SEQ == rtSeq);
			var result = f02020109s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.STOCK_NO, x.STOCK_SEQ,x.UCC_CODE,x.CAUSE }).Select(x => new PurchaseOrderReplyDefectDatas
			{
				DefectCause = x.Key.UCC_CODE,
				DefectCauseMemo = x.Key.CAUSE,
				DefectQty = x.Sum(y=>y.DEFECT_QTY).Value
			}).ToList();
			return result;
		}

		private List<PurchaseOrderReplyRcvSnData> GetRcvSnDatas(F010205 f010205)
		{
			var f02020104Repo = new F02020104Repository(Schemas.CoreSchema, _wmsTransaction);

			var res = f02020104Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == f010205.DC_CODE &&
																															o.GUP_CODE == f010205.GUP_CODE &&
																															o.CUST_CODE == f010205.CUST_CODE &&
																															o.PURCHASE_NO == f010205.STOCK_NO &&
																															o.RT_NO == f010205.RT_NO &&
																															o.ISPASS == "1").GroupBy(x => x.ITEM_CODE).Select(x => new PurchaseOrderReplyRcvSnData
																															{
																																ItemCode = x.Key,
																																SnList = x.Select(z => z.SERIAL_NO).ToArray()
																															}).ToList();

			return res;
		}

		private List<PurchaseOrderReplyAllocData> GetAllocDatas(F010205 f010205)
		{
			var f020202Repo = new F020202Repository(Schemas.CoreSchema, _wmsTransaction);

			var res = f020202Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == f010205.DC_CODE &&
																														o.GUP_CODE == f010205.GUP_CODE &&
																														o.CUST_CODE == f010205.CUST_CODE &&
																														o.STOCK_NO == f010205.STOCK_NO &&
																														o.RT_NO == f010205.RT_NO &&
																														o.ALLOCATION_NO == f010205.ALLOCATION_NO).GroupBy(x => new { x.STOCK_SEQ, x.ALLOCATION_NO, x.ALLOCATION_SEQ, x.ITEM_CODE, x.WAREHOUSE_ID, x.VALID_DATE, x.ENTER_DATE, x.SERIAL_NO, x.MAKE_NO }).Select(x => new PurchaseOrderReplyAllocData
																														{
																															ItemSeq = x.Key.STOCK_SEQ.ToString().PadLeft(2, '0'),
																															AllocNo = x.Key.ALLOCATION_NO,
																															AllocSeq = x.Key.ALLOCATION_SEQ.ToString(),
																															ItemCode = x.Key.ITEM_CODE,
																															WhNo = x.Key.WAREHOUSE_ID,
																															ActQty = x.Sum(z => z.TAR_QTY),
																															ValidDate = x.Key.VALID_DATE.ToString("yyyy/MM/dd"),
																															EnterDate = x.Key.ENTER_DATE.ToString("yyyy/MM/dd"),
																															Sn = string.IsNullOrWhiteSpace(x.Key.SERIAL_NO) ? "" : x.Key.SERIAL_NO,
																															MakeNo = x.Key.MAKE_NO
																														}).ToList();

			return res;
		}
	}
}