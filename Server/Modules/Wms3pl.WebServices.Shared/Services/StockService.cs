using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Enums;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Helper;
using Wms3pl.WebServices.Shared.ServiceEntites;

namespace Wms3pl.WebServices.Shared.Services
{
	/// <summary>
	/// 庫存共服服務
	/// </summary>
	public partial class StockService
	{
		private WmsTransaction _wmsTransaction;
		private WmsLogHelper _wmsLogHelper;
		private F1913Repository _f1913Repo;
		private CommonService _commonService;
		private List<F1913> _f1913List;
		private List<StockChange> _stockChangeList;
		private string _orderStockChangeMsgFormat = "[單據{0}] 儲位{1} 品號{2} 效期{3} 入庫日{4} 批號{5} 序號{6} 廠商{7} 箱號{8} 板號{9} 調整數量{10}";
		private string _outOfStockMsgFormat = "[物流中心:{0} 業主:{1} 貨主:{2}] 儲位{3} 品號{4} {5}庫存數不足{6}";
		private string _stockChangeMsgFormat = "[物流中心:{0} 業主:{1} 貨主:{2}]儲位{3} 品號{4} 效期{5} 入庫日{6} 批號{7} 序號{8} 廠商{9} 箱號{10} 板號{11} {12}庫存數{13}";
		private bool _isStartRecord = false;
	

		public StockService(WmsTransaction wmsTransaction = null,bool isWriteLog =true)
		{
			_wmsTransaction = wmsTransaction;
			_wmsLogHelper = new WmsLogHelper(_wmsTransaction, isWriteLog);
			_f1913Repo = new F1913Repository(Schemas.CoreSchema,_wmsTransaction);
			_commonService = new CommonService();
		}

		#region 庫存異動處理

		#region 庫存異動處理Common

		#region StockChange FindFunc
		private Func<StockChange, string, string, string, string, string, DateTime, DateTime, string, string, string, string, string, bool> _findFindStockChangeFunc = FindStockChange;
		private static bool FindStockChange(StockChange sc, string dcCode, string gupCode, string custCode,
			string locCode, string itemCode, DateTime validDate, DateTime enterDate, string makeNo,
			string serialNo, string vnrCode, string boxCtrlNo, string palletCtrlNo)
		{
			return sc.DcCode == dcCode && sc.GupCode == gupCode && sc.CustCode == custCode &&
				sc.LocCode == locCode && sc.ItemCode == itemCode && sc.ValidDate == validDate &&
				sc.EnterDate == enterDate && sc.MakeNo == makeNo && sc.SerialNo == serialNo &&
				sc.VnrCode == vnrCode && sc.BoxCtrlNo == boxCtrlNo && sc.PalletCtrlNo == palletCtrlNo;
		}

		#endregion

		#region F1913 FindFunc
		private Func<F1913, string, string, string, string, string, DateTime, DateTime, string, string, string, string, string, bool> _findF1913Func = FindF1913;
		private static bool FindF1913(F1913 f1913, string dcCode, string gupCode, string custCode,
			string locCode, string itemCode, DateTime validDate, DateTime enterDate, string makeNo,
			string serialNo, string vnrCode, string boxCtrlNo, string palletCtrlNo)
		{
			return f1913.DC_CODE == dcCode && f1913.GUP_CODE == gupCode && f1913.CUST_CODE == custCode &&
				f1913.LOC_CODE == locCode && f1913.ITEM_CODE == itemCode && f1913.VALID_DATE == validDate &&
				f1913.ENTER_DATE == enterDate && f1913.MAKE_NO == makeNo && f1913.SERIAL_NO == serialNo &&
				f1913.VNR_CODE == vnrCode && f1913.BOX_CTRL_NO == boxCtrlNo && f1913.PALLET_CTRL_NO == palletCtrlNo;
		}

		#endregion

		/// <summary>
		/// 增加/更新缺貨紀錄
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="locCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="validDate"></param>
		/// <param name="enterDate"></param>
		/// <param name="makeNo"></param>
		/// <param name="serialNo"></param>
		/// <param name="vnrCode"></param>
		/// <param name="boxCtrlNo"></param>
		/// <param name="palletCtrlNo"></param>
		/// <param name="lackQty"></param>
		private void AddOrUpdateLackStock(ref List<StockChange> lackStocks,string dcCode, string gupCode, string custCode,
		string locCode, string itemCode, DateTime validDate, DateTime enterDate, string makeNo,
		string serialNo, string vnrCode, string boxCtrlNo, string palletCtrlNo, long lackQty)
		{
			var lackStock = lackStocks.FirstOrDefault(x =>
			_findFindStockChangeFunc(x, dcCode, gupCode, custCode, locCode, itemCode, validDate, enterDate, makeNo,
			serialNo, vnrCode, boxCtrlNo, palletCtrlNo));

			if (lackStock == null)
				lackStocks.Add(new StockChange
				{
					DcCode = dcCode,
					GupCode = gupCode,
					CustCode = custCode,
					LocCode = locCode,
					ItemCode = itemCode,
					ValidDate = validDate,
					EnterDate = enterDate,
					MakeNo = makeNo,
					SerialNo = serialNo,
					VnrCode = vnrCode,
					BoxCtrlNo = boxCtrlNo,
					PalletCtrlNo = palletCtrlNo,
					Qty = lackQty
				});
			else
				lackStock.Qty += lackQty;
		}

		/// <summary>
		/// 增加/更新異動紀錄
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="locCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="validDate"></param>
		/// <param name="enterDate"></param>
		/// <param name="makeNo"></param>
		/// <param name="serialNo"></param>
		/// <param name="vnrCode"></param>
		/// <param name="boxCtrlNo"></param>
		/// <param name="palletCtrlNo"></param>
		/// <param name="changeQty"></param>
		private void AddOrUpdateStockChange(string dcCode, string gupCode, string custCode,
		string locCode, string itemCode, DateTime validDate, DateTime enterDate, string makeNo,
		string serialNo, string vnrCode, string boxCtrlNo, string palletCtrlNo, long changeQty)
		{
			if (_stockChangeList == null)
				_stockChangeList = new List<StockChange>();

			var stockChange = _stockChangeList.FirstOrDefault(x =>
			_findFindStockChangeFunc(x, dcCode, gupCode, custCode, locCode, itemCode, validDate, enterDate, makeNo,
			serialNo, vnrCode, boxCtrlNo, palletCtrlNo));

			if (stockChange == null)
				_stockChangeList.Add(new StockChange
				{
					DcCode = dcCode,
					GupCode = gupCode,
					CustCode = custCode,
					LocCode = locCode,
					ItemCode = itemCode,
					ValidDate = validDate,
					EnterDate = enterDate,
					MakeNo = makeNo,
					SerialNo = serialNo,
					VnrCode = vnrCode,
					BoxCtrlNo = boxCtrlNo,
					PalletCtrlNo = palletCtrlNo,
					Qty = changeQty
				});
			else
				stockChange.Qty += changeQty;
		}

		private F1913 GetF1913(string dcCode, string gupCode, string custCode,
		string locCode, string itemCode, DateTime validDate, DateTime enterDate, string makeNo,
		string serialNo, string vnrCode, string boxCtrlNo, string palletCtrlNo)
		{
			if (_f1913List == null)
			{
				_f1913List = new List<F1913>();
			}

			var f1913 = _f1913List.FirstOrDefault(x => _findF1913Func(x,dcCode,gupCode,custCode,locCode,itemCode,validDate,
				enterDate,makeNo,serialNo,vnrCode,boxCtrlNo,palletCtrlNo));

			if (f1913 == null)
			{
				f1913 = _f1913Repo.FindDataByKey(dcCode, gupCode, custCode, itemCode, locCode,
					validDate, enterDate, vnrCode, serialNo, boxCtrlNo, palletCtrlNo, makeNo);

				if (f1913 != null)
					_f1913List.Add(f1913);
			}

			return f1913;
		}

		private void CreateF1913(string dcCode, string gupCode, string custCode,
		string locCode, string itemCode, DateTime validDate, DateTime enterDate, string makeNo,
		string serialNo, string vnrCode, string boxCtrlNo, string palletCtrlNo, long qty)
		{
			_f1913List.Add(new F1913
			{
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				LOC_CODE = locCode,
				ITEM_CODE = itemCode,
				VALID_DATE = validDate,
				ENTER_DATE = enterDate,
				MAKE_NO = makeNo,
				SERIAL_NO = serialNo,
				VNR_CODE = vnrCode,
				BOX_CTRL_NO = boxCtrlNo,
				PALLET_CTRL_NO = palletCtrlNo,
				QTY = qty
			});
		}

		/// <summary>
		/// 取得缺貨訊息
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="locCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="validDate"></param>
		/// <param name="enterDate"></param>
		/// <param name="makeNo"></param>
		/// <param name="serialNo"></param>
		/// <param name="vnrCode"></param>
		/// <param name="boxCtrlNo"></param>
		/// <param name="palletCtrlNo"></param>
		/// <param name="outOfStockQty"></param>
		/// <returns></returns>
		private string GetLackStockMessage(string dcCode, string gupCode, string custCode,
		string locCode, string itemCode, DateTime validDate, DateTime enterDate, string makeNo,
		string serialNo, string vnrCode, string boxCtrlNo, string palletCtrlNo, long outOfStockQty)
		{
			var dcName = _commonService.GetDc(dcCode)?.DC_NAME;
			var gupName = _commonService.GetGup(gupCode)?.GUP_NAME;
			var custName = _commonService.GetCust(gupCode, custCode)?.SHORT_NAME;
			var addMsgList = new StringBuilder();
			if (validDate.ToString("yyyy/MM/dd") != "9999/12/31")
				addMsgList.Append(string.Format("效期{0} ", validDate.ToString("yyyy/MM/dd")));
			if (makeNo != "0")
				addMsgList.Append(string.Format("批號{0} ", makeNo));
			else
				addMsgList.Append(string.Format("入庫日{0} ", enterDate.ToString("yyyy/MM/dd")));
			if (serialNo != "0")
				addMsgList.Append(string.Format("序號{0} ", serialNo));
			if (boxCtrlNo != "0")
				addMsgList.Append(string.Format("箱號{0} ", boxCtrlNo));
			if (palletCtrlNo != "0")
				addMsgList.Append(string.Format("板號{0} ", palletCtrlNo));
			if (vnrCode != "000000")
				addMsgList.Append(string.Format("供應商{0} ", vnrCode));

			return string.Format(_outOfStockMsgFormat, dcName, gupName, custName, locCode, itemCode, addMsgList.ToString(), outOfStockQty);
		}

		#endregion

		public void AddStock(List<OrderStockChange> orderStockChanges)
		{
			if (!_isStartRecord)
			{
				_wmsLogHelper.StartRecord(WmsLogProcType.StockChange);
				_isStartRecord = true;
			}
			AddStock(orderStockChanges,false);
		}
		/// <summary>
		/// 增加庫存
		/// </summary>
		/// <param name="orderStockChanges"></param>
		private void AddStock(List<OrderStockChange> orderStockChanges, bool notLog)
		{
			
			if (!notLog)
				_wmsLogHelper.AddRecord(string.Format("[執行時間:{0}]單據庫存異動(加庫) 開始", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
			foreach (var osc in orderStockChanges)
			{
				var stock = GetF1913(osc.DcCode, osc.GupCode, osc.CustCode, osc.LocCode, osc.ItemCode, osc.VaildDate, osc.EnterDate, osc.MakeNo,
					osc.SerialNo, osc.VnrCode, osc.BoxCtrlNo, osc.PalletCtrlNo);

				if (stock != null)
					stock.QTY += osc.Qty;
				else
					CreateF1913(osc.DcCode, osc.GupCode, osc.CustCode, osc.LocCode, osc.ItemCode, osc.VaildDate, osc.EnterDate, osc.MakeNo,
					osc.SerialNo, osc.VnrCode, osc.BoxCtrlNo, osc.PalletCtrlNo, osc.Qty);

				// 新增或更新庫存異動紀錄
				AddOrUpdateStockChange(osc.DcCode, osc.GupCode, osc.CustCode, osc.LocCode, osc.ItemCode, osc.VaildDate, osc.EnterDate, osc.MakeNo,
					osc.SerialNo, osc.VnrCode, osc.BoxCtrlNo, osc.PalletCtrlNo, osc.Qty);

				// 寫入單據庫存異動紀錄
				if (!notLog)
					_wmsLogHelper.AddRecord(string.Format(_orderStockChangeMsgFormat, osc.WmsNo, osc.LocCode, osc.ItemCode,
						osc.VaildDate, osc.EnterDate, osc.MakeNo, osc.SerialNo, osc.VnrCode, osc.BoxCtrlNo, osc.PalletCtrlNo, osc.Qty));
			}
			if (!notLog)
				_wmsLogHelper.AddRecord(string.Format("[執行時間:{0}]單據庫存異動(加庫) 結束", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));

		}

		/// <summary>
		/// 扣除庫存
		/// </summary>
		/// <param name="orderStockChanges"></param>
		/// <returns>是否庫存足夠</returns>
		public ExecuteResult DeductStock(List<OrderStockChange> orderStockChanges)
		{
			if (!_isStartRecord)
			{
				_wmsLogHelper.StartRecord(WmsLogProcType.StockChange);
				_isStartRecord = true;
			}
			var result = new ExecuteResult(true);
			List<StockChange> lackStocks = new List<StockChange>();
			
			_wmsLogHelper.AddRecord(string.Format("[執行時間:{0}]單據庫存異動(扣庫) 開始", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
			var deductStockList = new List<OrderStockChange>();
			foreach (var osc in orderStockChanges)
			{
				var stock = GetF1913(osc.DcCode, osc.GupCode, osc.CustCode, osc.LocCode, osc.ItemCode, osc.VaildDate, osc.EnterDate, osc.MakeNo,
						osc.SerialNo, osc.VnrCode, osc.BoxCtrlNo, osc.PalletCtrlNo);

				if(stock == null || stock.QTY < osc.Qty)
				{
					AddOrUpdateLackStock(ref lackStocks,osc.DcCode, osc.GupCode, osc.CustCode, osc.LocCode, osc.ItemCode, osc.VaildDate, osc.EnterDate, osc.MakeNo,
						osc.SerialNo, osc.VnrCode, osc.BoxCtrlNo, osc.PalletCtrlNo, stock == null ? osc.Qty : osc.Qty - stock.QTY);
					if (stock != null)
						stock.QTY = 0;
					result.IsSuccessed = false;
				}
				else
				{
					stock.QTY -= osc.Qty;
					deductStockList.Add(osc);

					// 新增或更新庫存異動紀錄
					AddOrUpdateStockChange(osc.DcCode, osc.GupCode, osc.CustCode, osc.LocCode, osc.ItemCode, osc.VaildDate, osc.EnterDate, osc.MakeNo,
						osc.SerialNo, osc.VnrCode, osc.BoxCtrlNo, osc.PalletCtrlNo, -osc.Qty);

					// 寫入單據庫存異動紀錄
					_wmsLogHelper.AddRecord(string.Format(_orderStockChangeMsgFormat, osc.WmsNo, osc.LocCode, osc.ItemCode,
						osc.VaildDate, osc.EnterDate, osc.MakeNo, osc.SerialNo, osc.VnrCode, osc.BoxCtrlNo, osc.PalletCtrlNo, -osc.Qty));
				}
			}
			_wmsLogHelper.AddRecord(string.Format("[執行時間:{0}]單據庫存異動(扣庫) 結束", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));

			// 若有缺貨
			if (!result.IsSuccessed)
			{
				// 將未缺貨已扣除的庫存加回來
				AddStock(deductStockList, true);
				// 產生缺貨訊息
				foreach (var sc in lackStocks)
				{
					result.Message += (string.IsNullOrWhiteSpace(result.Message) ? "" : Environment.NewLine) + GetLackStockMessage(sc.DcCode, sc.GupCode, sc.CustCode, sc.LocCode, sc.ItemCode,
					sc.ValidDate, sc.EnterDate, sc.MakeNo, sc.SerialNo, sc.VnrCode, sc.BoxCtrlNo, sc.PalletCtrlNo, sc.Qty);
				}
			}
			return result;
		}

		/// <summary>
		/// 執行庫存異動 db commit
		/// </summary>
		public void SaveChange()
		{
      if (_stockChangeList == null)
				return;
			;

			//var addF1913List = new List<F1913>();
			//_wmsLogHelper.AddRecord(string.Format("[執行時間:{0}]庫存異動 開始", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));

			_stockChangeList = _stockChangeList.Where(x => x.Qty != 0).ToList();

			if (!_stockChangeList.Any())
			{
				_wmsLogHelper.StopRecord();
				return;
			}
				

			_f1913Repo.ExecSpStockChange(_stockChangeList.ToDataTable("UT_StockChange"), _wmsLogHelper.BatchNo);

			//foreach (var sc in _stockChangeList)
			//{
			//	if (sc.Qty == 0)
			//		continue;

			//	var stock = _f1913Repo.FindDataByKey(sc.DcCode, sc.GupCode, sc.CustCode, sc.ItemCode, sc.LocCode,
			//	sc.ValidDate, sc.EnterDate, sc.VnrCode, sc.SerialNo, sc.BoxCtrlNo, sc.PalletCtrlNo, sc.MakeNo);

			//	var now = DateTime.Now;
			//	if (stock == null && sc.Qty > 0)
			//		addF1913List.Add(new F1913
			//		{
			//			DC_CODE = sc.DcCode,GUP_CODE=sc.GupCode,CUST_CODE =sc.CustCode,LOC_CODE = sc.LocCode,
			//			ITEM_CODE = sc.ItemCode,VALID_DATE = sc.ValidDate,ENTER_DATE =sc.EnterDate,
			//			VNR_CODE = sc.VnrCode,SERIAL_NO =sc.SerialNo,BOX_CTRL_NO =sc.BoxCtrlNo,PALLET_CTRL_NO =sc.PalletCtrlNo,
			//			MAKE_NO = sc.MakeNo,QTY = sc.Qty,CRT_DATE = now,CRT_STAFF = Current.Staff,CRT_NAME = Current.StaffName
			//		});
			//	else
			//		_f1913Repo.AdjustStockQty(sc.DcCode, sc.GupCode, sc.CustCode, sc.LocCode, sc.ItemCode,
			//			sc.ValidDate, sc.EnterDate, sc.MakeNo, sc.SerialNo, sc.VnrCode, sc.BoxCtrlNo, sc.PalletCtrlNo, sc.Qty);

			//	_wmsLogHelper.AddRecord(string.Format(_stockChangeMsgFormat, sc.DcCode, sc.GupCode, sc.CustCode, sc.LocCode, sc.ItemCode,
			//	sc.ValidDate.ToString("yyyy/MM/dd"), sc.EnterDate.ToString("yyyy/MM/dd"), sc.MakeNo, sc.SerialNo, sc.VnrCode, sc.BoxCtrlNo, sc.PalletCtrlNo,
			//	sc.Qty >= 0 ? "增加" : "減少", Math.Abs(sc.Qty)));
			//}

			//if (addF1913List.Any())
			//	_f1913Repo.BulkInsert(addF1913List,true);

			//_wmsLogHelper.AddRecord(string.Format("[執行時間:{0}]庫存異動 結束", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
	
			// 清除暫存紀錄
			_f1913List = null;
			_stockChangeList = null;
			_wmsLogHelper.StopRecord(false);
		}

		#endregion

		

		#region 配庫商品鎖定處理

		public bool CheckAllotStockStatus(bool isAutoAllocStock,string allotBatchNo,List<ItemKey> itemCodes)
		{
			var _batchMaxCount = 200;
			var f0501Repo = new F0501Repository(Schemas.CoreSchema);
			var f5 = f0501Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
				new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }), () =>
				{
					f0501Repo.LockF0501();
					var canDo = false;
					var chkTimes = 0;
					do
					{
						chkTimes++;
						var f0501List = new List<F0501>();
						var groupByCusts = itemCodes.GroupBy(x => new { x.DcCode, x.GupCode, x.CustCode }).ToList();
						foreach (var group in groupByCusts)
						{
							var gItemCodes = group.Select(x => x.ItemCode).Distinct().ToList();
							var batchItems = new List<List<string>>();
							if (gItemCodes.Count > _batchMaxCount)
							{
								var pages = gItemCodes.Count / _batchMaxCount + (gItemCodes.Count % _batchMaxCount > 0 ? 1 : 0);
								for (var page = 0; page < pages; page++)
									batchItems.Add(gItemCodes.Skip(page * _batchMaxCount).Take(_batchMaxCount).ToList());
							}
							else
								batchItems.Add(gItemCodes);

							foreach (var batchItem in batchItems)
							{
								var f0501s = f0501Repo.GetF0501s(group.Key.DcCode, group.Key.GupCode, group.Key.CustCode, batchItem).ToList();
								f0501List.AddRange(f0501s);
							}
						}
						if (f0501List.Any(x => x.IS_LOCK == "1"))
						{
							if (!isAutoAllocStock)
								return new F0501 { IS_LOCK = "1" };

							System.Threading.Thread.Sleep(30000);
							return new F0501 { IS_LOCK = "1" };
						}
						else
						{
							f0501List.ForEach(x => { x.IS_LOCK = "1"; x.ALLOT_BATCH_NO = allotBatchNo; });
							f0501Repo.BulkUpdate(f0501List);
							canDo = true;
							return new F0501 { IS_LOCK = "0" };
						}
					} while (!canDo && chkTimes <= 6);
				});
			// IS_LOCK=0 代表可以配庫  IS_LOCK=1代表不可以配庫
			return f5.IS_LOCK == "0";
		}

		/// <summary>
		/// 更新配庫狀態
		/// </summary>
		/// <param name="status"></param>
		public void UpdateAllotStockStatusToNotAllot(string allotBatchNo)
		{
			var f0501Repo = new F0501Repository(Schemas.CoreSchema);
			f0501Repo.UnLockByAllotBatchNo(allotBatchNo);
		}

		#endregion

		#region 取得配庫模式
		/// <summary>
		/// 取得配庫模式(0:配揀合一模式 1:配揀分離模式)
		/// </summary>
		/// <returns></returns>
		public string GetAllotStockMode()
		{
			var mode = ConfigurationManager.AppSettings["AllotStockMode"].ToString();
			return mode;
		}

		#endregion

	}
}
