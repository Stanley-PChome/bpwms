using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Lms.Services;
using Wms3pl.WebServices.Shared.Lms.WebApiConnectSetting;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;

namespace Wms3pl.WebServices.ForeignWebApi.Business.LmsServices
{
	public class ExportOrderService : LmsBaseService
	{
		private WmsTransaction _wmsTransaction;
		private IQueryable<F055001> f055001s;
		private List<Datas.Shared.Entities.F051206LackData> f051206LackDatas;

		#region Repository
		private F050305Repository _f050305Repo;
		public F050305Repository F050305Repo
		{
			get { return _f050305Repo == null ? _f050305Repo = new F050305Repository(Schemas.CoreSchema) : _f050305Repo; }
			set { _f050305Repo = value; }
		}

		private F050101Repository _f050101Repo;
		public F050101Repository F050101Repo
		{
			get { return _f050101Repo == null ? _f050101Repo = new F050101Repository(Schemas.CoreSchema, _wmsTransaction) : _f050101Repo; }
			set { _f050101Repo = value; }
		}

		private F050102Repository _f050102Repo;
		public F050102Repository F050102Repo
		{
			get { return _f050102Repo == null ? _f050102Repo = new F050102Repository(Schemas.CoreSchema, _wmsTransaction) : _f050102Repo; }
			set { _f050102Repo = value; }
		}

		private F051206Repository _f051206Repo;
		public F051206Repository F051206Repo
		{
			get { return _f051206Repo == null ? _f051206Repo = new F051206Repository(Schemas.CoreSchema, _wmsTransaction) : _f051206Repo; }
			set { _f051206Repo = value; }
		}
		#endregion Repository

		private ExportService _exportService;
		public ExportService ExportService
		{
			get { return _exportService == null ? _exportService = new ExportService() : _exportService; }
			set { _exportService = value; }
		}

		private CommonService _commonService;
		public CommonService CommonService
		{
			get { return _commonService == null ? _commonService = new CommonService() : _commonService; }
			set { _commonService = value; }
		}

		public ExportOrderService(WmsTransaction wmsTransation)
		{
			_wmsTransaction = wmsTransation;
		}

		/// <summary>
		/// 匯出訂單出貨結果
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		public ApiResult ExportOrderResults(string dcCode, string gupCode, string custCode)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();
			TransApiBaseService tacService = new TransApiBaseService();

			var outputJsonInLog = CommonService.GetSysGlobalValue("OutputJsonInLog");
			bool isSaveWmsData = string.IsNullOrWhiteSpace(outputJsonInLog) ? false : outputJsonInLog == "1";

			#region 取得 訂單出貨未回檔資料
			var f050305s = F050305Repo.GetDatasForExport_Sql(dcCode, gupCode, custCode).ToList();
			#endregion

			f050305s.ForEach(f050305 =>
			{
				SaleOrderReplyReq req = new SaleOrderReplyReq { DcCode = dcCode, CustCode = custCode };

				#region 取得 WarehouseOuts資料
				req.WarehouseOuts = GetWarehouseOuts(f050305);
				#endregion

				// 取得 WarehouseOutDetails資料(STATUS=5才撈取資料)
				// 取得 RcvDatas資料(STATUS= 5 才撈取資料)
				if (f050305.STATUS == "5" && req.WarehouseOuts.Any())
				{
					req.WarehouseOuts[0].LackDetails = GetLackDetails(f050305);
					req.WarehouseOuts[0].WarehouseOutDetails = GetWarehouseOutDetails(f050305);
					req.WarehouseOuts[0].Packages = GetPackageDatas(f050305);
				}

				#region 訂單出貨結果回傳
				// 新增API Log[< 參數1 >,< 參數2 >,< 參數3 >, LmsApi_ ExportODResult, 傳入F050305, &LmsApiFunc, false]
				ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "ExportODResult", new { LmsApiUrl = LmsSetting.ApiUrl + "OutboundOrder/Reply", LmsToken = LmsSetting.ApiAuthToken, WmsCondition = f050305, LmsData = isSaveWmsData ? req : null }, () =>
				{
					ApiResult result = new ApiResult();
#if (DEBUG)
					result = LmsApiFuncTest(req, "OutboundOrder/Reply");
#else
         result = LmsApiFunc(req, "OutboundOrder/Reply");
#endif
					if (result != null)
					{
						if (result.IsSuccessed)
						{
							#region 更新處理狀態
							F050305Repo.UpdateProcFlag(f050305.ID, "1");
							#endregion
						}
						else
						{
							data.Add(new ApiResponse
							{
								No = $"ORD_NO：{f050305.ORD_NO} ",
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
			res.TotalCnt = f050305s.Count;
			res.MsgCode = "10005";
			res.MsgContent = string.Format(tacService.GetMsg("10005"),
							"訂單出貨結果回傳", res.TotalCnt - res.FailureCnt, res.FailureCnt, res.TotalCnt);

			return res;
		}

		private List<SaleOrderReplyWarehouseOut> GetWarehouseOuts(F050305 f050305)
		{
			var WarehouseOutExs = F050101Repo.GetWarehouseOutExByOrdNo(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, f050305.ORD_NO).ToList();

			var f055001s = ExportService.GetF055001s(f050305);

			var res = WarehouseOutExs.Select(x => new SaleOrderReplyWarehouseOut
			{
				CustOrdNo = x.CUST_ORD_NO,
				OrderType = x.CUST_COST != "MoveOut" ? "SO" : "OTO",
				Status = f050305.STATUS,
				WorkTime = f050305.CRT_DATE.ToString("yyyy/MM/dd HH:mm:ss"),
				TotalBoxNum = f055001s.Count()
			}).ToList();

			return res;
		}

		private List<SaleOrderReplyWarehouseOutDetail> GetWarehouseOutDetails(F050305 f050305)
		{
			//var f050102s = F050102Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == f050305.DC_CODE &&
			//																																																										 o.GUP_CODE == f050305.GUP_CODE &&
			//																																																										 o.CUST_CODE == f050305.CUST_CODE &&
			//																																																										 o.ORD_NO == f050305.ORD_NO);

			//var f05030202s = F05030202Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == f050305.DC_CODE &&
			//																																																										 o.GUP_CODE == f050305.GUP_CODE &&
			//																																																										 o.CUST_CODE == f050305.CUST_CODE &&
			//																																																										 o.ORD_NO == f050305.ORD_NO);


			//var res = (from A in f050102s
			//					 join B in f05030202s
			//					 on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.ORD_NO, A.ORD_SEQ } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.ORD_NO, B.ORD_SEQ } into subB
			//					 from B in subB.DefaultIfEmpty()
			//					 select new
			//					 {
			//						 F050102 = A,
			//						 F05030202 = B
			//					 }).GroupBy(x => new { x.F050102.ORD_NO, x.F050102.ORD_SEQ, x.F050102.ITEM_CODE }).Select(x => new SaleOrderReplyWarehouseOutDetail
			//					 {
			//						 ItemSeq = x.Key.ORD_SEQ,
			//						 ItemCode = x.Key.ITEM_CODE,
			//						 ActQty = x.Where(z => z.F05030202 != null).Any() ? Convert.ToInt32(x.Sum(z => z.F05030202.A_DELV_QTY)) : 0
			//					 }).ToList();

			var res = F050102Repo.GetWarehouseOutDetails(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, f050305.ORD_NO).ToList();
			return res;
		}



		private List<SaleOrderReplyPackage> GetPackageDatas(F050305 f050305)
		{
			//取得F055004
			var f055004s = ExportService.GetF055004s(f050305);
			//若查無資料，則產生F055004
			if (!f055004s.Any())
			{
				ExportService.CreateF055004(f050305);
				//產完F055004後再取F055004 (避免產生失敗)
				f055004s = ExportService.GetF055004s(f050305);
			}

			return CreatePackageDatas(f050305, f055004s);
		}

		private List<SaleOrderReplyPackage> CreatePackageDatas(F050305 f050305, List<F055004> f055004List)
		{
			var f055001s = ExportService.GetF055001s(f050305);
			var f055002s = ExportService.GetF055002s(f050305);

			var f055004s = f055004List.GroupBy(x => new { x.ITEM_CODE, x.ORD_SEQ, x.MAKE_NO, x.BOX_NO, x.VALID_DATE }).Select(x => new
			{
				x.Key.ITEM_CODE,
				ORD_SEQ = x.Key.ORD_SEQ.ToString(),
				x.Key.MAKE_NO,
				x.Key.BOX_NO,
				QTY = x.Sum(z => z.QTY),
				x.Key.VALID_DATE,
				SERIAL_NO_LIST = x.Where(w => !string.IsNullOrWhiteSpace(w.SERIAL_NO)).Select(s => s.SERIAL_NO).ToList()
			});

			var datas = from A in f055001s
									join B in f055002s
									on new { A.WMS_ORD_NO, A.PACKAGE_BOX_NO } equals new { B.WMS_ORD_NO, B.PACKAGE_BOX_NO }
									select new { F055001 = A, F055002 = B };


			var res = datas.GroupBy(x => new { x.F055001 })
										 .Select(package => new SaleOrderReplyPackage // Package層
										 {
											 WmsNo = package.Key.F055001.WMS_ORD_NO,
											 BoxNo = package.Key.F055001.PACKAGE_BOX_NO,
											 BoxNum = package.Key.F055001.BOX_NUM,
											 TransportCode = package.Key.F055001.PAST_NO,
											 TransportProvider = package.Key.F055001.LOGISTIC_CODE,
											 ShipmentTime = package.Key.F055001.AUDIT_DATE == null ? null : Convert.ToDateTime(package.Key.F055001.AUDIT_DATE).ToString("yyyy/MM/dd HH:mm:ss"),
											 Details = package.GroupBy(z => new { z.F055002.ORD_SEQ, z.F055002.ITEM_CODE, z.F055002.PACKAGE_BOX_NO })
																	 .OrderBy(z => z.Key.ORD_SEQ).Select(detail => new SaleOrderReplyPackageDetail // Detail層
																	 {
																		 ItemSeq = detail.Key.ORD_SEQ,
																		 ItemCode = detail.Key.ITEM_CODE,
																		 OutQty = Convert.ToInt32(detail.Sum(y => y.F055002.PACKAGE_QTY)),
																		 MakeNoDetails = f055004s.Where(z => z.ITEM_CODE == detail.Key.ITEM_CODE && z.ORD_SEQ == detail.Key.ORD_SEQ && z.BOX_NO == detail.Key.PACKAGE_BOX_NO.ToString()).Select(makeNoDetail => new SaleOrderReplyPackageMakeNoDetail
																		 {
																			 ValidDate = makeNoDetail.VALID_DATE.HasValue
																				 ? makeNoDetail.VALID_DATE.Value.ToString("yyyy/MM/dd")
																				 : "",
																			 MakeNo = makeNoDetail.MAKE_NO,
																			 MakeNoQty = makeNoDetail.QTY,
																			 SnList = makeNoDetail.SERIAL_NO_LIST
																		 }).ToList()
																	 }).ToList()
										 }).ToList();

			return res;
		}

		private List<LackDetail> GetLackDetails(F050305 f050305)
		{
			var res = new List<LackDetail>();

			//取得貨主設定是否允許訂單缺品出貨
			var AllowOrderLackShip = CommonService.GetCust(f050305.GUP_CODE, f050305.CUST_CODE)?.ALLOW_ORDER_LACKSHIP == "1";
			if (AllowOrderLackShip)
			{
				var wmsOrdNos = ExportService.GetWmsOrdNos(f050305);
				//取得出貨單缺貨紀錄
				f051206LackDatas = F051206Repo.GetF051206LackDatas(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, wmsOrdNos).ToList();

				var f05030202s = ExportService.GetF05030202s(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, new List<string> { f050305.ORD_NO });

				f051206LackDatas.ForEach(data =>
				{
					//該出貨項次剩餘缺貨數
					var lastLackQty = data.LACK_QTY;
					var curnF05030202LackExs = f05030202s.Where(x => x.WMS_ORD_NO == data.WMS_ORD_NO && x.WMS_ORD_SEQ == data.WMS_ORD_SEQ);

					foreach (var f05030202LackEx in curnF05030202LackExs)
					{
						//當前訂單項次缺貨數
						var curnLackQty = f05030202LackEx.B_DELV_QTY - f05030202LackEx.A_DELV_QTY;

						if (lastLackQty == 0 || curnLackQty == 0) continue;
						if (lastLackQty >= curnLackQty)
						{
							lastLackQty -= curnLackQty;
						}
						else
						{
							curnLackQty = lastLackQty;
							f05030202LackEx.A_DELV_QTY += lastLackQty;
							lastLackQty = 0;
						}

						res.Add(new LackDetail
						{
							ItemSeq = f05030202LackEx.ORD_SEQ,
							ItemCode = data.ITEM_CODE,
							LackQty = curnLackQty.Value,
							LackCause = data.REASON,
							LackCauseMemo = data.MEMO,
						});
					}
				});
			}

			return res.OrderBy(x => x.ItemSeq).ToList();
		}
	}
}
