using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.ToWmsWebApi.Business.mssql.Services
{
	public class InventoryAdjustConfirmService : BaseService
	{
		/// <summary>
		/// 盤點調整完成
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult InventoryAdjustConfirm(WcsImportReq req)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();

			// 新增API Log
			res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ImportInventoryAdjustConfirmResults", req, () =>
			{
				// 取得物流中心服務貨主檔
				CommonService commonService = new CommonService();
				var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);
				dcCustList.ForEach(item =>
							{
								var result = ImportInventoryAdjustConfirmResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);
								data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
							});
				res.Data = JsonConvert.SerializeObject(data);
				return res;
			}, true);

			return res;
		}

		/// <summary>
		/// 盤點調整完成
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		private ApiResult ImportInventoryAdjustConfirmResults(string dcCode, string gupCode, string custCode)
		{
			#region 變數設定
			ApiResult res = new ApiResult { IsSuccessed = true };

			var f060405Repo = new F060405Repository(Schemas.CoreSchema);
			var f151001Repo = new F151001Repository(Schemas.CoreSchema);
			var f151002Repo = new F151002Repository(Schemas.CoreSchema);
			var f060404Repo = new F060404Repository(Schemas.CoreSchema);
			var f140101QueryRepo = new F140101Repository(Schemas.CoreSchema);
			var f140106QueryRepo = new F140106Repository(Schemas.CoreSchema);
			var f060406QueryRepo = new F060406Repository(Schemas.CoreSchema);
			int successCnt = 0;
			#endregion

			#region 主要邏輯
			// 取得要執行盤點調整完成結果回傳資料
			var executeDatas = f060405Repo.GetDatasForExecute(dcCode, gupCode, custCode).OrderBy(x=> x.DOC_ID).ToList();

			if (executeDatas.Any())
			{
				var wmsNos = executeDatas.Select(x => x.WMS_NO).ToList();
				var docIds = executeDatas.Select(x => x.DOC_ID).ToList();

				// 取得盤點單盤點差異明細資料
				var f140106s = f140106QueryRepo.GetDatasByInventoryAdjustConfirm(dcCode, gupCode, custCode, wmsNos).ToList();

				// 取得盤點調整完成結果回傳明細資料
				var f060406s = f060406QueryRepo.GetDatasByInventoryAdjustConfirm(dcCode, gupCode, custCode, docIds).ToList();

				var f060406List = f060406s.GroupBy(x => x.DOC_ID).Select(x => new
				{
					DOC_ID = x.Key,
					F060406s = x.ToList()
				}).ToList();

				// 取得調撥單
				var f151001s = f151001Repo.GetDatasBySourceNos(dcCode, gupCode, custCode, wmsNos).ToList();

				// 取得調撥單明細
				var f151002s = f151002Repo.GetDatas(dcCode, gupCode, custCode, f151001s.Select(x => x.ALLOCATION_NO).Distinct().ToList()).ToList();

				// 取得AGV盤點調整任務資料
				var f060404s = f060404Repo.GetDatasByInventoryAdjustConfirm(dcCode, gupCode, custCode, docIds).ToList();

				// 取得盤點單資料
				var f140101s = f140101QueryRepo.GetDatasByInventoryAdjustConfirm(dcCode, gupCode, custCode, wmsNos).ToList();

				// Foreach #取得要執行盤點調整完成結果回傳資料
				executeDatas.ForEach(f060405 =>
				{
					var wmsTransaction = new WmsTransaction();
					var sharedService = new SharedService(wmsTransaction);
					var stockService = new StockService(wmsTransaction);
          var f140101Repo = new F140101Repository(Schemas.CoreSchema, wmsTransaction);
					var f140106Repo = new F140106Repository(Schemas.CoreSchema, wmsTransaction);
					var f060406Repo = new F060406Repository(Schemas.CoreSchema, wmsTransaction);
          sharedService.StockService = stockService;

					var f140101 = f140101s.Where(x => x.INVENTORY_NO == f060405.WMS_NO).FirstOrDefault();

					#region 更新 F060405 處理中狀態
					// 更新執行盤點調整完成結果回傳資料表處理中狀態
					f060405.STATUS = "1";
					f060405.PROC_DATE = DateTime.Now;
					f060405Repo.Update(f060405);
					#endregion

					#region 資料處理
					var currF140106s = f140106s.Where(x => x.INVENTORY_NO == f060405.WMS_NO).ToList();

					// 取得當前AGV盤點調整任務資料
					var currF060404 = f060404s.Where(x => x.DOC_ID == f060405.DOC_ID).FirstOrDefault();

					// 取得當前盤點調整結果回傳資料
					var currF060406s = f060406List.Where(x => x.DOC_ID == f060405.DOC_ID).FirstOrDefault();

					if (currF060406s != null)
					{
						var detailData = currF060406s.F060406s;

						#region 盤損調撥上架
						// 取得當前調撥單
						var currF151001s = f151001s.Where(x => x.SOURCE_NO == f060405.WMS_NO).ToList();

						// 建立調撥單上架
						currF151001s.ForEach(f151001 =>
						{
							// 調撥明細
							var currF151002 = f151002s.Where(x => x.ALLOCATION_NO == f151001.ALLOCATION_NO).FirstOrDefault();

							if (currF151002 != null)
							{
								var currF060406ByAllocNo = detailData.Where(x =>
																currF151002.ITEM_CODE == x.SKUCODE &&
																currF151002.VALID_DATE.ToString("yyyy/MM/dd") == x.EXPIRYDATE &&
																currF151002.MAKE_NO == x.OUTBATCHCODE).ToList();

								//找WMS狀態=需調整、自動倉狀態=需調整
								var currF140106ByAllocNo = f140106s.Where(x =>
								x.PROC_WMS_NO == f151001.ALLOCATION_NO &&
								x.WMS_STATUS == "0" && x.STATUS == "0").ToList();

								if (currF060406ByAllocNo.Any() && currF060406ByAllocNo.All(x => x.STATUS == "1"))
								{
									// 產生調撥上架處理資料
									AllocationConfirmParam data = new AllocationConfirmParam
									{
										DcCode = f151001.DC_CODE,
										GupCode = f151001.GUP_CODE,
										CustCode = f151001.CUST_CODE,
										AllocNo = f151001.ALLOCATION_NO,
										StartTime = Convert.ToDateTime(currF060404.PROC_DATE).ToString("yyyy/MM/dd HH:mm:ss"),
										CompleteTime = f060405.CRT_DATE.ToString("yyyy/MM/dd HH:mm:ss"),
										Operator = currF060404.CRT_STAFF,
										Details = new List<AllocationConfirmDetail>
											{
												new AllocationConfirmDetail
												{
													Seq = currF151002.ALLOCATION_SEQ,
													Qty = Convert.ToInt32(currF151002.TAR_QTY)
												}
											}
									};

									sharedService.AllocationConfirm(data);
									// 新增庫存異動處理
									sharedService.CreateStockaBnormal(dcCode, gupCode, custCode, f151001, new List<F151002> { currF151002 });
									// 新增盤點單盤損
									sharedService.CreateF140107(dcCode, gupCode, custCode, f151001, new List<F151002> { currF151002 });

									//如果AGV回傳有對應到F140106資料(品號 + 效期 + 批號)且調整狀態 = 1(調整成功)
									//取得F140106.PROC_WMS_NO調撥單號，將此調撥單直接上架完成，更新F140106.WMS_STATUS = 1(調整成功),F140106.STATUS = 1(調整成功)
									currF140106ByAllocNo.ForEach(f140106 =>
									{
										f140106.WMS_STATUS = "1";
										f140106.STATUS = "1";
										f140106Repo.Update(f140106);
									});
								}
								else
								{
									if (currF140106ByAllocNo.Any())
									{
										// 呼叫調撥共用函數刪除調撥單
										sharedService.DeleteAllocation(new DeletedAllocationParam
										{
											DcCode = dcCode,
											GupCode = gupCode,
											CustCode = custCode,
											DeleteAllocationType = DeleteAllocationType.AllocationNo,
											OrginalAllocationNo = f151001.ALLOCATION_NO,
										}, false, false);

										//如果自動倉回傳有對應到F140106資料(品號 + 效期 + 批號)且調整狀態 = 0(調整失敗)
										//取得F140106.PROC_WMS_NO調撥單號，將此調撥單取消，更新F140106.WMS_STATUS = 2(調整失敗),F140106.STATUS = 2(調整失敗)
										currF140106ByAllocNo.ForEach(f140106 =>
										{
											f140106.WMS_STATUS = "2";
											f140106.STATUS = "2";
											f140106.PROC_WMS_NO = null;
											f140106Repo.Update(f140106);
										});
									}
								}
							}
						});
						#endregion

						#region 盤盈建立調整單
						// 找出盤盈的資料
						var f140106sByAdjust = new List<F140106>();
						var surplusDatas = new List<F140106>();
						//找WMS狀態=需調整、自動倉狀態=需調整
						if (f140101.ISSECOND == "0")
							surplusDatas = currF140106s.Where(x => x.FIRST_STOCK_DIFF_QTY > 0 && x.FIRST_DIFF_QTY != 0 && x.WMS_STATUS == "0" && x.STATUS == "0").ToList();
						else
							surplusDatas = currF140106s.Where(x => x.SECOND_STOCK_DIFF_QTY > 0 && x.SECOND_DIFF_QTY != 0 && x.WMS_STATUS == "0" && x.STATUS == "0").ToList();

						if(surplusDatas.Any())
						{
							var adjustService = new AdjustOrderService(wmsTransaction);
							var f140107Repo = new F140107Repository(Schemas.CoreSchema, wmsTransaction);
							var f140107s = f140107Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == f140101.DC_CODE && x.GUP_CODE == f140101.GUP_CODE && x.CUST_CODE == f140101.CUST_CODE && x.INVENTORY_NO == f140101.INVENTORY_NO).ToList();
							var addF140107List = new List<F140107>();
							var updF140107List = new List<F140107>();
							var adjustStockDetailList = new List<AdjustStockDetail>();

							surplusDatas.ForEach(f140106 =>
							{
								var currF060406BySurplus = detailData.Where(x =>
								f140106.ITEM_CODE == x.SKUCODE &&
								f140106.VALID_DATE.ToString("yyyy/MM/dd") == x.EXPIRYDATE &&
								f140106.MAKE_NO == x.OUTBATCHCODE).ToList();

								// 成功(F060406.Status=1)才建立調整單
								if (currF060406BySurplus.Any() && currF060406BySurplus.All(x => x.STATUS == "1"))
								{
									var adjQty = Convert.ToInt32(f140106.SECOND_QTY == null ? f140106.FIRST_QTY : f140106.SECOND_QTY) - Convert.ToInt32(f140106.QTY) - (f140106.UNMOVE_STOCK_QTY ?? 0);
									adjustStockDetailList.Add(new AdjustStockDetail
									{
										LocCode = f140106.LOC_CODE,
										ItemCode = f140106.ITEM_CODE,
										ValidDate = f140106.VALID_DATE,
										EnterDate = f140106.ENTER_DATE,
										MakeNo = f140106.MAKE_NO,
										PalletCtrlNo = f140106.PALLET_CTRL_NO,
										BoxCtrlNo = f140106.BOX_CTRL_NO,
										WarehouseId = f140106.WAREHOUSE_ID,
										Cause = "999",
										CasueMemo = "盤盈損調整",
										WORK_TYPE = "0",
										AdjQty = adjQty,
									});

									f140106sByAdjust.Add(f140106);
								}
								else
								{
									//如果自動倉回傳有對應到F140106資料(品號 + 效期 + 批號)且調整狀態 = 0(調整失敗) or 沒對到資料
									//更新F140106.WMS_STATUS = 2(調整失敗),F140106.STATUS = 2(調整失敗)
									f140106.WMS_STATUS = "2";
									f140106.STATUS = "2";
									f140106Repo.Update(f140106);
								}
							});
							if (f140106sByAdjust.Any())
							{
								var adjustOrderParam = new AdjustOrderParam
								{
									DcCode = dcCode,
									GupCode = gupCode,
									CustCode = custCode,
									AdjustType = AdjustType.InventoryStock,
									CheckSerialItem = false,
									SourceType = "18",
									SourceNo = f060405.WMS_NO,
									WorkType = null,
									AdjustStockDetails = adjustStockDetailList
								};

								var result = adjustService.CreateAdjustOrder(adjustOrderParam);
								if (result.IsSuccessed)
								{

									//如果AGV回傳有對應到F140106資料(品號 + 效期 + 批號)且調整狀態 = 1(調整成功)
									//產生調整單，更新F140106.WMS_STATUS = 1(調整成功),F140106.STATUS = 1(調整成功)，F140106.PROC_WMS_NO = 調整單號
									f140106sByAdjust.ForEach(f140106 =>
									{
										f140106.WMS_STATUS = "1";
										f140106.STATUS = "1";
										f140106.PROC_WMS_NO = result.No;

										#region 寫入盤點差異紀錄表F140107
										var adjQty = Convert.ToInt32(f140106.SECOND_QTY == null ? f140106.FIRST_QTY : f140106.SECOND_QTY) - Convert.ToInt32(f140106.QTY) - Convert.ToInt32(f140106.UNMOVE_STOCK_QTY);

										var f140107 = f140107s.FirstOrDefault(o =>
																		o.INVENTORY_NO == f060405.WMS_NO &&
																		o.LOC_CODE == f140106.LOC_CODE &&
																		o.ITEM_CODE == f140106.ITEM_CODE &&
																		o.VALID_DATE == f140106.VALID_DATE &&
																		o.ENTER_DATE == f140106.ENTER_DATE &&
																		o.MAKE_NO == f140106.MAKE_NO &&
																		o.DC_CODE == dcCode &&
																		o.GUP_CODE == gupCode &&
																		o.CUST_CODE == custCode);

										if (f140107 == null)
										{
											// 把初盤/複盤庫差數寫入F140107
											addF140107List.Add(new F140107
											{
												INVENTORY_NO = f060405.WMS_NO,
												WAREHOUSE_ID = f060405.WAREHOUSE_ID,
												LOC_CODE = f140106.LOC_CODE,
												ITEM_CODE = f140106.ITEM_CODE,
												VALID_DATE = f140106.VALID_DATE,
												ENTER_DATE = f140106.ENTER_DATE,
												PROFIT_QTY = adjQty,
												LOSS_QTY = 0,
												FLUSHBACK = f140106.FLUSHBACK,
												DC_CODE = dcCode,
												GUP_CODE = gupCode,
												CUST_CODE = custCode,
												BOX_CTRL_NO = f140106.BOX_CTRL_NO,
												PALLET_CTRL_NO = f140106.PALLET_CTRL_NO,
												MAKE_NO = f140106.MAKE_NO
											});
										}
										else
										{
											f140107.PROFIT_QTY += adjQty;
											updF140107List.Add(f140107);
										}
										#endregion
									});
								}
								else
								{
									f140106sByAdjust.ForEach(f140106 =>
									{
										f140106.WMS_STATUS = "2";
										f140106.STATUS = "2";
										f140106.PROC_WMS_NO = null;
									});
								}
								f140106Repo.BulkUpdate(f140106sByAdjust);
								if (addF140107List.Any())
									f140107Repo.BulkInsert(addF140107List);
								if (updF140107List.Any())
									f140107Repo.BulkUpdate(updF140107List);
							}
						}
						#endregion

						#region 修改F140106 By WMS_STATUS = 3(不調整)，代表庫差數=0，且F140106.STATUS=0(需調整)
						currF140106s.Where(x => x.WMS_STATUS == "3" && x.STATUS == "0").ToList().ForEach(f140106 =>
						{
							var currResultData = detailData.Where(x =>
																f140106.ITEM_CODE == x.SKUCODE &&
																f140106.VALID_DATE.ToString("yyyy/MM/dd") == x.EXPIRYDATE &&
																f140106.MAKE_NO == x.OUTBATCHCODE).ToList();

							//F140106.WMS_STATUS = 3(不調整)，代表庫差數 = 0，且F140106.STATUS = 0(待調整)
							//1.如果AGV回傳有對應到F140106資料(品號 + 效期 + 批號)且調整狀態 = 1(調整成功):更新F140106.STATUS = 1(調整成功)
							//2.如果AGV回傳有對應到F140106資料(品號 + 效期 + 批號)且調整狀態 = 0(調整失敗):更新F140106.STATUS = 2(調整失敗)
							//3.如果F140106.STATUS = 0(待調整)，且AGV回傳無對應到資料 更新F140106.STATUS = 2(調整失敗)
							f140106.STATUS = currResultData.Any() && currResultData.All(x => x.STATUS == "1") ? "1" : "2";
							f140106Repo.Update(f140106);
						});
						#endregion
					}
					else
					{
						#region 更新F140106
						currF140106s.ForEach(f140106 =>
						{
							if (f140106.WMS_STATUS == "0")
							{
								if (!string.IsNullOrEmpty(f140106.PROC_WMS_NO))
								{
									// 呼叫調撥共用函數刪除調撥單
									sharedService.DeleteAllocation(new DeletedAllocationParam
									{
										DcCode = dcCode,
										GupCode = gupCode,
										CustCode = custCode,
										DeleteAllocationType = DeleteAllocationType.AllocationNo,
										OrginalAllocationNo = f140106.PROC_WMS_NO,
									}, false, false);
								}
								f140106.PROC_WMS_NO = null;
								f140106.WMS_STATUS = "2";
							}
							if (f140106.STATUS == "0")
								f140106.STATUS = "2";
						});
						f140106Repo.BulkUpdate(currF140106s);
						#endregion
					}

					#region 更新F140101
					if (f140101 != null)
					{
						//如果有找到盤點單 更新F140101.STATUS = 5(結案) F140101.POSTING_DATE = 系統日
						f140101.STATUS = "5";
						f140101.POSTING_DATE = DateTime.Now;
						f140101Repo.Update(f140101);
					}
          #endregion
          stockService.SaveChange();
					wmsTransaction.Complete();
					successCnt++;
					#endregion

					#region 更新 F060402 完成、錯誤、逾時狀態
					f060405.STATUS = "2";
					f060405Repo.Update(f060405);
					#endregion
				});
			}

			int failCnt = executeDatas.Count - successCnt;
			res.MsgCode = "10005";
			res.MsgContent = string.Format(_tacService.GetMsg("10005"), "盤點調整完成結果回傳", successCnt, failCnt, executeDatas.Count);
			res.TotalCnt = executeDatas.Count;
			res.SuccessCnt = successCnt;
			res.FailureCnt = failCnt;
			#endregion

			return res;
		}
	}
}
