using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F51;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.ToWmsWebApi.Business.mssql.Services
{
	public class StockSnapshotService : BaseService
	{
		/// <summary>
		/// WMS自動倉庫存備份
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult StockSnapshotConfirm(WcsImportReq req)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();

			// 新增API Log
			res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ImportStockSnapshotConfirmResults", req, () =>
			{
				// 取得物流中心服務貨主檔
				CommonService commonService = new CommonService();
				var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);
				dcCustList.ForEach(item =>
				{
					var result = ImportStockSnapshotConfirmResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);
					data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
				});
				res.Data = JsonConvert.SerializeObject(data);
				return res;
			}, true);

			return res;
		}

		/// <summary>
		/// WMS自動倉庫存備份
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		private ApiResult ImportStockSnapshotConfirmResults(string dcCode, string gupCode, string custCode)
		{
			// 新增API Log
			return ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "StockSnapshotResults", null, () =>
			{
				#region 變數設定
				var wmsTransaction = new WmsTransaction();
				var f51010501Repo = new F51010501Repository(Schemas.CoreSchema, wmsTransaction);
				var f510105Repo = new F510105Repository(Schemas.CoreSchema, wmsTransaction);
				var f1980Repo = new F1980Repository(Schemas.CoreSchema, wmsTransaction);
				#endregion

				var now = DateTime.Today.AddDays(-1);

				var f510105s = f510105Repo.GetDatasByCalDate(dcCode, gupCode, custCode, now);
				if (f510105s.Any())
				{
					var addF51010501Datas = f510105s.Select(AutoMapper.Mapper.DynamicMap<F51010501>).ToList();

					// 將移動至F51010501完成的資料，從F510105刪除
					f510105Repo.BulkDelete(f510105s.ToList());

					// 將資料寫入F51010501庫存比對歷史紀錄表
					f51010501Repo.BulkInsert(addF51010501Datas);
				}

				var todayIsExist = f510105Repo.GetDatasEqualCalDate(dcCode, gupCode, custCode, now.ToString("yyyy/MM/dd")).Any();
				if (todayIsExist)
				{
					wmsTransaction.Complete();
					return new ApiResult { IsSuccessed = false, MsgContent = "今天庫存已經備份，不可重複執行" };
				}
				else
				{
					#region 將非人工倉的庫存資料寫入庫存比對資料表中
					var addF510105s = f510105Repo.GetAddDatasByStockSnapshot(dcCode, gupCode, custCode, now);
					if (addF510105s.Any())
						f510105Repo.BulkInsert(addF510105s);
					#endregion

					wmsTransaction.Complete();
					return new ApiResult { IsSuccessed = true, MsgContent = "今天庫存備份成功" };
				}
			}, false);
		}
	}
}
