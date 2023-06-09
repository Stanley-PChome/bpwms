using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.Common
{
    public static class LocCodeHelper
    {
        public static string LocCodeConverter9(string locCode)
        {
            if (string.IsNullOrWhiteSpace(locCode))
            {
                return locCode;
            }
            string result = locCode.Replace("-", "");
            return result;

        }


        public static string LocCodeConverter13(string locCode)
        {
            if (locCode.Length == 9)
            {
                var tmpCode = string.Format("{0}-{1}-{2}-{3}-{4}", locCode.Substring(0, 1)
                                                                    , locCode.Substring(1, 2)
                                                                    , locCode.Substring(3, 2)
                                                                    , locCode.Substring(5, 2)
                                                                    , locCode.Substring(7, 2));
                return tmpCode;
            }
            else
            {
                return locCode;
            }
        }
    }

    
}
