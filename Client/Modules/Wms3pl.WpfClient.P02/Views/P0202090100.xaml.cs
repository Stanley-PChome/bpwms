using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.P02.ViewModel;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClients.SharedViews.Views;

namespace Wms3pl.WpfClient.P02.Views
{
  /// <summary>
  /// P0202090100.xaml 的互動邏輯
  /// </summary>
  public partial class P0202090100 : Wms3plWindow
  {
    private RecvRecords _selectedRecord;

    public P0202090100(string selectedDc, SelectionItem<RecvRecords> selectedRecord)
    {
      InitializeComponent();
      _selectedRecord = selectedRecord.Item;

      Vm.RePrintItemLabel += RePrintItemLabel;
      Vm.RePrintRecvNote += RePrintRecvNote;

      Vm.SelectedDc = selectedDc;
      Vm.SelectRecords = selectedRecord;

      SetRadioButton();

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

    private void SetRadioButton()
    {
      RadioButton_IDLabel.IsEnabled = _selectedRecord.IS_PRINT_ITEM_ID_RAW == "1" ? true : false;
      RadioButton_RecvNote.IsEnabled = _selectedRecord.IS_PRINT_RECVNOTE_RAW == "1" ? true : false;

      if (RadioButton_IDLabel.IsEnabled)
      {
        RadioButton_IDLabel.IsChecked = true;
      }
      else
      {
        RadioButton_RecvNote.IsChecked = true;
      }
    }

    private void RePrintItemLabel()
    {
      if (Vm.ItemLabelData != null && Vm.ItemLabelData.Any())
      {
        var report = ReportHelper.CreateAndLoadReport<Report.P0202090000>();
        report.SummaryInfo.ReportComments = Wms3plSession.Get<GlobalInfo>().DcCodeList.Find(x => x.Value.Equals(Vm.SelectedDc)).Name;
        report.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().CustName;
        report.SummaryInfo.ReportAuthor = Wms3plSession.CurrentUserInfo.AccountName;

        report.SetDataSource(Vm.ItemLabelDataTable);

        var crystalReportService = new CrystalReportService(report, Vm.SelectedF910501);
        crystalReportService.PrintToPrinter(PrinterType.Label, Convert.ToInt16(Vm.txtCpies));
      }
    }

    private void RePrintRecvNote()
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

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
