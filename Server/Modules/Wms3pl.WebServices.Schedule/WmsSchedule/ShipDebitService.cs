using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Common.Extensions;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Schedule.WmsSchedule
{
	public class ShipDebitService
	{
		private int _errorCnt;
		private List<string> _msgList;

		public ShipDebitService()
		{

		}

		public ApiResult ExecShipOrderDebit()
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			var canDebitShipOrders = f050801Repo.GetCanDebitShipOrders().ToList();
			if (!canDebitShipOrders.Any())
				return new ApiResult { IsSuccessed = true, MsgCode = "200", MsgContent = "無需扣帳出貨單資料，不需處理" };

			var res = ApiLogHelper.CreateApiLogInfo("0", "0", "0", "ShipDebit", canDebitShipOrders, () =>
				 {
					 _errorCnt = 0;
					 _msgList = new List<string>();
					 foreach (var canDebitShipOrder in canDebitShipOrders)
					 {
						 ShipOrderDebit(canDebitShipOrder);
					 }

					 return new ApiResult
					 {
						 IsSuccessed = _errorCnt == 0,
						 MsgCode = _errorCnt == 0 ? "200" : "99999",
						 MsgContent = string.Join(Environment.NewLine, _msgList)
					 };
				 }, true);
			return res;
		}

		public void ShipOrderDebit(CanDebitShipOrder canDebitShipOrder)
		{
			try
			{
				var wmsTransaton = new WmsTransaction();
				var orderService = new OrderService(wmsTransaton);
				var result = orderService.OneShipOrderDebit(canDebitShipOrder.DC_CODE, canDebitShipOrder.GUP_CODE, canDebitShipOrder.CUST_CODE, canDebitShipOrder.WMS_ORD_NO);
				if (!result.IsSuccessed)
				{
					_errorCnt++;
					_msgList.Add(string.Format("出貨單:{0} 出貨扣帳失敗，原因:{1}", canDebitShipOrder.WMS_ORD_NO, result.Message));
				}
				else
				{
					wmsTransaton.Complete();
					_msgList.Add(string.Format("出貨單:{0} 出貨扣帳成功", canDebitShipOrder.WMS_ORD_NO));
				}
			}
			catch (Exception ex)
			{
				_errorCnt++;
				_msgList.Add(string.Format("出貨單:{0} 出貨扣帳失敗，原因:{1}", canDebitShipOrder.WMS_ORD_NO, ex.Message));
			}
		}

		/// <summary>
		/// 跨庫調撥出貨分配扣帳排程
		/// </summary>
		/// <returns></returns>
		public ApiResult ExecMoveOutShipOrderDebit()
		{
			//Step1 揀貨單處理
			var f0534Repo = new F0534Repository(Schemas.CoreSchema);
			var f0535Repo = new F0535Repository(Schemas.CoreSchema);
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);

			var res = ApiLogHelper.CreateApiLogInfo("0", "0", "0", "MoveOutShipOrderDebit", null, () =>
			{
				_errorCnt = 0;
				_msgList = new List<string>();
				//(1)	[A]=取得揀貨單所有容器已放入跨庫箱尚未分配的揀貨單
				var notAllotPickList = f0534Repo.GetNotAllotPick().ToList();

				//開始針對每一張揀貨單處理Foreach [A1] in [A]，一筆揀貨單處理完就db commit
				foreach (var notAllotPick in notAllotPickList)
				{
					MoveOutShipAllot(notAllotPick);
				}
				// Step2 跨庫出貨單扣帳處理
				var notDebitOrderGroupList = f0535Repo.GetNotDebitOrder()
											.GroupBy(g => new { g.DC_CODE, g.GUP_CODE, g.CUST_CODE })
											.ToList();

				foreach (var notDebitOrderGroup in notDebitOrderGroupList)
				{
					//A.	[K] = 取得出貨單資料
					var f050801s = f050801Repo.GetDatasForWmsOrdNos(notDebitOrderGroup.Key.DC_CODE
																			, notDebitOrderGroup.Key.GUP_CODE
																			, notDebitOrderGroup.Key.CUST_CODE
																			, notDebitOrderGroup.Select(s => s.WMS_ORD_NO).ToList()).ToList();
					foreach (var notDebitOrder in notDebitOrderGroup)
					{
						var currentF050801 = f050801s.Find(x => x.WMS_ORD_NO == notDebitOrder.WMS_ORD_NO);
						MoveOutShipOrderDebit(notDebitOrder, currentF050801);
					}
				}
				//6.	Step3 跨庫箱扣帳處理
				OutContainerDebit();

				return new ApiResult
				{
					IsSuccessed = _errorCnt == 0,
					MsgCode = _errorCnt == 0 ? "200" : "99999",
					MsgContent = string.Join(Environment.NewLine, _msgList)
				};
			}, true);
			return res;
		}

		public void MoveOutShipAllot(F0534_NotAllotPick notAllotPick)
		{
			var dcCode = notAllotPick.DC_CODE;
			var gupCode = notAllotPick.GUP_CODE;
			var custCode = notAllotPick.CUST_CODE;
			var pickOrdNo = notAllotPick.PICK_ORD_NO;
			try
			{
				var wmsTransaction = new WmsTransaction();
				var orderService = new OrderService(wmsTransaction);
				var f051202Repo = new F051202Repository(Schemas.CoreSchema, wmsTransaction);
				var f1511Repo = new F1511Repository(Schemas.CoreSchema, wmsTransaction);
				var f051203Repo = new F051203Repository(Schemas.CoreSchema, wmsTransaction);
				var f053201Repo = new F053201Repository(Schemas.CoreSchema, wmsTransaction);
				var f053202Repo = new F053202Repository(Schemas.CoreSchema, wmsTransaction);
				var f053203Repo = new F053203Repository(Schemas.CoreSchema, wmsTransaction);
				var f051206Repo = new F051206Repository(Schemas.CoreSchema, wmsTransaction);
				var f0534Repo = new F0534Repository(Schemas.CoreSchema, wmsTransaction);
				var f0535Repo = new F0535Repository(Schemas.CoreSchema, wmsTransaction);
				var f053601Repo = new F053601Repository(Schemas.CoreSchema, wmsTransaction);
				var f0537Repo = new F0537Repository(Schemas.CoreSchema, wmsTransaction);
				var f191302Repo = new F191302Repository(Schemas.CoreSchema, wmsTransaction);
				var pickService = new SharedService(wmsTransaction);
				var f0531Repo = new F0531Repository(Schemas.CoreSchema, wmsTransaction);
				var returnStocks = new List<F1913>();
				var returnAllotList = new List<ReturnNewAllocation>();
				var updF051202List = new List<F051202>();
				var updF1511List = new List<F1511>();
				var addF191302List = new List<F191302>();

				var IsAllotContainerClose = f0531Repo.IsAllotContainerClose(dcCode, gupCode, custCode, pickOrdNo);
				if (!IsAllotContainerClose)
				{
					_msgList.Add(string.Format("跨庫揀貨單:{0} 分配失敗，原因:{1}", pickOrdNo, "尚有跨庫容器或取消訂單容器未關箱"));
					return;
				}

				//(1)	[B] = 取得揀貨單明細
				var f051202s = f051202Repo.GetNotCacnelDataByPickNo(dcCode, gupCode, custCode, pickOrdNo).ToList();
				//(2)	[C] = [B] 產生揀貨明細處理物件
				var pickDtls = new List<F051202Ex>();
				f051202s.ForEach((x) =>
				{
					F051202Ex y = new F051202Ex();
					x.CloneProperties(y);
					y.LACK_QTY = 0;
					pickDtls.Add(y);
				});
				//(3)	[V] = 取得虛擬庫存
				var f1511s = f1511Repo.GetNotCacnelDatasByPickNo(dcCode, gupCode, custCode, pickOrdNo).ToList();
				//(4)	[D] = 取得揀貨單彙總明細有缺貨的商品
				var f051203s = f051203Repo.GetLackPickDtl(dcCode, gupCode, custCode, pickOrdNo).ToList();
				//(5)	[E]=取得揀貨單跨庫箱與揀貨容器綁定檔ID清單
				var f053201_IDs = f053201Repo.GetF053201IdsByPickNo(dcCode, gupCode, custCode, pickOrdNo).ToList();
				//(6)	[F]=取得揀貨單跨庫箱明細檔
				var moveOutDtls = f053202Repo.GetMoveOutDtlByPickNo(dcCode, gupCode, custCode, pickOrdNo).ToList();

				//(7)	[Z] = 取得新稽核出庫取消訂單分貨缺的資料
				var f0537_LackDatas = f0537Repo.GetLackDatasByPickOrdNo(dcCode, gupCode, custCode, pickOrdNo).ToList();
				
				//(8) 分配取消訂單分貨缺貨數
				foreach (var f0537 in f0537_LackDatas)
				{
					var cancelOrderLackQty = f0537.B_LACK_QTY - f0537.A_LACK_QTY;
					var currentPickDtl= pickDtls.FirstOrDefault(x => x.PICK_ORD_SEQ == f0537.PICK_ORD_SEQ && x.PICK_STATUS == "0");
					if (currentPickDtl == null)
					{
						_msgList.Add(string.Format("揀貨單{0}分配訂單取消失敗，無符合揀貨明細可分配", notAllotPick.PICK_ORD_NO));
						return;
					}
					else
					{
						currentPickDtl.LACK_QTY += cancelOrderLackQty;
						if (currentPickDtl.B_PICK_QTY - currentPickDtl.A_PICK_QTY - currentPickDtl.LACK_QTY == 0)
							currentPickDtl.PICK_STATUS = "1";//揀貨完成
					}
				}

				//(9) [Y]=取得取消訂單的箱內明細檔
				var cancelOrdermoveOutDtls = moveOutDtls.Where(x => x.SOW_TYPE == "1").OrderByDescending(x=> x.SERIAL_NO).ToList();
				
				//(10) 分配取消訂單的箱內明細檔
				foreach (var cancelOrdermoveOutDtl in cancelOrdermoveOutDtls)
				{
					var cancelOrderQty = cancelOrdermoveOutDtl.QTY;
					var currentPickDtls = pickDtls.Where(x => x.WMS_ORD_NO == cancelOrdermoveOutDtl.WMS_ORD_NO && 
					x.WMS_ORD_SEQ == cancelOrdermoveOutDtl.WMS_ORD_SEQ && x.PICK_STATUS == "0");
					// 如果有序號
					if (!string.IsNullOrEmpty(cancelOrdermoveOutDtl.SERIAL_NO))
					{
						// 找揀貨明細有該指定序號
						var currentPickDtlSns = currentPickDtls.Where(x => x.SERIAL_NO == cancelOrdermoveOutDtl.SERIAL_NO);
						// 若沒有找揀貨明細無指定序號
						if (!currentPickDtlSns.Any())
							currentPickDtls = currentPickDtls.Where(x => string.IsNullOrEmpty(x.SERIAL_NO));
					}
					if (!currentPickDtls.Any())
					{
						_msgList.Add(string.Format("揀貨單{0}分配訂單取消失敗，無符合揀貨明細可分配", notAllotPick.PICK_ORD_NO));
						return;
					}
					do
					{
						var currentPickDtl = currentPickDtls.FirstOrDefault(w => w.PICK_STATUS == "0");
						if (currentPickDtl == null)
						{
							_msgList.Add(string.Format("揀貨單{0}分配訂單取消失敗，超過預計可揀數", notAllotPick.PICK_ORD_NO));
							return;
						}
						//c.	[C4] = [C3]預計揀貨數-[C3]實際揀貨數-[C3].缺貨數
						var calcQty = currentPickDtl.B_PICK_QTY - currentPickDtl.A_PICK_QTY - currentPickDtl.LACK_QTY;
						//d.	如果 [C4] > 取消訂單播種數[G]
						if (calcQty > cancelOrderQty)
						{
							//[C3].實際揀貨數+= [G]
							currentPickDtl.A_PICK_QTY += cancelOrderQty;
							//新增F053203 ([C3], [F1],[G])
							var f053203 = new F053203
							{
								F0531_ID = cancelOrdermoveOutDtl.F0531_ID,
								F0701_ID = cancelOrdermoveOutDtl.F0701_ID,
								OUT_CONTAINER_CODE = cancelOrdermoveOutDtl.OUT_CONTAINER_CODE,
								OUT_CONTAINER_SEQ = cancelOrdermoveOutDtl.OUT_CONTAINER_SEQ,
								DC_CODE = currentPickDtl.DC_CODE,
								GUP_CODE = currentPickDtl.GUP_CODE,
								CUST_CODE = currentPickDtl.CUST_CODE,
								PICK_ORD_NO = currentPickDtl.PICK_ORD_NO,
								PICK_ORD_SEQ = currentPickDtl.PICK_ORD_SEQ,
								WMS_ORD_NO = currentPickDtl.WMS_ORD_NO,
								WMS_ORD_SEQ = currentPickDtl.WMS_ORD_SEQ,
								ITEM_CODE = currentPickDtl.ITEM_CODE,
								SERIAL_NO = cancelOrdermoveOutDtl.SERIAL_NO,
								QTY = cancelOrderQty,
							};
							f053203Repo.Add(f053203);
							cancelOrderQty = 0;
						}
						//e.	ELSE
						else
						{
							//[C3].實際揀貨數 += [C4]
							currentPickDtl.A_PICK_QTY += calcQty;
							//[G] -= [C4]
							cancelOrderQty -= calcQty;
							//[C3].PICK_STATUS = 1 (揀貨完成)
							currentPickDtl.PICK_STATUS = "1";
							//新增F053203 ([C3], [F1],[G])
							var f053203 = new F053203
							{
								F0531_ID = cancelOrdermoveOutDtl.F0531_ID,
								F0701_ID = cancelOrdermoveOutDtl.F0701_ID,
								OUT_CONTAINER_CODE = cancelOrdermoveOutDtl.OUT_CONTAINER_CODE,
								OUT_CONTAINER_SEQ = cancelOrdermoveOutDtl.OUT_CONTAINER_SEQ,
								DC_CODE = currentPickDtl.DC_CODE,
								GUP_CODE = currentPickDtl.GUP_CODE,
								CUST_CODE = currentPickDtl.CUST_CODE,
								PICK_ORD_NO = currentPickDtl.PICK_ORD_NO,
								PICK_ORD_SEQ = currentPickDtl.PICK_ORD_SEQ,
								WMS_ORD_NO = currentPickDtl.WMS_ORD_NO,
								WMS_ORD_SEQ = currentPickDtl.WMS_ORD_SEQ,
								ITEM_CODE = currentPickDtl.ITEM_CODE,
								SERIAL_NO = cancelOrdermoveOutDtl.SERIAL_NO,
								QTY = calcQty,
							};
							f053203Repo.Add(f053203);
						}

					} while (cancelOrderQty > 0);
				}

				//(11)	分配揀貨單揀缺數 Foreach [D1] IN [D]
				foreach (var f051203 in f051203s)
				{
					//A.	[G] = D1.LACK_QTY
					var lastLackQty = f051203.LACK_QTY;
					//B.	[C1]=取得[C]資料 分配跨庫箱播種數失敗，無符合揀貨明細可分配
					var currentPickDtls = pickDtls.Where(w => w.PICK_STATUS == "0" &&
														w.PICK_LOC == f051203.PICK_LOC &&
														w.ITEM_CODE == f051203.ITEM_CODE &&
														w.VALID_DATE == f051203.VALID_DATE &&
														w.MAKE_NO == f051203.MAKE_NO);
					//C.	如果[D1].SERIAL_NO 有值
					if (!string.IsNullOrWhiteSpace(f051203.SERIAL_NO))
					{
						currentPickDtls = currentPickDtls.Where(w => w.SERIAL_NO == f051203.SERIAL_NO);
					}
					//D.	如果[D1].SRRIAL_NO無值
					else
					{
						currentPickDtls = currentPickDtls.Where(w => string.IsNullOrWhiteSpace(w.SERIAL_NO));
					}
					//F.	IF [C2] 無資料
					if (currentPickDtls == null || !currentPickDtls.Any())
					{
						_msgList.Add(string.Format("揀貨單{0}分配揀缺數失敗，無符合揀貨明細可分配", notAllotPick.PICK_ORD_NO));
						return;
					}
					//G.	IF [C2]有資料，開始分配缺貨數，do…while([G] >0)
					do
					{
						//E.	[C2]= [C1] 排序: [C1].WMS_ORD_NO DESC (揀缺用最新出貨單的優先設定缺貨)
						//a.	[C3] 取得還有可分配揀貨量 = [C2] 取得第一筆
						var currentPickDtl = currentPickDtls.Where(w => w.PICK_STATUS == "0").OrderByDescending(o => o.WMS_ORD_NO).FirstOrDefault();
						//b.	[C3] = NULL
						if (currentPickDtl == null)
						{
							_msgList.Add(string.Format("揀貨單{0}分配揀缺數失敗，超過預計可揀數", notAllotPick.PICK_ORD_NO));
							return;
						}
						//c.	[C4] = [C3]預計揀貨數-[C3]實際揀貨數-[C3].缺貨數
						var calcQty = currentPickDtl.B_PICK_QTY - currentPickDtl.A_PICK_QTY - currentPickDtl.LACK_QTY;
						//d.	如果 [C4] > 剩餘揀缺數[G]
						if (calcQty > lastLackQty)
						{
							//[C3].缺貨數+= [G]
							currentPickDtl.LACK_QTY += lastLackQty;
							lastLackQty = 0;
						}
						//e.	ELSE
						else
						{
							//[C3].缺貨數 += [C4]
							currentPickDtl.LACK_QTY += calcQty;
							//[G] -= [C4]
							lastLackQty -= calcQty;
							//[C3].PICK_STATUS = 1 (揀貨完成)
							currentPickDtl.PICK_STATUS = "1";
						}

					} while (lastLackQty > 0);
				}

				//(12)	[K]=取得新稽核出庫分貨缺的資料
				var f053601s = f053601Repo.GetLackDatas(dcCode, gupCode, custCode, pickOrdNo).ToList();
				//exceptSnPickDtls = F051202揀貨單明細排除F053202有序號綁儲位的商品
				//因為播缺只會有商品品號，為了避免存在序號的商品分配到播缺，要先把揀貨明細中存在序號的商品排除
				var bindingSnPickDtls = pickDtls.Where(w => moveOutDtls.Where(x => !string.IsNullOrWhiteSpace(x.SERIAL_NO)).Select(s => s.SERIAL_NO).Contains(w.SERIAL_NO));
				var exceptSnPickDtls = pickDtls.Except(bindingSnPickDtls);
				// (13)	分配新稽核出庫播缺數
				//C.	Foreach [K1] IN [K]
				foreach (var f053601 in f053601s)
				{
					var cancelOrderItemHasAllotQty = f0537_LackDatas.Where(x => x.ITEM_CODE == f053601.ITEM_CODE).Sum(x => x.B_LACK_QTY - x.A_LACK_QTY);
					//a.	[G] = K1.B_SET_QTY-K1.A_SET_QTY
					var lastSetQty = f053601.B_SET_QTY - f053601.A_SET_QTY - cancelOrderItemHasAllotQty;
					//分配播缺時，因為訂單取消已經分配了，所以扣除後播缺=0，需要跳過
					if (lastSetQty == 0) continue;
					//b.	開始分配播缺數Do…While([G]>0)
					do
					{
						//i.	[C1] = 從[C]取得第一筆
						var currentPickDtl = exceptSnPickDtls.FirstOrDefault(x => x.PICK_STATUS == "0" && x.ITEM_CODE == f053601.ITEM_CODE);
						//j.	如果[C1]無資料，新增Log[揀貨單XXX分配播缺數失敗，超過預計可揀數] Continue 換下一張揀貨單處理
						if (currentPickDtl == null)
						{
							_msgList.Add(string.Format("揀貨單{0}分配播缺數失敗，超過預計可揀數", notAllotPick.PICK_ORD_NO));
							return;
						}
						//k.	如果[C1]有資料，[C2] = [C1].B_PICK_QTY-[C1]-A_PICK_QTY-[C1].LACK_QTY
						var calcQty = currentPickDtl.B_PICK_QTY - currentPickDtl.A_PICK_QTY - currentPickDtl.LACK_QTY;
						//f.	如果[C2]> 剩餘播缺數[G]
						if (calcQty > lastSetQty)
						{
							//[C1].LACK_QTY+=剩餘播缺數[G]
							currentPickDtl.LACK_QTY += lastSetQty;
							lastSetQty = 0;
						}
						else
						{
							//[C1].LACK_QTY+=[C2]
							currentPickDtl.LACK_QTY += calcQty;
							//[C1].PICK_STATUS=1(揀貨完成)
							currentPickDtl.PICK_STATUS = "1";
							//[G]-=[C2]
							lastSetQty -= calcQty;
						}
					} while (lastSetQty > 0);
				}

				// (14) 取得跨庫箱已播明細
				var normalMoveOutDtls = moveOutDtls.Where(x => x.SOW_TYPE == "0").OrderByDescending(x => x.SERIAL_NO).ToList();
				//(15)	分配跨庫箱已播數: Foreach [F1] IN [F]  
				foreach (var moveOutDtl in normalMoveOutDtls)
				{
					//A.	[G] = F1.QTY
					var currentMoveOutQty = moveOutDtl.QTY;
					//如果可分配數=0則跳過此筆商品分配
					if (currentMoveOutQty == 0) continue;
					//B.	[C1]=取得[C]資料 
					var currentPickDtls = pickDtls.Where(w => w.PICK_STATUS == "0" &&
														w.ITEM_CODE == moveOutDtl.ITEM_CODE);
					//C.	如果[F1].SERIAL_NO 有值
					if (!string.IsNullOrWhiteSpace(moveOutDtl.SERIAL_NO))
					{
						var currentSnPickDtl = currentPickDtls.Where(w => w.SERIAL_NO == moveOutDtl.SERIAL_NO);
						if (!currentSnPickDtl.Any())
							currentSnPickDtl = currentPickDtls.Where(w => string.IsNullOrWhiteSpace(w.SERIAL_NO));
						currentPickDtls = currentSnPickDtl;
					}
					//D.	如果[D1].SRRIAL_NO無值
					else
					{
						currentPickDtls = currentPickDtls.Where(w => string.IsNullOrWhiteSpace(w.SERIAL_NO));
					}
					//F.	IF [C2] 無資料
					if (currentPickDtls == null || !currentPickDtls.Any())
					{
						_msgList.Add(string.Format("揀貨單{0}分配跨庫箱播種數失敗，無符合揀貨明細可分配", notAllotPick.PICK_ORD_NO));
						return;
					}
					//G.	IF [C2]有資料，開始分配缺貨數，do…while([G] >0)
					do
					{
						//E.	[C2]= [C1] 排序: [C1].WMS_ORD_NO (以出貨單最小的先分配)
						//a.	[C3] 取得還有可分配揀貨量 = [C2] 取得第一筆
						var currentPickDtl = currentPickDtls.Where(w => w.PICK_STATUS == "0").OrderBy(o => o.WMS_ORD_NO).FirstOrDefault();
						//b.	[C3] = NULL
						if (currentPickDtl == null)
						{
							_msgList.Add(string.Format("揀貨單{0}分配跨庫箱播種數失敗，超過預計可揀數", notAllotPick.PICK_ORD_NO));
							return;
						}
						//c.	[C4] = [C3]預計揀貨數-[C3]實際揀貨數-[C3].缺貨數
						var calcQty = currentPickDtl.B_PICK_QTY - currentPickDtl.A_PICK_QTY - currentPickDtl.LACK_QTY;
						//d.	如果 [C4] > 剩餘播種數[G]
						if (calcQty > currentMoveOutQty)
						{
							//[C3].實際揀貨數+= [G]
							currentPickDtl.A_PICK_QTY += currentMoveOutQty;
							//新增F053203 ([C3], [F1],[G])
							var f053203 = new F053203
							{
								F0531_ID = moveOutDtl.F0531_ID,
								F0701_ID = moveOutDtl.F0701_ID,
								OUT_CONTAINER_CODE = moveOutDtl.OUT_CONTAINER_CODE,
								OUT_CONTAINER_SEQ = moveOutDtl.OUT_CONTAINER_SEQ,
								DC_CODE = currentPickDtl.DC_CODE,
								GUP_CODE = currentPickDtl.GUP_CODE,
								CUST_CODE = currentPickDtl.CUST_CODE,
								PICK_ORD_NO = currentPickDtl.PICK_ORD_NO,
								PICK_ORD_SEQ = currentPickDtl.PICK_ORD_SEQ,
								WMS_ORD_NO = currentPickDtl.WMS_ORD_NO,
								WMS_ORD_SEQ = currentPickDtl.WMS_ORD_SEQ,
								ITEM_CODE = currentPickDtl.ITEM_CODE,
								SERIAL_NO = moveOutDtl.SERIAL_NO,
								QTY = currentMoveOutQty,
							};
							f053203Repo.Add(f053203);
              currentMoveOutQty = 0;
            }
						//e.	ELSE
						else
						{
							//[C3].實際揀貨數+= [C4]
							currentPickDtl.A_PICK_QTY += calcQty;
							//[G] -= [C4]
							currentMoveOutQty -= calcQty;
							//[C3].PICK_STATUS = 1 (揀貨完成)
							currentPickDtl.PICK_STATUS = "1";

							//新增F053203 ([C3], [F1],[C4])
							var f053203 = new F053203
							{
								F0531_ID = moveOutDtl.F0531_ID,
								F0701_ID = moveOutDtl.F0701_ID,
								OUT_CONTAINER_CODE = moveOutDtl.OUT_CONTAINER_CODE,
								OUT_CONTAINER_SEQ = moveOutDtl.OUT_CONTAINER_SEQ,
								DC_CODE = currentPickDtl.DC_CODE,
								GUP_CODE = currentPickDtl.GUP_CODE,
								CUST_CODE = currentPickDtl.CUST_CODE,
								PICK_ORD_NO = currentPickDtl.PICK_ORD_NO,
								PICK_ORD_SEQ = currentPickDtl.PICK_ORD_SEQ,
								WMS_ORD_NO = currentPickDtl.WMS_ORD_NO,
								WMS_ORD_SEQ = currentPickDtl.WMS_ORD_SEQ,
								ITEM_CODE = currentPickDtl.ITEM_CODE,
								SERIAL_NO = moveOutDtl.SERIAL_NO,
								QTY = calcQty,
							};
							f053203Repo.Add(f053203);
						}

					} while (currentMoveOutQty > 0);
				}
				
				//(16)	   如果[C]還有任何一筆PICK_STATUS=0
				if (pickDtls.Any(a => a.PICK_STATUS == "0"))
				{
					_msgList.Add(string.Format("揀貨單{0}尚有預計揀貨數未分配，處理失敗", notAllotPick.PICK_ORD_NO));
					return;
				}
				var sharedService = new SharedService(wmsTransaction);
				// 取得疑似遺失倉-倉庫編號
				var pickLossWHId = sharedService.GetPickLossWarehouseId(dcCode, gupCode, custCode);
				if (string.IsNullOrWhiteSpace(pickLossWHId))
				{
					_msgList.Add(string.Format("物流中心{0}疑似遺失倉未設定", dcCode));
					return;
				}
				// 疑似遺失倉第一個儲位
				var pickLossLocCode = sharedService.GetPickLossLoc(dcCode, pickLossWHId);

				//(17)	如果[C]無任何一筆PICK_STATUS=0，往下執行
				//(18)	回壓揀貨單實際揀貨數、揀貨虛擬實際揀貨數Foreach [C5]  IN [C]
				foreach (var pickDtl in pickDtls)
				{
					//A.	[J] = 取得揀貨明細[B] WHERE B.PICK_ORD_SEQ = [H1].PICK_ORD_SEQ
					var currentF051202 = f051202s.First(w => w.PICK_ORD_SEQ == pickDtl.PICK_ORD_SEQ);
					//B.	[K] = 取得虛擬庫存[V] WHERE ORDER_SEQ = [H1].PICK_ORD_SEQ
					var currentF1511 = f1511s.First(w => w.ORDER_SEQ == pickDtl.PICK_ORD_SEQ);
					//C.	[J].A_PICK_QTY = [C5].A_PICK_QTY;
					//D.	[J].PICK_STATUS = [C5].PICK_STATUS
					currentF051202.A_PICK_QTY = pickDtl.A_PICK_QTY;
					currentF051202.PICK_STATUS = pickDtl.PICK_STATUS;
					//E.	[K].A_PICK_QTY = [C5].A_PICK_QTY
					//F.	[K].STATUS = [C5].PICK_STATUS
					currentF1511.A_PICK_QTY = pickDtl.A_PICK_QTY;
					currentF1511.STATUS = pickDtl.PICK_STATUS;
					//G.	如果揀貨明細有缺貨[C5].LACK_QTY>0 ，新增缺貨記錄，並將缺貨搬到疑似遺失倉
					if (pickDtl.LACK_QTY > 0)
					{
						//a.	新增F051206(J)，STATUS=2(已確認)，RETURN_FLAG=1(缺品出貨),REASON=999,LACK_QTY=[C5].LACK_QTY
						var f051206 = new F051206
						{
							WMS_ORD_NO = pickDtl.WMS_ORD_NO,
							PICK_ORD_NO = pickDtl.PICK_ORD_NO,
							PICK_ORD_SEQ = pickDtl.PICK_ORD_SEQ,
							ITEM_CODE = pickDtl.ITEM_CODE,
							LACK_QTY = pickDtl.LACK_QTY,
							REASON = "999",
							STATUS = "2",
							RETURN_FLAG = "1",
							CUST_CODE = pickDtl.CUST_CODE,
							GUP_CODE = pickDtl.GUP_CODE,
							DC_CODE = pickDtl.DC_CODE,
							TRANS_FLAG = "0",
							LOC_CODE = pickDtl.PICK_LOC,
							ISDELETED = "0",
						};
						f051206Repo.Add(f051206);
						//b.	呼叫揀貨庫存異常處理PickService.CreateStockLackProcess([J],[K], [C5].LACK_QTY)
						var result = pickService.CreateStockLackProcess(new Shared.ServiceEntites.StockLack
						{
							DcCode = currentF051202.DC_CODE,
							CustCode = currentF051202.CUST_CODE,
							GupCode = currentF051202.GUP_CODE,
							F051202 = currentF051202,
							F1511 = currentF1511,
							PickLackLocCode = pickLossLocCode,
							PickLackWarehouseId = pickLossWHId,
							LackQty = pickDtl.LACK_QTY,
							ReturnStocks = returnStocks
						});
						if (!result.IsSuccessed)
						{
							_msgList.Add(string.Format("跨庫揀貨單:{0} 分配失敗，原因:{1}", pickOrdNo, result.Message));
							return;
						}
						returnStocks = result.ReturnStocks;
						returnAllotList.AddRange(result.ReturnNewAllocations);
						addF191302List.AddRange(result.AddF191302List);
						//若回傳成功，F051202與F1511替換成回傳的資料
						currentF051202 = result.UpdF051202;
						currentF1511 = result.UpdF1511;

					}
					updF051202List.Add(currentF051202);
					updF1511List.Add(currentF1511);
				}
				
				// 調撥單整批上架
				var result2 = pickService.BulkAllocationToAllUp(returnAllotList, returnStocks, false, addF191302List);
				if (!result2.IsSuccessed)
				{
					_msgList.Add(string.Format("跨庫揀貨單:{0} 分配失敗，原因:{1}", pickOrdNo, result2.Message));
					return;
				}
				// 調撥單整批寫入
				var result3 = pickService.BulkInsertAllocation(returnAllotList, returnStocks, true);
				if (!result3.IsSuccessed)
				{
					_msgList.Add(string.Format("跨庫揀貨單:{0} 分配失敗，原因:{1}", pickOrdNo, result3.Message));
					return;
				}

				f191302Repo.BulkInsert(addF191302List, true);

				//(19)	更新F053201.STATUS = 1(已分配) 
				f053201Repo.UpdateStatusByIds(f053201_IDs, "1");
				//(20)	更新F0534.STATUS=2(已分配) 
				f0534Repo.UpdateStatusByPickNo(dcCode, gupCode, custCode, pickOrdNo, "2");
				//(21)	更新F0535.STATUS=1(已完成跨庫作業可扣帳)
				f0535Repo.UpdateStatusByPickNo(dcCode, gupCode, custCode, pickOrdNo, "1");

				f051202Repo.BulkUpdate(updF051202List);
				f1511Repo.BulkUpdate(updF1511List);

				//(22)	DB Commit
				wmsTransaction.Complete();
				//_msgList.Add(string.Format("跨庫揀貨單:{0} 已分配", pickOrdNo));				
			}
			catch (Exception ex)
			{
				_errorCnt++;
				_msgList.Add(string.Format("跨庫揀貨單:{0} 分配失敗，原因:{1}", pickOrdNo, ex.Message));
			}
		}

		public void MoveOutShipOrderDebit(F0535_NotDebitOrder notDebitOrder, F050801 currentF050801)
		{
			var dcCode = notDebitOrder.DC_CODE;
			var gupCode = notDebitOrder.GUP_CODE;
			var custCode = notDebitOrder.CUST_CODE;
			var wmsOrdNo = notDebitOrder.WMS_ORD_NO;
			try
			{
				var wmsTransaction = new WmsTransaction();
				var f050801Repo = new F050801Repository(Schemas.CoreSchema, wmsTransaction);
				var f0535Repo = new F0535Repository(Schemas.CoreSchema, wmsTransaction);

				//B.	檢查出貨單是否取消[K.STATUS=9]，若出貨單取消
				if (currentF050801.STATUS == 9)
				{
					//a.	設定[K].SHIP_MODE=4
					currentF050801.SHIP_MODE = "4";
					//b.	更新出貨單[K]
					f050801Repo.UpdateShipMode(dcCode, gupCode, custCode, wmsOrdNo, "4");
					//c.	更新揀貨單與出貨單綁定資料[F0535]，狀態為9(出貨取消)
					f0535Repo.UpdateStatusByWmsOrdNo(dcCode, gupCode, custCode, wmsOrdNo, "9");
					//d.	DBCommit;
					wmsTransaction.Complete();
					//e.	Continue 換一筆出貨單處理
					return;
				}

				var orderService = new OrderService(wmsTransaction);
				var f053203Repo = new F053203Repository(Schemas.CoreSchema, wmsTransaction);
				var f05030202Repo = new F05030202Repository(Schemas.CoreSchema, wmsTransaction);
				var f055001Repo = new F055001Repository(Schemas.CoreSchema, wmsTransaction);
				var f055002Repo = new F055002Repository(Schemas.CoreSchema, wmsTransaction);
				var f050305Repo = new F050305Repository(Schemas.CoreSchema, wmsTransaction);
				var f050802Repo = new F050802Repository(Schemas.CoreSchema, wmsTransaction);
				var f1511Repo = new F1511Repository(Schemas.CoreSchema, wmsTransaction);
				var f0513Repo = new F0513Repository(Schemas.CoreSchema, wmsTransaction);
				var f2501Repo = new F2501Repository(Schemas.CoreSchema, wmsTransaction);

				//C.	出貨單沒取消，往下執行
				//D.	[B]= 取得出貨單跨庫出貨容器分配結果
				var f053203s = f053203Repo.GetDatasByWmsOrdNo(dcCode, gupCode, custCode, wmsOrdNo).ToList();
				//E.	[C] = 取得出貨單訂單配庫結果檔
				var f05030202s = f05030202Repo.GetByWmsOrdNo(dcCode, gupCode, custCode, wmsOrdNo).ToList();

				//F.	[M]=取得商品序號
				var snItemList = f053203s.Where(w => !string.IsNullOrWhiteSpace(w.SERIAL_NO));
				//G.	如果[M]有資料，
				if (snItemList.Any())
				{
					//a.	更新F2501.ORD_PROP = 出貨單[K].ORD_PROP, F2501.WMS_NO = 出貨單[K].WMS_ORD_NO
					f2501Repo.UpdateOrdPropAndWmsOrdNo(gupCode,
														custCode,
														currentF050801.ORD_PROP,
														currentF050801.WMS_ORD_NO,
														snItemList.Select(s => s.SERIAL_NO).ToList());
				}
				//H.	如果[M]沒資料，往下執行
				//I.	[D] = 由[B]產生箱頭檔
				//J.	[E] = 由[B]+[C]分配產生箱明細檔
				List<F055001> addF055001List = new List<F055001>();
				List<F055002> addF055002List = new List<F055002>();

				var f053203GroupList = f053203s.OrderBy(o => o.CRT_DATE)
												.GroupBy(g => new { g.DC_CODE, g.GUP_CODE, g.CUST_CODE, g.OUT_CONTAINER_CODE, g.OUT_CONTAINER_SEQ })
												.ToList();

				foreach (var f053203Group in f053203GroupList)
				{
					short currentBoxNo = (short)f053203Group.Key.OUT_CONTAINER_SEQ;
					var f055001 = new F055001
					{
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						DELV_DATE = currentF050801.DELV_DATE,
						PICK_TIME = currentF050801.PICK_TIME,
						PACKAGE_BOX_NO = currentBoxNo,
						BOX_NUM = f053203Group.Key.OUT_CONTAINER_CODE,
						WMS_ORD_NO = wmsOrdNo,
						PACKAGE_STAFF = f053203Group.FirstOrDefault().CRT_STAFF,
						PACKAGE_NAME = f053203Group.FirstOrDefault().CRT_NAME,
						STATUS = "0",
					};
					addF055001List.Add(f055001);

					int boxSeq = 1;
					foreach (var f053203 in f053203Group)
					{
						var allotQty = f053203.QTY;
						do
						{
							var f05030202 = f05030202s.FirstOrDefault(w => w.WMS_ORD_SEQ == f053203.WMS_ORD_SEQ && w.B_DELV_QTY > w.A_DELV_QTY);
							var canAllotQty = (f05030202.B_DELV_QTY - f05030202.A_DELV_QTY)?? 0;
							if(canAllotQty >  allotQty)
							{
								f05030202.A_DELV_QTY += allotQty;
								var f055002 = new F055002
								{
									DC_CODE = dcCode,
									GUP_CODE = gupCode,
									CUST_CODE = custCode,
									WMS_ORD_NO = wmsOrdNo,
									PACKAGE_BOX_NO = currentBoxNo,
									PACKAGE_BOX_SEQ = boxSeq++,
									ITEM_CODE = f053203.ITEM_CODE,
									SERIAL_NO = f053203.SERIAL_NO,
									PACKAGE_QTY = allotQty,
									ORD_NO = f05030202.ORD_NO,
									ORD_SEQ = f05030202.ORD_SEQ,
								};
								addF055002List.Add(f055002);
								allotQty = 0;
								
							}
							else
							{
								f05030202.A_DELV_QTY += canAllotQty;
								var f055002 = new F055002
								{
									DC_CODE = dcCode,
									GUP_CODE = gupCode,
									CUST_CODE = custCode,
									WMS_ORD_NO = wmsOrdNo,
									PACKAGE_BOX_NO = currentBoxNo,
									PACKAGE_BOX_SEQ = boxSeq++,
									ITEM_CODE = f053203.ITEM_CODE,
									SERIAL_NO = f053203.SERIAL_NO,
									PACKAGE_QTY = canAllotQty,
									ORD_NO = f05030202.ORD_NO,
									ORD_SEQ = f05030202.ORD_SEQ,
								};
								addF055002List.Add(f055002);
								allotQty -= canAllotQty;
							}
							
						}
						while (allotQty > 0);
					}
				}
				//K.	由[D]新增箱頭檔[F055001]
				//L.	由[E]新增箱明細[F055002]
				f055001Repo.BulkInsert(addF055001List);
				f055002Repo.BulkInsert(addF055002List);

				//M.	新增出貨回檔紀錄-開始包裝[F050305.STATUS=2]
				orderService.AddF050305(dcCode, gupCode, custCode, new List<string> { wmsOrdNo }, "2");
				//N.	新增出貨回檔紀錄-包裝完成[F050305.STATUS=3]
				orderService.AddF050305(dcCode, gupCode, custCode, new List<string> { wmsOrdNo }, "3");

				List<F0513> updF0513s;
				List<F05030202> updF05030202s;
				List<F050801> updF050801s;
				List<F050802> updF050802s;
				List<F1511> updF1511s;
				//O.	[F]=進行出貨單扣帳呼叫orderService.MultiShipOrderDebit
				var result = orderService.MultiShipOrderDebit(dcCode, gupCode, custCode, currentF050801, out updF050801s, out updF050802s, out updF05030202s, out updF1511s, out updF0513s);
				if (!result.IsSuccessed)
				{
					_msgList.Add(string.Format("跨庫出貨單:{0} 扣帳失敗", wmsOrdNo));
					return;
				}
				//P.	由[F]資料設定F050801.SHIP_MODE=4		
				updF050801s.ForEach(x => x.SHIP_MODE = "4");
				//Q.	由[F]資料更新F050801
				f050801Repo.BulkUpdate(updF050801s);
				//R.	由[F]資料更新F050802
				f050802Repo.BulkUpdate(updF050802s);
				//S.	由[F]資料更新F1511
				f1511Repo.BulkUpdate(updF1511s);
				//T.	由[F]資料更新F05030202
				f05030202Repo.BulkUpdate(updF05030202s);
				//U.	由[F]資料更新F0513
				f0513Repo.BulkUpdate(updF0513s);

				//V.	更新揀貨單與出貨單綁定資料[F0535]，狀態為2(扣帳完成)
				f0535Repo.UpdateStatusByWmsOrdNo(dcCode, gupCode, custCode, wmsOrdNo, "2");

				//W.	DB Commit
				wmsTransaction.Complete();
				//_msgList.Add(string.Format("跨庫出貨單:{0} 扣帳成功", wmsOrdNo));
			}
			catch (Exception ex)
			{
				_errorCnt++;
				_msgList.Add(string.Format("跨庫出貨單:{0} 扣帳失敗，原因:{1}", wmsOrdNo, ex.Message));
			}
		}

		public void OutContainerDebit()
		{
			var wmsTransaction = new WmsTransaction();
			var f0532Repo = new F0532Repository(Schemas.CoreSchema, wmsTransaction);
			var f0701Repo = new F0701Repository(Schemas.CoreSchema, wmsTransaction);

			//(1)	[A]= 取得跨庫箱:已關箱資料且綁定的揀貨單所有出貨單都已扣帳或出貨取消:
			var f0532_Ids = f0532Repo.GetCloseDebitContainerIds().ToList();
			//(2)	如果[A]有資料
			if (f0532_Ids != null && f0532_Ids.Any())
			{
				//A.	更新F0532.STATUS=2(已出貨) WHERE ID IN([A])
				f0532Repo.UpdateStatusByIds(f0532_Ids, "2");
				//B.	[B]=取得[A].F0701_ID有值資料
				//a.	釋放容器綁定
				f0701Repo.DeleteByF0532Id(f0532_Ids);
				//C.	DB Commit
				wmsTransaction.Complete();
			}
		}
	}
}