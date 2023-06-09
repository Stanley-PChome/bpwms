﻿using System;
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
		private WmsTransaction _wmsTransaction;

		public ExportVendorReturnService(WmsTransaction wmsTransation)
		{
			_wmsTransaction = wmsTransation;
		}

		public ApiResult ExportVendorReturnResults(string dcCode, string gupCode, string custCode)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();
			var f050305Repo = new F050305Repository(Schemas.CoreSchema);
			var f160204Repo = new F160204Repository(Schemas.CoreSchema);
			var f05030202Repo = new F05030202Repository(Schemas.CoreSchema);
			var f050802Repo = new F050802Repository(Schemas.CoreSchema);
			TransApiBaseService tacService = new TransApiBaseService();

			var commonService = new CommonService();
			var outputJsonInLog = commonService.GetSysGlobalValue("OutputJsonInLog");
			bool isSaveWmsData = string.IsNullOrWhiteSpace(outputJsonInLog) ? false : outputJsonInLog == "1";

			#region 取得 商品進倉未回檔資料

			var f050305s = f050305Repo.AsForUpdate().GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode
																																						&& o.GUP_CODE == gupCode
																																						&& o.CUST_CODE == custCode
																																						&& o.PROC_FLAG == "0"
																																						&& o.SOURCE_TYPE == "13").OrderBy(x => x.ID).ToList();

			var f160204s = f160204Repo.GetDatasByExportVendorReturn(dcCode, gupCode, custCode, f050305s.Select(x => x.SOURCE_NO).Distinct().ToList());

			var ordNos = f050305s.Select(x => x.ORD_NO).ToList();

			var f05030202s = f05030202Repo.GetDatasByOrdNos(dcCode, gupCode, custCode, ordNos);

			var f050802s = f050802Repo.GetDatas(dcCode, gupCode, custCode, f05030202s.Select(x => x.WMS_ORD_NO).ToList());

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
								f050305Repo.UpdateProcFlag(f050305.ID, "1");
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
			var f160204Repo = new F160204Repository(Schemas.CoreSchema, _wmsTransaction);
			return new VnrReturn
			{
				CustVnrReturnNo = custOrdNo,
				OrderType = "RO",
				Status = f050305.STATUS,
				WorkTime = f050305.CRT_DATE.ToString("yyyy/MM/dd HH:mm:ss"),
        //20211202 : 廠退出貨結果回傳排程需增加 【3:包裝完成】 的判斷
        VnrReturnDetails = (f050305.STATUS == "5" || f050305.STATUS == "3") ? f160204Repo.GetVnrReturnDetails(f050305, f055004List, f160204List, f05030202List, f050802List) : new List<VnrReturnDetail>() // 取得 VnrReturnDetails資料(STATUS=5才撈取資料)、分配Make_No並新增F055004
			};
		}

		private List<F055004> GetF055004List(F050305 f050305)
		{
			var f055004Repo = new F055004Repository(Schemas.CoreSchema);
			var exportService = new ExportService();

			var f055004s = f055004Repo.GetDatasByTrueAndCondition(o =>
			o.DC_CODE == f050305.DC_CODE &&
			o.GUP_CODE == f050305.GUP_CODE &&
			o.CUST_CODE == f050305.CUST_CODE &&
			o.ORD_NO == f050305.ORD_NO).ToList();

			return f055004s.Any() ? f055004s : (f050305.STATUS == "5" ? exportService.CreateF055004(f050305) : new List<F055004>());
		}
	}
}
