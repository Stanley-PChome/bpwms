using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Transactions;
using Wms3pl.Common.Extensions;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.Shared.TransApiServices.Common
{
	/// <summary>
	/// 批次訂單資料
	/// </summary>
	public class CommonOrderService
	{

		#region Private Property
		private WmsTransaction _wmsTransation;
		/// <summary>
		/// 貨主訂單資料物件清單
		/// </summary>
		private List<PostCreateOrdersModel> _orderList;
		/// <summary>
		/// 配送商資料清單
		/// </summary>
		private List<F1947> _f1947List;
		private List<F0002> _f0002List;
		/// <summary>
		/// 超取服務商清單
		/// </summary>
		private List<F194713> _eServiceList;
		/// <summary>
		/// 門市清單
		/// </summary>
		private List<F1910> _retailList;
		/// <summary>
		/// 出貨倉別資料
		/// </summary>
		private List<F1980> _warhouseList;
		/// <summary>
		/// 貨主單號已產生WMS訂單
		/// </summary>
		private List<CustWms_F050101> _custWmsList;
		/// <summary>
		/// 貨主資料
		/// </summary>
		private F1909 _custData;
		/// <summary>
		/// 物流中心資料
		/// </summary>
		private F1901 _dcData;
		/// <summary>
		/// 暫存新增的訂單清單
		/// </summary>
		private List<F050101> _f050101List = new List<F050101>();
		/// <summary>
		/// 暫存新增的訂單明細清單
		/// </summary>
		private List<F050102> _f050102List = new List<F050102>();
		/// <summary>
		/// 暫存新增的訂單明細延伸身檔清單
		/// </summary>
		private List<F05010301> _f05010301List = new List<F05010301>();
		/// <summary>
		/// 暫存新增的訂單延伸身檔清單
		/// </summary>
		private List<F050103> _f050103List = new List<F050103>();
		/// <summary>
		/// 暫存新增的訂單明細服務型商品資料清單
		/// </summary>
		private List<F050104> _f050104List = new List<F050104>();
		/// <summary>
		/// 暫存新增的訂單配送資訊檔清單
		/// </summary>
		private List<F050304> _f050304List = new List<F050304>();
		/// <summary>
		/// 暫存新增的訂單池主檔清單
		/// </summary>
		private List<F050001> _f050001List = new List<F050001>();
		/// <summary>
		/// 暫存新增的訂單池明細檔清單
		/// </summary>
		private List<F050002> _f050002List = new List<F050002>();
		/// <summary>
		/// 暫存訊息池清單
		/// </summary>
		private List<AddMessageReq> _addMessageTempList = new List<AddMessageReq>();
		private List<string> _failBatchNos;
		/// <summary>
		/// 紀錄有新增過F075102的CUST_ORD_NO，用以若檢核失敗 找出是否有新增，用以刪除
		/// </summary>
		private List<string> _IsAddF075102CustOrdNoList = new List<string>();
		/// <summary>
		/// 因序號重複需要排除的訂單
		/// </summary>
		private bool _designateSerialCheck = false;
		/// <summary>
		/// 蘋果廠商編號清單
		/// </summary>
		private List<string> _f0003AppleVendorList;
		/// <summary>
		/// API回傳結果
		/// </summary>
		private List<ApiResponse> apiRespData = new List<ApiResponse>();
		private string DcCode;
		private string GupCode;
		private string CustCode;
		#endregion

		#region Service

		private CommonService _commonService;
		public CommonService CommonService
		{
			get { return _commonService == null ? _commonService = new CommonService() : _commonService; }
			set { _commonService = value; }
		}

		private TransApiBaseService _tacService;
		public TransApiBaseService TacService
		{
			get { return _tacService == null ? _tacService = new TransApiBaseService() : _tacService; }
			set { _tacService = value; }
		}

		#endregion Service

		#region Repository

		private F075102Repository _f075102Repo;
		public F075102Repository F075102Repo
		{
			get { return _f075102Repo == null ? _f075102Repo = new F075102Repository(Schemas.CoreSchema) : _f075102Repo; }
			set { _f075102Repo = value; }
		}

		private F076103Repository _f076103Repo;
		public F076103Repository F076103Repo
		{
			get { return _f076103Repo == null ? _f076103Repo = new F076103Repository(Schemas.CoreSchema) : _f076103Repo; }
			set { _f076103Repo = value; }
		}

		private F0002Repository _f0002Repo;
		public F0002Repository F0002Repo
		{
			get { return _f0002Repo == null ? _f0002Repo = new F0002Repository(Schemas.CoreSchema) : _f0002Repo; }
			set { _f0002Repo = value; }
		}

		private F050101Repository _f050101Repo;
		public F050101Repository F050101Repo
		{
			get { return _f050101Repo == null ? _f050101Repo = new F050101Repository(Schemas.CoreSchema, _wmsTransation) : _f050101Repo; }
			set { _f050101Repo = value; }
		}

		private F050102Repository _f050102Repo;
		public F050102Repository F050102Repo
		{
			get { return _f050102Repo == null ? _f050102Repo = new F050102Repository(Schemas.CoreSchema, _wmsTransation) : _f050102Repo; }
			set { _f050102Repo = value; }
		}

		private F050103Repository _f050103Repo;
		public F050103Repository F050103Repo
		{
			get { return _f050103Repo == null ? _f050103Repo = new F050103Repository(Schemas.CoreSchema, _wmsTransation) : _f050103Repo; }
			set { _f050103Repo = value; }
		}

		private F050104Repository _f050104Repo;
		public F050104Repository F050104Repo
		{
			get { return _f050104Repo == null ? _f050104Repo = new F050104Repository(Schemas.CoreSchema, _wmsTransation) : _f050104Repo; }
			set { _f050104Repo = value; }
		}

		private F050304Repository _f050304Repo;
		public F050304Repository F050304Repo
		{
			get { return _f050304Repo == null ? _f050304Repo = new F050304Repository(Schemas.CoreSchema, _wmsTransation) : _f050304Repo; }
			set { _f050304Repo = value; }
		}

		private F05010301Repository _f05010301Repo;
		public F05010301Repository F05010301Repo
		{
			get { return _f05010301Repo == null ? _f05010301Repo = new F05010301Repository(Schemas.CoreSchema, _wmsTransation) : _f05010301Repo; }
			set { _f05010301Repo = value; }
		}

		private F050001Repository _f050001Repo;
		public F050001Repository F050001Repo
		{
			get { return _f050001Repo == null ? _f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransation) : _f050001Repo; }
			set { _f050001Repo = value; }
		}

		private F050002Repository _f050002Repo;
		public F050002Repository F050002Repo
		{
			get { return _f050002Repo == null ? _f050002Repo = new F050002Repository(Schemas.CoreSchema, _wmsTransation) : _f050002Repo; }
			set { _f050002Repo = value; }
		}

		private F051202Repository _f051202Repo;
		public F051202Repository F051202Repo
		{
			get { return _f051202Repo == null ? _f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransation) : _f051202Repo; }
			set { _f051202Repo = value; }
		}

		private F0003Repository _f0003Repo;
		public F0003Repository F0003Repo
		{
			get { return _f0003Repo == null ? _f0003Repo = new F0003Repository(Schemas.CoreSchema, _wmsTransation) : _f0003Repo; }
			set { _f0003Repo = value; }
		}
		#endregion Repository

		public CommonOrderService(WmsTransaction wmsTransation)
		{
			_wmsTransation = wmsTransation;
		}

		#region Insert
		/// <summary>
		/// Func1
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult RecevieApiDatas(PostCreateOrdersReq req)
		{
			TacService.CommonService = CommonService;
			CheckTransApiService ctaService = new CheckTransApiService();
			ctaService.CommonService = CommonService;
			ctaService.TacService = TacService;

			ApiResult res = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = TacService.GetMsg("10001") };

			#region 資料檢核

			// 檢核物流中心 必填、是否存在
			ctaService.CheckDcCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核貨主編號 必填、是否存在
			ctaService.CheckCustCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核Result
			ctaService.CheckResult(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核資料筆數
			int dataCnt = req.Result.Orders != null ? req.Result.Orders.Count : 0;
			if (req.Result.Orders == null || (req.Result.Orders != null && !TacService.CheckDataCount(req.Result.Total, dataCnt)))
				return new ApiResult { IsSuccessed = false, MsgCode = "20054", MsgContent = string.Format(TacService.GetMsg("20054"), req.Result.Total, dataCnt) };

			// 檢核總明細筆數是否超過[訂單明細最大筆數]筆,若超過回傳 & 取得訊息內容[20055, 總明細筆數,[訂單明細最大筆數]]
			int odMaxCnt = Convert.ToInt32(CommonService.GetSysGlobalValue("ODMaxCnt"));
			int itemTotalCnt = req.Result.Orders.Where(x => x.Details != null).Sum(x => x.Details.Count);
			if (itemTotalCnt > odMaxCnt)
				return new ApiResult { IsSuccessed = false, MsgCode = "20055", MsgContent = string.Format(TacService.GetMsg("20055"), itemTotalCnt) };
			#endregion

			#region 資料處理
			// 取得業主編號
			string gupCode = CommonService.GetGupCode(req.CustCode);
			res = CustProcessApiDatas_Order(req.DcCode, gupCode, req.CustCode, req.Result.Orders);
			#endregion

			return res;
		}
		#endregion

		#region 取消Wms訂單

		public void CancelOrder(List<PostCreateOrdersModel> cancelOrders)
		{
			var cancelCustOrdNoList = cancelOrders.Where(x => !string.IsNullOrWhiteSpace(x.CustOrdNo)).Select(x => x.CustOrdNo).Distinct().ToList();
			//[貨主單號已產生WMS訂單]
			var wmsList = GetDatasWithF050301(DcCode, GupCode, CustCode, cancelCustOrdNoList).ToList(); //F050101Repo.GetDatasWithF050301
			_custWmsList.AddRange(wmsList);

			cancelOrders.ForEach(item =>
			{
				var resl = CheckOrder(DcCode, GupCode, CustCode, item);

				if (!resl.IsSuccessed)
				{
					apiRespData.AddRange((List<ApiResponse>)resl.Data);

					// 驗證失敗，將不取消訂單，所以剃除
					_custWmsList = _custWmsList.Where(x => x.CUST_ORD_NO != item.CustOrdNo).ToList();
				}
			});

			#region 已存在訂單(by 每一筆訂單commit)

			//var cancelF050101List = 暫存新增的訂單清單篩選含[貨主單號已產生WMS單]
			var cancelF050101List = _f050101List.Where(x => x.DC_CODE == DcCode &&
																											x.GUP_CODE == GupCode &&
																											x.CUST_CODE == CustCode &&
																											_custWmsList.Select(z => z.CUST_ORD_NO).Contains(x.CUST_ORD_NO)).ToList();

			// Foreach[找到的WMS單] in [貨主單號已產生WMS訂單]
			foreach (var item in _custWmsList)
			{
				var isLock = false;

				try
				{
					WmsTransaction wmsTransaction2 = new WmsTransaction();

					SharedService sharedService = new SharedService(wmsTransaction2);
					OrderService orderService = new OrderService(wmsTransaction2);

					// Var isOk = [找到的WMS單].STATUS == -1
					var isOk = item.PROC_FLAG == "-1";

					var order = cancelOrders.Where(x => x.CustOrdNo == item.CUST_ORD_NO).LastOrDefault();

					// var cancelF050101 = CancelF050101List.Where(x => CUST_ORD_NO =[找到的WMS單].CUST_ORD_NO)
					var cancelF050101 = cancelF050101List.Where(x => x.CUST_ORD_NO == item.CUST_ORD_NO);

					var cancelOrdNos = cancelF050101.Select(x => x.ORD_NO).ToList();

					//var cancelF050102 = 暫存新增的訂單明細清單.Contain(cancelF050101.ORD_NO)
					var cancelF050102 = _f050102List.Where(x => cancelOrdNos.Contains(x.ORD_NO));

					//var cancelF050103 = 暫存新增的訂單延伸檔清單.Contain(cancelF050101.ORD_NO)
					var cancelF050103 = _f050103List.Where(x => cancelOrdNos.Contains(x.ORD_NO));

					//var cancelF05010301 = 暫存新增的訂單延伸明細檔清單.Contain(cancelF050101.ORD_NO)
					var cancelF05010301 = _f05010301List.Where(x => cancelOrdNos.Contains(x.ORD_NO));

					//var cancelF050104 = 暫存新增的訂單明細服務型商品資料清單.Contain(cancelF050101.ORD_NO)
					var cancelF050104 = _f050104List.Where(x => cancelOrdNos.Contains(x.ORD_NO));

					//var cancelF050304 = 暫存新增的訂單配送資訊檔清單.Contain(cancelF050101.ORD_NO)
					var cancelF050304 = _f050304List.Where(x => cancelOrdNos.Contains(x.ORD_NO));

					//var cancelF050001 = 暫存新增的訂單池主檔清單.Contain(cancelF050101.ORD_NO)
					var cancelF050001 = _f050001List.Where(x => cancelOrdNos.Contains(x.ORD_NO));

					//var cancelF050002 = 暫存新增的訂單池明細檔清單.Contain(cancelF050101.ORD_NO)
					var cancelF050002 = _f050002List.Where(x => cancelOrdNos.Contains(x.ORD_NO));

					if (isOk)
					{
						// (1)	&取消未配庫訂單[<參數1>,<參數2>,<參數3>,[找到的WMS單].ORD_NO]
						orderService.CancelNotAllocStockOrder(DcCode, GupCode, CustCode, item.ORD_NO, order.ProcFlag);

						if (cancelF050101.SingleOrDefault() == null)
							// &寫入行事曆訊息池[< 參數1 >.DcCode,[業主編號],<參數1>.CustCode, 20767,&取得訊息內容[20767, [找到的WMS單].ORD_NO],SCH]
							sharedService.AddMessagePool("9", DcCode, GupCode, CustCode, "API20767", string.Format(TacService.GetMsg("20767"), item.ORD_NO), "", "0", "SCH");
					}
					else
					{
						if (!string.IsNullOrWhiteSpace(item.F050301_ORD_NO))
						{
							var hasAudit = F051202Repo.AnyWmsOrdIntAudit(item.DC_CODE, item.GUP_CODE, item.CUST_CODE, item.F050301_ORD_NO);

							//檢查訂單是否鎖定
							try
							{
								F076103Repo.Add(new F076103()
								{
									DC_CODE = DcCode,
									CUST_ORD_NO = item.CUST_ORD_NO
								});
								isLock = true;
							}
							catch (SqlException sqlEx)
							{
								if (sqlEx.Number == 2627)
								{
									apiRespData.Add(new ApiResponse { MsgCode = "23066", MsgContent = string.Format(TacService.GetMsg("23066"), item.ORD_NO), No = item.CUST_ORD_NO });
									_failBatchNos.Add(item.F050301_ORD_NO);
									continue;
								}
								else
								{
									throw sqlEx;
								}
							}
							//var f076103Trans = F076103Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
							//	new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
							//	() =>
							//	{
							//		var lockF076103 = F076103Repo.LockF076103();
							//		var f076103 = F076103Repo.Find(o => o.DC_CODE == DcCode && o.CUST_ORD_NO == item.CUST_ORD_NO, isForUpdate: true, isByCache: false);

							//		if (f076103 == null)
							//		{
							//			F076103Repo.Add(new F076103()
							//			{
							//				DC_CODE = DcCode,
							//				CUST_ORD_NO = item.CUST_ORD_NO
							//			});

							//			isLock = true;
							//		}

							//		return f076103;
							//	});

							//if (f076103Trans != null)
							//{
							//	apiRespData.Add(new ApiResponse { MsgCode = "23066", MsgContent = string.Format(TacService.GetMsg("23066"), item.ORD_NO), No = item.CUST_ORD_NO });
							//	_failBatchNos.Add(item.F050301_ORD_NO);
							//	continue;
							//}

							var cancelRes = new ExecuteResult { IsSuccessed = false };
							if (!hasAudit)
							{
								// 新的共用訂單取消
								cancelRes = orderService.CancelAllocStockOrder(DcCode, GupCode, CustCode, new List<string> { item.F050301_ORD_NO }, "0", string.Empty, string.Empty, string.Empty, string.Empty, "999", "訂單取消");
							}

							if (cancelRes.IsSuccessed)
							{
								// &寫入行事曆訊息池[< 參數1 >.DcCode,[業主編號],<參數1>.CustCode, 20767,&取得訊息內容[20767, [找到的WMS單].ORD_NO],SCH]
								sharedService.AddMessagePool("9", DcCode, GupCode, CustCode, "API20767", string.Format(TacService.GetMsg("20767"), item.ORD_NO), "", "0", "SCH");
							}
							else
							{
								apiRespData.Add(new ApiResponse { MsgCode = "20788", MsgContent = string.Format(TacService.GetMsg("20788"), item.ORD_NO), No = item.CUST_ORD_NO });
								//取消訂單就直接下一筆，避免把取消失敗的訂單取消掉

								_failBatchNos.Add(item.F050301_ORD_NO);
								continue;
							}
						}
						else
						{
							// res.Data.add(則回傳&訊息內容[20769, [找到的WMS單].CUST_ORD_NO])
							apiRespData.Add(new ApiResponse { MsgCode = "20769", MsgContent = TacService.GetMsg("20769"), No = item.CUST_ORD_NO });

							var cancelF050101Data = cancelF050101.SingleOrDefault();

							if (cancelF050101Data != null)
							{
								_failBatchNos.Add(cancelF050101Data.BATCH_NO);

								// 暫存新增的訂單清單.Remove(cancelF050101)
								_f050101List = _f050101List.Except(cancelF050101).ToList();

								// 暫存新增的訂單明細清單.Except(cancelF050102)
								_f050102List = _f050102List.Except(cancelF050102).ToList();

								// 暫存新增的訂單延伸清單.Remove(cancelF050103)
								_f050103List = _f050103List.Except(cancelF050103).ToList();

								// 暫存新增的訂單明細延伸清單.Except(cancelF05010301)
								_f05010301List = _f05010301List.Except(cancelF05010301).ToList();

								// 暫存新增的訂單明細服務型商品資料清單.Remove(cancelF050104)
								_f050104List = _f050104List.Except(cancelF050104).ToList();

								// 暫存新增的訂單配送資訊清單.Remove(cancelF050304)
								_f050304List = _f050304List.Except(cancelF050304).ToList();

								// 暫存新增的訂單池主檔清單.Remove(cancelF050001)
								_f050001List = _f050001List.Except(cancelF050001).ToList();

								// 暫存新增的訂單池明細清單.Except(cancelF050102)
								_f050002List = _f050002List.Except(cancelF050002).ToList();
							}
						}
					}

					wmsTransaction2.Complete();
				}
				finally
				{
					if (isLock)
					{
						F076103Repo.Unlock(DcCode, item.CUST_ORD_NO);
					}
				}
			}

			#endregion

		}

		#endregion 取消Wms訂單

		#region 建立Wms訂單

		/// <summary>
		/// 檢查ProcFlag
		/// </summary>
		/// <param name="res"></param>
		/// <param name="warehouseIns"></param>
		/// <returns></returns>
		public ApiResponse CheckProcFlag(PostCreateOrdersModel order)
		{
			List<string> procFlags = new List<string> { "0", "D" };
			if (!procFlags.Contains(order.ProcFlag))
				return new ApiResponse { No = order.CustOrdNo, MsgCode = "20961", MsgContent = string.Format(TacService.GetMsg("20961"), order.CustOrdNo) };
			return null;
		}

		/// <summary>
		/// 資料處理2
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		public ApiResult CustProcessApiDatas_Order(string dcCode, string gupCode, string custCode, List<PostCreateOrdersModel> orders)
		{
			var res = new ApiResult();
			_custWmsList = new List<CustWms_F050101>();

			#region 變數
			DcCode = dcCode;
			GupCode = gupCode;
			CustCode = custCode;
			_orderList = orders;

			_failBatchNos = new List<string>();
			int insertCnt = 0;
			#endregion

			#region 先檢查ProcFlag

			orders.ForEach(order =>
			{
				var resp = CheckProcFlag(order);
				if (resp != null)
					apiRespData.Add(resp);
			});

			#endregion 先檢查ProcFlag

			#region 處理取消Wms訂單

			var cancelOrderList = _orderList.Where(w => w.ProcFlag == "D").ToList();
			if (cancelOrderList.Any())
				CancelOrder(cancelOrderList);

			#endregion 處理取消Wms訂單

			var createOrderList = _orderList.Except(cancelOrderList).ToList();
			if (createOrderList.Any())
			{
				#region Private Property
				var createCustOrdNoList = createOrderList.Where(x => !string.IsNullOrWhiteSpace(x.CustOrdNo)).Select(x => x.CustOrdNo).Distinct().ToList();

				// [貨主單號已產生WMS訂單]=
				//SELECT ORD_NO, CUST_ORD_NO, ISNULL(b.PROC_FLAG, "-1") PROC_FLAG FROM F050101 a left join F050301 b on b.DC_CODE = A.DC_CODE AND b.GUP_CODE = A.GUP_CODE AND b.CUST_CODE = A.CUST_CODE AND b.ORD_NO = a.ORD_NO WHERE A.DC_CODE = DC_CODE AND A.GUP_CODE= GUP_CODE AND A.CUST_CODE = CUST_CODE AND A.STATUS<> 9 AND A.WMS_ORD_NO IN(Distinct[貨主通路訂單資料物件清單].CustOrdNo)
				var wmsList = GetDatasWithF050301(dcCode, gupCode, custCode, createCustOrdNoList).ToList(); //F050101Repo.GetDatasWithF050301
				_custWmsList.AddRange(wmsList);

				// [配送商資料清單]=&取得配送商資料[< 參數1 >]
				_f1947List = CommonService.GetLogisticProviderList(dcCode);

				_f0002List = F0002Repo.getLogisticList(dcCode).ToList();

				// [超取服務商清單]= &取得超取服務商資料[DC_CODE, GUP_CODE, CUST_CODE, DISTINCT[貨主通路訂單資料物件清單].LogisticProvider, DISTINCT[貨主通路訂單資料物件清單].EServiceNo]
				_eServiceList = CommonService.GetEServiceList(dcCode, gupCode, custCode,
						orders.Where(x => !string.IsNullOrWhiteSpace(x.LogisticsProvider)).Select(x => x.LogisticsProvider).Distinct().ToList(),
						orders.Where(x => !string.IsNullOrWhiteSpace(x.EServiceNo)).Select(x => x.EServiceNo).Distinct().ToList());

				// [門市清單]= &取得門市資料[GUP_CODE, CUST_CODE, DISTINCT[貨主通路訂單資料物件清單].SalesBaseNo]
				_retailList = CommonService.GetRetailList(gupCode, custCode, orders.Where(x => !string.IsNullOrWhiteSpace(x.SalesBaseNo)).Select(x => x.SalesBaseNo).Distinct().ToList());

				// [出貨倉別資料]=&取得出貨倉別資料[DC_CODE, DISTINCT[貨主通路訂單資料物件清單].WarehouseId]
				_warhouseList = CommonService.GetWarhouseList(dcCode, orders.Where(x => !string.IsNullOrWhiteSpace(x.WarehouseId)).Select(x => x.WarehouseId).Distinct().ToList());

				// [貨主資料] = &取得貨主資料[< 參數2 >,< 參數3 >]
				_custData = CommonService.GetCust(gupCode, custCode);

				// [物流中心資料] = &取得物流中心資料[< 參數1 >]
				_dcData = CommonService.GetDc(dcCode);

				// [蘋果廠商編號清單] = &取得F0003設定檔[DC_CODE, GUP_CODE, CUST_CODE]
				_f0003AppleVendorList = F0003Repo.GetAppleVendor(dcCode, gupCode, custCode);
				#endregion

				#region 檢核 & 建立訂單資料
				CheckOrderAndCreateF050101();
				#endregion

				#region 處理純新增訂單
				// var addF050101List = 暫存新增的訂單清單排除[貨主單號已產生WMS訂單]
				var addF050101List = _f050101List.Where(x => x.DC_CODE == DcCode &&
																										 x.GUP_CODE == GupCode &&
																										 x.CUST_CODE == CustCode &&
																										 !_custWmsList.Select(z => z.CUST_ORD_NO).Contains(x.CUST_ORD_NO)).ToList();
				insertCnt = InsertOrderData(addF050101List);
				#endregion
			}

			#region 寫入行事曆訊息池
			AddOrderMessagePool();
			#endregion

			#region 回傳資訊
			res.IsSuccessed = !apiRespData.Any();
			res.Data = apiRespData;

			int totalCnt = _orderList.Count;
			int successedCnt = totalCnt - _failBatchNos.Count;

			res.MsgCode = "20770";
			res.MsgContent = string.Format(TacService.GetMsg("20770"), successedCnt, _failBatchNos.Count, totalCnt);
			res.TotalCnt = totalCnt;
			res.InsertCnt = insertCnt;
			res.UpdateCnt = successedCnt - insertCnt;
			res.FailureCnt = _failBatchNos.Count;

			#endregion

			return res;
		}

		/// <summary>
		/// 寫入行事曆訊息池
		/// </summary>
		/// <param name="guid"></param>
		/// <param name="ordNo"></param>
		private void AddMessagePool(SharedService sharedService, string guid, string ordNo)
		{
			// 寫入行事曆訊息池
			var addMessageList = _addMessageTempList.Where(x => x.Guid == guid).ToList();
			addMessageList.ForEach(o =>
			{
				o.MessageContent = o.MessageContent.Replace(guid, ordNo);
				sharedService.AddMessagePool(o.TicketType, o.DcCode, o.GupCode, o.CustCode, o.MsgNo, o.MessageContent, o.NotifyOrdNo, o.TargetType, o.TargetCode);
			});
		}

		#endregion

		#region 檢核 & 建立訂單資料
		public void CheckOrderAndCreateF050101()
		{
			// 避免若同一批參數有重複單號會把之前成功寫入的F075102的刪除，所以取得重複數
			var paramCustOrdNos = _orderList.Select(x => x.CustOrdNo).GroupBy(x => x).Select(x => new { CustOrdNo = x.Key, Cnt = x.Count() }).ToList();

			_orderList.ForEach(item =>
			{
				// #資料處理3[<參數1>,<參數2>,<參數3>,貨主通路訂單資料物件]
				var resl = CheckOrder(DcCode, GupCode, CustCode, item);

				if (!resl.IsSuccessed)
				{
					apiRespData.AddRange((List<ApiResponse>)resl.Data);

					var currCustOrdNo = paramCustOrdNos.Where(x => x.CustOrdNo == item.CustOrdNo).FirstOrDefault();

					// 驗證失敗，將不取消訂單，所以剃除
					_custWmsList = _custWmsList.Where(x => x.CUST_ORD_NO != item.CustOrdNo).ToList();

					// 若驗證失敗 刪除新增的F075102
					if (_IsAddF075102CustOrdNoList.Contains(item.CustOrdNo) && currCustOrdNo != null && currCustOrdNo.Cnt == 1)
					{
						F075102Repo.DelF075102ByKey(CustCode, item.CustOrdNo);
						_IsAddF075102CustOrdNoList = _IsAddF075102CustOrdNoList.Where(x => x != item.CustOrdNo).ToList();
					}
				}
			});
		}

		/// <summary>
		/// 資料處理3
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="order"></param>
		/// <returns></returns>
		public ApiResult CheckOrder(string dcCode, string gupCode, string custCode, PostCreateOrdersModel order)
		{
			ApiResult result = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			// 預設值設定
			data.AddRange((List<ApiResponse>)CheckDefaultSetting(dcCode, gupCode, custCode, order).Data);

			// 共用欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(dcCode, gupCode, custCode, order).Data);

			// 貨主自訂欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckCustomColumnType(dcCode, gupCode, custCode, order).Data);

			// 如果以上檢核成功
			if (!data.Any())
			{
				// 共用欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCommonColumnData(dcCode, gupCode, custCode, order).Data);

				// 貨主自訂欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCustomColumnValue(dcCode, gupCode, custCode, order).Data);

				// 如果以上檢核成功、產生WMS資料
				if (!data.Any())
				{
					// 產生WMS資料
					CreateWmsData(dcCode, gupCode, custCode, order);
				}
				else
				{
					_failBatchNos.Add(order.BatchNo);
				}
			}
			else
			{
				_failBatchNos.Add(order.BatchNo);
			}

			result.IsSuccessed = !data.Any();
			result.Data = data;

			return result;

		}

		#region Protected 檢核
		/// <summary>
		/// 預設值設定
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="order">貨主通路訂單資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckDefaultSetting(string dcCode, string gupCode, string custCode, PostCreateOrdersModel order)
		{
			// 請預留方法
			ApiResult res = new ApiResult { IsSuccessed = true };
			res.Data = new List<ApiResponse>();

			if (string.IsNullOrWhiteSpace(order.ShopperName))
				order.ShopperName = "NA";

			if (string.IsNullOrWhiteSpace(order.ReceiverPhone))
				order.ReceiverPhone = "NA";

			if (string.IsNullOrWhiteSpace(order.ReceiverAddress))
				order.ReceiverAddress = "NA";

			if (string.IsNullOrWhiteSpace(order.ReceiverName))
				order.ReceiverName = "NA";

			if (string.IsNullOrWhiteSpace(order.ReceiverMobile))
				order.ReceiverMobile = "NA";

			if (!order.IsCOD.HasValue)
				order.IsCOD = false;

			if (string.IsNullOrWhiteSpace(order.ReceiverZip))
				order.ReceiverZip = null;

			if (string.IsNullOrWhiteSpace(order.ItemOpType))
				order.ItemOpType = "0";

			return res;
		}

		/// <summary>
		/// 共用欄位格式檢核
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="order">貨主通路訂單資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckColumnNotNullAndMaxLength(string dcCode, string gupCode, string custCode, PostCreateOrdersModel order)
		{
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			#region 定義需檢核欄位、必填、型態、長度

			List<string> shipwayIsOneNotNullColumnList = new List<string> { "StoreId", "StoreName", "ShipDate", "ReturnDate", "EServiceNo" };

			List<string> b2bColumnIsNullColumnList = new List<string> { "SalesBaseNo" };

			// 訂單資料物件
			List<ApiCkeckColumnModel> orderCheckColumnList = new List<ApiCkeckColumnModel>();

			// 訂單資料明細資料
			List<ApiCkeckColumnModel> orderItemsCheckColumnList = new List<ApiCkeckColumnModel>();

			// 訂單資料明細服務資料
			List<ApiCkeckColumnModel> orderServiceItemsCheckColumnList = new List<ApiCkeckColumnModel>();

			List<ApiCkeckColumnModel> receiverOptCheckColumnList = new List<ApiCkeckColumnModel>();

			if (order.ProcFlag == "D")
			{
				orderCheckColumnList = new List<ApiCkeckColumnModel>
				{
					new ApiCkeckColumnModel{  Name = "CustOrdNo",             Type = typeof(string),   MaxLength = 50, Nullable = false },
					new ApiCkeckColumnModel{  Name = "ProcFlag",              Type = typeof(string),   MaxLength = 1, Nullable = false }
				};
			}
			else if (order.ProcFlag == "0")
			{
				orderCheckColumnList = new List<ApiCkeckColumnModel>
				{
					new ApiCkeckColumnModel{  Name = "CustOrdNo",             Type = typeof(string),   MaxLength = 50, Nullable = false },
					new ApiCkeckColumnModel{  Name = "OrderDate",             Type = typeof(string),   MaxLength = 10, Nullable = false, IsDate = true },
					new ApiCkeckColumnModel{  Name = "OrderType",             Type = typeof(string),   MaxLength = 1, Nullable = false },
					new ApiCkeckColumnModel{  Name = "BatchNo",               Type = typeof(string),   MaxLength = 50 },
					new ApiCkeckColumnModel{  Name = "ShipWay",               Type = typeof(string),   MaxLength = 10, Nullable = false },
					new ApiCkeckColumnModel{  Name = "TranCode",              Type = typeof(string),   MaxLength = 10 },
					new ApiCkeckColumnModel{  Name = "ChannelCode",           Type = typeof(string),   MaxLength = 20, Nullable = false },
					new ApiCkeckColumnModel{  Name = "SubChannelCode",        Type = typeof(string),   MaxLength = 20, Nullable = false },
					new ApiCkeckColumnModel{  Name = "LogisticsProvider",     Type = typeof(string),   MaxLength = 10, Nullable = order.ShipWay == "3" },
					new ApiCkeckColumnModel{  Name = "ExpDeliveryDate",       Type = typeof(string),   MaxLength = 10, Nullable = false, IsDate = true },
					new ApiCkeckColumnModel{  Name = "DeliveryPeriod",        Type = typeof(int),      MaxLength = 1, Nullable = false },
					new ApiCkeckColumnModel{  Name = "IsCOD",                 Type = typeof(bool),     Nullable = false },
					new ApiCkeckColumnModel{  Name = "CODAmount",             Type = typeof(decimal),  MaxLength = 11, Nullable = false },
					new ApiCkeckColumnModel{  Name = "SalesBaseNo",           Type = typeof(string),   MaxLength = 20 },
					new ApiCkeckColumnModel{  Name = "ShopperName",           Type = typeof(string),   MaxLength = 20, Nullable = false },
					new ApiCkeckColumnModel{  Name = "ShopperPhone",          Type = typeof(string),   MaxLength = 10 },
					new ApiCkeckColumnModel{  Name = "ReceiverName",          Type = typeof(string),   MaxLength = 20, Nullable = false },
					new ApiCkeckColumnModel{  Name = "ReceiverPhone",         Type = typeof(string),   MaxLength = 10, Nullable = false },
					new ApiCkeckColumnModel{  Name = "ReceiverMobile",        Type = typeof(string),   MaxLength = 10, Nullable = false },
					new ApiCkeckColumnModel{  Name = "ReceiverZip",           Type = typeof(string),   MaxLength = 5 },
					new ApiCkeckColumnModel{  Name = "ReceiverAddress",       Type = typeof(string),   MaxLength = 150, Nullable = false },
					new ApiCkeckColumnModel{  Name = "Memo",                  Type = typeof(string),   MaxLength = 200 },
					new ApiCkeckColumnModel{  Name = "WarehouseId",           Type = typeof(string),   MaxLength = 1, Nullable = false },
					new ApiCkeckColumnModel{  Name = "EServiceNo",            Type = typeof(string),   MaxLength = 10 },
					new ApiCkeckColumnModel{  Name = "ConsignNo",             Type = typeof(string),   MaxLength = 20 },
					new ApiCkeckColumnModel{  Name = "ShipDate",              Type = typeof(string),   MaxLength = 10, IsDate = true },
					new ApiCkeckColumnModel{  Name = "StoreId",               Type = typeof(string),   MaxLength = 20 },
					new ApiCkeckColumnModel{  Name = "StoreName",             Type = typeof(string),   MaxLength = 20 },
					new ApiCkeckColumnModel{  Name = "ReturnDate",            Type = typeof(string),   MaxLength = 10, IsDate = true },
					new ApiCkeckColumnModel{  Name = "InvoiceNo",             Type = typeof(string),   MaxLength = 20 },
					new ApiCkeckColumnModel{  Name = "InvoiceDate",           Type = typeof(string),   MaxLength = 10, IsDate = true },
					new ApiCkeckColumnModel{  Name = "ProcFlag",              Type = typeof(string),   MaxLength = 1,  Nullable = false },
					new ApiCkeckColumnModel{  Name = "PrintCustOrdNo",        Type = typeof(string),   MaxLength = 20, Nullable = false },
					new ApiCkeckColumnModel{  Name = "PrintMemo",             Type = typeof(string),   MaxLength = 20, Nullable = false },
					new ApiCkeckColumnModel{  Name = "CustCost",              Type = typeof(string),   MaxLength = 10 },
					new ApiCkeckColumnModel{  Name = "SuggestBoxNo",          Type = typeof(string),   MaxLength = 20 },
					new ApiCkeckColumnModel{  Name = "MoveOutTarget",         Type = typeof(string),   MaxLength = 10, Nullable = order.CustCost != "MoveOut" },
					new ApiCkeckColumnModel{  Name = "FastDealType",          Type = typeof(string),   MaxLength = 1 , Nullable = false },
					new ApiCkeckColumnModel{  Name = "ItemOpType",            Type = typeof(string),   MaxLength = 1,  Nullable = false },
					new ApiCkeckColumnModel{  Name = "SuggestLogisticCode",   Type = typeof(string),   MaxLength = 10,  },
				};

				// 訂單資料明細資料
				orderItemsCheckColumnList = new List<ApiCkeckColumnModel>
				{
					new ApiCkeckColumnModel{  Name = "ItemSeq",               Type = typeof(string),   MaxLength = 6 },
					new ApiCkeckColumnModel{  Name = "ItemCode",              Type = typeof(string),   MaxLength = 20, Nullable = false },
					new ApiCkeckColumnModel{  Name = "ChannelItemCode",       Type = typeof(string),   MaxLength = 20 },
					new ApiCkeckColumnModel{  Name = "VnrCode",               Type = typeof(string),   MaxLength = 20 },
					new ApiCkeckColumnModel{  Name = "ItemDesc",              Type = typeof(string),   MaxLength = 20 },
					new ApiCkeckColumnModel{  Name = "Qty",                   Type = typeof(int),      MaxLength = 6, Nullable = false },
					new ApiCkeckColumnModel{  Name = "MakeNo",                Type = typeof(string),   MaxLength = 40 },
					new ApiCkeckColumnModel{  Name = "SerialNo",              Type = typeof(string),   MaxLength = 50 }
				};

				// 訂單資料明細服務資料
				orderServiceItemsCheckColumnList = new List<ApiCkeckColumnModel>
				{
					new ApiCkeckColumnModel{  Name = "ServiceItemCode",       Type = typeof(string),   MaxLength = 20, Nullable = false },
					new ApiCkeckColumnModel{  Name = "ServiceItemName",       Type = typeof(string),   MaxLength = 50 },
				};

				receiverOptCheckColumnList = new List<ApiCkeckColumnModel>
				{
					new ApiCkeckColumnModel{  Name = "IsTPEKEL",              Type = typeof(string),   MaxLength = 1},
				};

			}
			#endregion

			#region 檢查訂單欄位必填、最大長度
			List<string> orderIsNullList = new List<string>();
			List<ApiCkeckColumnModel> orderIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();
			List<string> shipwayIsOneAndColumnIsNullList = new List<string>();
			List<string> b2bColumnIsNull = new List<string>();
			List<string> notDateColumn = new List<string>();

			// 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
			orderCheckColumnList.ForEach(column =>
			{
				// 必填
				if (!column.Nullable)
				{
					if (!DataCheckHelper.CheckRequireColumn(order, column.Name))
						orderIsNullList.Add(column.Name);
				}

				if (order.ProcFlag != "D")
				{
					// 判斷是否為日期格式(yyyy/MM/dd)字串
					var value = Convert.ToString(DataCheckHelper.GetRequireColumnValue(order, column.Name));
					if (column.IsDate && !string.IsNullOrWhiteSpace(value))
						if (!DataCheckHelper.CheckDataIsDate(order, column.Name))
							notDateColumn.Add(column.Name);

					// 最大長度
					if (column.MaxLength > 0)
					{
						if (column.Name == "CODAmount")
						{
							// 檢核是否符合decimal(11,2)
							if (!DataCheckHelper.CheckDataIsDecimal(order, column.Name, 9, 2))
								orderIsExceedMaxLenthList.Add(column);
						}
						else
						{
							if (!DataCheckHelper.CheckDataMaxLength(order, column.Name, column.MaxLength))
								orderIsExceedMaxLenthList.Add(column);
						}
					}

					// 如果是超取，下列應該檢核必填
					if (order.ShipWay == "1" && shipwayIsOneNotNullColumnList.Contains(column.Name))
					{
						if (!DataCheckHelper.CheckRequireColumn(order, column.Name))
							shipwayIsOneAndColumnIsNullList.Add(column.Name);
					}

					// 如果是B2B，下列應該檢核必填
					if (order.OrderType == "0" && b2bColumnIsNullColumnList.Contains(column.Name))
					{
						if (!DataCheckHelper.CheckRequireColumn(order, column.Name))
							b2bColumnIsNull.Add(column.Name);
					}
				}
			});

			// 必填訊息
			if (orderIsNullList.Any())
				// 回傳訊息內容[20058, < 參數4 >.CustInNo, 必填欄位未填寫的欄位清單(格式:[欄位名稱1]、[欄位名稱2])]
				data.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20058", MsgContent = string.Format(TacService.GetMsg("20058"), order.CustOrdNo, string.Join("、", orderIsNullList)) });

			// 日期格式判斷
			if (notDateColumn.Any())
				data.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20075", MsgContent = string.Format(TacService.GetMsg("20075"), order.CustOrdNo, string.Join("、", notDateColumn)) });

			// 最大長度訊息
			if (orderIsExceedMaxLenthList.Any())
			{
				List<string> errorMsgList = orderIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();

				string errorMsg = string.Join("、", errorMsgList);

				// 檢查進倉單欄位格式(參考2.8.2),如果檢核失敗，回傳 & 訊息內容[20059, < 參數4 >.CustInNo, 欄位格式錯誤清單(格式:[欄位名稱1]格式錯誤必須為X1、[欄位名稱2]格式錯誤必須為X2)]
				data.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20059", MsgContent = string.Format(TacService.GetMsg("20059"), order.CustOrdNo, errorMsg) });
			}

			// 超取必填訊息
			if (shipwayIsOneAndColumnIsNullList.Any())
				// 回傳訊息內容[20762, 必填欄位未填寫的欄位清單(格式:[欄位名稱1]、[欄位名稱2])]
				data.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20762", MsgContent = string.Format(TacService.GetMsg("20762"), order.CustOrdNo, string.Join("、", shipwayIsOneAndColumnIsNullList)) });

			// B2B必填訊息
			if (b2bColumnIsNull.Any())
				// 回傳訊息內容[20763,必填欄位未填寫的欄位清單(格式:[欄位名稱1]、[欄位名稱2])]』
				data.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20763", MsgContent = string.Format(TacService.GetMsg("20763"), order.CustOrdNo, string.Join("、", b2bColumnIsNull)) });

			#endregion

			#region 檢查收貨/店取額外資訊
			if (order.ProcFlag != "D")
			{
				var i = 0;
				List<string> orderItemsIsNullList = new List<string>();
				List<ApiCkeckColumnModel> orderItemsIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();
				if (order.ReceiverOpt != null)
				{
					receiverOptCheckColumnList.ForEach(o =>
					{
						// 必填
						if (!o.Nullable)
							if (!DataCheckHelper.CheckRequireColumn(order.ReceiverOpt, o.Name))
								orderItemsIsNullList.Add(o.Name);

						// 最大長度
						if (o.MaxLength > 0)
							if (!DataCheckHelper.CheckDataMaxLength(order.ReceiverOpt, o.Name, o.MaxLength))
								orderItemsIsExceedMaxLenthList.Add(o);
					});

					// 必填訊息
					if (orderItemsIsNullList.Any())
						// 檢查進倉單明細必填欄位(參考2.8.2) ,如果檢核失敗，回傳 & 訊息內容[20058, < 參數4 >.CustInNo, 必填欄位未填寫的欄位清單(格式:[欄位名稱1]、[欄位名稱2])]
						data.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20058", MsgContent = string.Format(TacService.GetMsg("20058"), $"{order.CustOrdNo}第{i + 1}筆明細", string.Join("、", orderItemsIsNullList)) });

					// 最大長度訊息
					if (orderItemsIsExceedMaxLenthList.Any())
					{
						List<string> errorMsgList = orderItemsIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();
						string errorMsg = string.Join("、", errorMsgList);
						// 檢查進倉單明細必填欄位(參考2.8.2) , 如果檢核失敗，回傳 & 訊息內容[20059, < 參數4 >.CustInNo, 欄位格式錯誤清單(格式:[欄位名稱1]格式錯誤必須為X1、[欄位名稱2]格式錯誤必須為X2)]
						data.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20059", MsgContent = string.Format(TacService.GetMsg("20059"), $"{order.CustOrdNo}第{i + 1}筆明細", errorMsg) });
					}

					i++;

				}
			}
			#endregion 檢查收貨/店取額外資訊

			#region 檢查訂單明細欄位必填、最大長度
			if (order.ProcFlag != "D")
			{
				List<string> orderItemsIsNullList;
				List<ApiCkeckColumnModel> orderItemsIsExceedMaxLenthList;
				List<string> orderServiceItemsIsNullList;
				List<ApiCkeckColumnModel> orderServiceItemsIsExceedMaxLenthList;

				// No.2073 檢核是否重複指定出貨序號
				var designateSerials = order.Details.Select(o => o.SerialNo).Where(o => !string.IsNullOrWhiteSpace(o)).ToList();

				// 訂單指定2個以上序號，當前訂單自己檢核
				if (designateSerials.Count() > 1)
				{
					var duplicateSerialNo = designateSerials.GroupBy(o => o).Where(x => x.Count() > 1).Select(x => x.Key);

					if (duplicateSerialNo.Any())
					{
						data.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20879", MsgContent = string.Format(TacService.GetMsg("20879"), string.Join("、", duplicateSerialNo)) });
					}
				}

				var designateSerialOrders = _orderList.Where(o => o.Details.Any(x => !string.IsNullOrWhiteSpace(x.SerialNo)) && o.CustOrdNo != order.CustOrdNo);

				// 同批次有2個以上訂單有指定序號，訂單之間相互檢核
				if (designateSerialOrders.Any())
				{
					var duplicateSerialNo = new List<string>();

					foreach (var designateSerial in designateSerials)
					{
						foreach (var designateSerialOrder in designateSerialOrders)
						{
							if (designateSerialOrder.Details.Any(o => o.SerialNo == designateSerial) && !duplicateSerialNo.Contains(designateSerial))
							{
								duplicateSerialNo.Add(designateSerial);
							}
						}
					}

					if (duplicateSerialNo.Any())
						data.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20880", MsgContent = string.Format(TacService.GetMsg("20880"), string.Join("、", duplicateSerialNo)) });
				}

				if (order.Details != null && order.Details.Any())
				{
					for (int i = 0; i < order.Details.Count; i++)
					{
						var currDetail = order.Details[i];

						orderItemsIsNullList = new List<string>();
						orderItemsIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();

						orderItemsCheckColumnList.ForEach(o =>
						{
							// 必填
							if (!o.Nullable)
								if (!DataCheckHelper.CheckRequireColumn(currDetail, o.Name))
									orderItemsIsNullList.Add(o.Name);

							// 最大長度
							if (o.MaxLength > 0)
								if (!DataCheckHelper.CheckDataMaxLength(currDetail, o.Name, o.MaxLength))
									orderItemsIsExceedMaxLenthList.Add(o);
						});

						// 必填訊息
						if (orderItemsIsNullList.Any())
							// 檢查進倉單明細必填欄位(參考2.8.2) ,如果檢核失敗，回傳 & 訊息內容[20058, < 參數4 >.CustInNo, 必填欄位未填寫的欄位清單(格式:[欄位名稱1]、[欄位名稱2])]
							data.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20058", MsgContent = string.Format(TacService.GetMsg("20058"), $"{order.CustOrdNo}第{i + 1}筆明細", string.Join("、", orderItemsIsNullList)) });

						// 最大長度訊息
						if (orderItemsIsExceedMaxLenthList.Any())
						{
							List<string> errorMsgList = orderItemsIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();
							string errorMsg = string.Join("、", errorMsgList);
							// 檢查進倉單明細必填欄位(參考2.8.2) , 如果檢核失敗，回傳 & 訊息內容[20059, < 參數4 >.CustInNo, 欄位格式錯誤清單(格式:[欄位名稱1]格式錯誤必須為X1、[欄位名稱2]格式錯誤必須為X2)]
							data.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20059", MsgContent = string.Format(TacService.GetMsg("20059"), $"{order.CustOrdNo}第{i + 1}筆明細", errorMsg) });
						}

						#region 檢核服務型品號資料
						if (currDetail.ServiceItemDetails.Any())
						{
							for (int j = 0; j < currDetail.ServiceItemDetails.Count; j++)
							{
								var currService = currDetail.ServiceItemDetails[j];

								orderServiceItemsCheckColumnList.ForEach(obj =>
								{
									orderServiceItemsIsNullList = new List<string>();
									orderServiceItemsIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();
									// 必填
									if (!obj.Nullable)
										if (!DataCheckHelper.CheckRequireColumn(currService, obj.Name))
											orderServiceItemsIsNullList.Add(obj.Name);
									// 最大長度
									if (obj.MaxLength > 0)
										if (!DataCheckHelper.CheckDataMaxLength(currService, obj.Name, obj.MaxLength))
											orderServiceItemsIsExceedMaxLenthList.Add(obj);
									// 必填訊息
									if (orderServiceItemsIsNullList.Any())
										data.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20058", MsgContent = string.Format(TacService.GetMsg("20058"), $"{order.CustOrdNo}第{i + 1}筆明細第{j + 1}筆服務", string.Join("、", orderServiceItemsIsNullList)) });

									// 最大長度訊息
									if (orderServiceItemsIsExceedMaxLenthList.Any())
									{
										List<string> errorMsgList = orderServiceItemsIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();
										string errorMsg = string.Join("、", errorMsgList);
										data.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20059", MsgContent = string.Format(TacService.GetMsg("20059"), $"{order.CustOrdNo}第{i + 1}筆明細第{j + 1}筆服務", errorMsg) });
									}
								});
							}
						}
						#endregion
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
		/// <param name="warehouseIns">進倉單資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnType(string dcCode, string gupCode, string custCode, PostCreateOrdersModel order)
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
		/// <param name="warehouseIns">進倉單資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckCommonColumnData(string dcCode, string gupCode, string custCode, PostCreateOrdersModel order)
		{
			CheckOrderService coService = new CheckOrderService();
			coService.CommonService = CommonService;
			coService.TacService = TacService;
			coService.F075102Repo = F075102Repo;
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			#region 主檔欄位資料檢核
			// 檢查ProcFlag
			coService.CheckProcFlag(data, order);

			// 檢查貨主單號是否存在
			var isAdd = coService.CheckCustExistForThirdPart(data, _custWmsList, order, custCode);
			if (isAdd)
				_IsAddF075102CustOrdNoList.Add(order.CustOrdNo);

			// 檢查廠商編號
			coService.CheckOrderType(data, order);

			// 檢查配送方式
			coService.CheckShipWay(data, order);

			// 檢查方便到貨時段
			coService.CheckDeliveryPeriod(data, order);

			// 檢核出貨倉別
			coService.CheckWarehouseId(data, order, _warhouseList);

			// 檢查物流商是否存在
			coService.CheckLogisticsProvider(data, order, _f1947List);

			// 檢查超取配送商
			coService.CheckEService(data, order, _eServiceList);

			// 檢查門市編號
			coService.CheckRetail(data, order, _retailList);

			// 檢查貨主自訂義分類
			coService.CheckCustCost(data, order);

			// 檢查優先處理旗標
			coService.CheckFastDealType(data, order);

			// 檢查商品處理類型
			coService.CheckItemOpType(data, order);

			// 檢查收貨/店取額外資訊(是否為北北基欄位)
			coService.CheckReceiverOpt(data, order);

			// 檢查ChannelCode通路編號是否存在於資料庫中
			coService.CheckChannelCode(data, order);

			// 檢查SubChannelCode子通路編號是否存在於資料庫中
			coService.CheckSubChannelCode(data, order);

			// 檢查建議物流商編號
			coService.CheckSuggestLogisticCod(data, order, _f0002List);
			#endregion

			#region 明細欄位資料檢核
			// 檢查明細筆數
			coService.CheckDetailCnt(data, order);

			// 檢核項次必須大於0，且同一張單據內的序號不可重複
			coService.CheckDetailSeq(data, order);

			// 檢查明細進倉數量
			coService.CheckDetailQty(data, order);
			#endregion

			#region 明細服務資料檢核
			coService.CheckServiceItemCode(data, order);
			#endregion

			#region 檢查資料是否完整
			// 檢查資料是否完整
			coService.CheckOrderData(data, gupCode, custCode, order);
			#endregion

			// 檢查貨主單號是否存在
			coService.CheckCustOrdNo(order, ref _f050101List, ref _f050102List, ref _f05010301List, ref _f050103List, ref _f050104List,
					ref _f050304List, ref _f050001List, ref _f050002List);

			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位資料檢核
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="warehouseIns">進倉單資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnValue(string dcCode, string gupCode, string custCode, PostCreateOrdersModel order)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}
		#endregion

		/// <summary>
		/// 產生WMS資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="order"></param>
		/// <returns></returns>
		public void CreateWmsData(string dcCode, string gupCode, string custCode, PostCreateOrdersModel order)
		{
			if (order.ProcFlag != "D")
			{
				// 建立訂單主檔F050101
				var f050101 = CreateF050101(dcCode, gupCode, custCode, order);

				// 建立訂單明細檔F050102+訂單明細延伸身檔
				var f050102WithF05010301WithF050104List = CreateF050102WithF05010301WithF050104List(f050101, order);

				// 建立訂單明細延伸頭檔F050103
				var f050103 = CreateF050103(f050101, order);

				// IF  ShipWay = 1 OR ConsignNo not null or not empty then
				if (order.ShipWay == "1" || !string.IsNullOrWhiteSpace(order.ConsignNo))
				{
					// 建立訂單配送資訊檔F050304[F050101,<參數4>] (參考4.1.4) else 不處理
					_f050304List.Add(CreateF050304(f050101, order));
				}

				//then #轉入訂單池[F050101,LIST<F050102>,<參數4>]
				CopyF050101ToF050001(f050101, f050102WithF05010301WithF050104List.F050102List, order);

				_f050101List.Add(f050101);
				_f050102List.AddRange(f050102WithF05010301WithF050104List.F050102List);
				_f05010301List.AddRange(f050102WithF05010301WithF050104List.F05010301List);
				_f050104List.AddRange(f050102WithF05010301WithF050104List.F050104List);
				_f050103List.Add(f050103);
			}
		}

		#region Protected Method
		/// <summary>
		/// 建立訂單主檔
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="order"></param>
		/// <returns></returns>
		protected F050101 CreateF050101(string dcCode, string gupCode, string custCode, PostCreateOrdersModel order)
		{
			F1910 retailData = _retailList.Where(x => x.RETAIL_CODE == order.SalesBaseNo).FirstOrDefault();
			DateTime now = DateTime.Today;

			DateTime ordDate = _custData.IS_ORDDATE_TODAY == "1" ? now : DateTime.ParseExact(order.OrderDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
			string custName = string.Empty;
			string tel = string.Empty;
			string address = string.Empty;
			string consignee = string.Empty;
			DateTime arrivalDate;
			string contact = string.Empty;
			string contactTel = string.Empty;
			string collect = null;
			decimal collectAmt = 0;
			string tel1 = string.Empty;
			string tranCode = string.Empty;

			// 客戶名稱、電話
			if (order.OrderType == "0")
			{
				custName = retailData.RETAIL_NAME;
				tel = retailData.TEL;
				consignee = retailData.RETAIL_NAME;
				contact = retailData.RETAIL_NAME;
				tranCode = "O1";
			}
			else
			{
				custName = order.ShopperName;
				tel = order.ReceiverPhone;
				consignee = order.ReceiverName;
				contact = order.ShopperName;
				tranCode = "O3";
			}

			// 地址
			if (order.OrderType == "0")
			{
				address = retailData.ADDRESS;
			}
			else if (order.ShipWay == "1")
			{
				address = $"{_dcData.ZIP_CODE}{_dcData.ADDRESS}";
			}
			else
			{
				address = $"{order.ReceiverAddress ?? "NA"}";
			}

			// 指定到貨日期
			if (_custData.IS_ORDDATE_TODAY == "1")
			{
				arrivalDate = now;
			}
			else if (!string.IsNullOrWhiteSpace(order.ExpDeliveryDate))
			{
				arrivalDate = DateTime.ParseExact(order.ExpDeliveryDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
			}
			else
			{
				arrivalDate = DateTime.ParseExact(order.OrderDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
			}

			// 聯絡電話、收件人聯絡電話1
			if (order.OrderType == "0")
			{
				contactTel = retailData.TEL;
				tel1 = retailData.TEL;
			}
			else if (order.ShipWay == "1")
			{
				contactTel = "1111111111";
				tel1 = "1111111111";
			}
			else if (!string.IsNullOrWhiteSpace(order.ShopperPhone))
			{
				contactTel = order.ShopperPhone;
				tel1 = order.ShopperPhone;
			}
			else
			{
				contactTel = order.ReceiverMobile;
				tel1 = order.ReceiverMobile;
			}

			// 是否代收、代收金額
			var isCod = Convert.ToBoolean(order.IsCOD);
			collect = isCod ? "1" : "0";
			collectAmt = isCod ? Convert.ToDecimal(order.CODAmount) : 0;

			return new F050101
			{
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				CHANNEL = string.IsNullOrWhiteSpace(order.ChannelCode) ? "00" : order.ChannelCode,
				ORD_NO = Guid.NewGuid().ToString(),
				CUST_ORD_NO = order.CustOrdNo,
				ORD_TYPE = order.OrderType,
				RETAIL_CODE = order.SalesBaseNo,
				ORD_DATE = ordDate,
				STATUS = "1",
				CUST_NAME = custName,
				SELF_TAKE = order.ShipWay == "3" ? "1" : "0",
				FRAGILE_LABEL = order.FragileLabel != null ? Convert.ToBoolean(order.FragileLabel) ? "1" : "0" : "0",
				GUARANTEE = "0",
				SA = "0",
				GENDER = "0",
				AGE = 0,
				TEL = tel,
				ADDRESS = address,
				CONSIGNEE = consignee,
				ARRIVAL_DATE = arrivalDate,
				TRAN_CODE = tranCode,
				BATCH_NO = string.IsNullOrWhiteSpace(order.BatchNo) ? DateTime.Now.ToString("yyyyMMddHHmmss") : order.BatchNo,
				POSM = "0",
				CONTACT = contact,
				CONTACT_TEL = contactTel,
				SPECIAL_BUS = "0",
				ALL_ID = order.LogisticsProvider,
				COLLECT = collect,
				COLLECT_AMT = collectAmt,
				MEMO = order.Memo,
				TYPE_ID = order.WarehouseId,
				CAN_FAST = "0",
				TEL_1 = tel1,
				TEL_AREA = string.Empty,
				PRINT_RECEIPT = "0",
				RECEIPT_NO = order.InvoiceNo,
				HAVE_ITEM_INVO = "0",
				NP_FLAG = "0",
				SA_CHECK_QTY = 0,
				DELV_PERIOD = order.DeliveryPeriod == null || order.DeliveryPeriod == 0 ? "4" : order.DeliveryPeriod.ToString(),
				CVS_TAKE = order.ShipWay == "1" ? "1" : "0",
				SUBCHANNEL = string.IsNullOrWhiteSpace(order.SubChannelCode) ? "00" : order.SubChannelCode,
				FOREIGN_WMSNO = null,
				FOREIGN_CUSTCODE = null,
				SP_DELV = "00",
				ROUND_PIECE = "0",
				SA_QTY = null,
				CUST_COST = order.CustCost,
				TEL_2 = null,
				RECEIPT_NO_HELP = null,
				RECEIPT_TITLE = null,
				RECEIPT_ADDRESS = null,
				BUSINESS_NO = null,
				DISTR_CAR_NO = null,
				CHECK_CODE = null,
				IMPORT_FLAG = "0",
				SUG_BOX_NO = order.SuggestBoxNo,
				MOVE_OUT_TARGET = order.MoveOutTarget,
				FAST_DEAL_TYPE = order.FastDealType,
				PACKING_TYPE = order.PackingType,
				ISPACKCHECK = order.IsPackCheck,
				ORDER_PROC_TYPE = order.ItemOpType.ToString(),
				ORDER_ZIP_CODE = order.ReceiverZip,
				IS_NORTH_ORDER = order.ReceiverOpt?.IsTPEKEL?.ToString() ?? null,
				SUG_LOGISTIC_CODE = string.IsNullOrWhiteSpace(order.SuggestLogisticCode) ? null : order.SuggestLogisticCode
			};
		}

		/// <summary>
		/// 建立訂單明細檔F050102+訂單明細延伸身檔F05010301
		/// </summary>
		/// <param name="f050101"></param>
		/// <param name="order"></param>
		/// <returns></returns>
		protected F050102WithF05010301WithF50104Model CreateF050102WithF05010301WithF050104List(F050101 f050101, PostCreateOrdersModel order)
		{
			F050102WithF05010301WithF50104Model result = new F050102WithF05010301WithF50104Model();
			result.F050102List = new List<F050102>();
			result.F05010301List = new List<F05010301>();
			result.F050104List = new List<F050104>();

			var data = order.Details.GroupBy(x => new { x.ItemCode, x.ItemSeq, x.ItemDesc, x.ChannelItemCode, x.VnrCode, x.MakeNo, x.ServiceItemDetails, x.SerialNo })
																 .Select(x => new CreateF050102TmpModel
																 {
																	 ChannelItemNo = x.Key.ChannelItemCode,
																	 ItemCode = x.Key.ItemCode,
																	 ItemSeq = x.Key.ItemSeq,
																	 ItemDesc = x.Key.ItemDesc,
																	 Qty = x.Sum(z => z.Qty),
																	 VnrCode = x.Key.VnrCode,
																	 MakeNo = x.Key.MakeNo,
																	 serialNo = x.Key.SerialNo,
																	 ServiceItemDetails = x.Key.ServiceItemDetails
																 }).ToList();

			// 如果ItemSeq有空或null將重新排序
			bool isFormatSeq = data.Where(x => string.IsNullOrWhiteSpace(x.ItemSeq)).Any();

			for (int i = 0; i < data.Count; i++)
			{
				var currData = data[i];

				// 重新排序
				if (isFormatSeq)
					data[i].ItemSeq = Convert.ToString(i + 1);

				result.F050102List.Add(new F050102
				{
					DC_CODE = f050101.DC_CODE,
					GUP_CODE = f050101.GUP_CODE,
					CUST_CODE = f050101.CUST_CODE,
					ORD_NO = f050101.ORD_NO,
					ORD_SEQ = currData.ItemSeq,
					ITEM_CODE = currData.ItemCode,
					ORD_QTY = currData.Qty,
					NO_DELV = "0",
					SERIAL_NO = string.IsNullOrWhiteSpace(currData.serialNo) ? "" : currData.serialNo,
					VNR_CODE = currData.VnrCode,
					MAKE_NO = currData.MakeNo,
				});

				result.F05010301List.Add(new F05010301
				{
					DC_CODE = f050101.DC_CODE,
					GUP_CODE = f050101.GUP_CODE,
					CUST_CODE = f050101.CUST_CODE,
					ORD_NO = f050101.ORD_NO,
					ORD_SEQ = currData.ItemSeq,
					CHANNEL_ITEM_CODE = currData.ChannelItemNo,
					ITEM_CODE = currData.ItemCode,
					ITEM_NO = currData.ItemSeq,
					ITEM_DETAIL = currData.ItemDesc,
					PRICE = null,
					GIFTQTY = null,
					UNIT = null,
					AMOUNT = null
				});

				result.F050104List.AddRange(currData.ServiceItemDetails.Select(x => new F050104
				{
					DC_CODE = f050101.DC_CODE,
					GUP_CODE = f050101.GUP_CODE,
					CUST_CODE = f050101.CUST_CODE,
					ORD_NO = f050101.ORD_NO,
					ORD_SEQ = currData.ItemSeq,
					ITEM_CODE = currData.ItemCode,
					SERVICE_ITEM_CODE = x.ServiceItemCode,
					SERVICE_ITEM_NAME = x.ServiceItemName,
					STATUS = "0"
				}).ToList());
			}

			return result;
		}

		/// <summary>
		/// 建立訂單明細延伸頭檔F050103
		/// </summary>
		/// <param name="f050101"></param>
		/// <param name="order"></param>
		/// <returns></returns>
		protected F050103 CreateF050103(F050101 f050101, PostCreateOrdersModel order)
		{
			var retailData = _retailList.Where(x => x.RETAIL_CODE == order.SalesBaseNo).SingleOrDefault();

			return new F050103
			{
				DC_CODE = f050101.DC_CODE,
				GUP_CODE = f050101.GUP_CODE,
				CUST_CODE = f050101.CUST_CODE,
				ORD_NO = f050101.ORD_NO,
				CUST_ORD_NO = order.CustOrdNo,
				AMT = null,
				PAY_WAY = null,
				STORE_PAY = Convert.ToBoolean(order.IsCOD) ? "貨到付款" : "純配送",
				INVOICE = order.InvoiceNo,
				INVOICE_DATE = !string.IsNullOrWhiteSpace(order.InvoiceDate) ? DateTime.ParseExact(order.OrderDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) : default(DateTime?),
				INVOICE_DESC = null,
				IDENTIFIER = null,
				DELV_MEMO = null,
				EXP_DELV_DATE = DateTime.ParseExact(order.ExpDeliveryDate, "yyyy/MM/dd", CultureInfo.InvariantCulture),
				SHIPPING_FLAG = "1",
				DELV_NO = string.IsNullOrWhiteSpace(order.SalesBaseNo) ? string.Empty : retailData != null ? retailData.DELV_NO : string.Empty,
				SITENAME = null,
				UNI_FORM = null,
				ORDERTAXAMOUNT = null,
				ORDERTOTALAMOUNT = null,
				PRINT_CUST_ORD_NO = order.PrintCustOrdNo,
				PRINT_MEMO = order.PrintMemo
			};
		}

		/// <summary>
		/// 建立訂單配送資訊檔F050304
		/// </summary>
		/// <param name="f050101"></param>
		/// <param name="order"></param>
		/// <returns></returns>
		protected F050304 CreateF050304(F050101 f050101, PostCreateOrdersModel order)
		{
			// 託運單號
			var eServiceData = _eServiceList.Where(x => x.ALL_ID == f050101.ALL_ID && x.ESERVICE == order.EServiceNo).SingleOrDefault();
			var consignNo = f050101.CVS_TAKE == "0" || eServiceData != null ? order.ConsignNo : string.Empty;

			// 超取服務商
			var eService = eServiceData != null ? eServiceData.ESERVICE : string.Empty;

			return new F050304
			{
				DC_CODE = f050101.DC_CODE,
				GUP_CODE = f050101.GUP_CODE,
				CUST_CODE = f050101.CUST_CODE,
				ORD_NO = f050101.ORD_NO,
				ALL_ID = f050101.ALL_ID,
				BATCH_NO = !string.IsNullOrWhiteSpace(f050101.BATCH_NO) ? f050101.BATCH_NO : null,
				DELV_RETAILCODE = order.StoreId,
				DELV_RETAILNAME = order.StoreName,
				CONSIGN_NO = consignNo,
				DELV_DATE = !string.IsNullOrWhiteSpace(order.ShipDate) ? DateTime.ParseExact(order.ShipDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) : default(DateTime?),
				RETURN_DATE = !string.IsNullOrWhiteSpace(order.ReturnDate) ? DateTime.ParseExact(order.ReturnDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) : default(DateTime?),
				ESERVICE = eService
			};
		}

		/// <summary>
		/// 轉入訂單池
		/// </summary>
		/// <param name="f050101"></param>
		/// <param name="f050102List"></param>
		/// <param name="order">貨主通路訂單資料物件</param>
		public void CopyF050101ToF050001(F050101 f050101, List<F050102> f050102List, PostCreateOrdersModel order)
		{
			SharedService sharedService = new SharedService();

			#region COPY F050101 TO F050001
			F050001 f050001 = new F050001();
			f050101.CloneProperties(f050001);

			// 調整F050001內容
			f050001.PROC_FLAG = "0";
			f050001.TICKET_ID = sharedService.GetTicketID(f050101.DC_CODE, f050101.GUP_CODE, f050101.CUST_CODE, f050101.ORD_TYPE == "0" ? "O1" : "O2");
			f050001.ZIP_CODE = order.ReceiverZip;
			f050001.INVO_PRINTED = "0";
			f050001.INVO_TAX_TYPE = "1";
			f050001.ORD_DATE = f050101.ORD_DATE == null ? DateTime.Today : Convert.ToDateTime(f050101.ORD_DATE);
			f050001.ARRIVAL_DATE = f050101.ARRIVAL_DATE;

			_f050001List.Add(f050001);

			#endregion

			#region COPY List<F050102> TO LIST<F050002>

			var f050002List = f050102List.Select(a => AutoMapper.Mapper.DynamicMap<F050002>(a)).ToList();

			// 調整F050002內容
			f050002List.ForEach(o => { o.WELCOME_LETTER = "0"; });

			_f050002List.AddRange(f050002List);

			#endregion
		}
		#endregion

		#endregion 檢核 & 建立訂單資料

		#region 處理純新增訂單

		public int InsertOrderData(List<F050101> addF050101List)
		{
			SharedService sharedService = new SharedService(_wmsTransation);
			int insertCnt = 0;

			//// var addF050101List = 暫存新增的訂單清單排除[貨主單號已產生WMS訂單]
			//var addF050101List = _f050101List.Where(x => x.DC_CODE == DcCode &&
			//																						 x.GUP_CODE == GupCode &&
			//																						 x.CUST_CODE == CustCode &&
			//																						 !_custWmsList.Select(z => z.CUST_ORD_NO).Contains(x.CUST_ORD_NO)).ToList();

			if (addF050101List.Any())
			{
				insertCnt = addF050101List.Count;
				// F050101.ORD_NO
				var ordNos = addF050101List.Select(z => z.ORD_NO);

				// var addF050102List = 暫存新增的訂單明細清單.Contain(addF050101List.ORD_NO)
				var addF050102List = _f050102List.Where(x => ordNos.Contains(x.ORD_NO));

				// var addF050103List = 暫存新增的訂單延伸身檔.Contain(addF050101List.ORD_NO)
				var addF050103List = _f050103List.Where(x => ordNos.Contains(x.ORD_NO));

				// var addF05010301List = 暫存新增的訂單明細延伸身檔.Contain(addF050101List.ORD_NO)
				var addF05010301List = _f05010301List.Where(x => ordNos.Contains(x.ORD_NO));

				// var addF050104List = 暫存新增的訂單明細服務型商品資料清單.Contain(addF050101List.ORD_NO)
				var addF050104List = _f050104List.Where(x => ordNos.Contains(x.ORD_NO));

				// var addF050304List = 暫存新增的訂單配送資訊檔.Contain(addF050101List.ORD_NO)
				var addF050304List = _f050304List.Where(x => ordNos.Contains(x.ORD_NO));

				// var addF050001List = 暫存新增的訂單池主檔.Contain(addF050101List.ORD_NO)
				var addF050001List = _f050001List.Where(x => ordNos.Contains(x.ORD_NO));

				// var addF050002List = 暫存新增的訂單池明細檔.Contain(addF050101List.ORD_NO)
				var addF050002List = _f050002List.Where(x => ordNos.Contains(x.ORD_NO));


				var newOrdNos = sharedService.GetNewOrdStackCodes("S", addF050101List.Count);
				// Foreach addF050101 in addF050101List
				addF050101List.ForEach(item =>
				{
					var addF050101Data = item;

					//// [取得訂單單號]= sharedService.GetNewOrdCode("S")
					//var ordNo = sharedService.GetNewOrdCode("S");
					var ordNo = newOrdNos.Pop();

					// var guid = addF050101.ORD_NO;
					var guid = item.ORD_NO;

					/* Step1. [取得訂單明細]= addF050102List.WHERE(x=>ORD_NO=guid)
					 * Step2. Foreach[訂單明細] =  [取得訂單明細]
					 * Strp3. [訂單明細].ORD_NO = [取得訂單單號] */
					var addF050102Data = addF050102List.Where(x => x.ORD_NO == guid).ToList();
					addF050102Data.ForEach(f050102 => { f050102.ORD_NO = ordNo; });

					/* Step1. [取得訂單延伸檔]= addF050103List.WHERE(x=>ORD_NO=guid)
					 * Step2. [訂單延伸檔].ORD_NO = [取得訂單單號] */
					var addF050103Data = addF050103List.Where(x => x.ORD_NO == guid).SingleOrDefault();
					addF050103Data.ORD_NO = ordNo;

					/* Step1. [取得訂單延伸明細檔]=addF05010301List.WHERE(x=>ORD_NO=guid)
					 * Step2. Foreach[訂單延伸明細檔] =  [取得訂單延伸明細檔]
					 * Step3. [訂單延伸明細檔].ORD_NO = [取得訂單單號] */
					var addF05010301Data = addF05010301List.Where(x => x.ORD_NO == guid).ToList();
					addF05010301Data.ForEach(f05010301 => { f05010301.ORD_NO = ordNo; });

					/* Step1. [取得訂單明細]= addF050104List.WHERE(x=>ORD_NO=guid)
					 * Step2. Foreach[訂單明細服務型商品] =  [取得訂單明細服務型商品]
					 * Strp3. [訂單明細服務型商品].ORD_NO = [取得訂單單號] */
					var addF050104Data = addF050104List.Where(x => x.ORD_NO == guid).ToList();
					addF050104Data.ForEach(f050104 => { f050104.ORD_NO = ordNo; });

					/* Step1. [訂單配送資訊檔] = addF050304List.WHERE(x=>ORD_NO=guid)
					 * Step2. [訂單配送資訊檔].ORD_NO = [取得訂單單號]*/
					F050304 addF050304Data = null;
					if (addF050304List.Where(x => x.ORD_NO == guid).Any())
					{
						addF050304Data = addF050304List.Where(x => x.ORD_NO == guid).SingleOrDefault();
						addF050304Data.ORD_NO = ordNo;
					}

					/* Step1. [訂單池主檔] = addF050001List.WHERE(x=>ORD_NO=guid)
					 * Step2. [訂單池主檔].ORD_NO = [取得訂單單號] */
					F050001 addF050001Data = null;
					if (addF050001List.Where(x => x.ORD_NO == guid).Any())
					{
						addF050001Data = addF050001List.Where(x => x.ORD_NO == guid).SingleOrDefault();
						addF050001Data.ORD_NO = ordNo;
					}

					/* Step1. [取得訂單池明細檔]=addF050002List.WHERE(x=>ORD_NO=guid)
					 * Step2. Foreach[訂單明細檔] =  [取得訂單池明細檔]
					 * Step3. [訂單明細檔].ORD_NO = [取得訂單單號] */
					List<F050002> addF050002Data = addF050002List.Where(x => x.ORD_NO == guid).ToList();
					addF050002Data.ForEach(f050002 => { f050002.ORD_NO = ordNo; });

					//// 寫入行事曆訊息池
					//AddMessagePool(sharedService, guid, ordNo);

					// addF050101.ORD_NO =[取得訂單單號]
					addF050101Data.ORD_NO = ordNo;

					//檢查 本訂單中所有的商品 的廠商編號(F1903.VNR_CODE)，其中有一個商品廠商編號在蘋果廠商編號清單設定檔
					var productList = CommonService.GetProductList(GupCode, CustCode, addF050102Data.Select(x => x.ITEM_CODE).Distinct().ToList());
					if (productList.Any(x => _f0003AppleVendorList.Contains(x.ORI_VNR_CODE) || x.ISAPPLE == "1"))
					{
						addF050101Data.NP_FLAG = "1";
						if (addF050001Data != null)
							addF050001Data.NP_FLAG = "1";
					}

					//addF050101.Add(addF050101Data);
					F050101Repo.Add(addF050101Data);
					F050102Repo.BulkInsert(addF050102Data);
					F050103Repo.Add(addF050103Data);
					F050104Repo.BulkInsert(addF050104Data);
					F05010301Repo.BulkInsert(addF05010301Data);
					if (addF050304Data != null)
						F050304Repo.Add(addF050304Data);
					if (addF050001Data != null)
						F050001Repo.Add(addF050001Data);
					if (addF050002Data.Any())
						F050002Repo.BulkInsert(addF050002Data);
				});
			}

			_wmsTransation.Complete();

			return insertCnt;
		}

		#endregion 處理純新增訂單

		#region 寫入行事曆訊息池
		public void AddOrderMessagePool()
		{
			var wmsTransation3 = new WmsTransaction();
			var sharedService = new SharedService(wmsTransation3);
			// Foreach addF050101List  GROUP BY BatchNo 
			_orderList.GroupBy(x => x.BatchNo).ToList()
			.ForEach(item =>
			{
				int total = item.Count();
				int fail = _failBatchNos.Where(x => x == item.Key).Count();
				int successed = total - fail;

				// 取得訊息內容[20766, BatchNo, 成功訂單單數, 異常訂單筆數, 失敗訂單單數, 合計單數]
				var msgContent = string.Format(TacService.GetMsg("20766"),
						item.Key,
						successed,
						fail,
						total);
				// 寫入行事曆訊息池[< 參數1 >,< 參數2 >,< 參數3 >, 20766, &取得訊息內容[20766,[通路資料, < 參數4 >].NAME, BatchNo, 成功訂單單數, 異常訂單筆數, 失敗訂單單數, 合計單數], SCH]
				sharedService.AddMessagePool("9", DcCode, GupCode, CustCode, "API20766", msgContent, "", "0", "SCH");
			});

			wmsTransation3.Complete();
		}
		#endregion 寫入行事曆訊息池

		public List<CustWms_F050101> GetDatasWithF050301(string dcCode, string gupCode, string custCode, List<string> custOrdNos)
		{
			var details = F050101Repo.GetCustWmsDetails(dcCode, gupCode, custCode, custOrdNos);

			var result = details.GroupBy(x => new { x.ORD_NO, x.CUST_ORD_NO, x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.PROC_FLAG, x.F050301_ORD_NO })
				.Select(x => new CustWms_F050101
				{
					ORD_NO = x.Key.ORD_NO,
					CUST_ORD_NO = x.Key.CUST_ORD_NO,
					DC_CODE = x.Key.DC_CODE,
					GUP_CODE = x.Key.GUP_CODE,
					CUST_CODE = x.Key.CUST_CODE,
					PROC_FLAG = x.Key.PROC_FLAG,
					F050301_ORD_NO = x.Key.F050301_ORD_NO,
					F050801_STATUS_List = x.Select(s => s.F050801_STATUS).ToList()
				}).ToList();

			return result;
		}
	}
}
