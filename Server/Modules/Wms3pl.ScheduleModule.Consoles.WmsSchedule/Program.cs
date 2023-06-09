using ConsoleUtility.Helpers;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Schedule;

namespace Wms3pl.ScheduleModule.Consoles.WmsSchedule
{
	class Program
	{
		private static WmsScheduleParam _param;
		private static AppConfig _config;
		static void Main(string[] args)
		{
			SetArgConfig(args);
			Execute();
		}
    /// <summary>
    /// 設定指定參數物件
    /// </summary>
    /// <param name="args"></param>
    private static void SetArgConfig(string[] args)
		{
			_config = new AppConfig()
			{
				FilePath = string.Format(ConfigurationManager.AppSettings["FilePath"], DateTime.Today.ToString("yyyyMMdd"))
			};
			_param = new WmsScheduleParam();
			if (args.Any())
			{
				ConsoleHelper.ArgumentsTransform(args, _param);
				ConsoleHelper.ArgumentsTransform(args, _config);
			}
#if (DEBUG)
			//param.DcCode = "12";
			//param.GupCode = "10";
			//param.CustCode = "010001";
			_config.SchemaName = "PHWMS_DEV";
			_config.ScheduleNo = "01";
#endif
            Schemas.CoreSchema = _config.SchemaName;
      ConsoleHelper.FilePath = string.Format(ConfigurationManager.AppSettings["ExcelFilePath"], GetScheduleName(), DateTime.Today.ToString("yyyyMMdd"));
    }
		
		private static void Execute()
		{
			var res = new ApiResult();
			Log($" *** {GetScheduleName()}排程Start ***");
			try
			{
				switch (_config.ScheduleNo)
				{
					case "01": //自動產生揀貨資料排程
						res = WmsScheduleService.ExecCreatePick("01");
						break;
					case "02": //自動產生快速補揀單排程
						res = WmsScheduleService.ExecCreatePick("02");
						break;
					case "03": //自動揀缺配庫排程
						res = WmsScheduleService.ExecPickLackAllotStock();
						break;
					case "04": //集貨出場確認排程
						res = WmsScheduleService.ExecCollOutboundConfirm();
            break;
          case "05": //碼頭收貨每日備份排程
            res = WmsScheduleService.ExecDockDataTransferToHistory();
            break;
          case "06": //批次快速移轉庫存
            res = WmsScheduleService.ExecBatchFastTransferStock();
            break;
          case "07": //移除歷史Log紀錄排程
            res = WmsScheduleService.RemoveHistroyLog();
            break;
					case "08": //自動解鎖排程鎖定
						res = WmsScheduleService.ExecAutomaticUnlock(); 
						break;
					case "09": //跨庫出貨配庫補貨排程
            res = WmsScheduleService.ExecAllocateReplenishment(_param);
						break;
          case "10": //批次快速移轉序號排程-新增處理
            res = WmsScheduleService.ExecBatchFastSerialNoTransfer("01");
            break;
          case "11": //批次快速移轉序號排程-刪除處理
            res = WmsScheduleService.ExecBatchFastSerialNoTransfer("02");
            break;
					case "12": // 出貨扣帳排程
						res = WmsScheduleService.ExecShipOrderDebit();
						break;
					case "13": //商品進倉結果回傳排程
            res = WmsScheduleService.ExportWarehouseInResults(_param);
            break;
          case "14": //客戶訂單結果回傳排程
            res = WmsScheduleService.ExportOrderResults(_param);
            break;
          case "15": //廠退出貨結果回傳排程
            res = WmsScheduleService.ExportVendorReturnResults(_param);
            break;
          case "16": //庫存警示回報排程
            res = WmsScheduleService.ExpStockAlertResults(_param);
            break;
          case "17": //單箱出貨扣帳結果回傳排程
            res = WmsScheduleService.ExportHomeDeliveryResults(_param);
            break;
          case "18": //庫存異動回報排程
            res = WmsScheduleService.ExportStockMovementResults(_param);
            break;
          case "19": //LMS系統異常通知
            res = WmsScheduleService.ExportSysErrorNotifyResults(_param);
            break;
          case "20": //更新儲位容積排程
            res = WmsScheduleService.ExecUpdateLocVolumn(_param);
            break;
          case "21": //每日補貨排程
            res = WmsScheduleService.GetReplensihStock(_param);
            break;
          case "22": //自動配庫
            res = WmsScheduleService.AutoAllotStocks();
            break;
          case "23": //每日庫存結算
            _param.SettleType = "DailyStockSettle";
            res = WmsScheduleService.DailySettle(_param);
            break;
          case "24": //每日費用結算
            _param.SettleType = "DailyFeeSettle";
            res = WmsScheduleService.DailySettle(_param);
            break;
          case "25": //每日報表結算
            _param.SettleType = "SettleReport";
            res = WmsScheduleService.DailySettle(_param);
            break;
          case "26": //每日運費結算
            _param.SettleType = "DailyShipFeeSettle";
            res = WmsScheduleService.DailySettle(_param);
            break;
          case "27": //商品低周轉統計
            res = WmsScheduleService.ItemTurnoverRate();
            break;
          case "28": //一般揀貨轉黃金揀貨區
            res = WmsScheduleService.MoveGoldLocs();
            break;
          case "29": //黃金揀貨轉一般揀貨區
            res = WmsScheduleService.RemoveGoldLoc();
            break;
          case "30":  //使用上次計算儲位容積時間更新儲位容積
            res = WmsScheduleService.ExecUpdateLocVolumnByCalvolumnTime(_param);
            break;
          default:
						break;
				}
				if (res.IsSuccessed)
				{
					Log($"處理狀況：成功");
					Log($"處理結果代碼：[{res.MsgCode}]");
					Log($"處理結果訊息：{res.MsgContent}");
				}
				else
				{
					Log($"處理狀況：失敗");
					Log($"處理結果代碼：[{res.MsgCode}]");
					Log($"處理結果訊息：{res.MsgContent}");
				}
			}
			catch(Exception ex)
			{
				Log($"發生錯誤：{ex.ToString()}");
			}
			finally
			{
				Log($"*** {GetScheduleName()}排程 End ***");
				Log(string.Empty, false);
			}
		}

    private static string GetScheduleName()
    {
      switch (_config.ScheduleNo)
      {
        case "01": //自動產生揀貨資料排程
          return "AutoCreatePickOrder";
        case "02": //自動產生快速補揀單排程
          return "AutoCreateLackPickOrder";
        case "03": //自動揀缺配庫
          return "AutoPickLackAllotStock";
        case "04": //集貨出場確認排程
          return "CollOutboundConfirm";
        case "05": //碼頭收貨每日備份排程
          return "ExecDockDataTransferToHistory";
        case "06":
          return "ExecBatchFastTransferStock";
        case "07": //移除歷史Log紀錄排程
          return "RemoveHistroyLog";
        case "08": //自動解鎖排程鎖定
          return "AutomaticUnlock";
        case "09": //跨庫出貨配庫補貨排程
          return "ExecAllocateReplenishment";
        case "10": //批次快速移轉序號排程-新增
          return "ExecBatchFastSerialNoTransfer";
        case "11": //批次快速移轉序號排程-刪除
          return "ExecBatchFastSerialNoTransferDelete";
        case "12": // 出貨扣帳排程
          return "ExecShipOrderDebit";
        case "13": //商品進倉結果回傳排程
          return "ExportWarehouseInResults";
        case "14": //客戶訂單結果回傳排程
          return "ExportOrderResults";
        case "15": //廠退出貨結果回傳排程
          return "ExportVendorReturnResults";
        case "16": //庫存警示回報排程
          return "ExpStockAlertResults";
        case "17": //單箱出貨扣帳結果回傳排程
          return "ExportHomeDeliveryResults";
        case "18": //庫存異動回報排程
          return "ExportStockMovementResults";
        case "19": //LMS系統異常通知
          return "ExportSysErrorNotifyResults";
        case "20": //更新儲位容積排程
          return "ExecUpdateLocVolumn";
        case "21": //每日補貨排程
          return "GetReplensihStock";
        case "22": //自動配庫
          return "AutoAllotStocks";
        case "23": //每日庫存結算
          return "DailyStockSettle";
        case "24": //每日費用結算
          return "DailyFeeSettle";
        case "25": //每日報表結算
          return "SettleReport";
        case "26": //每日運費結算
          return "DailyShipFeeSettle";
        case "27": //商品低周轉統計
          return "ItemTurnoverRate";
        case "28": //一般揀貨轉黃金揀貨區
          return "MoveGoldLocs";
        case "29": //黃金揀貨轉一般揀貨區
          return "RemoveGoldLoc";
        case "30": //使用上次計算儲位容積時間更新儲位容積
          return "ExecUpdateLocVolumnByCalvolumnTime";

        default:
          return "NoSchedule";
      }
    }



    #region Log
    /// <summary>
    /// ����Log��ExportResults.txt
    /// </summary>
    /// <param name="message"></param>
    private static void Log(string message,bool isShowDatetime = true)
		{
			if (!Directory.Exists(_config.FilePath))
				Directory.CreateDirectory(_config.FilePath);

            var fileFullName = Path.Combine(_config.FilePath, $"WcsSchedule_{GetScheduleName()}.txt");

            using (var sw = new StreamWriter(fileFullName, true, Encoding.GetEncoding(950))) //BIG5
                sw.WriteLine(string.Format("{0} {1}", isShowDatetime ? DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") : string.Empty, message));
        }
        #endregion
    }
}
