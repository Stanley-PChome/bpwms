using System.Configuration;

namespace Wms3pl.WebServices.Shared.Wcssr.WebApiConnectSetting
{

    public class WcssrSetting
    {
        public static string ApiUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["WcssrApiUrl"];
            }
        }
        public static string ApiAuthToken
        {
            get
            {
                return ConfigurationManager.AppSettings["WcssrApiAuthToken"];
            }
        }
    }
}
