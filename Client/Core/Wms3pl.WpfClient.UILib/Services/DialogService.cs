using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Wms3pl.WpfClient.UILib.Services
{
  public partial class DialogService
  {
    static DialogService()
    {
      IUnityContainer container = new UnityContainer();
      var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
      section.Configure(container, "");
      _currentService = container.Resolve<IDialogService>();
      //預設使用 WpfDialigService
      //if (_currentService == null) _currentService = new WpfDialogService();
    }

    private static readonly IDialogService _currentService;
    public static IDialogService Current
    {
      get { return _currentService; }
    }
  }
}
