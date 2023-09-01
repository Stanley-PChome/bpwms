using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Lms.Services;
using Wms3pl.WebServices.Shared.Lms.WebApiConnectSetting;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;
using static Wms3pl.Datas.Shared.ApiEntities.VendorReturnReq;

namespace Wms3pl.WebServices.ForeignWebApi.Business.LmsServices
{
    public class ExportVendorReturnService : LmsBaseService
	{
		#region Repository
		private F050305Repository _f050305Repo;
		public F050305Repository F050305Repo
		{
			get { return _f050305Repo == null ? _f050305Repo = new F050305Repository(Schemas.CoreSchema) : _f050305Repo; }
			set { _f050305Repo = value; }
		}

		private F160204Repository _f160204Repo;
		public F160204Repository F160204Repo
		{
			get { return _f160204Repo == null ? _f160204Repo = new F160204Repository(Schemas.CoreSchema) : _f160204Repo; }
			set { _f160204Repo = value; }
		}

		private F050802Repository _f050802Repo;
		public F050802Repository F050802Repo
		{
			get { return _f050802Repo == null ? _f050802Repo = new F050802Repository(Schemas.CoreSchema) : _f050802Repo; }
			set { _f050802Repo = value; }
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

		private WmsTransaction _wmsTransaction;

		public ExportVendorReturnService(WmsTransaction wmsTransation)
		{
			_wmsTransaction = wmsTransation;
		}

		public ApiResult ExportVendorReturnResults(string dcCode, string gupCode, string custCode)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();
			TransApiBaseService tacService = new TransApiBaseService();

			var outputJsonInLog = CommonService.GetSysGlobalValue("OutputJsonInLog");
			bool isSaveWmsData = string.IsNullOrWhiteSpace(outputJsonInLog) ? false : outputJsonInLog == "1";

      #region 取得 商品進倉未回檔資料

      var f050305s = F050305Repo.GetDatasFor_SourceType13(dcCode, gupCode, custCode).ToList();

      var f160204s = F160204Repo.GetDatasByExportVendorReturn(dcCode, gupCode, custCode, f050305s.Select(x => x.SOURCE_NO).Distinct().ToList());

			var ordNos = f050305s.Select(x => x.ORD_NO).Distinct().ToList();

			var f05030202s = ExportService.GetF05030202s(dcCode, gupCode, custCode, ordNos);

			var f050802s = F050802Repo.GetDatas(dcCode, gupCode, custCode, f05030202s.Select(x => x.WMS_ORD_NO).Distinct().ToList());

			#endregion

			f050305s.ForEach(f050305 =>
			{
				VendorReturnReq req = new VendorReturnReq { DcCode = dcCode, CustCode = custCode };

				#region 取得 VnrReturns、VnrReturnDetails 資料
				var currF160204s = f160204s.Where(x => x.RTN_WMS_NO == f050305.SOURCE_NO).GroupBy(x => new { x.RTN_VNR_NO, x.CUST_ORD_NO } ).Select(x => new { x.Key.RTN_VNR_NO, x.Key.CUST_ORD_NO , F160204s = x.ToList() }).ToList();
				var currF055004s = GetF055004List(f050305);
				var currF05030202s = f05030202s.Where(x => x.ORD_NO == f050305.ORD_NO).ToList();
				var currF050802s = f050802s.Where(x => currF05030202s.Select(z => z.WMS_ORD_NO).Contains(x.WMS_ORD_NO)).ToList();

				currF160204s.ForEach(f160204 =>
				{
					req.VnrReturns.Add(GetVnrReturns(f050305, f160204.CUST_ORD_NO, currF055004s, f160204.F160204s, currF05030202s, currF050802s));
				});
				#endregion

				#region 新增API Log
				ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "ExportVnrRtnResult", new { LmsApiUrl = LmsSetting.ApiUrl + "VendorReturnOrder/Reply", LmsToken = LmsSetting.ApiAuthToken, WmsCondition = f050305, LmsData = isSaveWmsData ? req : null }, () =>
					{
						ApiResult result = new ApiResult();
#if (DEBUG)
						result = LmsApiFuncTest(req, "VendorReturnOrder/Reply");
#else
            result = LmsApiFunc(req, "VendorReturnOrder/Reply");
#endif
						if (result != null)
						{
							if (result.IsSuccessed)
							{
								#region 更新處理狀態
								F050305Repo.UpdateProcFlag(f050305.ID, "1");
								#endregion
							}
						}
						else
						{
							data.Add(new ApiResponse
							{
								MsgCode = result.MsgCode,
								MsgContent = result.MsgContent
							});

							res.FailureCnt++;
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
					"廠退出貨結果回傳", res.TotalCnt - res.FailureCnt, res.FailureCnt, res.TotalCnt);

			return res;
		}

		/// <summary>
		/// 取得 VnrReturns 資料
		/// </summary>
		/// <param name="f050305"></param>
		/// <param name="f160204List"></param>
		/// <returns></returns>
		private VnrReturn GetVnrReturns(F050305 f050305, string custOrdNo, List<F055004> f055004List, List<F160204> f160204List, List<F05030202> f05030202List, List<F050802> f050802List)
		{
			return new VnrReturn
			{
				CustVnrReturnNo = custOrdNo,
				OrderType = "RO",
				Status = f050305.STATUS,
				WorkTime = f050305.CRT_DATE.ToString("yyyy/MM/dd HH:mm:ss"),
        //20211202 : 廠退出貨結果回傳排程需增加 【3:包裝完成】 的判斷
        VnrReturnDetails = (f050305.STATUS == "5" || f050305.STATUS == "3") ? GetVnrReturnDetails(f050305, f055004List, f160204List, f05030202List, f050802List) : new List<VnrReturnDetail>() // 取得 VnrReturnDetails資料(STATUS=5才撈取資料)、分配Make_No並新增F055004
			};
		}

		private List<F055004> GetF055004List(F050305 f050305)
		{
			//取得F055004
			var f055004s = ExportService.GetF055004s(f050305);
			//若查無資料，則產生F055004
			if (!f055004s.Any())
			{
				if (f050305.STATUS == "5")
					ExportService.CreateF055004(f050305);
			}
			//產完F055004後再取F055004 (避免產生失敗)
			f055004s = ExportService.GetF055004s(f050305);

			return f055004s;
		}

		public List<VnrReturnDetail> GetVnrReturnDetails(F050305 f050305, List<F055004> f055004List, List<F160204> f160204List, List<F05030202> f05030202List, List<F050802> f050802List)
		{
			var rtnWmsSeqs = f160204List.Select(x => x.RTN_WMS_SEQ.ToString()).ToList();

			var f05030202s = f05030202List.Where(o => f160204List.Select(x => x.RTN_WMS_SEQ.ToString()).Contains(o.ORD_SEQ)).ToList();

			var f055004s = f055004List.GroupBy(x => new { x.ITEM_CODE, x.ORD_SEQ, x.MAKE_NO }).Select(x => new
			{
				ItemCode = x.Key.ITEM_CODE,
				Seq = x.Key.ORD_SEQ,
				MakeNo = x.Key.MAKE_NO,
				Qty = x.Sum(z => z.QTY),
				SerialNoList = x.Where(w => !string.IsNullOrWhiteSpace(w.SERIAL_NO)).Select(s => s.SERIAL_NO).ToList()
			});

			List<VnrReturnDetailTemp> detailDatas = new List<VnrReturnDetailTemp>();

			// 當F050305.STATUS=3(包裝完成)
			if (f050305.STATUS == "3")
			{
				var detailDatasTmp = (from A in f05030202s
															join B in f160204List
															on A.ORD_SEQ equals B.RTN_WMS_SEQ.ToString()
															select new VnrReturnDetailCal
															{
																WmsOrdNo = A.WMS_ORD_NO,
																WmsOrdSeq = A.WMS_ORD_SEQ,
																Seq = A.ORD_SEQ,
																ItemSeq = B.RTN_VNR_SEQ.ToString(),
																ItemCode = B.ITEM_CODE,
																B_ActQty = A.B_DELV_QTY,
																A_ActQty = Convert.ToInt32(A.A_DELV_QTY),
															}).ToList();

				// 取得出貨單明細F050802.B_DELV_QTY去分配F05030202.A_DELV_QTY
				detailDatasTmp.ForEach(detail =>
				{
					if (detail.A_ActQty != detail.B_ActQty)
					{
						var currF050802s = f050802List.Where(x => x.WMS_ORD_NO == detail.WmsOrdNo && x.WMS_ORD_SEQ == detail.WmsOrdSeq && x.B_DELV_QTY > 0).ToList();

						currF050802s.ForEach(f050802 =>
						{
							if (f050802.B_DELV_QTY == detail.B_ActQty)
							{
								detail.A_ActQty = detail.B_ActQty;
								f050802.B_DELV_QTY = 0;
							}
							else if (f050802.B_DELV_QTY > detail.B_ActQty)
							{
								detail.A_ActQty = detail.B_ActQty;
								f050802.B_DELV_QTY -= detail.B_ActQty;
							}
							else
							{
								detail.A_ActQty = Convert.ToInt32(f050802.B_DELV_QTY);
								f050802.B_DELV_QTY = 0;
							}
						});
					}
				});

				detailDatas = detailDatasTmp.Select(x => new VnrReturnDetailTemp
				{
					Seq = x.Seq,
					ItemSeq = x.ItemSeq,
					ItemCode = x.ItemCode,
					ActQty = x.A_ActQty
				}).ToList();
			}
			else
			{
				detailDatas = (from A in f05030202s
											 join B in f160204List
											 on A.ORD_SEQ equals B.RTN_WMS_SEQ.ToString()
											 select new VnrReturnDetailTemp
											 {
												 Seq = A.ORD_SEQ,
												 ItemSeq = B.RTN_VNR_SEQ.ToString(),
												 ItemCode = B.ITEM_CODE,
												 ActQty = Convert.ToInt32(A.A_DELV_QTY),
											 }).ToList();
			}

			List<VnrReturnDetail> result = new List<VnrReturnDetail>();
			if (detailDatas.Any())
			{
				result = detailDatas.GroupBy(x => new { x.ItemSeq, x.ItemCode })
															.Select(x => new VnrReturnDetail
															{
																ItemSeq = x.Key.ItemSeq.PadLeft(2, '0'),
																ItemCode = x.Key.ItemCode,
																ActQty = x.Sum(z => z.ActQty),
																MakeNoDetails = f055004s.Where(z => x.Select(y => y.Seq).Contains(z.Seq) && z.ItemCode == x.Key.ItemCode)
																												.Select(z => new VnrReturnMakeNoDetail
																												{
																													MakeNo = z.MakeNo,
																													MakeNoQty = z.Qty,
																													SnList = z.SerialNoList
																												}).ToList()
															}).ToList();
			}

			return result;
		}
	}
}
