using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;

namespace Wms3pl.WpfClient.Services
{
    public class DeviceWindowService
    {
        F91Entities _proxy;
        F00Entities _proxyF00;
        public List<F910501> GetDeviceSettings(string functionCode, string clientIp, string dcCode)
        {

            _proxy = ConfigurationHelper.GetProxy<F91Entities>(false, functionCode);
            var existF910501 = _proxy.F910501s.Where(x => x.DEVICE_IP == clientIp && x.DC_CODE == dcCode).ToList();
            return existF910501;
        }

        public F0003 GetSysSetting(string functionCode, string dcCode, string apName)
        {
            _proxyF00 = ConfigurationHelper.GetProxy<F00Entities>(false, functionCode);
            var f0003 = _proxyF00.F0003s.Where(x => x.DC_CODE == dcCode && x.AP_NAME == apName).FirstOrDefault();
            return f0003;
        }
    }
}
