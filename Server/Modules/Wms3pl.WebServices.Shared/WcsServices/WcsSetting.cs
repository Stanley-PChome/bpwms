using System;
using System.Configuration;

namespace Wms3pl.WebServices.Shared.Wcs.WcsApiConnectSetting
{
	public static class WcsSetting
	{
		public static string ApiUrl
		{
			get
			{
				return ConfigurationManager.AppSettings["WcsApiUrl"];
			}
		}
		public static string ApiAuthToken
		{
			get
			{
				return ConfigurationManager.AppSettings["WcsApiAuthToken"];
			}
		}
		public static int ItemMaxCnt
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["ItemMaxCnt"]);
			}
		}
	}
}
