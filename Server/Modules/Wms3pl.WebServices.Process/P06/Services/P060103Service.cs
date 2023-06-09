
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P20.Services;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.F06;

namespace Wms3pl.WebServices.Process.P06.Services
{
	public partial class P060103Service
	{
		private WmsTransaction _wmsTransaction;
		public P060103Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 取得虛擬儲位查詢結果
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="delvDate">批次日期</param>
		/// <param name="pickTime">批次時段</param>
		/// <param name="custOrdNo">貨主單號</param>
		/// <param name="ordNo">訂單編號</param>
		/// <returns></returns>
		public IQueryable<F05030101Ex> GetP060103Data(string gupCode, string custCode, string dcCode, DateTime delvDate, string pickTime, string custOrdNo, string ordNo, string itemCode)
		{
			// 確認要回復虛擬儲位時，會先做檢查是否已取消，所以加入交易。

			var f05030101Repository = new F05030101Repository(Schemas.CoreSchema, _wmsTransaction);
			return f05030101Repository.GetP060103Data(gupCode, custCode, dcCode, delvDate, pickTime, custOrdNo, ordNo, itemCode);
		}

		//虛擬儲位回復商業邏輯:
		//Step 1. 使用者會先以"批次日期、批次時段、貨主單號、訂單編號"查詢出可取消的訂單清單，
		//		  其中併單時，訂單處置狀態必須全部為取消或者出貨單為不出貨，否則不回復且提示資料錯誤。
		//Step 2. 當使用者勾選要取消的訂單，以訂單編號找到關聯的訂單號、出貨單號、揀貨單號、揀貨序號、項目編號、項目序號、項目效期、來源儲位、倉別，
		//		  其中並單時，必須勾選併單的全部訂單，否則不回復且提示訊息。
		//Step 3. 將該揀貨單號、揀貨序號關聯的虛擬儲位數量設為 0, 並將狀態設為取消。
		//Step 4. 建立調撥單(F151001, F151002)，欄位格式如下:
		//-----調撥單主檔----
		//調撥單日期、建立調撥單日期: DateTime.Now.Date
		//調撥單號: TyyyyMMdd00SEQ (GetNewOrdCode("T")
		//調撥單狀態: 3(已下架處理)
		//目的倉別、來源倉別: 一樣從 F1912 的儲位找到關聯的倉別 WAREHOUSE_ID
		//目的物流中心、來源物流中心: DCCODE
		//物流中心、業主、貨主: 填來源
		//來源單據: P(揀貨單) *先不填
		//來源單號: 揀貨單號  *先不填
		//-----調撥單明細-----
		//調撥狀態: default
		//商品編號: 訂單明細的 ITEM_CODE
		//來源儲位、目的儲位、建議上架儲位: 填 1511 SRC_LOC
		//下架數: 0
		//上架數: 實際揀貨數
		//實際上架數、實際下架數: default
		//商品效期: 揀貨單明細的效期
		//是否已刷讀序號: default
		//物流中心、業主、貨主: 填來源
		//商品序號: 如果訂單明細有的話
		//來源單據: P(揀貨單) *先不填
		//來源單號: 揀貨單號  *先不填
		public ExecuteResult ConfirmP060103(string gupCode, string custCode, string dcCode, List<string> ordNoList)
		{
			ExecuteResult result;

			// 檢查輸入參數(來源參考GetP060103Data)
			if (gupCode == null || custCode == null || dcCode == null || ordNoList == null || !ordNoList.Any())
			{
				return new ExecuteResult() { Message = Properties.Resources.P060103Service_CheckNeeRecoverVirtualLoc };
			}

			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema, _wmsTransaction);   // 訂單與出貨單關聯表
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);           // 虛擬儲位(對應揀貨單明細)
			var f051201Repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
      var f051301Repo = new F051301Repository(Schemas.CoreSchema);
			var f05500101Repo = new F05500101Repository(Schemas.CoreSchema, _wmsTransaction);
			var sharedService = new SharedService(_wmsTransaction);                             // 單據號碼
      var orderService = new OrderService(_wmsTransaction);
      var shipPackageService = new ShipPackageService(_wmsTransaction);
			var containerService = new ContainerService(_wmsTransaction);
			List<F060302> addF060302 = new List<F060302>();
			List<F0701> deleteF0701 = new List<F0701>();
      List<F1511> updF1511 = new List<F1511>();

      orderService.SharedService = sharedService;

      // 取得要回復虛擬儲位的相關資料
      if (!ordNoList.Any())
        return new ExecuteResult(false, "請選擇訂單");

			var f05030101WithF051202Data = f05030101Repo.GetF05030101WithF051202Data(gupCode, custCode, dcCode, ordNoList).ToList();

      #region 資料檢核


      #region 檢查是否還有虛擬庫存未回復
      //只處理F1511.STATUS=0 & 1的內容
      f05030101WithF051202Data = f05030101WithF051202Data.Where(x => new[] { "0", "1" }.Contains(x.F1511_STATUS)).ToList();
      if (!f05030101WithF051202Data.Any())
        return new ExecuteResult(false, $"訂單{string.Join(",", ordNoList.Distinct())}無虛擬庫需要回復，請重新查詢");
      #endregion 檢查是否還有虛擬庫存未回復

      #region 訂單未揀貨完成，無法進行儲位回復
      List<string> errorOrdNoList = new List<string>();

			var wmsOrdNos = f05030101WithF051202Data.Select(x => x.WMS_ORD_NO).Distinct().ToList();
			var notFinishOrdNos = f051201Repo.GetNotFinishOrdNos(dcCode, gupCode, custCode, wmsOrdNos).ToList();

			if (notFinishOrdNos.Any())
				return new ExecuteResult() { IsSuccessed = false, Message = string.Format(Properties.Resources.P060103Service_CannotBeRecovered, string.Join("、", notFinishOrdNos)) };
      #endregion

      #region 檢查出貨單集貨狀態
      var f051301s = f051301Repo.GetDatasByWmsOrdNos(dcCode, gupCode, custCode, wmsOrdNos).ToList();
      var CollectionMessage = new List<string>();
      if (f051301s.Any(x => x.COLLECTION_POSITION == "0" && x.STATUS != "1"))
      {
        //撈出出貨批次需集貨資料
        var UnCollectionWmsOrdNos = f051301s.Where(x => x.COLLECTION_POSITION == "0" && new[] { "0", "2" }.Contains(x.STATUS)).Select(x => x.WMS_NO).Distinct();
        if (UnCollectionWmsOrdNos.Any())
          //撈出原始訂單編號
          CollectionMessage.Add($"該出貨單{string.Join(",", UnCollectionWmsOrdNos)}尚未完成集貨，請先完成集貨");

        UnCollectionWmsOrdNos = f051301s.Where(x => x.COLLECTION_POSITION == "0" && x.STATUS == "3").Select(x => x.WMS_NO).Distinct();
        if (UnCollectionWmsOrdNos.Any())
          CollectionMessage.Add($"該出貨單{string.Join(",", UnCollectionWmsOrdNos)}尚未集貨出場，請先進行集貨出場");

        if (CollectionMessage.Any())
          return new ExecuteResult(false, string.Join("\r\n", CollectionMessage));
      }
      #endregion 檢查出貨單集貨狀態

			// 驗證某筆出貨單狀態不是不出貨，且關聯的訂單處置狀態有一個不是取消
			if (!TryValidateNotCancelOrder(f05030101WithF051202Data, out result))
				return result;

			// 驗證選擇的訂單在同一張出貨單內是否沒被全部選擇
			if (!TryValidateSelectedOrderInWmsOrder(ordNoList, f05030101WithF051202Data, out result))
				return result;

      #endregion 資料檢核
      //回復包材
      // 取得要取消的出貨單號
      foreach (var wmsOrd in wmsOrdNos)
			{

				var shareResult = sharedService.ReturnBoxQty(dcCode, gupCode, custCode, wmsOrd);
				if (!shareResult.IsSuccessed)
					return shareResult;
			}

      //Step 4 : 將未揀貨的揀貨明細回復至庫存儲位中
      var f1511s = f1511Repo.AsForUpdate()
        .GetDatas(dcCode, gupCode, custCode, f05030101WithF051202Data.Select(x => x.ORDER_NO).Distinct().ToList())
        .ToList();

      //只要把STATUS=0的丟去處裡虛擬庫存回復
      orderService.RestoreVirtualStock(f1511s.Where(x => x.STATUS == "0").ToList());

      // Step 4: by 倉別建立調撥單
      var allocationList = new List<string>();
			var today = DateTime.Now.Date;

			//調撥單資料暫存
			List<ReturnNewAllocation> allocationListTemp = new List<ReturnNewAllocation>();

			//庫存暫存
			var stockListTemp = new List<F1913>() { };

			//調撥單號
			string ticketMessage = string.Empty;

      foreach (var warehouseIdGroup in f05030101WithF051202Data.Where(o => o.A_PICK_QTY > 0 && o.F1511_STATUS == "1").GroupBy(item => item.WAREHOUSE_ID))
      {
        var tmpUpdF1511s = (from f1511 in f1511s
                            join item in warehouseIdGroup
                             on new { f1511.ORDER_NO, f1511.ORDER_SEQ } equals new { item.ORDER_NO, item.ORDER_SEQ }
                            select f1511).ToList();
        tmpUpdF1511s.ForEach(x =>
        {
          x.STATUS = "9";
          x.A_PICK_QTY = 0;
        });
        updF1511.AddRange(tmpUpdF1511s);


        // 由於可能會併單，避免重複新增將揀貨資料
        var srcItemLocQtyList = (from item in warehouseIdGroup
                                 group item by new
																 {
                                   item.ORD_NO,
																	 item.ORDER_NO,
																	 item.ORDER_SEQ,
																	 item.ITEM_CODE,
																	 item.SERIAL_NO,
																	 item.PICK_LOC,
																	 item.A_PICK_QTY,
																	 item.VALID_DATE,
																	 item.DC_CODE,
																	 item.GUP_CODE,
																	 item.CUST_CODE,
																	 item.VNR_CODE,
																	 item.ENTER_DATE,
																	 item.BOX_CTRL_NO,
																	 item.PALLET_CTRL_NO,
																	 item.MAKE_NO
																 } into g
																 select new StockDetail
																 {
                                   CustCode = custCode,
																	 GupCode = gupCode,
																	 SrcDcCode = dcCode,
																	 TarDcCode = dcCode,
																	 SrcWarehouseId = "",
																	 TarWarehouseId = warehouseIdGroup.Key,
																	 SrcLocCode = g.Key.PICK_LOC,
																	 TarLocCode = g.Key.PICK_LOC,
																	 ItemCode = g.Key.ITEM_CODE,
																	 ValidDate = g.Key.VALID_DATE,
                                   EnterDate = g.Key.ENTER_DATE,
                                   Qty = g.Key.A_PICK_QTY,
                                   VnrCode = g.Key.VNR_CODE,
                                   SerialNo = g.Key.SERIAL_NO,
                                   BoxCtrlNo = g.Key.BOX_CTRL_NO,
                                   PalletCtrlNo = g.Key.PALLET_CTRL_NO,
                                   MAKE_NO = g.Key.MAKE_NO,
                                   SourceType = "01",
                                   SourceNo = g.Key.ORD_NO,
                                   ReferenceNo = g.Key.ORDER_NO,
                                   ReferenceSeq=g.Key.ORDER_SEQ
                                 }).ToList();
				NewAllocationItemParam allocationDate = new NewAllocationItemParam() { StockDetails = srcItemLocQtyList, SrcDcCode = dcCode, TarDcCode = dcCode, GupCode = gupCode, CustCode = custCode, ReturnStocks = stockListTemp, AllocationType = Shared.Enums.AllocationType.NoSource, Memo = Properties.Resources.P060103Service_RecoverVirtualLoc, AllocationTypeCode = "1" };
				var createAllocationResult = sharedService.CreateOrUpdateAllocation(allocationDate);

				if (!createAllocationResult.Result.IsSuccessed)
					return createAllocationResult.Result;
				else
				{
					ticketMessage = string.IsNullOrWhiteSpace(ticketMessage) ? string.Join(",", createAllocationResult.AllocationList.Select(o => o.Master.ALLOCATION_NO).ToArray()) : string.Format("{0},{1}", ticketMessage, string.Join(",", createAllocationResult.AllocationList.Select(o => o.Master.ALLOCATION_NO).ToArray()));
					stockListTemp = createAllocationResult.StockList;
					allocationListTemp.AddRange(createAllocationResult.AllocationList);
				}
			}

			var allocationResult = sharedService.BulkInsertAllocation(allocationListTemp, stockListTemp);
			if (!allocationResult.IsSuccessed)
				return allocationResult;
			allocationList.AddRange(ticketMessage.Split(','));

			foreach (var allocationNo in allocationList)
			{
				var shareResult = sharedService.UpdateSourceNoStatus(SourceType.Allocation, dcCode, gupCode, custCode, allocationNo, "3");
				if (!shareResult.IsSuccessed)
					return shareResult;
			}


      // 出貨單已經完成出貨包裝時，若有序號商品，則需要將已通過的序號商品的狀態回復為A1 
      var f05500101s = f05500101Repo.GetItemCodeAndSerialNo(dcCode, gupCode, custCode, wmsOrdNos).ToList();
			
			// 若查無資料表示不是序號商品
			if (f05500101s.Any())
			{
        //把序號改回A1，如果序號狀態本來就是A1(進貨)，就不用更新
        foreach (var item in f05500101s.GroupBy(x => x.WMS_ORD_NO))
          shipPackageService.UpdateF2501(dcCode, gupCode, custCode, item.Key);


        //    var groupF05500101s = f05500101s.GroupBy(g => new { g.DC_CODE, g.GUP_CODE, g.CUST_CODE, g.ITEM_CODE, g.SERIAL_NO });
        //foreach (var f05500101 in f05500101s)
        //{
        //	var f2501 = f2501Repo.GetDatasByTrueAndCondition(x =>  x.GUP_CODE == gupCode &&
        //											x.CUST_CODE == custCode &&
        //											x.SERIAL_NO == f05500101.SERIAL_NO ).FirstOrDefault();
        //	if (f2501 != null)
        //	{
        //		f2501.STATUS = "A1";
        //		f2501.WMS_NO = string.Empty;
        //	}
        //	updateF2501Data.Add(f2501);
        //}
        //f2501Repo.BulkUpdate(updateF2501Data);
      }

			#region 把自動倉揀貨完成的任務序號改為未同步

			var normalWmsNos = f05030101WithF051202Data.Where(x => x.F050301_CUST_COST != "MoveOut").Select(x => x.WMS_ORD_NO).Distinct().ToList();
			var crossWmsNos = f05030101WithF051202Data.Where(x => x.F050301_CUST_COST == "MoveOut").Select(x => x.WMS_ORD_NO).Distinct().ToList();
			var serialNoList = new List<string>();
			if(normalWmsNos.Any())
			{
				// 一般出貨，取得自動倉回傳的序號
				var f060202Repo = new F060202Repository(Schemas.CoreSchema);
				var restoreSerialNos = f060202Repo.GetAutoPickSerialNosByWmsNos(dcCode, gupCode, custCode, wmsOrdNos).ToList();
				serialNoList.AddRange(restoreSerialNos.Where(x => !string.IsNullOrWhiteSpace(x.SERIALNUMLIST)).SelectMany(x => x.SERIALNUMLIST.Split(',')).ToList());
			}
			if (crossWmsNos.Any())
			{
				// 跨庫訂單，取得稽核出庫取消訂單分出來的序號並且揀貨單為自動倉
				var f05290501Repo = new F05290501Repository(Schemas.CoreSchema);
				var restoreDatas = f05290501Repo.GetF05290501ByWmsNos(dcCode, gupCode, custCode, wmsOrdNos).ToList();
				serialNoList.AddRange(restoreDatas.Where(x => !string.IsNullOrWhiteSpace(x.SERIAL_NO)).Select(x => x.SERIAL_NO).Distinct().ToList());
			}
		  if(serialNoList.Any())
			{
				var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
				f2501Repo.UpdateIsAsync("N", gupCode, custCode, serialNoList);
			}
			#endregion 自動倉揀貨完成將序號改為未同步

			#region 釋放容器
			foreach (var wmsOrdNo in wmsOrdNos)
			{
				var delRes = containerService.DelContainer(dcCode, gupCode, custCode, wmsOrdNo);
				if (!delRes.IsSuccessed)
					return delRes;
			}
			#endregion 釋放容器

      //解決完全揀缺，做虛擬儲位回復後，F1511沒有壓成9
      var allLacks = f1511s.Where(x => x.STATUS == "1" && x.B_PICK_QTY == 0).ToList();
      if (allLacks.Any())
      {
        foreach (var item in allLacks)
          item.STATUS = "9";
        updF1511.AddRange(allLacks);
      }

      if (updF1511.Any())
        f1511Repo.BulkUpdate(updF1511);

			return new ExecuteResult()
			{
				IsSuccessed = true,
				Message = Properties.Resources.P060103Service_RecoverOrder + (string.IsNullOrWhiteSpace(allocationList.FirstOrDefault()) ? "" : string.Format(Environment.NewLine + Properties.Resources.P060103Service_TransORDNO, string.Join("," + Environment.NewLine, allocationList.ToArray())))
			};
		}

		/// <summary>
		/// 驗證某筆出貨單狀態不是不出貨，且關聯的訂單處置狀態有一個不是取消，則會顯示資料錯誤，不可回復
		/// </summary>
		/// <param name="f05030101WithF051202Data"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		private bool TryValidateNotCancelOrder(List<F05030101WithF051202> f05030101WithF051202Data, out ExecuteResult result)
		{
			result = null;

			var wmsOrder = (from item in f05030101WithF051202Data
											group item by new { item.WMS_ORD_NO, item.F581_STATUS } into g
											// 某筆出貨單狀態不是不出貨，且關聯的訂單處置狀態有一個不是取消，表示訂單資料錯誤，不可回復
											where g.Key.F581_STATUS != 9 && g.Where(item => item.PROC_FLAG != "9").Any()
											select g).FirstOrDefault();

			if (wmsOrder != null)
			{
				// Sxxxxxx1, Sxxxxxx2 訂單資料錯誤，不可回復
				var ordNos = string.Join(", ", wmsOrder.Select(item => item.ORD_NO).Distinct());
				result = new ExecuteResult() { Message = ordNos + Properties.Resources.P060103Service_ORD_DataError };
			}

			return (result == null);
		}

		/// <summary>
		/// 驗證選擇的訂單在同一張出貨單內是否沒被全部選擇，則會顯示 為合併訂單，需全部回復
		/// </summary>
		/// <param name="selectedOrdNoList"></param>
		/// <param name="f05030101WithF051202Data"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		private bool TryValidateSelectedOrderInWmsOrder(List<string> selectedOrdNoList, List<F05030101WithF051202> f05030101WithF051202Data, out ExecuteResult result)
		{
			result = null;
			var wmsOrder = (from item in f05030101WithF051202Data
											group item by item.WMS_ORD_NO into g
											// 檢查同一個出貨單下的訂單，是否沒有被選擇
											where g.Select(item => item.ORD_NO)
													.Distinct()
													.Where(ordNo => !selectedOrdNoList.Contains(ordNo)).Any()
											select g).FirstOrDefault();

			if (wmsOrder != null)
			{
				// Sxxxxxx1, Sxxxxxx2 為合併訂單，需全部回復
				var ordNos = string.Join(", ", wmsOrder.Select(item => item.ORD_NO).Distinct());
				result = new ExecuteResult() { Message = ordNos + Properties.Resources.P060103Service_NeedAllRecover };
			}

			return (result == null);
		}

		public ExecuteResult GetContainerBarcode(string dcCode,string gupCode,string custCode, List<string> ordNos)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f070101Repo = new F070101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransaction);
			//var f050301Repo = new F050301Repository(Schemas.CoreSchema, _wmsTransaction);

			var f05030101s = f05030101Repo.GetOrdNoDatas(dcCode,gupCode,custCode, ordNos);
			//出貨單清單
			var wmsOrdNos = f05030101s.Select(x => x.WMS_ORD_NO);
			if(wmsOrdNos == null || !wmsOrdNos.Any())
			{
				result.IsSuccessed = false;
				result.Message = "訂單尚未配庫";
				return result;
			}

			//容器編號ID清單
			var f0701Ids = f070101Repo.GetDatasByWmsOrdNos(dcCode, gupCode, custCode, wmsOrdNos.ToList()).Select(x=>x.F0701_ID);
			if (!f0701Ids.Any())
			{
				result.IsSuccessed = false;
				result.Message = "無容器資料";
				return result;
			}
			else
			{
				//使用中容器清單
				var f0701s = f0701Repo.GetDatasByF0701Ids(dcCode, gupCode, custCode, f0701Ids.ToList());

				if (!f0701s.Any())
				{
					result.IsSuccessed = false;
					result.Message = "無容器資料";
					return result;

				}
				result.No = string.Join(",", f0701s.Select(item => item.CONTAINER_CODE).Distinct());
			}
			return result;
			
		}
	}
}
