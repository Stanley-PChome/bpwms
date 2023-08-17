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
#if (PhTest)
        return ConfigurationManager.AppSettings["WcsApiUrl_" + DcCode];
#else
        return ConfigurationManager.AppSettings["WcsApiUrl"];
#endif
      }
    }
		public static string ApiAuthToken
		{
			get
			{
#if (PhTest)
        return ConfigurationManager.AppSettings["WcsApiAuthToken_" + DcCode];
#else
        return ConfigurationManager.AppSettings["WcsApiAuthToken"];
#endif
      }
    }
		public static int ItemMaxCnt
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["ItemMaxCnt"]);
			}
		}

    public static string DcCode { get; set; }
  }
}
