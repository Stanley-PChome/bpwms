using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Enums;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Helper;

namespace Wms3pl.WebServices.Shared.Services
{
	public partial class SharedService
	{
		#region 集貨出場確認排程
		/// <summary>
		/// 集貨出場確認排程
		/// </summary>
		/// <returns></returns>
		public ApiResult CollOutboundConfirm()
		{
			return ApiLogHelper.CreateApiLogInfo("0", "0", "0", "CollOutboundConfirm", new object { }, () =>
			{
				var result = CollOutboundConfirmMain();
				return new ApiResult { IsSuccessed = result.IsSuccessed, MsgCode = "", MsgContent = result.Message };
			}, true);
		}

		/// <summary>
		/// 集貨出場確認主流程
		/// </summary>
		/// <returns></returns>
		public ExecuteResult CollOutboundConfirmMain()
		{
			_wmsLogHelper = new WmsLogHelper();
			_wmsLogHelper.StartRecord(WmsLogProcType.CollOutboundConfirm);
			StockService = new StockService(_wmsTransaction);
			_commonService = new CommonService();
			var f051301Repo = new F051301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051401Repo = new F051401Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema);
			var f051206Repo = new F051206Repository(Schemas.CoreSchema);
			var f05120601Repo = new F05120601Repository(Schemas.CoreSchema);
			var f050801Repo = new F050801Repository(Schemas.CoreSchema,_wmsTransaction);
			var f070101Repo = new F070101Repository(Schemas.CoreSchema);
			var f0701Repo = new F0701Repository(Schemas.CoreSchema);
			var f000904Repo = new F000904Repository(Schemas.CoreSchema);
			var updF051301Datas = new List<F051301>();
			try
			{
				// 1.	需集貨的出貨單清單 = 撈出 F051301.wms_ord_no, cust_code, gup_code by STATUS=2 (集貨中) Group by DC_CODE, COLLECTION_CODE, WMS_ORD_NO Order by DC_CODE, COLLECTION_CODE, WMS_ORD_NO
				var f051301s = f051301Repo.GetDatasByTrueAndCondition(o => o.STATUS == "2").ToList();

				if (!f051301s.Any())
					_wmsLogHelper.AddRecord($"無狀態2(集貨中)的出貨單");

				var nextStepList = f000904Repo.GetF000904Data("F051301", "NEXT_STEP");

				// 取得未完成的揀貨單資料
				var f051202s = f051202Repo.GetCollectionOutboundDatas().ToList();

				// 取得未完成的揀貨缺貨處理資料
				var f051206s = f051206Repo.GetCollectionOutboundDatas().ToList();

				// 取得已完成且以找到商品的揀貨缺貨處理資料
				var f051206sByFindItem = f051206Repo.GetCollectionOutboundDatasByFindItem().ToList();

				// 取得揀缺等待扣庫資料
				var f05120601s = f05120601Repo.GetCollectionOutboundDatas().ToList();

				// 取得訂單資料
				var f050801s = f050801Repo.GetCollectionOutboundDatas().ToList();

				// 取得容器資料
				var f070101s = f070101Repo.GetCollectionOutboundDatas().ToList();

				// 取得容器資料
				var f0701s = f0701Repo.GetCollectionOutboundDatas().ToList();

				var f051301G = f051301s.GroupBy(o => new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, o.COLLECTION_CODE, o.WMS_NO })
					.OrderBy(o => o.Key.DC_CODE)
					.ThenBy(o => o.Key.GUP_CODE)
					.ThenBy(o => o.Key.CUST_CODE)
					.ThenBy(o => o.Key.COLLECTION_CODE)
					.ThenBy(o => o.Key.WMS_NO)
					.Select(o => new { o.Key.DC_CODE, o.Key.GUP_CODE, o.Key.CUST_CODE, o.Key.COLLECTION_CODE, o.Key.WMS_NO }).ToList();

				foreach (var item in f051301G)
				{

					var currF051301s = f051301s.Where(x =>
						x.DC_CODE == item.DC_CODE &&
						x.GUP_CODE == item.GUP_CODE &&
						x.CUST_CODE == item.CUST_CODE &&
						x.WMS_NO == item.WMS_NO &&
						x.COLLECTION_CODE == item.COLLECTION_CODE).ToList();

					#region (1)	該出貨單的所有揀貨明細是否都完成揀貨? (F051202)
					var currF051202s = f051202s.Where(x =>
					x.WMS_ORD_NO == item.WMS_NO &&
					x.GUP_CODE == item.GUP_CODE &&
					x.CUST_CODE == item.CUST_CODE);
					// 若未完成的揀貨單筆數 > 0，表示尚未揀完貨，這筆不繼續做，先跳下一筆
					if (currF051202s.Any())
						continue;
					#endregion

					#region (2)	是否有尚未處理完成的揀貨缺貨處理(F051206)
					var currF051206s = f051206s.Where(x =>
					x.WMS_ORD_NO == item.WMS_NO &&
					x.GUP_CODE == item.GUP_CODE &&
					x.CUST_CODE == item.CUST_CODE);
					// 若未完成的揀貨單筆數 > 0，表示有揀缺尚未處理，這筆不繼續做，先跳下一筆
					if (currF051206s.Any())
						continue;
					#endregion

					#region (3)	是否有揀缺等待扣庫的資料(F05120601)
					var currF05120601s = f05120601s.Where(x =>
					x.WMS_ORD_NO == item.WMS_NO &&
					x.GUP_CODE == item.GUP_CODE &&
					x.CUST_CODE == item.CUST_CODE);
					// 若未完成的揀貨單筆數 > 0，表示有揀缺尚未處理，這筆不繼續做，先跳下一筆
					if (currF05120601s.Any())
						continue;
					#endregion

					#region (5)	是否所有容器都到達集貨場?
					var currF070101s = f070101s.Where(x =>
					x.WMS_NO == item.WMS_NO &&
					x.GUP_CODE == item.GUP_CODE &&
					x.CUST_CODE == item.CUST_CODE);

					if (currF070101s.Any())
					{
						var currF0701s = f0701s.Where(x => currF070101s.Select(z => z.F0701_ID).Contains(x.ID));
						if (currF0701s.Count() > 1)
							continue;
					}
					#endregion

					#region (4)	檢查訂單是否被取消? 
					var currF050801 = f050801s.Where(x =>
					x.WMS_ORD_NO == item.WMS_NO &&
					x.GUP_CODE == item.GUP_CODE &&
					x.CUST_CODE == item.CUST_CODE).FirstOrDefault();
					// 若未完成的揀貨單筆數 > 0，表示有揀缺尚未處理，這筆不繼續做，先跳下一筆
					if (currF050801 != null)
					{
						// B.	若F050801.STATUS=9 (取消)，指示人員將容器帶到異常區
						if (currF051301s.Any())
						{
							if (currF050801.STATUS == 9)
							{
								// 異常區
								currF051301s.ForEach(f051301 => { f051301.NEXT_STEP = "4"; });
								updF051301Datas.AddRange(currF051301s);
								_wmsLogHelper.AddRecord($"出貨單號{item.WMS_NO}下一站作業為異常區");
							}
							else
							{
								// 檢查揀貨缺貨處理完成的原因? 
								var currF05120601sByFindItem = f051206sByFindItem.Where(x =>
								x.WMS_ORD_NO == item.WMS_NO &&
								x.GUP_CODE == item.GUP_CODE &&
								x.CUST_CODE == item.CUST_CODE);

								if (currF05120601sByFindItem.Any())
								{
									// 異常區
									currF051301s.ForEach(f051301 => { f051301.NEXT_STEP = "4"; });
									updF051301Datas.AddRange(currF051301s);
									_wmsLogHelper.AddRecord($"出貨單號{item.WMS_NO}下一站作業為異常區");
								}
								else
								{
									var nextStep = currF051301s.FirstOrDefault().NEXT_STEP;
									if (string.IsNullOrWhiteSpace(nextStep))
									{
										_wmsLogHelper.AddRecord($"出貨單號{item.WMS_NO}無下一站作業");
									}
									else
									{
										var nextStepName = nextStepList.Where(x => x.VALUE == currF051301s.FirstOrDefault().NEXT_STEP).First().NAME;
										_wmsLogHelper.AddRecord($"出貨單號{item.WMS_NO}下一站作業為{nextStepName}");
									}
								}
							}
						}
					}
					#endregion

					#region 不論訂單是否取消，都更新F051401.STATUS = 3 (已到齊)
					f051401Repo.UpdateStatus("3", item.DC_CODE, item.WMS_NO);

					currF051301s.ForEach(f051301 =>
					{
						var currUpdF051301 = updF051301Datas.Where(x => x.DC_CODE == f051301.DC_CODE &&
						x.GUP_CODE == f051301.GUP_CODE &&
						x.CUST_CODE == f051301.CUST_CODE &&
						x.DELV_DATE == f051301.DELV_DATE &&
						x.PICK_TIME == f051301.PICK_TIME &&
						x.WMS_NO == f051301.WMS_NO).FirstOrDefault();

						if (currUpdF051301 == null)
						{
							f051301.STATUS = "3";
							updF051301Datas.Add(f051301);
						}
						else
						{
							currUpdF051301.STATUS = "3";
						}
					});

					#endregion
				}

				if(updF051301Datas.Any())
				{
					// 更新出貨單揀貨完成時間
					var isFinishedWmsOrdNos = updF051301Datas.Where(x => (x.STATUS == "3" || x.NEXT_STEP == "4") && x.WMS_NO.StartsWith("O"))
						.GroupBy(x=> new { x.DC_CODE,x.GUP_CODE,x.CUST_CODE }).ToList();
					isFinishedWmsOrdNos.ForEach(x =>
					{
						f050801Repo.UpdatePickFinished(x.Key.DC_CODE, x.Key.GUP_CODE, x.Key.CUST_CODE, x.Select(y => y.WMS_NO).Distinct().ToList());
					});

				}

				if (updF051301Datas.Any())
					f051301Repo.BulkUpdate(updF051301Datas);
				_wmsLogHelper.AddRecord("執行db commit 開始");
				_wmsTransaction.Complete();
				_wmsLogHelper.AddRecord("執行db commit 結束");
			}
			finally
			{
			}

			_wmsLogHelper.StopRecord();
			return new ExecuteResult(true);
		}
		#endregion
	}
}
