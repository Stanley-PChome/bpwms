using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

namespace Wms3pl.WpfClient.Common
{
	public static class Schemas
	{
		public static string CoreSchema{
			get{
				///是否需要讀取init檔案設定
				if (ConfigurationManager.AppSettings["IsNeedInitFile"] == "1")
				{
					var directoryInfo = new DirectoryInfo(ConfigurationManager.AppSettings["InitFolder"]);
					if (!directoryInfo.Exists)
						directoryInfo.Create();
					var initFileName = "bpwms.init";
					var fileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, initFileName));
					if (!fileInfo.Exists)
						throw new Exception("啟動金鑰尚未授權，請聯絡系統管理員");
					var sr = new StreamReader(fileInfo.FullName);
					var initValue = sr.ReadLine();
					initValue = string.Format("BP{0}WMS", initValue);
					return AesCryptor.Current.Encode(initValue);
				}
				else
				{
					return "";
				}
			}
			}
		public const string LongTermSchema = "WMSLongTerm";

		public static string GetLogSetting
		{
			get
			{
				var directoryInfo = new DirectoryInfo(ConfigurationManager.AppSettings["InitFolder"]);
				if (!directoryInfo.Exists)
					directoryInfo.Create();
				var initFileName = "log.init";
				var fileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, initFileName));
				if (fileInfo.Exists)
				{
					var sr = new StreamReader(fileInfo.FullName);
					var initValue = sr.ReadLine();
					return initValue;
				}
				return "0";
			}
		}
	}
}
