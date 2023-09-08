using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F00.Interfaces;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;

namespace Wms3pl.WebServices.Shared.ApiService
{
  public static class ApiLogHelper
  {
    /// <summary>
    /// 建立Pda Log
    /// </summary>
    /// <param name="dcCode">物流中心編號</param>
    /// <param name="gupCode">業主編號</param>
    /// <param name="custCode">貨主編號</param>
    /// <param name="name">Api名稱</param>
    /// <param name="sendData">Api傳入物件</param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static ApiResult CreatePdaLogInfo(string dcCode, string gupCode, string custCode, string name, object sendData, Func<ApiResult> func, Func<ApiResult, ApiResult> finallyFunc = null)
    {
      return CreateApiLogInfo(ApiLogType.PDA, dcCode, gupCode, custCode, name, sendData, func, false, finallyFunc);
    }

    /// <summary>
    /// 建立Api Log
    /// </summary>
    /// <param name="dcCode">物流中心編號</param>
    /// <param name="gupCode">業主編號</param>
    /// <param name="custCode">貨主編號</param>
    /// <param name="name">Api名稱</param>
    /// <param name="sendData">Api傳入物件</param>
    /// <param name="func"></param>
    /// <param name="checkActivated">檢查是否正在執行中</param>
    /// <param name="finallyFunc">例外的finally要執行的內容，回傳結果不影響任何內容</param>
    /// <returns></returns>
    public static ApiResult CreateApiLogInfo(string dcCode, string gupCode, string custCode, string name, object sendData, Func<ApiResult> func, bool checkActivated = false, Func<ApiResult, ApiResult> finallyFunc = null)
    {
      return CreateApiLogInfo(ApiLogType.WMSAPI_DF, dcCode, gupCode, custCode, name, sendData, func, checkActivated, finallyFunc);
    }




    /// <summary>
    /// 建立Api Log
    /// </summary>
    /// <param name="dcCode">物流中心編號</param>
    /// <param name="gupCode">業主編號</param>
    /// <param name="custCode">貨主編號</param>
    /// <param name="name">Api名稱</param>
    /// <param name="sendData">Api傳入物件</param>
    /// <param name="func"></param>
    /// <param name="checkActivated">檢查是否正在執行中</param>
    /// <param name="finallyFunc">例外的finally要執行的內容，回傳結果不影響任何內容</param>
    /// <returns></returns>
    public static ApiResult CreateApiLogInfo(ApiLogType apiLogType, string dcCode, string gupCode, string custCode, string name, object sendData, Func<ApiResult> func, bool checkActivated = false, Func<ApiResult, ApiResult> finallyFunc = null)
    {
      switch (apiLogType)
      {
        case ApiLogType.WMSAPI_OD:
        case ApiLogType.WMSAPI_F009001:
          var f009001Repo = new F009001Repository(Schemas.CoreSchema);
          return CreateApiLogInfoImpl<F009001, F009001Repository>(f009001Repo, dcCode, gupCode, custCode, name, sendData, func, checkActivated, finallyFunc);
        case ApiLogType.WMSAPI_WI:
        case ApiLogType.WMSAPI_CR:
        case ApiLogType.WMSAPI_VR:
        case ApiLogType.WMSAPI_RT:
        case ApiLogType.WMSAPI_PL:
        case ApiLogType.WMSAPI_PD:
        case ApiLogType.WMSAPI_PC:
        case ApiLogType.WMSAPI_PB:
        case ApiLogType.WMSAPI_VD:
        case ApiLogType.WMSAPI_TS:
        case ApiLogType.WMSAPI_SD:
        case ApiLogType.WMSAPI_SN:
        case ApiLogType.WMSAPI_FT:
        case ApiLogType.WMSAPI_F009002:
          var f009002Repo = new F009002Repository(Schemas.CoreSchema);
          return CreateApiLogInfoImpl<F009002, F009002Repository>(f009002Repo, dcCode, gupCode, custCode, name, sendData, func, checkActivated, finallyFunc);
        case ApiLogType.WCSAPI_OW:
        case ApiLogType.WCSAPI_SDB:
        case ApiLogType.WCSAPI_F009003:
        case ApiLogType.WCSAPI_OPA:
          var f009003Repo = new F009003Repository(Schemas.CoreSchema);
          return CreateApiLogInfoImpl<F009003, F009003Repository>(f009003Repo, dcCode, gupCode, custCode, name, sendData, func, checkActivated, finallyFunc);
        case ApiLogType.WCSAPI_IW:
        case ApiLogType.WCSAPI_IT:
        case ApiLogType.WCSAPI_IA:
        case ApiLogType.WCSAPI_SS:
        case ApiLogType.WCSAPI_ITEM:
        case ApiLogType.WCSAPI_ITEMSN:
        case ApiLogType.WCSAPI_F009004:
        case ApiLogType.WCSAPI_CRQ:
          var f009004Repo = new F009004Repository(Schemas.CoreSchema);
          return CreateApiLogInfoImpl<F009004, F009004Repository>(f009004Repo, dcCode, gupCode, custCode, name, sendData, func, checkActivated, finallyFunc);
        case ApiLogType.WMSSH_PLS:
        case ApiLogType.WMSSH_PLO:
        case ApiLogType.WMSSH_CPO:
        case ApiLogType.WMSSH_SDB:
        case ApiLogType.WCSSCH_ITEM:
        case ApiLogType.WCSSCH_ITEMSN:
        case ApiLogType.WCSSH_F009005:
          var f009005Repo = new F009005Repository(Schemas.CoreSchema);
          return CreateApiLogInfoImpl<F009005, F009005Repository>(f009005Repo, dcCode, gupCode, custCode, name, sendData, func, checkActivated, finallyFunc);
        case ApiLogType.LMSAPI_F000906:
        case ApiLogType.WCSSH_F009006:
          var f009006Repo = new F009006Repository(Schemas.CoreSchema);
          return CreateApiLogInfoImpl<F009006, F009006Repository>(f009006Repo, dcCode, gupCode, custCode, name, sendData, func, checkActivated, finallyFunc);
        case ApiLogType.WCSSRAPI_F009007:
          var f009007Repo = new F009007Repository(Schemas.CoreSchema);
          return CreateApiLogInfoImpl<F009007, F009007Repository>(f009007Repo, dcCode, gupCode, custCode, name, sendData, func, checkActivated, finallyFunc);
        case ApiLogType.WCSSH_F009008:
          var f009008Repo = new F009008Repository(Schemas.CoreSchema);
          return CreateApiLogInfoImpl<F009008, F009008Repository>(f009008Repo, dcCode, gupCode, custCode, name, sendData, func, checkActivated, finallyFunc);
        case ApiLogType.PDA:
          var f0091Repo = new F0091Repository(Schemas.CoreSchema);
          return CreateApiLogInfoImpl<F0091, F0091Repository>(f0091Repo, dcCode, gupCode, custCode, name, sendData, func, checkActivated, finallyFunc);
        case ApiLogType.WMSSH_DPR:
        default:
          var f0090Repo = new F0090Repository(Schemas.CoreSchema);
          return CreateApiLogInfoImpl<F0090, F0090Repository>(f0090Repo, dcCode, gupCode, custCode, name, sendData, func, checkActivated, finallyFunc);
      }
    }

    /// <summary>
    /// 建立Api Log
    /// </summary>
    /// <param name="dcCode">物流中心編號</param>
    /// <param name="gupCode">業主編號</param>
    /// <param name="custCode">貨主編號</param>
    /// <param name="name">Api名稱</param>
    /// <param name="sendData">Api傳入物件</param>
    /// <param name="func"></param>
    /// <param name="checkActivated">檢查是否正在執行中</param>
    /// <returns></returns>
    private static ApiResult CreateApiLogInfoImpl<T, TRep>(TRep logRepo, string dcCode, string gupCode, string custCode, string name, object sendData, Func<ApiResult> func, bool checkActivated = false, Func<ApiResult, ApiResult> finallyFunc = null)
			where T : F0090Base
			where TRep : IApiLogRepository<T>
		{
     
			DateTime now = DateTime.Now;

      ApiResult res = null;
      string exceptionMsg = string.Empty;
      string exceptionMsg2 = string.Empty;
      //資料長度超過4000，只取前4000的內容寫入db
      var jsonSendData = JsonConvert.SerializeObject(sendData);
      jsonSendData = jsonSendData?.Length >= 4000 ? jsonSendData.Substring(0, 4000) : jsonSendData;
      var apiLogDbMode = ConfigurationManager.AppSettings["ApiLogDbMode"];
      if (string.IsNullOrWhiteSpace(apiLogDbMode))
      {
        apiLogDbMode = "1";
      }
      var apiLogTxtMode = ConfigurationManager.AppSettings["ApiLogTxtMode"];
      if (string.IsNullOrWhiteSpace(apiLogTxtMode))
      {
        apiLogTxtMode = "1";
      }
      var apiLogTxtFolder = ConfigurationManager.AppSettings["ApiLogTxtFolder"];
			var f0000Repo = new F0000Repository(Schemas.CoreSchema);
			var tableName = typeof(T).Name;
				//處理lock table
			if (checkActivated)
        if(!f0000Repo.LockTable(tableName, name))
          return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = "此排程正在執行中，無法執行" };
      try
      {
        res = func();
      }
      catch (Exception ex)
      {
        if (res == null)
          res = new ApiResult();
        res.IsSuccessed = false;
        exceptionMsg = ex.Message;
        exceptionMsg2 = ex.ToString();
      }
      finally
      {
        var OriReturnData = res.OriAPIReturnMessage;
				var httpCode = res.HttpCode;
				var httpContent = res.HttpContent;
        if (!string.IsNullOrWhiteSpace(exceptionMsg) && (apiLogDbMode != "2" || apiLogTxtMode != "2"))
        {
          res = new ApiResult
          {
            IsSuccessed = false,
            MsgCode = "99999",
            MsgContent = "[WebApi發生錯誤]" + exceptionMsg
          };

          if (apiLogDbMode != "2")
          {
						if (apiLogTxtMode != "2" && !string.IsNullOrWhiteSpace(apiLogTxtFolder))
						{
							apiLogTxtFolder = Path.Combine(apiLogTxtFolder, "ApiLogException", DateTime.Now.ToString("yyyyMMdd"), name);
							LogApiMsgToTxt(apiLogTxtFolder, sendData, "", OriReturnData, now, httpCode, httpContent, exceptionMsg2);
						}

						var errMsg = $"[99999] {exceptionMsg}";
						errMsg = CutErrMsg(errMsg);

              logRepo.InsertLog(dcCode ?? "0", gupCode ?? "0", custCode ?? "0", name, jsonSendData, exceptionMsg2, errMsg, "9", now);
          }

        }
        else if (apiLogDbMode == "1" || apiLogTxtMode == "1")
        {
          string retrunData = null;

          if (res.Data != null && res.Data is List<ApiResponse>)
          {
            retrunData = ((List<ApiResponse>)res.Data).Count > 0 ? JsonConvert.SerializeObject(res.Data) : JsonConvert.SerializeObject(res);
          }
          else
          {
            retrunData = JsonConvert.SerializeObject(res);
          }

					if (apiLogTxtMode == "1" && !string.IsNullOrWhiteSpace(apiLogTxtFolder))
					{
						apiLogTxtFolder = Path.Combine(apiLogTxtFolder, "ApiLog", DateTime.Now.ToString("yyyyMMdd"), name);
						LogApiMsgToTxt(apiLogTxtFolder, sendData, retrunData, OriReturnData, now, httpCode, httpContent, "");
					}

					var errMsg = res.IsSuccessed ? null : (!string.IsNullOrWhiteSpace(res.MsgCode) ? $"[{res.MsgCode}] {res.MsgContent}" : res.MsgContent);
					errMsg = CutErrMsg(errMsg);
					var status = res.IsSuccessed ? "1" : "0";
          if (apiLogDbMode == "1")
          {
            // 資料長度超過4000，只取前4000的內容寫入db,
            retrunData = retrunData?.Length >= 4000 ? retrunData.Substring(0, 4000) : retrunData;
            logRepo.InsertLog(dcCode ?? "0", gupCode ?? "0", custCode ?? "0", name, jsonSendData, retrunData, errMsg, status, now);

          }
        }

        //有要lock table才有需要做還原動作
        if (checkActivated)
          f0000Repo.UnlockTable(tableName, name);
        finallyFunc?.Invoke(res);
			}

      return res;
    }

    private static async Task LogApiMsgToTxt(string apiLogTxtFolder, object sendData, string retrunData, string OriReturnData, DateTime startTime, string httpCode, string httpContent, string fullExceptionMsg)
    {
      if (!Directory.Exists(apiLogTxtFolder))
      {
        Directory.CreateDirectory(apiLogTxtFolder);
      }
      var fileName = $"{Current.Staff}.log";
      var fullFileName = Path.Combine(apiLogTxtFolder, fileName);

      try
      {
        using (var sw = new StreamWriter(fullFileName, true))
        {
          await WriteFileContentAsync(sendData, retrunData, OriReturnData, startTime, httpCode, httpContent, fullExceptionMsg, sw);
        }
      }
      catch (Exception)
      {
        fileName = $"{Current.Staff}_{Guid.NewGuid().ToString()}.log";
        fullFileName = Path.Combine(apiLogTxtFolder, fileName);
        using (var sw = new StreamWriter(fullFileName, true))
        {
          await WriteFileContentAsync(sendData, retrunData, OriReturnData, startTime, httpCode, httpContent, fullExceptionMsg, sw);
        }
      }

    }

    private static async Task WriteFileContentAsync(object sendData, string retrunData, string OriReturnData, DateTime startTime, string httpCode, string httpContent, string fullExceptionMsg, StreamWriter sw)
    {
      await sw.WriteLineAsync($"執行開始時間：{ startTime.ToString("yyyy-MM-dd HH:mm:ss")}");
      await sw.WriteLineAsync($"執行結束時間：{ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
      await sw.WriteLineAsync($"Http Code：{ httpCode }");
      await sw.WriteLineAsync($"Http Content：{ httpContent }");
      await sw.WriteLineAsync($"傳入資料：{ JsonConvert.SerializeObject(sendData)}");
      await sw.WriteLineAsync($"傳出資料：{ retrunData }");
      await sw.WriteLineAsync($"原始資料：{ OriReturnData }");
      await sw.WriteLineAsync($"錯誤內容：{ fullExceptionMsg }");
      await sw.WriteLineAsync($"================================================================================================================================");
      await sw.WriteLineAsync();
      await sw.WriteLineAsync();
    }

    /// <summary>
    /// 把要寫入F009x過長的ErrMsg截斷
    /// </summary>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    private static string CutErrMsg(string ErrMsg)
    {
      var ErrMsgLength = 200;
      return ErrMsg?.Length > ErrMsgLength ? ErrMsg.Substring(0, ErrMsgLength) : ErrMsg;
    }

  }
}
