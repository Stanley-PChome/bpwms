using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.Wcs.WcsApiConnectSetting;
using Wms3pl.WebServices.Shared.WcsService;

namespace Wms3pl.WebServices.ToWcsWebApi.Business.mssql.Services
{
	public class WcsUserServices : WcsBaseService
	{
		/// <summary>
		/// 人員資訊
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult User(WcsExportReq req)
		{
			var res = new ApiResult { IsSuccessed = true };
			var data = new List<ApiResponse>();

			// 新增API Log
			res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ExportUserResults", req, () =>
			{
				// 取得物流中心服務貨主檔
				CommonService commonService = new CommonService();
				var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);
				dcCustList.ForEach(item =>
				{
					var result = ExportUserResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);
					data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
				});
				res.Data = JsonConvert.SerializeObject(data);
				return res;
			}, true);

			return res;
		}

		/// <summary>
		/// 人員資訊
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		private ApiResult ExportUserResults(string dcCode, string gupCode, string custCode)
		{
			#region 變數設定
			var res = new ApiResult { IsSuccessed = true };
			var f060301Repo = new F060301Repository(Schemas.CoreSchema);
			var successCnt = 0;
      #endregion

      #region 主要邏輯
      var SettingsResendCount = GetMidApiRelmt();
      // 取得要執行的人員清單
      var executeDatas = f060301Repo.GetWcsExecuteDatas(dcCode, new List<string> { "0", "T" }, SettingsResendCount);

      int totalCnt = executeDatas.Count();

			if (executeDatas.Any())
			{
				// Group倉庫編號、新增/修改刪除 用以分批發送Params
				var sendDatas = executeDatas.GroupBy(x => new { WAREHOUSE_ID = x.F060301.WAREHOUSE_ID, CMD_TYPE = x.F060301.CMD_TYPE }).Select(x => new
				{
					CmdType = x.Key.CMD_TYPE,
					WarehouseId = x.Key.WAREHOUSE_ID,
					Reqs = x.Select(z => z.UserData).ToList(),
					F060301s = x.Select(z => z.F060301).ToList()
				}).OrderBy(x => x.CmdType).ToList();

				// 依照倉庫編號分批發送
				sendDatas.ForEach(obj =>
				{
					#region 更新 F060301 處理中狀態
					// 更新人員資料表處理中狀態
					f060301Repo = new F060301Repository(Schemas.CoreSchema);
					obj.F060301s.ForEach(f060301 => { f060301.STATUS = "1"; });
					f060301Repo.BulkUpdate(obj.F060301s);
					#endregion

					#region 執行人員資訊
					DateTime now = DateTime.Now;
					// 執行次數累加
					obj.F060301s.ForEach(f060301 =>
					{
						f060301.RESENT_CNT++;
						f060301.PROC_DATE = now;
					});

					// 以倉庫編號分批送出
					ApiResult result = new ApiResult { IsSuccessed = true };

					string url = $"v1/{dcCode}/{obj.WarehouseId}/User";// 人員資訊Url

					var currReq = new WcsUserReq
					{
						OwnerCode = custCode,
						UserTotal = obj.Reqs.Count,
						UserList = obj.Reqs
					};

					// 呼叫WcsApi-人員資訊
					ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "WcsUserResult", new { WcsApiUrl = $"{WcsSetting.ApiUrl}{url}", WcsToken = WcsSetting.ApiAuthToken, WcsData = isSaveWcsData ? currReq : null, F060301s = obj.F060301s }, () =>
          {
#if (DEBUG)
            result = WcsApiFuncTest(currReq, "User");
#else
            result = WcsApiFunc(currReq, url);
#endif

            if (result.IsSuccessed)
            {
              obj.F060301s.ForEach(f060301 =>
              {
                f060301.STATUS = "2";//成功狀態改為2
                f060301.MESSAGE = result.MsgContent;
              });
              successCnt += obj.F060301s.Count;
            }
            else
            {
              var errorDatas = JsonConvert.DeserializeObject<List<WcsUserResData>>(
                  JsonConvert.SerializeObject(result.Data)
                  );

              if (errorDatas != null && errorDatas.Any())
              {
                // 找出失敗的做處理
                var errorUserIds = errorDatas.Where(x => !string.IsNullOrWhiteSpace(x.UserId)).Select(z => z.UserId);
                obj.F060301s.Where(x => x.CMD_TYPE == obj.CmdType && errorUserIds.Contains(x.EMP_ID)).ToList().ForEach(f060301 =>
                {
                  f060301.STATUS = "F";//失敗狀態改為F
                  f060301.MESSAGE = result.MsgContent;
                });

                // 找出成功的做處理
                var successDatas = obj.F060301s.Where(x => x.CMD_TYPE == obj.CmdType && !errorUserIds.Contains(x.EMP_ID)).ToList();
                successDatas.ForEach(f060301 =>
                {
                  f060301.STATUS = "2";//成功狀態改為2
                                  f060301.MESSAGE = "Success";
                });
                successCnt += successDatas.Count;
              }
              else
              {
                obj.F060301s.ForEach(f060301 =>
                {
                  f060301.STATUS = "2";//成功狀態改為2
                                  f060301.MESSAGE = "Success";
                });
                successCnt += obj.F060301s.Count;
              }
            }

            return result;
          }, false,
          (fResult) =>
          {
            if(!fResult.IsSuccessed)
              obj.F060301s.ForEach(f060301 =>
              {
                if (f060301.RESENT_CNT >= SettingsResendCount)
                  f060301.STATUS = "F";//錯誤將狀態改為F
              else
                  f060301.STATUS = "T";
                f060301.MESSAGE = fResult.MsgContent;
              });

            return new ApiResult();
          });
          #endregion

          #region 更新 F060301 完成、錯誤、逾時狀態、執行次數
          f060301Repo = new F060301Repository(Schemas.CoreSchema);
					f060301Repo.BulkUpdate(obj.F060301s);
					#endregion
				});
			}
			int failCnt = totalCnt - successCnt;
			res.MsgCode = "10005";
			res.MsgContent = string.Format(_tacService.GetMsg("10005"), "人員資訊", successCnt, failCnt, totalCnt);
			res.TotalCnt = totalCnt;
			res.SuccessCnt = successCnt;
			res.FailureCnt = failCnt;
			#endregion

			return res;
		}
	}
}
