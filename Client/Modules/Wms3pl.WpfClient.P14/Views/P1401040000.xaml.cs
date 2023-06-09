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
using Wms3pl.WpfClient.P14.Report;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P14.Views
{
  /// <summary>
  /// P1401040000.xaml 的互動邏輯
  /// </summary>
  public partial class P1401040000 : Wms3plUserControl
  {
    public P1401040000()
    {
      InitializeComponent();

      Vm.DoPrint += GetReport;
      Vm.DoExpandFilter += ExpandFilter;
      Vm.DoExpandDetail += ExpandDetail;
    }

    private void GetReport(PrintType printType)
    {
      var list = Vm.DetailRecords;

      if (list == null || list.Count == 0)
      {
        ShowMessage(Properties.Resources.P1401010000xamlcs_QueryDataEmpty);
        return;
      }
      var report = new R1401040000();
      report.Load(@"R1401040000.rpt");
      report.SetDataSource(list);

      var dcName = list[0].DC_NAME;
      var gupName = list[0].GUP_NAME;
      var custName = list[0].CUST_NAME;
      report.SetText("txtReportTitle", gupName + "－" + custName + Properties.Resources.P1401040000xamlcs_BalanceSheet);
      report.SetText("txtDcName", dcName);
      report.SetText("txtUserName", Wms3plSession.Get<UserInfo>().AccountName);
      report.SetText("txtDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
      var win = new Wms3plViewer { Owner = System.Windows.Window.GetWindow(this) };
      win.CallReport(report, printType);
    }

    private void ExpandFilter(bool expand)
    {
      ExpFilter.IsExpanded = expand;
    }

    private void ExpandDetail(bool expand)
    {
      ExpDetail.IsExpanded = expand;
    }
  }
}
