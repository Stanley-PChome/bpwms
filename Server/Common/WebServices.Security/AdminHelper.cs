using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Win32;

namespace Wms3pl.WebServices.Security
{
  public class AdminHelper
  {
    public static AdminHelper Current = new AdminHelper();

    public AdminHelper()
    {
      SetIdAndPwd();
    }


    private void SetIdAndPwd()
    {
      using (RegistryKey softwareKey = Registry.LocalMachine.OpenSubKey("Software"))
      {
        using (RegistryKey productKey = softwareKey.OpenSubKey("Wms3pl"))
        {
          if (productKey != null)
          {
            SaId = productKey.GetValue("SAID").ToString();
            SaPwdHash = productKey.GetValue("SAPWD").ToString();
          }
        }
      }
    }


    public string SaId { get; set; }
    public string SaPwdHash { get; set; }
  }
}