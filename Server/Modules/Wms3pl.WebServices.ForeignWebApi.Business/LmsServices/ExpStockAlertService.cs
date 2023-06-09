using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
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
    public class ExpStockAlertService : LmsBaseService
	{
		private WmsTransaction _wmsTransaction;
    private CommonService _commonService;

		public ExpStockAlertService(WmsTransaction wmsTransation)
		{
			_wmsTransaction = wmsTransation;
		}

		public ApiResult ExpStockAlertResults(string dcCode, string gupCode, string custCode)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();
			var f1913Repo = new F1913Repository(Schemas.CoreSchema);
			TransApiBaseService tacService = new TransApiBaseService();

      if (_commonService == null)
      {
        _commonService = new CommonService();
      }

			var outputJsonInLog = _commonService.GetSysGlobalValue("OutputJsonInLog");
			bool isSaveWmsData = string.IsNullOrWhiteSpace(outputJsonInLog) ? false : outputJsonInLog == "1";

			#region 取得 庫存超過商品警示天數資料
			var stockAlertReturn = f1913Repo.GetDatasByExpStockAlert(dcCode, gupCode, custCode).ToList();

			//var f1913s = stockAlertReturn.GroupBy(x => new { x.ItemCode, x.ValidDate, x.MakeNo, x.AllShp }).ToList();
			#endregion


			if (stockAlertReturn.Any())
			{
				// 取的最大筆數
				var expStockAlertMax = Convert.ToInt32(_commonService.GetSysGlobalValue("ExpStockAlertMax"));
				// 取得執行次數
				int index = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(stockAlertReturn.Count) / expStockAlertMax));

				var currReq = new StockAlertReturnReq { DcCode = dcCode, CustCode = custCode };

				for (int i = 0; i < index; i++)
				{
					var currDatas = stockAlertReturn.Skip(i * expStockAlertMax).Take(expStockAlertMax).ToList();

					currReq.Data = currDatas.Select(x => new StockAlertReturn
					{
						ItemCode = x.ItemCode,
						ValidDate = x.ValidDate.ToString("yyyy/MM/dd"),
						MakeNo = x.MakeNo,
						Qty = x.Qty,
						AllShp = x.AllShp
					}).ToList();
					#region 新增API Log
					ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "LmsApi_ExpStockAlertResults", new { LmsApiUrl = LmsSetting.ApiUrl + "Inventory/Alert", LmsToken = LmsSetting.ApiAuthToken, WmsCondition = new { DC_CODE = dcCode, GUP_CODE = gupCode, CUST_CODE = custCode }, LmsData = isSaveWmsData ? currReq : null }, () =>
					{
						ApiResult result = new ApiResult() { IsSuccessed = true };
#if (DEBUG)
						result = LmsApiFuncTest(currReq, "Inventory/Alert");
#else
            result = LmsApiFunc(currReq, "Inventory/Alert");
#endif
						if (!result.IsSuccessed)
						{
							var resultData = (List<ApiResponse>)(object)result.Data;
							data.AddRange(resultData);
							res.FailureCnt += result.FailureCnt;
							
						}

						return result;
					}, false);
					#endregion
				}
			}

			if (data.Any())
				res.Data = data;

			res.IsSuccessed = res.FailureCnt == 0;
			res.TotalCnt = stockAlertReturn.Count;
			res.MsgCode = "10005";
			res.MsgContent = string.Format(tacService.GetMsg("10005"),
					"庫存警示回報", res.TotalCnt - res.FailureCnt, res.FailureCnt, res.TotalCnt);

			return res;
		}
	}
}
