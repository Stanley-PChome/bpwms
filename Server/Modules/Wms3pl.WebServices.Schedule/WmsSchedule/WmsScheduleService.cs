//using Wms3pl.Datas.Shared.ApiEntities;
//using Wms3pl.WebServices.DataCommon;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Schedule.Export.Services;
using Wms3pl.WebServices.Schedule.ReplenishStock.Services;
using Wms3pl.WebServices.Schedule.S00.Services;
using Wms3pl.WebServices.Schedule.S19.Services;
using Wms3pl.WebServices.Schedule.WmsSchedule;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Transaction.T05;
using Wms3pl.WebServices.Transaction.T05.Services;
//using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Schedule
{
	public class WmsScheduleService
	{
    /// <summary>
    /// 自動產生揀貨資料
    /// </summary>
    /// <param name="source">資料來源[01:訂單;02:揀缺;03:LMS揀貨批次分配失敗]</param>
    /// <returns></returns>
		public static ApiResult ExecCreatePick(string source)
    {
      var wmsTransation = new WmsTransaction();
      var pickServie = new SharedService(wmsTransation);
      var result = pickServie.AutoCreatePick(source);
			return result;
    }


    public static ApiResult ExecPickLackAllotStock()
		{
			var wmsTransation = new WmsTransaction();
			var pickServie = new SharedService(wmsTransation);
			var result = pickServie.AutoPickLackAllotStock();
			return result;
		}

		public static ApiResult ExecCollOutboundConfirm()
		{
			var wmsTransation = new WmsTransaction();
			var collectionServie = new SharedService(wmsTransation);
			var result = collectionServie.CollOutboundConfirm();
			return result;
		}

    public static ApiResult ExecDockDataTransferToHistory()
    {
      var wmsTransation = new WmsTransaction();
      var DockDataServie = new DockReceivingService(wmsTransation);
      var result = DockDataServie.DockDataTransferToHistory();
      return result;
    }

    public static ApiResult ExecBatchFastTransferStock()
    {
      var service = new BatchFastTransferStockService();
      var result = service.ExecBatchFastTransferStock();
      return result;
    }

    public static ApiResult RemoveHistroyLog()
    {
      var wmsTransation = new WmsTransaction();
      var service = new RemoveHistroyLogService(wmsTransation);
      var result = service.RemoveHistroyLog();
      return result;
    }

    /// <summary>
    /// 批次快速移轉序號排程 01:新增處理 02:刪除處理
    /// </summary>
    /// <param name="Mode">01:新增處理 02:刪除處理</param>
    /// <returns></returns>
    public static ApiResult ExecBatchFastSerialNoTransfer(string Mode)
    {
      var service = new BatchFastSerialNoTransferService();
      var result = service.ExecBatchFastSerialNoTransfer(Mode);
      return result;
    }

    public static ApiResult ExecAutomaticUnlock()
    {
      var wmsTransation = new WmsTransaction();
      var service = new AutomaticUnlockService(wmsTransation);
      var result = service.ExecAutomaticUnlock();
			return result;
		}

    public static ApiResult ExecAllocateReplenishment(WmsScheduleParam param)
    {
        var service = new AllocateReplenishmentService();
        var result = service.AllocateReplenishment(param);
        return result;
    }

		/// <summary>
		/// 執行出貨扣帳排程
		/// </summary>
		/// <returns></returns>
		public static ApiResult ExecShipOrderDebit()
		{
			var service = new ShipDebitService();
			return service.ExecShipOrderDebit();
		}
	

    /// <summary>
    /// 商品進倉結果回傳排程
    /// </summary>
    /// <returns></returns>
    public static ApiResult ExportWarehouseInResults(WmsScheduleParam param)
    {
      ExportServices epService = new ExportServices();
      var req = new ExportResultReq { DcCode = param.DcCode, GupCode = param.GupCode, CustCode = param.CustCode };
      return epService.ExportWarehouseInResults(req);
    }

    /// <summary>
    /// 客戶訂單結果回傳排程
    /// </summary>
    /// <returns></returns>
    public static ApiResult ExportOrderResults(WmsScheduleParam param)
    {
      ExportServices epService = new ExportServices();
      var req = new ExportResultReq { DcCode = param.DcCode, GupCode = param.GupCode, CustCode = param.CustCode };
      return epService.ExportOrderResults(req);

    }

    /// <summary>
    /// 廠退出貨結果回傳排程
    /// </summary>
    /// <returns></returns>
    public static ApiResult ExportVendorReturnResults(WmsScheduleParam param)
    {
      ExportServices epService = new ExportServices();
      var req = new ExportResultReq { DcCode = param.DcCode, GupCode = param.GupCode, CustCode = param.CustCode };
      return epService.ExportVendorReturnResults(req);
    }

    /// <summary>
    /// 庫存警示回報排程
    /// </summary>
    /// <returns></returns>
    public static ApiResult ExpStockAlertResults(WmsScheduleParam param)
    {
      ExportServices epService = new ExportServices();
      var req = new ExportResultReq { DcCode = param.DcCode, GupCode = param.GupCode, CustCode = param.CustCode };
      return epService.ExpStockAlertResults(req);
    }

    /// <summary>
    /// 單箱出貨扣帳結果回傳排程
    /// </summary>
    /// <returns></returns>
    public static ApiResult ExportHomeDeliveryResults(WmsScheduleParam param)
    {
      ExportServices epService = new ExportServices();
      var req = new ExportResultReq { DcCode = param.DcCode, GupCode = param.GupCode, CustCode = param.CustCode };
      return epService.ExportHomeDeliveryResults(req);
    }

    /// <summary>
    /// 庫存異動回報排程
    /// </summary>
    /// <returns></returns>
    public static ApiResult ExportStockMovementResults(WmsScheduleParam param)
    {
      ExportServices epService = new ExportServices();
      var req = new ExportResultReq { DcCode = param.DcCode, GupCode = param.GupCode, CustCode = param.CustCode };
      return epService.ExportStockMovementResults(req);
    }

    /// <summary>
    /// LMS系統異常通知
    /// </summary>
    /// <returns></returns>
    public static ApiResult ExportSysErrorNotifyResults(WmsScheduleParam param)
    {
      ExportServices epService = new ExportServices();
      var req = new ExportResultReq { DcCode = param.DcCode, GupCode = param.GupCode, CustCode = param.CustCode };
      return epService.ExportSysErrorNotifyResults(req);
    }

    /// <summary>
    /// 更新儲位容積排程
    /// </summary>
    /// <returns></returns>
    public static ApiResult ExecUpdateLocVolumn(WmsScheduleParam param)
    {
      var service = new LocService(new WmsTransaction());
      return service.ExecUpdateLocVolumn(param);
    }

    /// <summary>
    /// 每日補貨排程
    /// </summary>
    /// <returns></returns>
    public static ApiResult GetReplensihStock(WmsScheduleParam param)
    {
      var srv = new ReplenishStockService();
      var req = new BatchTransApiOrdersReq { DcCode = param.DcCode, GupCode = param.GupCode, CustCode = param.CustCode };
      return srv.ProcessApiDatas_Order(req);
    }

    /// <summary>
    /// 自動配庫
    /// </summary>
    /// <returns></returns>
    public static ApiResult AutoAllotStocks()
    {
      Current.DefaultStaff = "Trans";
      Current.DefaultStaffName = "Trans";
      var wmsTransaction = new WmsTransaction();
      var exeRes = new List<ExecuteResult>();
      var stockService = new StockService();
      if (stockService.GetAllotStockMode() == "0")
      {
        var srv = new T050101Service(wmsTransaction);
        exeRes = srv.AllotStocks().ToList();
        wmsTransaction.Complete();
      }
      else
      {
        var srv = new T050102Service(wmsTransaction);
        exeRes = srv.AllotStocks().ToList();
      }

      var res = new ApiResult() { IsSuccessed = exeRes.All(x => x.IsSuccessed) };
      res.MsgContent = string.Join("\r\n", exeRes.Select(x => x.Message));
      return res;
    }

    public static ApiResult DailySettle(WmsScheduleParam param)
    {
      var srv = new SettleService();
      return srv.ProcessApiDatas(param);
    }

    public static ApiResult ItemTurnoverRate()
    {
			return ApiLogHelper.CreateApiLogInfo("0", "0", "0", "ItemTurnoverRate", null, () =>
			{
				var srv = new RefineService();
				srv.ItemTurnoverRate();
				return new ApiResult() { IsSuccessed = true, MsgCode = "10001", MsgContent = "商品低周轉統計完成" };
				}, true);

    }

    public static ApiResult MoveGoldLocs()
    {
			return ApiLogHelper.CreateApiLogInfo("0", "0", "0", "MoveGoldLocs", null, () =>
			{
				var srv = new RefineService();
				srv.MoveGoldLocs();
				return new ApiResult() { IsSuccessed = true, MsgCode = "10001", MsgContent = "一般揀貨轉黃金揀貨區完成" };
			}, true);
    }

    public static ApiResult RemoveGoldLoc()
    {
			return ApiLogHelper.CreateApiLogInfo("0", "0", "0", "RemoveGoldLoc", null, () =>
			{
				var srv = new RefineService();
				srv.RemoveGoldLoc();
				return new ApiResult() { IsSuccessed = true, MsgCode = "10001", MsgContent = "黃金揀貨轉一般揀貨區" };
			}, true);
    }
    /// <summary>
    /// 使用上次計算儲位容積時間更新儲位容積
    /// </summary>
    /// <returns></returns>
    public static ApiResult ExecUpdateLocVolumnByCalvolumnTime(WmsScheduleParam param)
    {
      var service = new LocService(new WmsTransaction());
      var req = new ExecUpdateLocVolumnParam { DcCode = param.DcCode };
      return service.ExecUpdateLocVolumnByCalvolumnTime(req);
    }


		/// <summary>
		/// 跨庫調撥出貨分配扣帳排程
		/// </summary>
		/// <returns></returns>
		public static ApiResult MoveOutShipOrderDebit()
		{
			var service = new ShipDebitService();
			return service.ExecMoveOutShipOrderDebit();
		}

    /// <summary>
    /// 跨庫調撥出貨分配扣帳排程
    /// </summary>
    /// <returns></returns>
    public static ApiResult ImmediateItemSchedule()
    {
      var service = new ImmediateItemScheduleService();
      return service.ExecImmediateItemSchedule();
    }

    /// <summary>
    /// 出貨單據完成通知排程
    /// </summary>
    /// <returns></returns>
    public static ApiResult ShipFinishConfirmNotify()
    {
      var service = new ShipFinishConfirmNotifyService();
      return service.ExecShipFinishConfirmNotify();
    }


  }
}
