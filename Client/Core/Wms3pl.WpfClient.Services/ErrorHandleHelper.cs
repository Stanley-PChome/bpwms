using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.Common;
using System.Security.Principal;
using Wms3pl.WpfClient.Services.Utility;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Wms3pl.WpfClient.Services
{
	public static class ErrorHandleHelper
	{
		public static WindowsIdentity WI { get; set; }
		public static WindowsImpersonationContext WIC { get; set; }
		public static int GetSystemErrorCode(Exception ex)
		{
			//Ref: https://msdn.microsoft.com/zh-tw/library/windows/desktop/ms690088%28v=vs.85%29.aspx
			// MSDN HRESULT Define
			//HRESULT的格式定義，後面16碼為Error Code
			//65535轉2進位=>1111111111111111再and，可以取出所需代碼
			return Marshal.GetHRForException(ex) & 65535;
		}

		/// <summary>
		/// 取得自訂錯誤描述
		/// </summary>
		/// <param name="ex">Exception ex</param>
		/// <param name="defineMsg">
		/// 預設錯誤資訊(當錯誤不在其中，則顯示[預設錯誤資訊+ex.Message]，
		/// 例如:匯入失敗 + [Message])
		/// </param>
		/// <param name="replace">
		/// 若選是，找不到代碼時，則不會帶出ex.Message，會直接回傳defineMsg
		/// (因為有些UI介面不要顯示詳細資訊)
		/// </param>
		/// <returns></returns>
		public static string GetCustomErrorCodeDescription(Exception ex, string defineMsg = "", bool replace = false)
		{
			var errorHeader = (string.IsNullOrWhiteSpace(defineMsg)
				? string.Empty
				: defineMsg);
			var errorMsg = errorHeader + (replace ? string.Empty : ex.Message);	
			var errorCode = GetSystemErrorCode(ex);
			switch (errorCode)
			{
				//Ref: https://msdn.microsoft.com/en-us/library/windows/desktop/ms681382%28v=vs.85%29.aspx
				// MSDN System Error Codes(錯誤代碼表)
				// ERROR_SHARING_VIOLATION(32): The process cannot access the file because it is being used by another process.
				// ERROR_LOCK_VIOLATION(33): The process cannot access the file because another process has locked a portion of the file.
				case 32:
				case 33:
					errorMsg = errorHeader + "檔案已被開啟或鎖定!";
					break;
				case 87:
					errorMsg = errorHeader + "檔案路徑不得空白!";
					break;
                case 6434:
                    errorMsg = errorHeader + "Excel欄位名稱重複!";
                    break;
                case 5378:
                    errorMsg = errorHeader + "欄位格式有問題!";
                    break;
                case 16387:
                    errorMsg = errorHeader + "請勿插入空白欄位!";
                    break;

            }
			if (!string.IsNullOrEmpty(errorMsg))
				ErrorHandleHelper.HandleException(ex);
			else
				throw new Exception("Import File Error", ex);
			return errorMsg;
		}



		private static Exception _lastException = null;

		public static void HandleException(Exception ex)
		{
			if (ex == null) return;
			//相同的錯誤訊息不再寫 log
			if (_lastException != null && ex.Message == _lastException.Message)
				return;

			_lastException = ex;
			if (Wms3plSession.CurrentUserInfo != null)
			{
				var myEx = new Exception(ex.Message, ex);
				myEx.Data.Add("Account", Wms3plSession.CurrentUserInfo.Account);
				var gi = Wms3plSession.Get<GlobalInfo>();

				var info = new AssemblyHelper().GetAssembliesInfo();
				myEx.Data.Add("Loaded assemblies", info);
				ExceptionPolicy.HandleException(myEx, ExceptionPolicyNames.DefaultPolicy);
			}
			else
				ExceptionPolicy.HandleException(ex, ExceptionPolicyNames.DefaultPolicy);

			//儲存發生錯誤時的 screen
			var util = new ScreenUtility();
			string fileName = string.Format(ConfigurationManager.AppSettings["ErrorScreenFormatPath"],
				Wms3plSession.CurrentUserInfo == null ? string.Empty : Wms3plSession.CurrentUserInfo.Account, DateTime.Now);

			var directoryName = Path.GetDirectoryName(fileName);
			if (!Directory.Exists(directoryName))
				Directory.CreateDirectory(directoryName);

			util.SaveImage((int)System.Windows.SystemParameters.PrimaryScreenWidth,
				(int)System.Windows.SystemParameters.PrimaryScreenHeight, fileName);
#if DEBUG

			var entryAssemblyName = Assembly.GetEntryAssembly().GetName().Name;
			var logFileName = string.Format(ConfigurationManager.AppSettings["ClientExceptionLogFormatPath"], entryAssemblyName);
			if (File.Exists(logFileName))
			{
				var process = Process.Start(logFileName);
				NativeWin32.SetForegroundWindow(process.MainWindowHandle);
				Thread.Sleep(200);
				SendKeys.SendWait("^{End}");
				Thread.Sleep(200);
			}
#endif

		}

	}
}
