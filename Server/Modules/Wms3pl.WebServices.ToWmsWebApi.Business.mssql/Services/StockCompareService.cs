using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F51;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.ToWmsWebApi.Business.mssql.Services
{
	public class StockCompareService : BaseService
	{
		/// <summary>
		/// 庫存比對
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult StockCompareConfirm(WcsImportReq req)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();

			// 新增API Log
			res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ImportStockCompareResults", req, () =>
			{
				// 取得物流中心主檔
				CommonService commonService = new CommonService();
				var dcCodeList = commonService.GetDcCodeList(req.DcCode);
				dcCodeList.ForEach(dcCode =>
				{
					var result = ImportStockCompareResults(dcCode, req.StockCompareDate);
					data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{dcCode}" });
				});
				res.Data = JsonConvert.SerializeObject(data);
				return res;
			}, true);

			return res;
		}

		/// <summary>
		/// 庫存比對
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="compareDate"></param>
		/// <returns></returns>
		private ApiResult ImportStockCompareResults(string dcCode, DateTime? compareDate)
		{
			#region 變數設定
			ApiResult res = new ApiResult { IsSuccessed = true };
			var calDate = compareDate == null ? DateTime.Today.AddDays(-1).ToString("yyyy/MM/dd") : Convert.ToDateTime(compareDate).ToString("yyyy/MM/dd");
			var wmsTransaction = new WmsTransaction();
			var f060601Repo = new F060601Repository(Schemas.CoreSchema);
			var f060602Repo = new F060602Repository(Schemas.CoreSchema);
			var f510105Repo = new F510105Repository(Schemas.CoreSchema);
			var f000904Repo = new F000904Repository(Schemas.CoreSchema);
			var f1901Repo = new F1901Repository(Schemas.CoreSchema);
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
            #endregion

            // 檢查在同一個庫存比對日期(cal_date)中，是否有同一頁數(current_page) 數字相同 且 proc_flag=0的資料
            // 如果有，請保留ID最大的那一筆，其他proc_flag改成9
            f060601Repo.CancelRepeatData(dcCode, calDate);

            #region 取得下位系統的庫存原始總表資料
            // 取得下位系統的庫存原始總表資料
            var f060601s = f060601Repo.GetDatasByDcWithCalDate(dcCode, calDate).ToList();

            if (!f060601s.Any())
            {
                f510105Repo.UpdateProcFlag(dcCode, calDate, "2");
                return new ApiResult { IsSuccessed = false, MsgContent = "無快照資料可比對" };
            }
            #endregion

            #region 檢查每一個物流中心+系統的總頁數都到齊
            var srcSystemDatas = f000904Repo.GetDatas("SRC_SYSTEM", "SRC_SYSTEM");

			var pageDatas = f060601s.GroupBy(x => new { x.DC_CODE, x.SRC_SYSTEM, x.TOTAL_PAGE }).Select(x => new
			{
				x.Key.DC_CODE,
				x.Key.SRC_SYSTEM,
				SRC_SYSTEM_NAME = srcSystemDatas.Where(z => z.VALUE == x.Key.SRC_SYSTEM).First().NAME,
				x.Key.TOTAL_PAGE,
				UploadedPageCnt = x.Select(z => z.CURRENT_PAGE).Distinct().Count()
			});

			var notFinishPage = pageDatas.Where(x => x.TOTAL_PAGE > x.UploadedPageCnt).ToList();

			if (notFinishPage.Any())
			{
				var dcName = f1901Repo.Find(o => o.DC_CODE == dcCode).DC_NAME;

				var errMsgs = new List<string>();

				notFinishPage.ForEach(obj =>
				{
					errMsgs.Add($"系統[{obj.SRC_SYSTEM_NAME}]的總頁數應為 {obj.TOTAL_PAGE} 頁，但目前有只有 { obj.UploadedPageCnt } 頁");
				});

				return new ApiResult { IsSuccessed = false, MsgContent = $"物流中心[{dcName}]，{string.Join("；", errMsgs)}" };
			}
			#endregion

			#region 將F060601狀態改為處理中
			f060601s.ForEach(f060601 => { f060601.PROC_FLAG = "1"; });
			f060601Repo.BulkUpdate(f060601s);
			#endregion

			#region 取得庫存比對資料，狀態改為比對中
			var stockCompareDatas = f060602Repo.GetData(f060601s.Select(x => x.ID).ToList()).GroupBy(x => new
			{
				x.GUP_CODE,
				x.CUST_CODE,
				x.WAREHOUSE_ID,
				x.ITEM_CODE,
				x.VALID_DATE,
				x.MAKE_NO
			}).Select(x => new StockCompareDetail
			{
				GUP_CODE = x.Key.GUP_CODE,
				CUST_CODE = x.Key.CUST_CODE,
				WAREHOUSE_ID = x.Key.WAREHOUSE_ID,
				ITEM_CODE = x.Key.ITEM_CODE,
				VALID_DATE = x.Key.VALID_DATE,
				MAKE_NO = x.Key.MAKE_NO,
				QTY = x.Sum(z => z.QTY)
			}).ToList();

			var f510105s = f510105Repo.GetDataByPending(dcCode, calDate).ToList();
            f510105s = f510105s.Where(x => stockCompareDatas.Any(z =>
            z.GUP_CODE == x.GUP_CODE &&
            z.CUST_CODE == x.CUST_CODE &&
            z.WAREHOUSE_ID == x.WAREHOUSE_ID &&
            z.ITEM_CODE == x.ITEM_CODE &&
            z.VALID_DATE == x.VALID_DATE.ToString("yyyy/MM/dd") &&
            z.MAKE_NO == x.MAKE_NO)).ToList();

            f510105s.ForEach(f510105 => { f510105.PROC_FLAG = "1"; });
			f510105Repo.BulkUpdate(f510105s);
			#endregion

			#region 將下位系統的庫存寫入比對資料中 
			stockCompareDatas.ForEach(currObj =>
			{
				var currF510105 = f510105s.Where(x => x.GUP_CODE == currObj.GUP_CODE &&
				x.CUST_CODE == currObj.CUST_CODE &&
				x.WAREHOUSE_ID == currObj.WAREHOUSE_ID &&
				x.ITEM_CODE == currObj.ITEM_CODE &&
				x.VALID_DATE == Convert.ToDateTime(currObj.VALID_DATE) &&
				x.MAKE_NO == currObj.MAKE_NO).FirstOrDefault();

				if (currF510105 == null)
				{
					var f1912 = f1912Repo.GetDatasByTrueAndCondition(o =>
					o.DC_CODE == dcCode &&
					o.GUP_CODE == currObj.GUP_CODE &&
					o.CUST_CODE == currObj.CUST_CODE &&
					o.WAREHOUSE_ID == currObj.WAREHOUSE_ID).OrderBy(x => x.LOC_CODE).FirstOrDefault();

					f510105Repo.Add(new F510105
					{
						DC_CODE = dcCode,
						GUP_CODE = currObj.GUP_CODE,
						CUST_CODE = currObj.CUST_CODE,
						WAREHOUSE_ID = currObj.WAREHOUSE_ID,
						CAL_DATE = calDate,
						LOC_CODE = f1912 == null ? null : f1912.LOC_CODE,
						ITEM_CODE = currObj.ITEM_CODE,
						VALID_DATE = Convert.ToDateTime(currObj.VALID_DATE),
						MAKE_NO = currObj.MAKE_NO,
						WMS_QTY = 0,
						WCS_QTY = currObj.QTY,
						BOOKING_QTY = 0,
						DIFF_QTY = 0 - currObj.QTY,
						PROC_FLAG = "2"
					});
				}
				else
				{
					currF510105.WCS_QTY += currObj.QTY;
					currF510105.DIFF_QTY = currF510105.WMS_QTY + currF510105.BOOKING_QTY - currF510105.WCS_QTY;
					currF510105.PROC_FLAG = "2";
					f510105Repo.Update(currF510105);
				}
			});
			#endregion

			#region 將F060601狀態改為完成
			f060601s.ForEach(f060601 => { f060601.PROC_FLAG = "2"; });
			f060601Repo.BulkUpdate(f060601s);
            #endregion

            #region 將沒比對到的F510105.PROC_FLAG改為2
            f510105Repo.UpdateProcFlag(dcCode, calDate, "2");
            #endregion

            #region 匯出差異的資料(待補)
            #endregion

            return new ApiResult { IsSuccessed = true, MsgContent = "比對完成" };
		}
	}

}
