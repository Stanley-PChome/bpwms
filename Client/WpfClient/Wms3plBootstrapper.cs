using System.Windows;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient
{
  public partial class Wms3plBootstrapper : UnityBootstrapper
  {
    protected override DependencyObject CreateShell()
    {
      var shell = Container.Resolve<MainWindow>();
      return shell;
    }

    protected override void InitializeShell()
    {
      base.InitializeShell();
      App.Current.MainWindow = (Window) this.Shell;
      App.Current.MainWindow.Show();
    }

    /// <summary>
    /// 註冊所有的 instance
    /// </summary>
    protected override void ConfigureContainer()
    {
      base.ConfigureContainer();
      //var functionService = new FakeFunctionService();
      var functionService = new FunctionService();
      this.Container.RegisterInstance(typeof(IFunctionService), functionService);
      this.Container.RegisterInstance(typeof(IDcService), new DcService());
      RegisterTypeIfMissing(typeof(IServiceLocator), typeof(UnityServiceLocatorAdapter), true);
      this.Container.RegisterInstance(typeof(IFormService), new FormService());
      //RegisterTypeIfMissing(typeof(IDialogService), typeof(WpfDialogService), false);
    }
  }
}
