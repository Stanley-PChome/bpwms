using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Wms3pl.WpfClient.Common
{
	/// <summary>
	/// 讀取圖檔
	/// </summary>
	public static class FileHelper
	{
		public static WindowsIdentity WI { get; set; }
		public static WindowsImpersonationContext WIC { get; set; }
		

		/// <summary>
		/// 各類檔案共用路徑
		/// </summary>
		public static string ShareFolderItemFiles
		{
			get { return System.Configuration.ConfigurationManager.AppSettings["ShareFolderItemFiles"]; }
		}

		/// <summary>
		/// 標籤檔路徑
		/// </summary>
		public static string ShareFolderLabel
		{
			get {
				
				return Path.Combine(Directory.GetCurrentDirectory(), "Label");
				//return System.Configuration.ConfigurationManager.AppSettings["ShareFolderLabel"];
			}
			}

		/// <summary>
		/// Bartender License主機IP
		/// </summary>
		public static string BartenderLicenseIp
		{
			get { return System.Configuration.ConfigurationManager.AppSettings["BartenderLicenseIp"]; }
		}

		public static string GetShareFolderPath(string[] folderArr)
		{
			var resultPath = ShareFolderItemFiles;

			foreach (var folders in folderArr)
			{
				resultPath = string.IsNullOrEmpty(folders) ? resultPath : Path.Combine(resultPath, folders + @"\");
			}

			if (!Directory.Exists(resultPath))
				Directory.CreateDirectory(resultPath);

			return resultPath;

		}

		public static string GetPath(string[] folderArr)
		{
			return folderArr.Aggregate(string.Empty, (current, folders) => string.IsNullOrEmpty(folders) ? current : Path.Combine(current, folders + @"\"));
		}

	}
}
