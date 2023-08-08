using System.Linq;
using System.Windows;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClients.SharedViews.Views;

namespace Wms3pl.WpfClient.P02.Views
{
  /// <summary>
  /// P0202090000.xaml 的互動邏輯
  /// </summary>
  public partial class P0202090000 : Wms3plUserControl
  {
    public P0202090000()
    {
      InitializeComponent();
      Vm.PrintItemLabel += PrintItemLabel;
      Vm.PrintRecvNote += PrintRecvNote;

      var openDeviceWindow = OpenDeviceWindow(Vm.FunctionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, Vm.SelectedDc);
      if (openDeviceWindow.Any())
      {
        Vm.SelectedF910501 = openDeviceWindow.FirstOrDefault();
      }
      else
      {
        var deviceWindow = new DeviceWindow(Vm.SelectedDc);
        deviceWindow.Owner = this.Parent as Window;
        deviceWindow.ShowDialog();
        Vm.SelectedF910501 = deviceWindow.SelectedF910501;
      }
    }

    private void PrintItemLabel()
    {
      if (Vm.ItemLabelData != null && Vm.ItemLabelData.Any())
      {
        var report = ReportHelper.CreateAndLoadReport<Report.P0202090000>();
        report.SummaryInfo.ReportComments = Wms3plSession.Get<GlobalInfo>().DcCodeList.Find(x => x.Value.Equals(Vm.SelectedDc)).Name;
        report.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().CustName;
        report.SummaryInfo.ReportAuthor = Wms3plSession.CurrentUserInfo.AccountName;

        report.SetDataSource(Vm.ItemLabelDataTable);

        var crystalReportService = new CrystalReportService(report, Vm.SelectedF910501);
        crystalReportService.PrintToPrinter(PrinterType.Label, Vm.ItemLabelData.FirstOrDefault().RECV_QTY);
      }
    }

    private void PrintRecvNote()
    {
      //列印驗收單(良品)
      if (Vm.AcceptanceReportData != null && Vm.AcceptanceReportData.Any())
      {
        var report = ReportHelper.CreateAndLoadReport<Report.P0202030000>();
        report.SummaryInfo.ReportComments = Wms3plSession.Get<GlobalInfo>().DcCodeList.Find(x => x.Value.Equals(Vm.SelectedDc)).Name;
        report.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().CustName;
        report.SummaryInfo.ReportAuthor = Wms3plSession.CurrentUserInfo.AccountName;
        report.SetText("txtPrintStaffName", Wms3plSession.CurrentUserInfo.AccountName);
        report.SetText("txtTicketNo", Vm.SelectedPurchaseNo);
        report.SetText("TxtRecvQty", "良品數");
        report.SetDataSource(Vm.AcceptanceReportDataTable);
        report.Subreports[0].SetDataSource(Vm.F151001ReportByAcceptanceDataTable);
        var crystalReportService = new CrystalReportService(report, Vm.SelectedF910501);
        crystalReportService.PrintToPrinter();
      }

      if (Vm.AcceptanceReportData1 != null && Vm.AcceptanceReportData1.Any())
      {
        //列印驗收單(不良品)
        var report1 = ReportHelper.CreateAndLoadReport<Report.P0202030001>();
        report1.SummaryInfo.ReportComments = Wms3plSession.Get<GlobalInfo>().DcCodeList.Find(x => x.Value.Equals(Vm.SelectedDc)).Name;
        report1.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().CustName;
        report1.SummaryInfo.ReportAuthor = Wms3plSession.CurrentUserInfo.AccountName;
        report1.SetText("txtPrintStaffName", Wms3plSession.CurrentUserInfo.AccountName);
        report1.SetText("txtTicketNo", Vm.SelectedPurchaseNo);
        report1.SetText("TxtRecvQty", "不良品數");
        report1.SetDataSource(Vm.AcceptanceReportDataTable1);
        report1.Subreports[0].SetDataSource(Vm.F151001ReportByAcceptanceDefectDataTable);
        report1.Subreports[1].SetDataSource(Vm.DefectDetailReportDataTable);
        var crystalReportService = new CrystalReportService(report1, Vm.SelectedF910501);
        crystalReportService.PrintToPrinter();
      }
    }

    public void RePrint_OnClick(object sender, RoutedEventArgs e)
    {
      var win = new P0202090100(Vm.SelectedDc, Vm.SelectRecords);
      win.ShowDialog();
    }
  }
}
