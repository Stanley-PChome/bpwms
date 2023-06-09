
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;

namespace Wms3pl.WpfClient.Common
{
	public class GlobalVariables
	{
		//參考資料 http://zh.wikipedia.org/wiki/%E7%B5%B1%E4%B8%80%E6%B5%81%E9%80%9A%E6%AC%A1%E9%9B%86%E5%9C%98
		/// <summary>
		/// 檔案大小限制. 預設200KB.
		/// </summary>
		public static long FileSizeLimit
		{
			get
			{
				var tmp = System.Configuration.ConfigurationManager.AppSettings["FileSizeLimit"];
				long size;
				if (!long.TryParse(tmp, out size)) return 200 * 1024;
				return size * 1024;
			}
		}
	}
}
