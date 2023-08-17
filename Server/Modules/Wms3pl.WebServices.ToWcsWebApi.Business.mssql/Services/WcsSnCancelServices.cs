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
	public class WcsSnCancelServices : WcsBaseService
	{
		/// <summary>
		/// 序號刪除
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult SnCancel(WcsExportReq req)
		{
			var res = new ApiResult { IsSuccessed = true };
			var data = new List<ApiResponse>();
      WcsSetting.DcCode = req.DcCode;

      // 新增API Log
      res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ExportSnCancelResults", req, () =>
			{
				// 取得物流中心服務貨主檔
				CommonService commonService = new CommonService();
				var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);
				dcCustList.ForEach(item =>
				{
					var result = ExportSnCancelResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);
					data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
				});
				res.Data = JsonConvert.SerializeObject(data);
				return res;
			}, true);

			return res;
		}

		/// <summary>
		/// 序號刪除
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		private ApiResult ExportSnCancelResults(string dcCode, string gupCode, string custCode)
		{
			#region 變數設定
			var res = new ApiResult { IsSuccessed = true };
			var f060501Repo = new F060501Repository(Schemas.CoreSchema);
			var itemService = new WcsItemServices();
      var successCnt = 0;
      
      #endregion

            #region 主要邏輯
            var SettingsResendCount = GetMidApiRelmt();

      // 取得要執行的人員清單
      var executeDatas = f060501Repo.GetWcsExecuteDatas(dcCode, gupCode, custCode, new List<string> { "0", "T" }, SettingsResendCount);

      int totalCnt = executeDatas.Count();

      if (executeDatas.Any())
			{
				// Group倉庫編號、新增/修改刪除 用以分批發送Params
				var sendDatas = executeDatas.GroupBy(x => x.WAREHOUSE_ID).Select(x => new
				{
					WarehouseId = x.Key,
					F060501s = x.ToList()
				}).OrderBy(x => x.WarehouseId).ToList();

				// 依照倉庫編號分批發送
				sendDatas.ForEach(obj =>
				{
					obj.F060501s.ForEach(f060501 =>
					{
						#region 更新 F060501 處理中狀態
						// 更新人員資料表處理中狀態
						f060501Repo = new F060501Repository(Schemas.CoreSchema);
						f060501.STATUS = "1";
						f060501Repo.Update(f060501);
						#endregion

						#region 執行序號刪除
						// 執行次數累加
						f060501.RESENT_CNT++;
						f060501.PROC_DATE = DateTime.Now;

						// 以單號分批送出
						var result = itemService.WcsItemSnCancel(dcCode, gupCode, custCode, f060501);
						f060501.MESSAGE = result.MsgContent;

            // 若有其中一個倉Timeout就掛T
            if (result.MsgCode == "99998")
              f060501.STATUS = "T";//逾時將狀態改為T
            else if (result.MsgCode == "99999")
            {
              //重試次數超過設定值後才改失敗
              if (f060501.RESENT_CNT >= SettingsResendCount)
                f060501.STATUS = "F";//錯誤將狀態改為F
              else
                f060501.STATUS = "T";
            }
            else
            {
              if (result.IsSuccessed)
              {
                f060501.STATUS = "2";//成功狀態改為2
                successCnt++;
              }
              else
              {
                f060501.STATUS = "F";//失敗狀態改為F
              }
            }
            #endregion

            #region 更新 F060501 完成、錯誤、逾時狀態、執行次數
            f060501Repo = new F060501Repository(Schemas.CoreSchema);
						f060501Repo.Update(f060501);
						#endregion
					});
				});
			}
			int failCnt = totalCnt - successCnt;
			res.MsgCode = "10005";
			res.MsgContent = string.Format(_tacService.GetMsg("10005"), "序號刪除", successCnt, failCnt, totalCnt);
			res.TotalCnt = totalCnt;
			res.SuccessCnt = successCnt;
			res.FailureCnt = failCnt;
			#endregion

			return res;
		}
	}
}
