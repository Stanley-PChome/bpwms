using Newtonsoft.Json;
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
    public class ExportStockMovementService : LmsBaseService
	{
		private WmsTransaction _wmsTransaction;
    private CommonService _commonService;

		public ExportStockMovementService(WmsTransaction wmsTransation)
		{
			_wmsTransaction = wmsTransation;
		}

		public ApiResult ExportStockMovementResults(string dcCode, string gupCode, string custCode)
		{
			ApiResult res = new ApiResult() { IsSuccessed = true };
			List<StockMovementResultsRes> data = new List<StockMovementResultsRes>();
			TransApiBaseService tacService = new TransApiBaseService();
			var f191303Repo = new F191303Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0003epo = new F0003Repository(Schemas.CoreSchema, _wmsTransaction);

      if (_commonService == null)
      {
        _commonService = new CommonService();
      }

			var outputJsonInLog = _commonService.GetSysGlobalValue("OutputJsonInLog");
			bool isSaveWmsData = string.IsNullOrWhiteSpace(outputJsonInLog) ? false : outputJsonInLog == "1";

			// 取得庫存跨倉移動紀錄表
			var f191303s = f191303Repo.GetProcessData(dcCode, gupCode, custCode);

			if (f191303s.Any())
			{
				// 取得最大筆數
				var expStockMovementMax = Convert.ToInt32(_commonService.GetSysGlobalValue("ExpStockMovementMax"));

				// 取得執行次數
				int index = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(f191303s.Count()) / expStockMovementMax));

				for (int i = 0; i < index; i++)
				{
					var currDatas = f191303s.Skip(i * expStockMovementMax).Take(expStockMovementMax).ToList();
					var stockMovementResults = currDatas.Select(x => new StockMovement
					{
						ShiftWmsNo = x.SHIFT_WMS_NO,
						ShiftType = x.SHIFT_TYPE,
						SourceWhType = x.SRC_WAREHOUSE_TYPE,
						SourceWhNo = x.SRC_WAREHOUSE_ID,
						TargetWhType = x.TAR_WAREHOUSE_TYPE,
						TargetWhNo = x.TAR_WAREHOUSE_ID,
						ItemCode = x.ITEM_CODE,
						ShiftCause = x.SHIFT_CAUSE,
						ShiftCaseMemo = x.SHIFT_CAUSE_MEMO,
						ShiftTime = x.SHIFT_TIME == null ? null : Convert.ToDateTime(x.SHIFT_TIME).ToString("yyyy/MM/dd HH:mm:ss"),
						ShiftQty = x.SHIFT_QTY,
                        MakeNo = x.MAKE_NO
					}).ToList();

					var currReq = new StockMovementResultsReq { DcCode = dcCode, CustCode = custCode, Data = stockMovementResults };

					ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "LmsApi_ExportStockMovementResults", new { LmsApiUrl = LmsSetting.ApiUrl + "Inventory/Shift", LmsToken = LmsSetting.ApiAuthToken, WmsCondition = new { DC_CODE = dcCode, GUP_CODE = gupCode, CUST_CODE = custCode }, LmsData = isSaveWmsData ? currReq : null }, () =>
					{

						ApiResult result = new ApiResult() { IsSuccessed = true };
#if (DEBUG)
						result = LmsApiFuncTest(currReq, "Inventory/Shift");
#else
            result = LmsApiFunc(currReq, "Inventory/Shift");
#endif
						if (result.IsSuccessed)
						{
							#region 更新處理狀態
							foreach (var currData in currDatas)
							{
								currData.PROC_FLAG = "1";
								currData.TRANS_DATE = DateTime.Now;
								res.SuccessCnt++;
							}
							f191303Repo.BulkUpdate(currDatas);
							#endregion
						}
						else
						{
							var errorDatas = JsonConvert.DeserializeObject<List<StockMovementResultsRes>>(
							JsonConvert.SerializeObject(result.Data));

							if (errorDatas != null && errorDatas.Any())
							{
								var shiftWmsNos = errorDatas.Select(x => x.ShiftWmsNo);
								var isSuccessData = currDatas.Where(x => !shiftWmsNos.Contains(x.SHIFT_WMS_NO));
								foreach (var successData in isSuccessData)
								{
									successData.PROC_FLAG = "1";
									successData.TRANS_DATE = DateTime.Now;
									res.SuccessCnt++;
								}
								f191303Repo.BulkUpdate(currDatas);
								data.AddRange(errorDatas.Select(x => new StockMovementResultsRes
								{
									MsgCode = x.MsgCode,
									MsgContent = x.MsgContent,
									ShiftWmsNo = x.ShiftWmsNo,
								}));
							}
							else
							{
								foreach (var currData in currDatas)
								{
									currData.PROC_FLAG = "1";
									currData.TRANS_DATE = DateTime.Now;
								}
								f191303Repo.BulkUpdate(currDatas);
							}
						}
						_wmsTransaction.Complete();

						return result;
					}, false);
				}
			}

			if (data.Any())
			{
				res.Data = data;
			}

			res.TotalCnt = f191303s.Count();
			res.FailureCnt = res.TotalCnt - res.SuccessCnt;
			res.MsgCode = "10005";
			res.MsgContent = string.Format(tacService.GetMsg("10005"),
					"庫存異動回報", res.SuccessCnt, res.FailureCnt, res.TotalCnt);

			return res;
		}
	}
}
