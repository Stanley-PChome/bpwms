using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.P71.Report;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.Views
{
  /// <summary>
  /// P7108100000.xaml 的互動邏輯
  /// </summary>
  public partial class P7108100000 : Wms3plUserControl
  {
    public P7108100000()
    {
      InitializeComponent();

      Vm.DoPrint += GetReport;
    }

    private void GetReport(PrintType printType)
    {
      var list = Vm.Records;

      if (list == null || list.Count == 0)
      {
        ShowMessage(Properties.Resources.P7101080000xamlcs_listNoData);
        return;
      }
      var report = new R7108100000();
      report.Load(@"R9301010000.rpt");
      report.SetDataSource(list);

      //var dcName = ((NameValuePair<string>)cbSelectedDcCode.SelectedItem).Name;
      //report.SetText("txtHeader", dcName + "－" + Wms3plSession.Get<Wms3pl.WpfClient.Common.GlobalInfo>().GupName + "－" + Wms3plSession.Get<Wms3pl.WpfClient.Common.GlobalInfo>().CustName);
      report.SetText("txtUserName", Wms3plSession.Get<UserInfo>().AccountName);
      report.SetText("txtDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
      var win = new Wms3plViewer { Owner = System.Windows.Window.GetWindow(this) };
      win.CallReport(report, printType);
    }
  }
}
