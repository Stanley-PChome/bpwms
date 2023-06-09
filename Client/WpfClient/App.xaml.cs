using System.Diagnostics;
using System.IO;
using System.Windows;
using Wms3pl.WpfClients.SharedViews;
using Telerik.Windows.Controls.Animation;

namespace Wms3pl.WpfClient
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Wms3plAppBase
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      AnimationManager.IsGlobalAnimationEnabled = false;
    }

    protected override void RunMainWindow()
    {
      RunBootstrapper();

      this.ShutdownMode = ShutdownMode.OnMainWindowClose;
    }


    private void RunBootstrapper()
    {
      Bootstrapper = new Wms3plBootstrapper();
      Bootstrapper.Run();
    }

    

    public static Wms3plBootstrapper Bootstrapper { get; set; }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
			//var result = e.ApplicationExitCode;
			//var tempPath = GetTempPath();
			//File.WriteAllText(tempPath, result.ToString());
    }

    private static string GetTempPath()
    {
      var id = Process.GetCurrentProcess().Id;
      string path = Path.Combine(Path.GetTempPath(), id.ToString());
      return path;
    }
  }
}
