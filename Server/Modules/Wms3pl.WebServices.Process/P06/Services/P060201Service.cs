
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P06.Services
{
	public partial class P060201Service
	{
		private WmsTransaction _wmsTransaction;
		public P060201Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		

		public ExecuteResult UpdateF051206(List<F051206LackList> data, string userId)
		{
			var f051206Repo = new F051206Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050802Repo = new F050802Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0003Repo = new F0003Repository(Schemas.CoreSchema, _wmsTransaction);
			var f191302Repo = new F191302Repository(Schemas.CoreSchema, _wmsTransaction);
			var sharedService = new SharedService(_wmsTransaction);
			var updF051206s = new List<F051206>();
			var updF051202s = new List<F051202>();
			var updF1511s = new List<F1511>();
			var updF050802s = new List<F050802>();
			var addF191302s = new List<F191302>();
			var returnAllotList = new List<ReturnNewAllocation>();
			var returnStocks = new List<F1913>();
			var group = data.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.WMS_ORD_NO }).ToList();

			foreach (var items in group)
			{
        // 取得疑似遺失倉倉庫編號
        var pickLossWHId = sharedService.GetPickLossWarehouseId(items.Key.DC_CODE, items.Key.GUP_CODE, items.Key.CUST_CODE);
        if (string.IsNullOrWhiteSpace(pickLossWHId))
				{
					var msg = string.Format(Shared.Properties.Resources.PickLossWHIdNotSetting, items.Key.DC_CODE);
					return new ExecuteResult(false, msg);
				}
				// 疑似遺失倉第一個儲位
				var pickLossLocCode = sharedService.GetPickLossLoc(items.Key.DC_CODE, pickLossWHId);

				var f051206s = f051206Repo.AsForUpdate().GetNotDeleteDatasByWmsOrdNo(items.Key.DC_CODE, items.Key.GUP_CODE, items.Key.CUST_CODE, items.Key.WMS_ORD_NO).ToList();
				var f051202s = f051202Repo.AsForUpdate().GetDatasByWmsOrdNo(items.Key.DC_CODE, items.Key.GUP_CODE, items.Key.CUST_CODE, items.Key.WMS_ORD_NO).ToList();
				var f1511s = f1511Repo.AsForUpdate().GetDatasByWmsOrdNo(items.Key.DC_CODE, items.Key.GUP_CODE, items.Key.CUST_CODE, items.Key.WMS_ORD_NO).ToList();
				var f050802s = f050802Repo.AsForUpdate().GetDatas(items.Key.DC_CODE, items.Key.GUP_CODE, items.Key.CUST_CODE, items.Key.WMS_ORD_NO).ToList();
				foreach (var f051206Lack in items)
				{
					var f051206 = f051206s.FirstOrDefault(x => x.LACK_SEQ == f051206Lack.LACK_SEQ);
					if(f051206== null)
						return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.DataDelete };

					f051206.RETURN_FLAG = f051206Lack.RETURN_FLAG;
					f051206.LACK_QTY = f051206Lack.LACK_QTY;
					f051206.REASON = f051206Lack.REASON;
					f051206.MEMO = f051206Lack.REASON != "999" ? "" : f051206Lack.MEMO;
					f051206.STATUS = f051206.STATUS == "0" ? "1" : ((string.IsNullOrWhiteSpace(f051206.RETURN_FLAG)) ? "1" : "2");


					var f051202 = f051202s.First(x => x.PICK_ORD_NO == f051206.PICK_ORD_NO && x.PICK_ORD_SEQ == f051206.PICK_ORD_SEQ);
					var f1511 = f1511s.First(x => x.ORDER_NO == f051206.PICK_ORD_NO && x.ORDER_SEQ == f051206.PICK_ORD_SEQ);
					// 找到商品
					if (f051206.RETURN_FLAG == "3")
					{
						f051202.A_PICK_QTY = f051202.B_PICK_QTY;
						f1511.A_PICK_QTY = f1511.B_PICK_QTY;
					}
					// 缺品出貨
					if(f051206.RETURN_FLAG == "1")
					{
						f051202.A_PICK_QTY = f051202.B_PICK_QTY - f051206.LACK_QTY ?? 0;
						f1511.A_PICK_QTY = f1511.B_PICK_QTY - f051206.LACK_QTY ?? 0;

						var res = sharedService.CreateStockLackProcess(new StockLack
						{
							DcCode = f051206.DC_CODE,
							GupCode = f051206.GUP_CODE,
							CustCode = f051206.CUST_CODE,
							LackQty = f051206.LACK_QTY??0,
							F051202 = f051202,
							F1511 = f1511,
							ReturnStocks = returnStocks,
						  PickLackWarehouseId = pickLossWHId,
							PickLackLocCode = pickLossLocCode
						});
						if (!res.IsSuccessed)
							return new ExecuteResult(res.IsSuccessed, res.Message);
						else
						{
							returnStocks = res.ReturnStocks;
							returnAllotList.AddRange(res.ReturnNewAllocations);
							f051202 = res.UpdF051202;
							f1511 = res.UpdF1511;
							addF191302s.AddRange(res.AddF191302List);
						}
					}
					updF051206s.Add(f051206);
					updF051202s.Add(f051202);
					updF1511s.Add(f1511);
				}
				var groupWmsSeqs = f051202s.GroupBy(x => new { x.WMS_ORD_SEQ }).ToList();
				foreach(var wmsSeqs in groupWmsSeqs)
				{
					var f050802 = f050802s.First(x => x.WMS_ORD_SEQ == wmsSeqs.Key.WMS_ORD_SEQ);
					var delvQty = wmsSeqs.Where(x=> x.PICK_STATUS!="9").Sum(x => (x.PICK_STATUS == "0") ? x.B_PICK_QTY : x.A_PICK_QTY);
					// 出貨數有異動才更新
					if (f050802.B_DELV_QTY != delvQty)
					{
						f050802.B_DELV_QTY = delvQty;
						updF050802s.Add(f050802);
					}
				}
			}

			if(returnAllotList.Any())
			{
                // 調撥單整批上架
                sharedService.BulkAllocationToAllUp(returnAllotList, returnStocks, false, addF191302s);
                sharedService.BulkInsertAllocation(returnAllotList, returnStocks, true);
			}

			if (addF191302s.Any())
				f191302Repo.BulkInsert(addF191302s);

			if (updF050802s.Any())
				f050802Repo.BulkUpdate(updF050802s);

			if (updF051206s.Any())
				f051206Repo.BulkUpdate(updF051206s);

			if (updF051202s.Any())
				f051202Repo.BulkUpdate(updF051202s);
			if (updF1511s.Any())
				f1511Repo.BulkUpdate(updF1511s);

			return new ExecuteResult(true);
		}


		public ExecuteResult DeleteF051206(List<F051206LackList> data, string userId)
		{
			var repo = new F051206Repository(Schemas.CoreSchema, _wmsTransaction);
			foreach (var f051206LackList in data)
			{
				f051206LackList.RETURN_FLAG = "3";
			}
			var result = UpdateF051206(data, userId);
			if (!result.IsSuccessed)
				return result;
			var updF051206List = new List<F051206>();
			foreach (var p in data)
			{
				var tmp = repo.Find(x => x.LACK_SEQ == p.LACK_SEQ);
				if (tmp == null) return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.DataDelete };

				tmp.ISDELETED = "1";
				tmp.REASON = p.REASON;
				tmp.MEMO = p.REASON != "999" ? "" : p.MEMO;
				tmp.UPD_DATE = DateTime.Now;
				tmp.UPD_STAFF = userId;
				updF051206List.Add(tmp);
			}

			repo.BulkUpdate(updF051206List);
			return new ExecuteResult() { IsSuccessed = true };
		}

		public ExecuteResult InsertF051206(List<F051206LackList> data, string userId)
		{
			var repo = new F051206Repository(Schemas.CoreSchema, _wmsTransaction);
			var f052901repo = new F052901Repository(Schemas.CoreSchema);
			if (data.Any())
			{
				var f052901Datas = f052901repo.GetDataByPickOrdNo(data.First().DC_CODE, data.First().GUP_CODE,
					data.First().CUST_CODE, data.First().PICK_ORD_NO);
				if (f052901Datas.Any())
					return new ExecuteResult(false, Properties.Resources.P060201Service_f052901DatasHasData);
				var f050801Repo = new F050801Repository(Schemas.CoreSchema);
				var f050801 = f050801Repo.Find(x => x.DC_CODE == data.First().DC_CODE && x.GUP_CODE == data.First().GUP_CODE && x.CUST_CODE == data.First().CUST_CODE && x.WMS_ORD_NO == data.First().WMS_ORD_NO);

				var f055001Repo = new F055001Repository(Schemas.CoreSchema);
				var f055001s = f055001Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == data.First().DC_CODE && x.GUP_CODE == data.First().GUP_CODE && x.CUST_CODE == data.First().CUST_CODE && x.WMS_ORD_NO == data.First().WMS_ORD_NO).ToList();
				// 出貨包裝狀態大於0 或是包裝資料已存在 則不可以設定揀缺
				if ((f050801 != null && f050801.STATUS != 0) || f055001s.Any())
					return new ExecuteResult(false, Properties.Resources.WmsNoMustUnPackagedToAddLack);

			}

			var addF051206List = new List<F051206>();

			foreach (var p in data)
			{
				var item = repo.GetExistsNoApproveData(p.DC_CODE, p.GUP_CODE, p.CUST_CODE, p.PICK_ORD_NO, p.PICK_ORD_SEQ);
				if (item != null)
					return new ExecuteResult(false, Properties.Resources.P060201Service_WMS_ORD_NO + p.WMS_ORD_NO + Environment.NewLine + Properties.Resources.P060201Service_PICK_ORD_NO +
							p.PICK_ORD_NO + Environment.NewLine + Properties.Resources.P060201Service_ITEM_CODE + p.ITEM_CODE + Environment.NewLine + Properties.Resources.P060201Service_PICK_ORD_SEQ + p.PICK_ORD_SEQ + Environment.NewLine + Properties.Resources.P060201Service_OutofStockRecordExist);

				var approveHasLackQty = repo.GetApproveHasLackDatas(p.DC_CODE, p.GUP_CODE, p.CUST_CODE, p.PICK_ORD_NO, p.PICK_ORD_SEQ).Sum(x => x.LACK_QTY);
				if (p.B_PICK_QTY - approveHasLackQty < p.LACK_QTY)
				{
					return new ExecuteResult(false, Properties.Resources.P060201Service_WMS_ORD_NO + p.WMS_ORD_NO + Environment.NewLine + Properties.Resources.P060201Service_PICK_ORD_NO +
			p.PICK_ORD_NO + Environment.NewLine + Properties.Resources.P060201Service_ITEM_CODE + p.ITEM_CODE + Environment.NewLine + Properties.Resources.P060201Service_PICK_ORD_SEQ + p.PICK_ORD_SEQ + Environment.NewLine + string.Format(Properties.Resources.P060201Service_LackQtyOverMustQty, p.B_PICK_QTY, approveHasLackQty, p.LACK_QTY));
				}



				var tmp = new F051206
				{
					WMS_ORD_NO = p.WMS_ORD_NO,
					PICK_ORD_NO = p.PICK_ORD_NO,
					PICK_ORD_SEQ = p.PICK_ORD_SEQ,
					ITEM_CODE = p.ITEM_CODE,
					LACK_QTY = p.LACK_QTY,
					REASON = p.REASON,
					MEMO = p.REASON != "999" ? "" : p.MEMO,
					STATUS = "1", //貨主待確認
					RETURN_FLAG = p.RETURN_FLAG,
					CUST_CODE = p.CUST_CODE,
					GUP_CODE = p.GUP_CODE,
					DC_CODE = p.DC_CODE,
					CRT_STAFF = userId,
					CRT_DATE = p.CRT_DATE,
					CRT_NAME = p.CRT_NAME,
					ISDELETED = "0",
					LOC_CODE = p.LOC_CODE,
					TRANS_FLAG = "0"
				};
				addF051206List.Add(tmp);

			}
			repo.BulkInsert(addF051206List);
			return new ExecuteResult() { IsSuccessed = true };
		}

		public ExecuteResult InsertF151003AndUpdateF151002WithF1511(List<F051206LackList_Allot> data, string userId)
		{
			var f151003Repo = new F151003Repository(Schemas.CoreSchema, _wmsTransaction);
			var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);

			// 找出已存在調撥單資料
			var f151002s = f151002Repo.GetDataByF051206LackList_Allots(data);

			// 找出已存在虛擬儲位資料
			var f1511s = f1511Repo.GetDatasByF051206LackListAllot(data);

			// 先找出已存在的缺貨資料
			var sourceF151003 = f151003Repo.GetDatasByF051206LackListAllot(data, new List<string> { "0", "2" });

			List<string> errorList = new List<string>();

			foreach (var item in data)
			{
				List<string> currErrorList = new List<string>();

				// 該項次的調撥明細
				var currF151002 = f151002s.Where(x => x.DC_CODE == item.DC_CODE && x.GUP_CODE == item.GUP_CODE && x.CUST_CODE == item.CUST_CODE && x.ALLOCATION_NO == item.ALLOCATION_NO && x.ALLOCATION_SEQ == item.ALLOCATION_SEQ).FirstOrDefault();

				if (currF151002 != null)
				{
					#region 驗證
					// 該項次的已存在缺貨資料
					var currF151003 = sourceF151003.Where(x => x.ALLOCATION_NO == item.ALLOCATION_NO && x.ALLOCATION_SEQ == item.ALLOCATION_SEQ);

					// 扣除畫面上的缺貨數
					var currSourceLackQty = currF151003.Sum(x => x.LACK_QTY);

					// 缺貨數超過總調撥數
					var scrQty = currF151002.SRC_QTY - item.LACK_QTY - currSourceLackQty;
					if (scrQty < 0)
						currErrorList.Add($"缺貨數超過下架數 { Math.Abs(scrQty) } ");

					// 缺貨數超過總調撥數
					//var tarQty = currF151002.TAR_QTY - item.LACK_QTY - currSourceLackQty;
					//if (tarQty < 0)
					//    currErrorList.Add($"缺貨數超過上架數 { Math.Abs(tarQty) } ");

					string errorStr = string.Join("、", currErrorList);

					if (!string.IsNullOrWhiteSpace(errorStr))
					{
						errorList.Add($"調撥項次：{item.ALLOCATION_SEQ.ToString().PadLeft(3, '0')} {errorStr}");
						continue;
					}
					#endregion

					#region Insert F151003
					f151003Repo.Add(new F151003
					{
						ALLOCATION_NO = item.ALLOCATION_NO,
						ALLOCATION_SEQ = item.ALLOCATION_SEQ,
						ITEM_CODE = item.ITEM_CODE,
						MOVE_QTY = item.SRC_QTY,
						LACK_QTY = item.LACK_QTY,
						REASON = item.REASON,
						MEMO = item.REASON != "999" ? "" : item.MEMO,
						STATUS = "0",
						CUST_CODE = item.CUST_CODE,
						GUP_CODE = item.GUP_CODE,
						DC_CODE = item.DC_CODE,
						LACK_TYPE = "0",
						CRT_STAFF = userId,
						CRT_DATE = DateTime.Now,
						CRT_NAME = userId
					});
					#endregion

					#region Update F151002
					currF151002.A_SRC_QTY = scrQty;
					currF151002.TAR_QTY = currF151002.A_SRC_QTY;
					f151002Repo.Update(currF151002);
					#endregion

					#region Update F1511
					var f1511 = f1511s.Where(x => x.DC_CODE == item.DC_CODE &&
									x.GUP_CODE == item.GUP_CODE &&
									x.CUST_CODE == item.CUST_CODE &&
									x.ORDER_NO == item.ALLOCATION_NO &&
									x.ORDER_SEQ == item.ALLOCATION_SEQ.ToString()).FirstOrDefault();

					if (f1511 != null)
					{
						// 更新 F1511.A_PICK_QTY = A_SRC_QTY by order_no=調撥單號+order_seq=調撥單明細
						f1511.A_PICK_QTY = Convert.ToInt32(currF151002.A_SRC_QTY);

						f1511Repo.Update(f1511);
					}

					#endregion
				}
			}

			if (errorList.Any())
				return new ExecuteResult { IsSuccessed = false, Message = string.Join("\r\n", errorList) };
			else
				return new ExecuteResult { IsSuccessed = true };
		}

		public ExecuteResult UpdateF151003(List<F051206LackList_Allot> data, string userId)
		{
			var allNos = data.Select(x => x.ALLOCATION_NO).Distinct().ToList();
			var repo = new F151003Repository(Schemas.CoreSchema, _wmsTransaction);
			var f151001repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f151002repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1913repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			var f191204repo = new F191204Repository(Schemas.CoreSchema, _wmsTransaction);
			List<F151001> updF151001s = new List<F151001>();
			List<F151002> updF151002s = new List<F151002>();
			var f151002s = f151002repo.GetAllocNoDataByLackSeqs(data.Select(x => x.LACK_SEQ).ToList());

			foreach (var p in data)
			{
				var tmp = repo.Find(x => x.LACK_SEQ == p.LACK_SEQ);
				if (tmp == null) return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.DataDelete };

				#region 若結案(已確認缺貨) ,更新F1511狀態 , 復原 f1913庫存量 , 更新 F151001 STATUS , LOCK_STATUS
				if (p.STATUS == "2")
				{
					var f151002ItemData = f151002s.Where(x => x.DC_CODE == tmp.DC_CODE && x.GUP_CODE == tmp.GUP_CODE && x.CUST_CODE == tmp.CUST_CODE
											&& x.ALLOCATION_NO == tmp.ALLOCATION_NO && x.ALLOCATION_SEQ == tmp.ALLOCATION_SEQ).First();
					if (f151002ItemData != null)
					{
						var f1511ItemData = f1511repo.Find(x => x.DC_CODE == tmp.DC_CODE && x.GUP_CODE == tmp.GUP_CODE && x.CUST_CODE == tmp.CUST_CODE
										&& x.ORDER_NO == f151002ItemData.ALLOCATION_NO && x.ITEM_CODE == f151002ItemData.ITEM_CODE
										&& x.ORDER_SEQ == f151002ItemData.ALLOCATION_SEQ.ToString());

						if (f1511ItemData != null)
						{
							f1511ItemData.STATUS = "1";
							f1511repo.Update(f1511ItemData);
						}


						// 更新 F151001 STATUS , LOCK_STATUS
						var f151001ItemData = f151001repo.Find(x => x.DC_CODE == tmp.DC_CODE && x.GUP_CODE == tmp.GUP_CODE && x.CUST_CODE == tmp.CUST_CODE
											&& x.ALLOCATION_NO == tmp.ALLOCATION_NO);
						if (f151001ItemData != null)
						{
							var f191204s = f191204repo.AsForUpdate().GetDatasByTrueAndCondition(o => o.DC_CODE == tmp.DC_CODE && o.GUP_CODE == tmp.GUP_CODE && o.CUST_CODE == tmp.CUST_CODE && o.ALLOCATION_NO == tmp.ALLOCATION_NO).ToList();
							f191204s.ForEach(o => o.STATUS = "1");
							f191204repo.BulkUpdate(f191204s);

							bool isUP = false; //是否為上架
							if (f151001ItemData.STATUS == "3") //上架
								isUP = true;

							var f151002AllDatas = f151002s.Where(x => x.DC_CODE == tmp.DC_CODE && x.GUP_CODE == tmp.GUP_CODE && x.CUST_CODE == tmp.CUST_CODE && x.ALLOCATION_NO == tmp.ALLOCATION_NO);
							//找出若下架是否有商品未完成下架處理 ; 上架是否有商品未完成上架處理
							if (f151002AllDatas.Where(o => (isUP && o.STATUS != "2") || (!isUP && o.STATUS != "1")).Count() == 0)
							{
								if (isUP)
								{
									f151001ItemData.STATUS = "5";
									f151001ItemData.LOCK_STATUS = "4";
								}
								else
								{
									f151001ItemData.STATUS = "3";
									f151001ItemData.LOCK_STATUS = "2";
								}

								updF151001s.Add(f151001ItemData);
							}
						}

						if (f151002ItemData.TAR_QTY == 0)
							// 當缺貨類型=0(下架)，-更新F151002.status = 1(下架完成上架未處理)
							f151002ItemData.STATUS = "2";
						else
							if (tmp.LACK_TYPE == "0")
							f151002ItemData.STATUS = "1";

						f151002repo.Update(f151002ItemData);

					}
				}
				#endregion

				#region 若已找到商品時 ,將 F151001 , F151002
				//F151001 LockSatus (下架 :0  ; 上架 :1) 
				//F151002 下架 Status :0 TAR_QTY = SRC_QTY ; 上架 Status = 1

				if (p.STATUS == "3")
				{
					var f151002ItemData = f151002s.Where(x => x.DC_CODE == tmp.DC_CODE && x.GUP_CODE == tmp.GUP_CODE && x.CUST_CODE == tmp.CUST_CODE
											&& x.ALLOCATION_NO == tmp.ALLOCATION_NO && x.ALLOCATION_SEQ == tmp.ALLOCATION_SEQ).First();
					if (f151002ItemData != null)
					{
						if (f151002ItemData.STATUS != "0")
						{
							//更新F151001
							var f151001ItemData = f151001repo.Find(x => x.DC_CODE == tmp.DC_CODE && x.GUP_CODE == tmp.GUP_CODE && x.CUST_CODE == tmp.CUST_CODE
												&& x.ALLOCATION_NO == tmp.ALLOCATION_NO);
							if (f151001ItemData != null)
							{
								if (f151001ItemData.LOCK_STATUS == "1") //下架
								{
									f151001ItemData.LOCK_STATUS = "0";
								}
								else if (f151001ItemData.STATUS == "3") //上架
								{
									f151001ItemData.LOCK_STATUS = "2";
								}
								updF151001s.Add(f151001ItemData);
							}

							//更新F151002
							if (f151002ItemData.STATUS == "1") //下架
							{
								f151002ItemData.STATUS = "0";
								f151002ItemData.TAR_QTY = f151002ItemData.SRC_QTY;
							}
							else if (f151002ItemData.STATUS == "2") //上架
							{
								f151002ItemData.STATUS = "1";
							}
						}
						// 更新 F151002.A_SRC_QTY += 畫面上的缺貨數by 該調撥單明細
						// 更新 F151002.TAR_QTY = A_SRC_QTY
						// 更新 F1511.A_PICK_QTY = A_PICK_QTY
						// 更新F151002.status = 1(下架完成上架未處理)
						// 更新F1511.status = 1
						if (tmp.LACK_TYPE == "0")
						{
							f151002ItemData.STATUS = "1";
							f151002ItemData.A_SRC_QTY += tmp.LACK_QTY;
							f151002ItemData.TAR_QTY = f151002ItemData.A_SRC_QTY;

							var f1511ItemData = f1511repo.Find(x => x.DC_CODE == tmp.DC_CODE && x.GUP_CODE == tmp.GUP_CODE && x.CUST_CODE == tmp.CUST_CODE
										&& x.ORDER_NO == f151002ItemData.ALLOCATION_NO && x.ITEM_CODE == f151002ItemData.ITEM_CODE
										&& x.ORDER_SEQ == f151002ItemData.ALLOCATION_SEQ.ToString());

							if (f1511ItemData != null)
							{
								f1511ItemData.A_PICK_QTY = Convert.ToInt32(f151002ItemData.A_SRC_QTY);
								f1511ItemData.STATUS = "1";
								f1511repo.Update(f1511ItemData);
							}
						}

						f151002repo.Update(f151002ItemData);
					}
				}
				#endregion

				tmp.STATUS = p.STATUS;
				tmp.LACK_QTY = p.LACK_QTY;
				tmp.REASON = p.REASON;
				tmp.MEMO = p.REASON != "999" ? "" : p.MEMO;
				tmp.UPD_DATE = DateTime.Now;
				tmp.UPD_STAFF = userId;
				repo.Update(tmp);

			}

			#region UpdateF151001
			updF151001s = updF151001s.Distinct().ToList();

			var f151002List = f151002s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.ALLOCATION_NO }).Select(x => new
			{
				DcCode = x.Key.DC_CODE,
				GupCode = x.Key.GUP_CODE,
				CustCode = x.Key.CUST_CODE,
				AllocNo = x.Key.ALLOCATION_NO,
				StatusList = x.Select(z => z.STATUS).ToList(),
				ASrcQtyList = x.Select(z => z.A_SRC_QTY).ToList()
			}).ToList();

			f151002List.ForEach(o =>
			{
				var updF151001 = updF151001s.Where(x => x.DC_CODE == o.DcCode && x.GUP_CODE == o.GupCode && x.CUST_CODE == o.CustCode && x.ALLOCATION_NO == o.AllocNo).FirstOrDefault();

				if (updF151001 == null)
				{
					var f151001 = f151001repo.Find(x => x.DC_CODE == o.DcCode && x.GUP_CODE == o.GupCode && x.CUST_CODE == o.CustCode && x.ALLOCATION_NO == o.AllocNo);

					if (f151001 != null)
					{
						if (o.StatusList.All(x => x == "2") && o.ASrcQtyList.All(x => x == 0))
							f151001.STATUS = "5";// 檢查該調撥單明細是否全缺，若全缺則結案
						else if (o.StatusList.All(x => x == "1"))
							f151001.STATUS = "3";// 檢查該調撥單所有明細狀態都等於F151002.status = 1(下架完成上架未處理)，則更新F151001.status = 3(已下架處理)

						updF151001s.Add(f151001);
					}
				}
				else
				{
					if (o.StatusList.All(x => x == "2") && o.ASrcQtyList.All(x => x == 0))
						updF151001.STATUS = "5";// 檢查該調撥單明細是否全缺，若全缺則結案
					else if (o.StatusList.All(x => x == "1"))
						updF151001.STATUS = "3";// 檢查該調撥單所有明細狀態都等於F151002.status = 1(下架完成上架未處理)，則更新F151001.status = 3(已下架處理)
				}
			});

			if (updF151001s.Any())
				f151001repo.BulkUpdate(updF151001s);

			#endregion
			return new ExecuteResult() { IsSuccessed = true };
		}

		public ExecuteResult DeleteF151003(List<F051206LackList_Allot> data, string userId)
		{
			var repo = new F151003Repository(Schemas.CoreSchema, _wmsTransaction);
			foreach (var f051206LackListAllot in data)
			{
				f051206LackListAllot.STATUS = "3";
			}
			UpdateF151003(data, userId);
			foreach (var p in data)
			{
				var tmp = repo.Find(x => x.LACK_SEQ == p.LACK_SEQ);
				if (tmp == null) return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.DataDelete };

				tmp.STATUS = "9";
				tmp.UPD_DATE = DateTime.Now;
				tmp.UPD_STAFF = userId;

				repo.Update(tmp);
			}
			return new ExecuteResult() { IsSuccessed = true };
		}

		public ExecuteResult ModifyLackQty(string dcCode, string gupCode, string custCode, int lackSeq, string pickOrdNo, string pickOrdSeq, int lackQty)
		{
			var repo = new F051206Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051206 = repo.Find(a => a.LACK_SEQ == lackSeq && a.DC_CODE == dcCode && a.GUP_CODE == gupCode && a.CUST_CODE == custCode, true);
			var f051202 = f051202Repo.Find(a => a.PICK_ORD_NO == pickOrdNo && a.PICK_ORD_SEQ == pickOrdSeq && a.DC_CODE == dcCode && a.GUP_CODE == gupCode && a.CUST_CODE == custCode, true);
			var f1511 = f1511Repo.Find(a => a.ORDER_NO == pickOrdNo && a.ORDER_SEQ == pickOrdSeq && a.DC_CODE == dcCode && a.GUP_CODE == gupCode && a.CUST_CODE == custCode, true);
			var diffQty = f051206.LACK_QTY.Value - lackQty;

      #region 資料檢查
      if (f051206.STATUS == "2" && new[] { "3", "1" }.Contains(f051206.RETURN_FLAG))
        return new ExecuteResult { IsSuccessed = false, Message = "該筆已結案不可進行找到商品" };

      if (f051206.STATUS == "2" && new[] { "2" }.Contains(f051206.RETURN_FLAG))
        return new ExecuteResult { IsSuccessed = false, Message = "該筆已結案不可進行找到商品，揀缺數系統已搬到疑似遺失倉，若要找到商品，請改用[庫存異動處理]功能處理" };
      #endregion 資料檢查

      if (f051206.LACK_QTY.Value != lackQty && lackQty != 0)
			{
				var f051206New = new F051206
				{
					ALLOCATION_NO = f051206.ALLOCATION_NO,
					ALLOCATION_SEQ = f051206.ALLOCATION_SEQ,
					CUST_ORD_NO = f051206.CUST_ORD_NO,
					ORD_NO = f051206.ORD_NO,
					TRANS_DATE = f051206.TRANS_DATE,
					TRANS_NAME = f051206.TRANS_NAME,
					TRANS_STAFF = f051206.TRANS_STAFF,
					WMS_ORD_NO = f051206.WMS_ORD_NO,
					PICK_ORD_NO = f051206.PICK_ORD_NO,
					PICK_ORD_SEQ = f051206.PICK_ORD_SEQ,
					ITEM_CODE = f051206.ITEM_CODE,
					LACK_QTY = diffQty,
					REASON = f051206.REASON,
					MEMO = f051206.MEMO,
					STATUS = "2", //已確認
					RETURN_FLAG = "3", //找到商品
					CUST_CODE = f051206.CUST_CODE,
					GUP_CODE = f051206.GUP_CODE,
					DC_CODE = f051206.DC_CODE,
					CRT_STAFF = Current.Staff,
					CRT_DATE = DateTime.Now,
					CRT_NAME = Current.StaffName,
					ISDELETED = "0",
					LOC_CODE = f051206.LOC_CODE,
					TRANS_FLAG = f051206.TRANS_FLAG
				};
				repo.Add(f051206New);

				f051206.LACK_QTY = lackQty;
				repo.Update(f051206);
			}
			else if (lackQty == 0)
			{
				f051206.RETURN_FLAG = "3"; //找到商品
				f051206.STATUS = "2"; //已確認
				repo.Update(f051206);
			}

			f051202.A_PICK_QTY += diffQty;
			f051202Repo.Update(f051202);

			f1511.A_PICK_QTY += diffQty;
			f1511Repo.Update(f1511);

			return new ExecuteResult { IsSuccessed = true };
		}

	}
}
