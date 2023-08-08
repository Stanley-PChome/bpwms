using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.Shared.TransApiServices.Common
{
	public class CommonVendorReturnService
	{
		private WmsTransaction _wmsTransation;
		public CommonVendorReturnService(WmsTransaction wmsTransation)
		{
			_wmsTransation = wmsTransation;
		}

		#region Private Property

		/// <summary>
		/// 廠商編號資料清單
		/// </summary>
		private List<F1908> _vnrCodeList;

		/// <summary>
		/// 退貨類型資料清單
		/// </summary>
		private List<F160203> _vnrReturnTypeList;

		/// <summary>
		/// 退貨原因資料清單
		/// </summary>
		private List<string> _vnrReturnCauseList;

		/// <summary>
		/// 第一個廠退原因
		/// </summary>
		private string _firstVnrReturnCause;

		/// <summary>
		/// 出貨倉別
		/// </summary>
		private List<string> _typeIdList;

		/// <summary>
		/// 單據狀態
		/// </summary>
		private string _proc_status;

		/// <summary>
		/// 廠商退貨單主檔
		/// </summary>
		private List<F160201> _f160201List = new List<F160201>();

		/// <summary>
		/// 廠商退貨單明細
		/// </summary>
		private List<F160202> _f160202List = new List<F160202>();

		/// <summary>
		/// 已存在廠商退貨單編號清單
		/// </summary>
		private List<ThirdPartVendorReturns> _thirdPartVnrReturnsList;

		/// <summary>
		/// 失敗廠商退貨單數
		/// </summary>
		private int _failCnt = 0;

		/// <summary>
		/// 暫存訊息池清單
		/// </summary>
		private List<AddMessageReq> _addMessageTempList = new List<AddMessageReq>();

		/// <summary>
		/// 紀錄有新增過F075103的CUST_ORD_NO，用以若檢核失敗 找出是否有新增，用以刪除
		/// </summary>
		private List<string> _IsAddF075103CustOrdNoList = new List<string>();
		#endregion

		#region Main Method
		/// <summary>
		/// Func1
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult RecevieApiDatas(PostCreateVendorReturnsReq req)
		{
			CheckTransApiService ctaService = new CheckTransApiService();
			TransApiBaseService tacService = new TransApiBaseService();
			SharedService sharedService = new SharedService();
			CommonService commonService = new CommonService();
			ApiResult res = new ApiResult { IsSuccessed = true };

			#region 資料檢核1

			// 檢核物流中心 必填、是否存在
			ctaService.CheckDcCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核貨主編號 必填、是否存在
			ctaService.CheckCustCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核Returns
			ctaService.CheckResult(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核VnrReturns
			if (req.Result.VnrReturns == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };

			// 檢核資料筆數
			int reqTotal = req.Result.Total != null ? Convert.ToInt32(req.Result.Total) : 0;
			if (req.Result.VnrReturns == null || (req.Result.VnrReturns != null && !tacService.CheckDataCount(reqTotal, req.Result.VnrReturns.Count)))
				return new ApiResult { IsSuccessed = false, MsgCode = "20054", MsgContent = string.Format(tacService.GetMsg("20054"), reqTotal, req.Result.VnrReturns.Count) };

			// 檢核總明細筆數是否超過[廠商退貨明細最大筆數]筆
			int vrdMaxCnt = Convert.ToInt32(commonService.GetSysGlobalValue("VRDMaxCnt"));
			int detailTotalCnt = req.Result.VnrReturns.Where(x => x.VnrReturnDetails != null).Sum(x => x.VnrReturnDetails.Count);
			if (detailTotalCnt > vrdMaxCnt)
				return new ApiResult { IsSuccessed = false, MsgCode = "20055", MsgContent = string.Format(tacService.GetMsg("20055"), detailTotalCnt) };

			#endregion

			// 取得業主編號
			string gupCode = commonService.GetGupCode(req.CustCode);

			// 資料處理1
			return ProcessApiDatas(req.DcCode, gupCode, req.CustCode, req.Result.VnrReturns);
		}

		/// <summary>
		/// 資料處理1
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="vnrReturns">廠商退貨單資料物件清單</param>
		/// <returns></returns>
		public ApiResult ProcessApiDatas(string dcCode, string gupCode, string custCode, List<PostCreateVendorReturnsModel> vnrReturns)
		{
			F075103Repository f075103Repo = new F075103Repository(Schemas.CoreSchema);
			F160201Repository f160201Repo = new F160201Repository(Schemas.CoreSchema, _wmsTransation);
			F160202Repository f160202Repo = new F160202Repository(Schemas.CoreSchema, _wmsTransation);
			F1903Repository f1903Repo = new F1903Repository(Schemas.CoreSchema);
			TransApiBaseService tacService = new TransApiBaseService();
			VendorReturnService vrtService = new VendorReturnService(_wmsTransation);
			SharedService sharedService = new SharedService(_wmsTransation);
			int insertCnt = 0;

			#region Private Property

      if (vnrReturns.Any(o => o.ProcFlag != "D"))
      {
        // 取得廠商編號清單
        F1908Repository f1908Repo = new F1908Repository(Schemas.CoreSchema);
        _vnrCodeList = f1908Repo.GetDatas(gupCode, custCode, vnrReturns.Select(x => x.SupCode).Distinct().ToList()).ToList();

        // 取得退貨類型清單
        F160203Repository f160203Repo = new F160203Repository(Schemas.CoreSchema);
        _vnrReturnTypeList = f160203Repo.GetALLF160203().ToList();

        // 取得退貨原因清單
        F1951Repository f1951Repo = new F1951Repository(Schemas.CoreSchema);
        _vnrReturnCauseList = f1951Repo.GetDatasByTrueAndCondition(o => o.UCT_ID == "RV").Select(x => x.UCC_CODE).ToList();

			  // 取得第一筆廠退原因
			  var cause = _vnrReturnCauseList.FirstOrDefault();
			  _firstVnrReturnCause = cause == null ? string.Empty : cause;

        // 取得倉別清單
        F198001Repository f198001Repo = new F198001Repository(Schemas.CoreSchema);
        _typeIdList = f198001Repo.GetDatasByTypeIds(vnrReturns.Select(x => x.TypeId).ToList()).Select(x => x.TYPE_ID).ToList();
      }
			
			// 取得已存在退貨單編號清單
			_thirdPartVnrReturnsList = f160201Repo.GetDatasByCustVnrRetrunNos(dcCode, gupCode, custCode, vnrReturns.Select(x => x.CustVnrRetrunNo).ToList()).Select(x => new ThirdPartVendorReturns { CUST_ORD_NO = x.CUST_ORD_NO, RTN_VNR_NO = x.RTN_VNR_NO }).ToList();
			#endregion

			#region Foreach [廠商退貨單資料物件] 檢核
			var res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			// 避免若同一批參數有重複單號會把之前成功寫入的F075103的刪除，所以取得重複數
			var paramCustOrdNos = vnrReturns.Select(x => x.CustVnrRetrunNo).GroupBy(x => x).Select(x => new { CustVnrRetrunNo = x.Key, Cnt = x.Count() }).ToList();

			vnrReturns.ForEach(currVnrReturn =>
			{
        if (currVnrReturn.ProcFlag != "D")
        {
				  // 資料處理2
				  var res1 = CheckReturn(dcCode, gupCode, custCode, currVnrReturn);

				  if (!res1.IsSuccessed)
				  {
					  data.AddRange((List<ApiResponse>)res1.Data);

					  _thirdPartVnrReturnsList = _thirdPartVnrReturnsList.Where(x => x.CUST_ORD_NO != currVnrReturn.CustVnrRetrunNo).ToList();

					  var currCustOrdNo = paramCustOrdNos.Where(x => x.CustVnrRetrunNo == currVnrReturn.CustVnrRetrunNo).FirstOrDefault();

					  // 若驗證失敗 刪除新增的F075103
					  if (_IsAddF075103CustOrdNoList.Contains(currVnrReturn.CustVnrRetrunNo) && currCustOrdNo != null && currCustOrdNo.Cnt == 1)
					  {
						  f075103Repo.DelF075103ByKey(custCode, currVnrReturn.CustVnrRetrunNo);
						  _IsAddF075103CustOrdNoList = _IsAddF075103CustOrdNoList.Where(x => x != currVnrReturn.CustVnrRetrunNo).ToList();
					  }
				  }
        }
			});
			#endregion

			#region 處理純新增廠商退貨單
			// 暫存新增的廠商退貨單清單排除[貨主單號已產生WMS訂單]
			var addF160201List = _f160201List.Where(x => !_thirdPartVnrReturnsList.Select(z => z.CUST_ORD_NO).Contains(x.CUST_ORD_NO)).ToList();

			if (addF160201List.Any())
			{
				// 暫存新增的廠商退貨單明細清單.Contain(addF010201List.STOCK_NO)
				var addF160202List = _f160202List.Where(x => addF160201List.Select(z => z.RTN_VNR_NO).Contains(x.RTN_VNR_NO)).ToList();

				addF160201List.ForEach(addF160201 =>
				{
					// [取得廠商退貨單號]
					var rtnVnrNo = sharedService.GetNewOrdCode("V");

					// 寫入行事曆訊息池
					AddMessagePool(_wmsTransation, addF160201.RTN_VNR_NO, rtnVnrNo);

					addF160202List.Where(x => x.RTN_VNR_NO == addF160201.RTN_VNR_NO).ToList().ForEach(addF160202 =>
									{
										addF160202.RTN_VNR_NO = rtnVnrNo;
									});

					addF160201.RTN_VNR_NO = rtnVnrNo;
				});

				// BulkInsert addF160201List, addF010202List
				if (addF160201List.Any())
				{
					f160201Repo.BulkInsert(addF160201List);
					insertCnt = addF160201List.Count;
				}

				if (addF160202List.Any())
					f160202Repo.BulkInsert(addF160202List);

				if (addF160201List.Any() || addF160202List.Any())
					_wmsTransation.Complete();
			}
			#endregion

			#region 已存在廠商退貨單(by 每一筆廠商退貨單commit)

			// 暫存新增的廠商退貨單清單篩選含[貨主單號已產生WMS廠商退貨單]
			var cancelF160201List = _f160201List.Where(x => _thirdPartVnrReturnsList.Select(z => z.CUST_ORD_NO).Contains(x.CUST_ORD_NO)).ToList();
			//    Foreach[找到的WMS單] in [貨主單號已產生WMS廠商退貨單]
			_thirdPartVnrReturnsList.ForEach(o =>
			{
				var wmsTransation2 = new WmsTransaction();
				sharedService = new SharedService(wmsTransation2);
				vrtService = new VendorReturnService(wmsTransation2);

				var vnrReturn = vnrReturns.Where(x => x.CustVnrRetrunNo == o.CUST_ORD_NO).LastOrDefault();

				// 取消廠商退貨單
				var isOk = vrtService.CancelNotProcessVnrReturn(dcCode, gupCode, custCode, o.RTN_VNR_NO, vnrReturn.ProcFlag);

				// var cancelF010201 = CancelF010201List.Where(x => CUST_ORD_NO =[找到的WMS單].CUST_ORD_NO)
				var cancelF160201 = cancelF160201List.Where(x => x.CUST_ORD_NO == o.CUST_ORD_NO).SingleOrDefault();

				if (isOk)
				{
					if (cancelF160201 == null)
						// 寫入行事曆訊息池
						sharedService.AddMessagePool("9", dcCode, gupCode, custCode, "API21055", string.Format(tacService.GetMsg("21055"), o.RTN_VNR_NO), "", "0", "SCH");
				}
				else
				{
					// [單號{0}]此單據WMS廠商退貨單已在作業中，無法取消或更新單據
					data.Add(new ApiResponse { MsgCode = "20073", MsgContent = string.Format(tacService.GetMsg("20073"), o.CUST_ORD_NO), No = o.CUST_ORD_NO });

					_failCnt++;

					if (cancelF160201 != null)
					{
						// 暫存新增的廠商退貨單清單.Remove(cancelF010201)
						_f160201List.Remove(cancelF160201);

						// var cancelF010202 = 暫存新增的廠商退貨單明細清單.Contain(cancelF010201.STOCK_NO)
						var cancelF160202 = _f160202List.Where(x => x.RTN_VNR_NO == cancelF160201.RTN_VNR_NO);

						// 暫存新增的廠商退貨明細清單.Except(cancelF010202)
						_f160202List = _f160202List.Except(cancelF160202).ToList();
					}
				}

				wmsTransation2.Complete();
			});

			#endregion

			#region 寫入行事曆訊息池
			var wmsTransation3 = new WmsTransaction();
			sharedService = new SharedService(wmsTransation3);

			// 取得訊息內容 21054
			int total = vnrReturns.Count;
			int successedCnt = total - _failCnt;

			string msgContent = string.Format(tacService.GetMsg("21054"),
					successedCnt,
					_failCnt,
					total);

			// 寫入行事曆訊息池
			sharedService.AddMessagePool("9", dcCode, gupCode, custCode, "API21054", msgContent, "", "0", "SCH");

			wmsTransation3.Complete();
			#endregion

			res.IsSuccessed = !data.Any();
			res.MsgCode = "21054";
			res.MsgContent = msgContent;
			res.InsertCnt = insertCnt;
			res.UpdateCnt = total - insertCnt - _failCnt;
			res.FailureCnt = _failCnt;
			res.TotalCnt = total;
			res.Data = data.Any() ? data : null;

			return res;
		}

		/// <summary>
		/// 寫入行事曆訊息池
		/// </summary>
		/// <param name="guid"></param>
		/// <param name="rtnVnrNo"></param>
		private void AddMessagePool(WmsTransaction wmsTransaction, string guid, string rtnVnrNo)
		{
			SharedService sharedService = new SharedService(wmsTransaction);

			// 寫入行事曆訊息池
			var addMessageList = _addMessageTempList.Where(x => x.Guid == guid).ToList();
			addMessageList.ForEach(o =>
			{
				o.MessageContent = o.MessageContent.Replace(guid, rtnVnrNo);
				sharedService.AddMessagePool(o.TicketType, o.DcCode, o.GupCode, o.CustCode, o.MsgNo, o.MessageContent, o.NotifyOrdNo, o.TargetType, o.TargetCode);
			});
		}

		/// <summary>
		/// 資料處理2
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="vnrReturns">廠商退貨單資料物件</param>
		/// <returns></returns>
		private ApiResult CheckReturn(string dcCode, string gupCode, string custCode, PostCreateVendorReturnsModel vnrReturns)
		{
			ApiResult result = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			// 預設proc_status = 0
			_proc_status = "0";

			// 預設值設定
			data.AddRange((List<ApiResponse>)CheckDefaultSetting(dcCode, gupCode, custCode, vnrReturns).Data);

			// 共用欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(dcCode, gupCode, custCode, vnrReturns).Data);

			// 貨主自訂欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckCustomColumnType(dcCode, gupCode, custCode, vnrReturns).Data);

			// 如果以上檢核成功
			if (!data.Any())
			{
				// 共用欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCommonColumnData(dcCode, gupCode, custCode, vnrReturns).Data);

				// 貨主自訂欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCustomColumnValue(dcCode, gupCode, custCode, vnrReturns).Data);

				// 如果以上檢核成功
				if (!data.Any())
				{
					// 產生退貨單資料
					CreateVnrReturn(dcCode, gupCode, custCode, vnrReturns);
				}
				else
				{
					_failCnt++;
				}
			}
			else
			{
				_failCnt++;
			}

			result.IsSuccessed = !data.Any();
			result.Data = data;

			return result;
		}

		/// <summary>
		/// 產生退貨單資料
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="vnrReturns">廠商退貨單資料物件</param>
		private void CreateVnrReturn(string dcCode, string gupCode, string custCode, PostCreateVendorReturnsModel vnrReturns)
		{
			// 如果<參數5>.Status<> D 才往下執行
			if (vnrReturns.ProcFlag != "D")
			{
				// 建立廠商退貨單主檔F160201
				F160201 f160201 = CreateF160201(dcCode, gupCode, custCode, vnrReturns);

				// 建立廠商退貨單明細檔F160202
				List<F160202> f160202List = CreateF160202List(vnrReturns, f160201);

				_f160201List.Add(f160201);
				_f160202List.AddRange(f160202List);
			}
		}
		#endregion

		#region Protected 檢核
		/// <summary>
		/// 預設值設定
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="vnrReturns">廠商退貨單資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckDefaultSetting(string dcCode, string gupCode, string custCode, PostCreateVendorReturnsModel vnrReturns)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}

		/// <summary>
		/// 共用欄位格式檢核
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="vnrReturns">廠商退貨單資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckColumnNotNullAndMaxLength(string dcCode, string gupCode, string custCode, PostCreateVendorReturnsModel vnrReturns)
		{
			TransApiBaseService tacService = new TransApiBaseService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			#region 定義需檢核欄位、必填、型態、長度
			// 廠商退貨單資料物件
			List<ApiCkeckColumnModel> warehouseInsCheckColumnList = new List<ApiCkeckColumnModel>();

			// 廠商退貨單明細資料
			List<ApiCkeckColumnModel> warehouseInDetailCheckColumnList = new List<ApiCkeckColumnModel>();

			if (vnrReturns.ProcFlag == "D")
			{
				warehouseInsCheckColumnList = new List<ApiCkeckColumnModel>
				{
						new ApiCkeckColumnModel{  Name = "CustVnrRetrunNo",    Type = typeof(string),   MaxLength = 20, Nullable = false },
						new ApiCkeckColumnModel{  Name = "ProcFlag",           Type = typeof(string),   MaxLength = 1, Nullable = false }
				};
			}
			else if (vnrReturns.ProcFlag == "0")
			{
				warehouseInsCheckColumnList = new List<ApiCkeckColumnModel>
				{
						new ApiCkeckColumnModel{  Name = "CustVnrRetrunNo",    Type = typeof(string),   MaxLength = 20, Nullable = false },
						new ApiCkeckColumnModel{  Name = "SupCode",            Type = typeof(string),   MaxLength = 20, Nullable = false },
						new ApiCkeckColumnModel{  Name = "ReturnDate",         Type = typeof(DateTime), MaxLength = 0,  Nullable = false },
						new ApiCkeckColumnModel{  Name = "VnrReturnType",      Type = typeof(string),   MaxLength = 5,  Nullable = false },
						new ApiCkeckColumnModel{  Name = "CustCategory",       Type = typeof(string),   MaxLength = 20 },
						new ApiCkeckColumnModel{  Name = "Memo",               Type = typeof(string),   MaxLength = 100 },
						new ApiCkeckColumnModel{  Name = "ProcFlag",           Type = typeof(string),   MaxLength = 1, Nullable = false },
						new ApiCkeckColumnModel{  Name = "BatchNo",            Type = typeof(string),   MaxLength = 50 },
						new ApiCkeckColumnModel{  Name = "DeliveryWay",        Type = typeof(string),   MaxLength = 1, Nullable = true },
						new ApiCkeckColumnModel{  Name = "TypeId",						 Type = typeof(string),   MaxLength = 1, Nullable = false }
				};

				warehouseInDetailCheckColumnList = new List<ApiCkeckColumnModel>
				{
						new ApiCkeckColumnModel{  Name = "ItemCode",        Type = typeof(string),   MaxLength = 20,  Nullable = false },
						new ApiCkeckColumnModel{  Name = "ItemSeq",         Type = typeof(string),   MaxLength = 5,   Nullable = false },
						new ApiCkeckColumnModel{  Name = "ReturnQty",       Type = typeof(int),      MaxLength = 6,   Nullable = false },
						new ApiCkeckColumnModel{  Name = "MakeNo",				  Type = typeof(string),   MaxLength = 40,  Nullable = true },
						new ApiCkeckColumnModel{  Name = "VnrReturnCause",  Type = typeof(string),   MaxLength = 5,   Nullable = false },
						new ApiCkeckColumnModel{  Name = "VnrReturnMemo",   Type = typeof(string),   MaxLength = 50,  Nullable = true }
				};
			}
			#endregion

			#region 檢查廠商退貨單欄位必填、最大長度
			List<string> returnIsNullList = new List<string>();
			List<ApiCkeckColumnModel> returnIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();

			// 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
			warehouseInsCheckColumnList.ForEach(column =>
			{
				// 必填
				if (!column.Nullable)
				{
					if (!DataCheckHelper.CheckRequireColumn(vnrReturns, column.Name))
						returnIsNullList.Add(column.Name);
				}

				// 最大長度
				if (column.MaxLength > 0)
				{
					if (!DataCheckHelper.CheckDataMaxLength(vnrReturns, column.Name, column.MaxLength))
						returnIsExceedMaxLenthList.Add(column);
				}
			});

			// 必填訊息
			if (returnIsNullList.Any())
			{
				data.Add(new ApiResponse { No = vnrReturns.CustVnrRetrunNo, MsgCode = "20058", MsgContent = string.Format(tacService.GetMsg("20058"), vnrReturns.CustVnrRetrunNo, string.Join("、", returnIsNullList)) });
			}

			// 最大長度訊息
			if (returnIsExceedMaxLenthList.Any())
			{
				List<string> errorMsgList = returnIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();

				string errorMsg = string.Join("、", errorMsgList);

				data.Add(new ApiResponse { No = vnrReturns.CustVnrRetrunNo, MsgCode = "20059", MsgContent = string.Format(tacService.GetMsg("20059"), vnrReturns.CustVnrRetrunNo, errorMsg) });
			}

			#endregion

			#region 檢查廠商退貨單明細欄位必填、最大長度
			if (vnrReturns.ProcFlag != "D")
			{
				List<string> returnDetailIsNullList;
				List<ApiCkeckColumnModel> returnDetailIsExceedMaxLenthList;

				if (vnrReturns.VnrReturnDetails != null && vnrReturns.VnrReturnDetails.Any())
				{
					for (int i = 0; i < vnrReturns.VnrReturnDetails.Count; i++)
					{
						var currDetail = vnrReturns.VnrReturnDetails[i];

						returnDetailIsNullList = new List<string>();
						returnDetailIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();

						warehouseInDetailCheckColumnList.ForEach(o =>
						{
							// 必填
							if (!o.Nullable)
							{
								if (!DataCheckHelper.CheckRequireColumn(currDetail, o.Name))
									returnDetailIsNullList.Add(o.Name);
							}

							// 最大長度
							if (o.MaxLength > 0)
							{
								if (!DataCheckHelper.CheckDataMaxLength(currDetail, o.Name, o.MaxLength))
									returnDetailIsExceedMaxLenthList.Add(o);
							}
						});

						// 必填訊息
						if (returnDetailIsNullList.Any())
						{
							data.Add(new ApiResponse { No = vnrReturns.CustVnrRetrunNo, MsgCode = "20058", MsgContent = string.Format(tacService.GetMsg("20058"), $"{vnrReturns.CustVnrRetrunNo}第{i + 1}筆明細", string.Join("、", returnDetailIsNullList)) });
						}

						// 最大長度訊息
						if (returnDetailIsExceedMaxLenthList.Any())
						{
							List<string> errorMsgList = returnDetailIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();

							string errorMsg = string.Join("、", errorMsgList);

							data.Add(new ApiResponse { No = vnrReturns.CustVnrRetrunNo, MsgCode = "20059", MsgContent = string.Format(tacService.GetMsg("20059"), $"{vnrReturns.CustVnrRetrunNo}第{i + 1}筆明細", errorMsg) });
						}
					}
				}
			}
			#endregion

			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位格式檢核
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="vnrReturns">廠商退貨單資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnType(string dcCode, string gupCode, string custCode, PostCreateVendorReturnsModel vnrReturns)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}

		/// <summary>
		/// 共用欄位資料檢核
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="vnrReturns">廠商退貨單資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckCommonColumnData(string dcCode, string gupCode, string custCode, PostCreateVendorReturnsModel vnrReturns)
		{
			CheckVendorReturnService cvrService = new CheckVendorReturnService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			#region 主檔欄位資料檢核
			// 檢查ProcFlag
			cvrService.CheckProcFlag(data, vnrReturns);

			// 檢查貨主單號是否存在
			var isAdd = cvrService.CheckCustExistForThirdPart(data, _thirdPartVnrReturnsList, vnrReturns, custCode);
			if(isAdd)
				_IsAddF075103CustOrdNoList.Add(vnrReturns.CustVnrRetrunNo);

			// 檢查廠商編號是否存在
			cvrService.CheckVnrCodeExist(data, _vnrCodeList, vnrReturns);

			// 檢查貨主單號是否存在
			cvrService.CheckCustExist(ref _f160201List, ref _f160202List, dcCode, gupCode, custCode, vnrReturns);

			// 檢查配送方式是否正確
			cvrService.CheckDeliveryWay(data, vnrReturns);

			// 檢查出貨倉別是否存在
			cvrService.CheckTypeId(data, _typeIdList, vnrReturns);
			
			#endregion

			#region 明細欄位資料檢核

			// 檢查明細筆數
			cvrService.CheckDetailCnt(data, vnrReturns);

			// 檢核項次必須大於0，且同一張單據內的序號不可重複
			cvrService.CheckDetailSeq(data, vnrReturns);

			// 檢查明細廠商退貨數量
			cvrService.CheckDetailQty(data, vnrReturns);

			// 檢查廠商退貨原因
			cvrService.CheckVnrReturnCause(data, vnrReturns, _vnrReturnCauseList);

			#endregion

			#region 檢查資料是否完整
			cvrService.CheckReturnData(data, gupCode, custCode, vnrReturns, _vnrReturnTypeList);
			#endregion

			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位資料檢核
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="vnrReturns">廠商退貨單資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnValue(string dcCode, string gupCode, string custCode, PostCreateVendorReturnsModel vnrReturns)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}
		#endregion

		#region Protected 建立廠商退貨單主檔、明細、檢核資料
		/// <summary>
		/// 建立廠商退貨單主檔
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="returns"></param>
		/// <returns></returns>
		protected F160201 CreateF160201(string dcCode, string gupCode, string custCode, PostCreateVendorReturnsModel vnrReturns)
		{
			var f1908 = _vnrCodeList.Where(x => x.VNR_CODE == vnrReturns.SupCode).FirstOrDefault();
			var deliveryWay = string.IsNullOrWhiteSpace(vnrReturns.DeliveryWay) && f1908 != null ? f1908.DELIVERY_WAY : vnrReturns.DeliveryWay;
			
			return new F160201
			{
				RTN_VNR_NO = Guid.NewGuid().ToString(),
				RTN_VNR_DATE = vnrReturns.ReturnDate ?? Convert.ToDateTime(vnrReturns.ReturnDate),
				STATUS = _proc_status,
				ORD_PROP = "V1",
				RTN_VNR_TYPE_ID = vnrReturns.VnrReturnType,
				RTN_VNR_CAUSE = _firstVnrReturnCause,
				SELF_TAKE = deliveryWay == "0" ? "1" :"0",
				VNR_CODE = vnrReturns.SupCode,
				COST_CENTER = vnrReturns.CustCategory,
				MEMO = vnrReturns.Memo,
				POSTING_DATE = null,
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				CUST_ORD_NO = vnrReturns.CustVnrRetrunNo,
				FOREIGN_WMSNO = vnrReturns.CustVnrRetrunNo,
				FOREIGN_CUSTCODE = custCode,
				BATCH_NO = vnrReturns.BatchNo,
				IMPORT_FLAG = "0",
				DELIVERY_WAY = deliveryWay,
				TYPE_ID = vnrReturns.TypeId
			};
		}

		/// <summary>
		/// 建立廠商退貨單明細
		/// </summary>
		/// <param name="returns"></param>
		/// <param name="f160201"></param>
		/// <returns></returns>
		protected List<F160202> CreateF160202List(PostCreateVendorReturnsModel vnrReturns, F160201 f160201)
		{
			List<F160202> result = new List<F160202>();

			if (vnrReturns.VnrReturnDetails != null || (vnrReturns.VnrReturnDetails != null && vnrReturns.VnrReturnDetails.Any()))
			{
				result = vnrReturns.VnrReturnDetails.Select(x => new F160202
				{
					RTN_VNR_NO = f160201.RTN_VNR_NO,
					RTN_VNR_SEQ = Convert.ToInt32(x.ItemSeq),
					ORG_WAREHOUSE_ID = null,
					WAREHOUSE_ID = "R01",
					LOC_CODE = "999999999",
					ITEM_CODE = x.ItemCode,
					RTN_VNR_QTY = x.ReturnQty,
					RTN_WMS_QTY = 0,
					DC_CODE = f160201.DC_CODE,
					GUP_CODE = f160201.GUP_CODE,
					CUST_CODE = f160201.CUST_CODE,
					MEMO = x.VnrReturnMemo,
					RTN_VNR_CAUSE = x.VnrReturnCause,
					MAKE_NO = x.MakeNo
				}).ToList();
			}

			return result;
		}
		#endregion
	}
}
