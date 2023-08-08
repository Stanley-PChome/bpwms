using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Schedule;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.Schedule.WmsSchedule
{
	public class RemoveHistroyLogService
	{
		F0003Repository _f0003Repo;
		private WmsTransaction _wmsTransaction;

		// 紀錄DeleteDirectory
		private List<ExecuteResult> DelResult = new List<ExecuteResult>();

		public RemoveHistroyLogService(WmsTransaction wmsTransaction = null)
		{
			if (wmsTransaction == null)
				_wmsTransaction = new WmsTransaction();
			else
				_wmsTransaction = wmsTransaction;
		}

		public ApiResult RemoveHistroyLog()
		{
			var exeRes = new List<ExecuteResult>();

			var result = ApiLogHelper.CreateApiLogInfo("0", "0", "0", "RemoveHistroyLog", new object { }, () =>
			 {
				 if (_f0003Repo == null)
					 _f0003Repo = new F0003Repository(Schemas.CoreSchema);

         exeRes.Add(RemoveHistoryDBLog());
         exeRes.Add(RemoveScheduleJobResultDBLog());
         exeRes.Add(ClearNotExHistoryTxtLog());
         exeRes.Add(ClearExHistoryTxtLog());
         //exeRes.Add(ClearHistoryHttpNomalTxtLog());
         //exeRes.Add(ClearHistoryHttpExceptionTxtLog());
         exeRes.Add(ClearBakApiLogTxtLog());
         exeRes.Add(ClearBakApiLogExceptionTxtLog());

         return new ApiResult
				 {
					 IsSuccessed = !exeRes.Any(x => !x.IsSuccessed),
					 MsgCode = exeRes.Any(x => !x.IsSuccessed) ? "99999" : "200",
					 MsgContent = String.Join(Environment.NewLine, exeRes.Where(x => !string.IsNullOrWhiteSpace(x.Message)).Select(x => x.Message))
				 };
			 });

      // 合併MsgContent
      result.MsgContent += DelResult.Any() ? Environment.NewLine + String.Join(Environment.NewLine, DelResult.Select(x => x.Message)) : "";
      return result;
    }

    /// <summary>
    /// 清除F009x系列DB紀錄
    /// </summary>
    /// <returns></returns>
    private ExecuteResult RemoveHistoryDBLog()
		{
			var f0090Repo = new F0090Repository(Schemas.CoreSchema, _wmsTransaction);

      var GetSettingData = _f0003Repo.Filter(x => x.DC_CODE == "00" && x.GUP_CODE == "00" && x.CUST_CODE == "00" && new string[] { "IsClearNotExHistoryDBLog", "HistoryDBLogSaveDay" }.Contains(x.AP_NAME)).ToList();
      if ((GetSettingData?.Count() ?? 0) != 2)
				return new ExecuteResult() { IsSuccessed = false, Message = "缺少清除DB歷史Log設定" };

			//是否清除DB歷史Log
			var IsClearNotExHistoryDBLog = GetSettingData.First(x => x.AP_NAME == "IsClearNotExHistoryDBLog").SYS_PATH == "1";
			if (!IsClearNotExHistoryDBLog)
				return new ExecuteResult(true, "清除F0090x，設定不執行清除DB歷史Log");

			//DB歷史Log保存天數
			var HistoryDBLogSaveDay = Int32.Parse(GetSettingData.First(x => x.AP_NAME == "HistoryDBLogSaveDay").SYS_PATH);
			var RemoveDate = DateTime.Now.Date.AddDays(-HistoryDBLogSaveDay);

      // 查詢F009X資料
      var f009Xs = f0090Repo.GetDelF009XData(RemoveDate);
      var f009xg = f009Xs.GroupBy(x => new { x.NAME, x.STATUS, CRT_DATE = x.CRT_DATE.ToString("yyyyMMdd") });
      f009xg.ToList().ForEach(x =>
      {

        if (x.Key.STATUS == "1")
        {
          var status1Path = Path.Combine(ConfigurationManager.AppSettings["ApiLogTxtFolder"], "BakApiLog", x.Key.CRT_DATE);
					if (!Directory.Exists(status1Path))
						Directory.CreateDirectory(status1Path);
					using (var sw = new StreamWriter($"{status1Path}\\{x.Key.NAME}.log", true)) //BIG5
					{
						x.ToList().ForEach(y =>
						{
							WriteFileContent(y.CRT_DATE, y.UPD_DATE, y.SEND_DATA, y.RETURN_DATA, y.ERRMSG, sw);
						});
					}
				}
				else
				{
					var status0Path = Path.Combine(ConfigurationManager.AppSettings["ApiLogTxtFolder"], "BakApiLogException", x.Key.CRT_DATE);
					if (!Directory.Exists(status0Path))
						Directory.CreateDirectory(status0Path);
					using (var sw = new StreamWriter($"{status0Path}\\{x.Key.NAME}.log", true)) //BIG5
					{
						x.ToList().ForEach(y =>
						{
							WriteFileContent(y.CRT_DATE, y.UPD_DATE, y.SEND_DATA, y.RETURN_DATA, y.ERRMSG, sw);
						});
					}
				}
			});

      //要清除的table清單
      f0090Repo.RemoveHistroyLog(RemoveDate);
      return new ExecuteResult() { IsSuccessed = true, Message = $"執行F0090x DB刪除資料並備份Txt成功，共處理{f009Xs.Count()}筆資料" };
    }

    /// <summary>
    /// 清除ScheduleJobResult DB紀錄
    /// </summary>
    /// <returns></returns>
    private ExecuteResult RemoveScheduleJobResultDBLog()
		{
			var SCHEDULE_JOB_RESULTRepo = new SCHEDULE_JOB_RESULTRepository(Schemas.CoreSchema, _wmsTransaction);

			var GetSettingData = _f0003Repo.Filter(x => x.DC_CODE == "00" && x.GUP_CODE == "00" && x.CUST_CODE == "00" && new string[] { "IsClearNotExHistoryDBLog", "HistoryDBLogSaveDay" }.Contains(x.AP_NAME)).ToList();
			if ((GetSettingData?.Count() ?? 0) != 2)
				return new ExecuteResult() { IsSuccessed = false, Message = "缺少清除DB歷史Log設定" };

			//是否清除DB歷史Log
			var IsClearNotExHistoryDBLog = GetSettingData.First(x => x.AP_NAME == "IsClearNotExHistoryDBLog").SYS_PATH == "1";
			if (!IsClearNotExHistoryDBLog)
				return new ExecuteResult(true, "清除ScheduleJobResult，設定不執行清除DB歷史Log");

			//DB歷史Log保存天數
			var HistoryDBLogSaveDay = Int32.Parse(GetSettingData.First(x => x.AP_NAME == "HistoryDBLogSaveDay").SYS_PATH);
			var RemoveDate = DateTime.Now.Date.AddDays(-HistoryDBLogSaveDay);

      var getDeleteCount = SCHEDULE_JOB_RESULTRepo.GetDatasByTrueAndCondition(x => x.EXEDATE < RemoveDate).ToList();
      SCHEDULE_JOB_RESULTRepo.RemoveHistroyLog(RemoveDate);
      return new ExecuteResult() { IsSuccessed = true, Message = $"執行SCHEDULE_JOB_RESULT DB刪除資料並備份Txt成功，共處理{getDeleteCount.Count()}筆資料" };
    }

    /// <summary>
    /// 清除非異常Txt歷史
    /// </summary>
    /// <returns></returns>
    private ExecuteResult ClearNotExHistoryTxtLog()
		{
			var GetSettingData = _f0003Repo.Filter(x => x.DC_CODE == "00" && x.GUP_CODE == "00" && x.CUST_CODE == "00" && new string[] { "IsClearNotExHistoryTxtLog", "HistoryTxtLogSaveDay" }.Contains(x.AP_NAME)).ToList();
			if ((GetSettingData?.Count() ?? 0) != 2)
				return new ExecuteResult() { IsSuccessed = false, Message = "缺少清除非異常Txt歷史設定" };

			//是否清除非異常Txt歷史Log
			var IsClearNotExHistoryTxtLog = GetSettingData.First(x => x.AP_NAME == "IsClearNotExHistoryTxtLog").SYS_PATH == "1";
			if (!IsClearNotExHistoryTxtLog)
				return new ExecuteResult(true, "清除非異常Txt歷史，設定不執行清除非異常Txt歷史Log");

			//Txt歷史Log保存天數
			var HistoryTxtLogSaveDay = Int32.Parse(GetSettingData.First(x => x.AP_NAME == "HistoryTxtLogSaveDay").SYS_PATH);

			var ApiLogTxtFolder = Path.Combine(ConfigurationManager.AppSettings["ApiLogTxtFolder"], "ApiLog");

      DelResult.AddRange(DeleteDirectory(ApiLogTxtFolder, HistoryTxtLogSaveDay));

      return new ExecuteResult() { IsSuccessed = true };
    }

    /// <summary>
    /// 清除異常Txt歷史
    /// </summary>
    /// <returns></returns>
    private ExecuteResult ClearExHistoryTxtLog()
		{
			var GetSettingData = _f0003Repo.Filter(x => x.DC_CODE == "00" && x.GUP_CODE == "00" && x.CUST_CODE == "00" && new string[] { "IsClearExHistoryTxtLog", "HistoryExTxtLogSaveDay" }.Contains(x.AP_NAME)).ToList();
			if ((GetSettingData?.Count() ?? 0) != 2)
				return new ExecuteResult() { IsSuccessed = false, Message = "缺少清除非異常Txt歷史設定" };

			//是否清除異常Txt歷史Log
			var IsClearExHistoryTxtLog = GetSettingData.First(x => x.AP_NAME == "IsClearExHistoryTxtLog").SYS_PATH == "1";
			if (!IsClearExHistoryTxtLog)
				return new ExecuteResult(true, "清除異常Txt歷史，設定不執行清除非異常Txt歷史Log");

			//Txt歷史異常Log保存天數
			var HistoryExTxtLogSaveDay = Int32.Parse(GetSettingData.First(x => x.AP_NAME == "HistoryExTxtLogSaveDay").SYS_PATH);

			var ApiLogTxtFolder = Path.Combine(ConfigurationManager.AppSettings["ApiLogTxtFolder"], "ApiLogException");

      DelResult.AddRange(DeleteDirectory(ApiLogTxtFolder, HistoryExTxtLogSaveDay));

			return new ExecuteResult() { IsSuccessed = true };
		}

		/*/// <summary>
		/// 清除正常Http Txt歷史
		/// </summary>
		/// <returns></returns>
		public ExecuteResult ClearHistoryHttpNomalTxtLog()
		{
			var GetSettingData = _f0003Repo.Filter(x => x.DC_CODE == "00" && x.GUP_CODE == "00" && x.CUST_CODE == "00" && new string[] { "IsClearHistoryHttpNomalTxtLog", "HistoryHttpNomalTxtLogSaveDay" }.Contains(x.AP_NAME)).ToList();
			if ((GetSettingData?.Count() ?? 0) != 2)
				return new ExecuteResult() { IsSuccessed = false, Message = "缺少清除正常Http Txt歷史設定" };

			//是否清除正常Http Txt歷史Log
			var IsClearHistoryHttpNomalTxtLog = GetSettingData.First(x => x.AP_NAME == "IsClearHistoryHttpNomalTxtLog").SYS_PATH == "1";
			if (!IsClearHistoryHttpNomalTxtLog)
				return new ExecuteResult(true, "清除正常Http Txt歷史，設定不執行清除正常Http Txt歷史Log");

			//Txt歷史Log保存天數
			var HistoryHttpNomalTxtLogSaveDay = Int32.Parse(GetSettingData.First(x => x.AP_NAME == "HistoryHttpNomalTxtLogSaveDay").SYS_PATH);

			var HttpClientLogFolder = Path.Combine(ConfigurationManager.AppSettings["HttpClientLogFolder"], "HttpClientNormalLog");

      DelResult.AddRange(DeleteDirectory(HttpClientLogFolder, HistoryHttpNomalTxtLogSaveDay));

      return new ExecuteResult() { IsSuccessed = true };
		}

		/// <summary>
		/// 清除異常Http Txt歷史
		/// </summary>
		/// <returns></returns>
		public ExecuteResult ClearHistoryHttpExceptionTxtLog()
		{
			var GetSettingData = _f0003Repo.Filter(x => x.DC_CODE == "00" && x.GUP_CODE == "00" && x.CUST_CODE == "00" && new string[] { "IsClearHistoryHttpExceptionTxtLog", "HistoryHttpExceptionTxtLogSaveDay" }.Contains(x.AP_NAME)).ToList();
			if ((GetSettingData?.Count() ?? 0) != 2)
				return new ExecuteResult() { IsSuccessed = false, Message = "缺少清除異常Http Txt歷史設定" };

			//是否清除異常Http Txt歷史Log
			var IsClearHistoryHttpExceptionTxtLog = GetSettingData.First(x => x.AP_NAME == "IsClearHistoryHttpExceptionTxtLog").SYS_PATH == "1";
			if (!IsClearHistoryHttpExceptionTxtLog)
				return new ExecuteResult(true, "清除異常Http Txt歷史，設定不執行清除異常Http Txt歷史Log");

			//Txt歷史Log保存天數
			var HistoryHttpExceptionTxtLogSaveDay = Int32.Parse(GetSettingData.First(x => x.AP_NAME == "HistoryHttpExceptionTxtLogSaveDay").SYS_PATH);

			var HttpClientLogFolder = Path.Combine(ConfigurationManager.AppSettings["HttpClientLogFolder"], "HttpClientLog");

      DelResult.AddRange(DeleteDirectory(HttpClientLogFolder, HistoryHttpExceptionTxtLogSaveDay));

      return new ExecuteResult() { IsSuccessed = true };
		}*/

		/// <summary>
		/// 移除目錄
		/// </summary>
		/// <param name="DirectoryPath">要移除的目錄位置</param>
		/// <param name="RemoveDays">刪除差多少日以上的目錄</param>
		/// <returns>刪除的目錄記錄</returns>
		private List<ExecuteResult> DeleteDirectory(String DirectoryPath, int RemoveDays)
		{
			DateTime ForderDate;
			List<ExecuteResult> results = new List<ExecuteResult>();
			if (Directory.Exists(DirectoryPath))
				foreach (var DirInfoitem in new DirectoryInfo(DirectoryPath).GetDirectories())
				{
					if (!Regex.IsMatch(DirInfoitem.Name, @"\d{8}") || DirInfoitem.Name.Length != 8)
					{
						results.Add(new ExecuteResult(false, $"無法識別的目錄:{DirInfoitem.FullName}"));
						continue;
					}
					if (!DateTime.TryParse(DirInfoitem.Name.Insert(6, "-").Insert(4, "-"), out ForderDate))
					{
						results.Add(new ExecuteResult(false, $"無法識別的目錄:{DirInfoitem.FullName}"));
						continue;
					}
					if ((DateTime.Now.Date - ForderDate).TotalDays > RemoveDays)
					{
						DirInfoitem.Delete(true);
						results.Add(new ExecuteResult(true, $"刪除目錄:{DirInfoitem.FullName}"));
					}
				}
			return results;
		}

		public ExecuteResult ClearBakApiLogTxtLog()
		{
			var GetSettingData = _f0003Repo.Filter(x => x.DC_CODE == "00" && x.GUP_CODE == "00" && x.CUST_CODE == "00" && new string[] { "IsClearBakExTxtLog", "BakExTxtLogSaveDay" }.Contains(x.AP_NAME)).ToList();
			if ((GetSettingData?.Count() ?? 0) != 2)
				return new ExecuteResult() { IsSuccessed = false, Message = "缺少清除正常BakExTxt歷史設定" };

			//是否清除異常BakApiTxt歷史Log
			var IsClearHistoryHttpExceptionTxtLog = GetSettingData.First(x => x.AP_NAME == "IsClearBakExTxtLog").SYS_PATH == "1";
			if (!IsClearHistoryHttpExceptionTxtLog)
				return new ExecuteResult(true, "清除正常BakExTxt歷史，設定不執行清除正常BakExTxt歷史Log");

			//BakApi歷史Log保存天數
			var BakExTxtLogSaveDay = Int32.Parse(GetSettingData.First(x => x.AP_NAME == "BakExTxtLogSaveDay").SYS_PATH);

			var HttpClientLogFolder = Path.Combine(ConfigurationManager.AppSettings["ApiLogTxtFolder"], "BakApiLog");

      DelResult.AddRange(DeleteDirectory(HttpClientLogFolder, BakExTxtLogSaveDay));

			return new ExecuteResult() { IsSuccessed = true };
		}
		

		public ExecuteResult ClearBakApiLogExceptionTxtLog()
		{
			var GetSettingData = _f0003Repo.Filter(x => x.DC_CODE == "00" && x.GUP_CODE == "00" && x.CUST_CODE == "00" && new string[] { "IsClearBakNotExTxtLog", "BakNotExTxtLogSaveDay" }.Contains(x.AP_NAME)).ToList();
			if ((GetSettingData?.Count() ?? 0) != 2)
				return new ExecuteResult() { IsSuccessed = false, Message = "缺少清除異常BakNotExTxt歷史設定" };

			//是否清除異常BakNotExTxt歷史Log
			var IsClearHistoryHttpExceptionTxtLog = GetSettingData.First(x => x.AP_NAME == "IsClearBakNotExTxtLog").SYS_PATH == "1";
			if (!IsClearHistoryHttpExceptionTxtLog)
				return new ExecuteResult(true, "清除異常Http Txt歷史，設定不執行清除異常BakNotExTxt歷史Log");

			//BakApi歷史Log保存天數
			var bakNotExTxtLogSaveDay = Int32.Parse(GetSettingData.First(x => x.AP_NAME == "BakNotExTxtLogSaveDay").SYS_PATH);

			var HttpClientLogFolder = Path.Combine(ConfigurationManager.AppSettings["ApiLogTxtFolder"], "BakApiLogException");

      DelResult.AddRange(DeleteDirectory(HttpClientLogFolder, bakNotExTxtLogSaveDay));

			return new ExecuteResult() { IsSuccessed = true };
		}

		private static void WriteFileContent(DateTime crtDate ,DateTime? updDate,string sendData, string retrunData, string fullExceptionMsg, StreamWriter sw)
		{
			sw.WriteLine($"開始時間：{ crtDate.ToString("yyyy-MM-dd HH:mm:ss")}");
			sw.WriteLine($"傳入資料：{ sendData}");
			sw.WriteLine($"傳出資料：{ retrunData }");
			sw.WriteLine($"錯誤內容：{ fullExceptionMsg }");
			sw.WriteLine($"結束時間：{ updDate?.ToString("yyyy-MM-dd HH:mm:ss")}");
			sw.WriteLine($"================================================================================================================================");
			sw.WriteLine();
			sw.WriteLine();
		}

	}
}
