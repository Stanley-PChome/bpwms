using ConsoleUtility.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.WcsService;
using Wms3pl.WebServices.Shared.WcsServices;
using Wms3pl.WebServices.ToWcsWebApi.Business.mssql.Services;

namespace Wms3pl.ScheduleModule.Consoles.WcsExportData
{
  /// <summary>
  /// 
  /// </summary>
  class Program
  {
    private static AppConfig _appConfig;
    private static WcsExportReq param;

    static void Main(string[] args)
    {
      SetAppConfig();
      SetArgConfig(args);
      Init();
      Execute();
    }

    private static void SetAppConfig()
    {
      _appConfig = new AppConfig()
      {
        FilePath = string.Format(ConfigurationManager.AppSettings["FilePath"], DateTime.Today.ToString("yyyyMMdd"))
      };
    }

    /// <summary>
    /// 設定指定參數物件
    /// </summary>
    /// <param name="args"></param>
    private static void SetArgConfig(string[] args)
    {
      param = new WcsExportReq();
      if (args.Any())
      {
        ConsoleHelper.ArgumentsTransform(args, param);
        ConsoleHelper.ArgumentsTransform(args, _appConfig);
      }
#if (DEBUG)
			_appConfig.SchemaName = "PHWMS_DEV";
			//param.DcCode = "12";
			//param.GupCode = "10";
			//param.CustCode = "010001";
			param.ScheduleNo = "10";
#endif
      Schemas.CoreSchema = _appConfig.SchemaName;
    } 

    /// <summary>
    /// 起始
    /// </summary>
    private static void Init()
    {
      if (!Directory.Exists(_appConfig.FilePath))
        Directory.CreateDirectory(_appConfig.FilePath);
    }

    #region 排程
    private static void Execute()
    {
      switch (param.ScheduleNo)
      {
        case "01":
          ExecuteSchedule("User", "人員資訊");
          break;
        case "02":
          ExecuteSchedule("Inbound", "入庫任務");
          break;
        case "03":
          ExecuteSchedule("InboundCancel", "入庫任務取消");
          break;
        case "04":
          ExecuteSchedule("Outbound", "出庫任務");
          break;
        case "05":
          ExecuteSchedule("OutboundCancel", "出庫任務取消");
          break;
        case "06":
          ExecuteSchedule("Container", "容器釋放");
          break;
        case "07":
          ExecuteSchedule("ItemSnCancel", "序號刪除");
          break;
        case "08":
          ExecuteSchedule("StockCheck", "盤點任務");
          break;
        case "09":
          ExecuteSchedule("InventoryAdjust", "盤點調整任務");
          break;
        case "10":
          ExecuteSchedule("ItemAsync", "商品主檔同步");
          break;
        case "11":
          ExecuteSchedule("ItemSnAsync", "商品序號同步");
          break;
        case "12":
          ExecuteSchedule("CollectionStatus", "集貨等待通知");
          break;

        case "":
        case "999":
          ExecuteSchedule("WorkStation", "工作站狀態同步");
          break;
        case null:
          ExecuteSchedule("User", "人員資訊");
          ExecuteSchedule("Inbound", "入庫任務");
          ExecuteSchedule("InboundCancel", "入庫任務取消");
          ExecuteSchedule("Outbound", "出庫任務");
          ExecuteSchedule("OutboundCancel", "出庫任務取消");
          break;
      }
    }

    /// <summary>
    /// 排程執行
    /// </summary>
    private static void ExecuteSchedule(string type, string name)
    {
      ConsoleHelper.TypeName = type;
      ConsoleHelper.FilePath = _appConfig.FilePath;
      ApiResult res = new ApiResult();
      ConsoleHelper.Log($" *** {name}排程Start ***");
      try
      {
        switch (type)
        {
          case "User": // 人員主檔
            res = new WcsUserServices().User(param);
            break;
          case "Inbound": // 入庫任務
            res = new WcsInboundServices().Inbound(param);
            break;
          case "InboundCancel": // 入庫任務取消
            res = new WcsInboundServices().InboundCancel(param);
            break;
          case "Outbound": // 出庫任務
            res = new WcsOutboundServices().Outbound(param);
            break;
          case "OutboundCancel": // 出庫任務取消
            res = new WcsOutboundServices().OutboundCancel(param);
            break;
          case "Container": // 容器釋放
            res = new WcsContainerServices().Container(param);
            break;
          case "ItemSnCancel": // 序號刪除
            res = new WcsSnCancelServices().SnCancel(param);
            break;
          case "StockCheck": // 盤點任務
            res = new WcsInventoryConfirmService().StockCheck(param);
            break;
          case "InventoryAdjust": // 盤點調整任務
            res = new WcsInventoryConfirmService().InventoryAdjust(param);
            break;
          case "ItemSnAsync": // 商品序號主檔
            res = new WcsItemServices().ItemSnAsync(param);
            break;
          case "ItemAsync": // 商品主檔同步
            res = new WcsItemServices().ItemAsync(param);
            break;
          case "CollectionStatus": // 集貨等待通知排程
            res = new WcsCollectionServices().CollectionServices(param);
            break;
          case "WorkStation": //工作站狀態同步(先用排程做測試用)
            var workStation = new WcsStationReq
            {
              OwnerCode = "010001",
              StationTotal = 2,   //列表筆數
              StationList = new List<WcsStationModel> {
                new WcsStationModel
                {
                  StationCode = "AG001",	//工作站編號
									StationType = "PA1",	// 工作站類型(PA1、PA2)
									Status = 0,				//工作站狀態(0=關站, 1=開站, 2=暫停 )
								}
              }
            };
            res = new WcsWorkStationServices().Station(workStation, "12");
            break;
        }

        if (res.IsSuccessed)
        {
          ConsoleHelper.Log($"處理狀況：成功");

          var apiDataList = res.Data != null ? JsonConvert.DeserializeObject<List<ApiResponse>>(res.Data.ToString()) : null;
          if (apiDataList != null && apiDataList.Any())
          {
            ConsoleHelper.Log($"處理明細：");

            apiDataList.ForEach(o =>
            {
              if (!string.IsNullOrWhiteSpace(o.No))
                ConsoleHelper.Log($"物流中心服務貨主：[{o.No}]");

              ConsoleHelper.Log($"處理結果：{o.MsgContent}");
            });
          }
        }
        else
        {
          ConsoleHelper.Log($"處理狀況：失敗");
          ConsoleHelper.Log($"錯誤訊息：[{res.MsgCode}]{res.MsgContent}");
        }
      }
      catch (Exception ex)
      {
        ConsoleHelper.Log($"發生錯誤：{ex.ToString()}");
      }

      ConsoleHelper.Log($"*** {name}排程 End ***");
      ConsoleHelper.Log(string.Empty, false);
    }
    #endregion

    #region Log
    /// <summary>
    /// 紀錄Log至ExportResults.txt
    /// </summary>
    /// <param name="message"></param>
    private static void Log(string message, bool isShowDatetime = true)
    {
      var fileFullName = Path.Combine(_appConfig.FilePath, "WcsExportResults.txt");
      using (var sw = new StreamWriter(fileFullName, true, Encoding.GetEncoding(950))) //BIG5
        sw.WriteLine(string.Format("{0} {1}", isShowDatetime ? DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") : string.Empty, message));
    }
    #endregion
  }
}
