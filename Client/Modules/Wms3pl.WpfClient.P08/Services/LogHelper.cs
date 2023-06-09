using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.Common;

namespace Wms3pl.WpfClient.P08.Services
{
	public static class LogHelper
	{
		#region Log
		/// <summary>
		/// 文字檔Log
		/// </summary>
		/// <param name="message"></param>
		public static async Task Log(string functionCode,string message, bool isShowDatetime = true)
		{
			if(Schemas.GetLogSetting == "1")
			{
				var filePath = ConfigurationManager.AppSettings["ShareFolderTemp"].ToString();
				if (!Directory.Exists(filePath))
					Directory.CreateDirectory(filePath);

				var fileFullName = Path.Combine(filePath, $"{functionCode}.txt");

				using (var sw = new StreamWriter(fileFullName, true, Encoding.GetEncoding(950))) //BIG5
					await sw.WriteLineAsync(string.Format("{0} {1}", isShowDatetime ? DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.FFF") : string.Empty, message));
			}
		}

		#endregion
	}
}
