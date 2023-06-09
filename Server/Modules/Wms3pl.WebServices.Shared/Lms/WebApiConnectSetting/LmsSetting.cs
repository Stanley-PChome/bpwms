using System.Configuration;

namespace Wms3pl.WebServices.Shared.Lms.WebApiConnectSetting
{
    public static class LmsSetting
    {
        public static string ApiUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["LmsApiUrl"];
            }
        }
        public static string ApiAuthToken
        {
            get
            {
                return ConfigurationManager.AppSettings["LmsApiAuthToken"];
            }
        }
    }
}
