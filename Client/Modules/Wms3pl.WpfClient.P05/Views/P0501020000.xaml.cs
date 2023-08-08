using CrystalDecisions.CrystalReports.Engine;
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
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.P05.Report;
using Wms3pl.WpfClient.P05.ViewModel;
using Wms3pl.WpfClient.P08.Report;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClients.SharedViews.Views;

namespace Wms3pl.WpfClient.P05.Views
{
	/// <summary>
	/// P0501020000.xaml 的互動邏輯
	/// </summary>
	public partial class P0501020000 : Wms3plUserControl
	{
		public P0501020000()
		{
			InitializeComponent();
			Vm.DoPrintReport += GetReport;
            Vm.OnDcCodeChanged += ShowDeviceWindowAndSetF910501;
        }
		/// <summary>
		/// 列印揀貨單或撥種單
		/// </summary>
		/// <param name="printType"></param>
		private void GetReport(PrintType printType)
		{
			

      if (Vm.F051201ReportDataAs.Any())
      {
          var list1 = Vm.F051201ReportDataAs;

          if (list1 == null || !list1.Any())
          {
              Vm.DialogService.ShowMessage(Properties.Resources.P0501020000_AllotStocksSuccess);
              return;
          }

          list1.ForEach(x =>
          {
              x.SerialNoBarcode = BarcodeConverter128.StringToBarcode(x.SERIAL_NO == null ? string.Empty : x.SERIAL_NO);
              x.PickOrdNoBarcode = BarcodeConverter128.StringToBarcode(x.PICK_ORD_NO);
              x.WmsOrdNoBarcode = BarcodeConverter128.StringToBarcode(x.WMS_ORD_NO);
              x.ItemCodeBarcode = BarcodeConverter128.StringToBarcode(x.ITEM_CODE);
              x.PickLocBarcode = BarcodeConverter128.StringToBarcode(x.PICK_LOC.Replace("-", ""));
          });
          //var report1 = new RP0501010001();
          //report1.Load(@"RP0501010001.rpt");

          var report1 = ReportHelper.CreateAndLoadReport<RP0501010006>();
          report1.SetDataSource(list1.ToDataTable());
          report1.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().GupName + "－" +
                        Wms3plSession.Get<GlobalInfo>().CustName +
                        Properties.Resources.RP0501010006_TITLE;
          report1.SetParameterValue("StaffName", Wms3plSession.CurrentUserInfo.AccountName);
					PrintReport(report1, Vm.SelectedF910501, printType, PrinterType.A4);
      }

      if (Vm.RP0501010004Model.Any())
      {
          var list2 = Vm.RP0501010004Model;
          list2.ForEach(x =>
          {
              x.BARCODE = BarcodeConverter128.StringToBarcode(x.WMS_ORD_NO);
          });
          var report2 = ReportHelper.CreateAndLoadReport<RP0501010004>();
          report2.SetDataSource(list2.ToDataTable());
					PrintReport(report2, Vm.SelectedF910501, printType, PrinterType.Label);
			}

         
      if (Vm.P050103ReportData.Any())
      {

          var list3 = Vm.P050103ReportData;
          list3.ForEach(x =>
          {
              x.ITEM_CODE_BARCODE = BarcodeConverter128.StringToBarcode(x.ITEM_CODE);
              x.PICK_LOC_BARCODE = BarcodeConverter128.StringToBarcode(x.PICK_LOC);
              x.PICK_ORD_NO_BARCODE = BarcodeConverter128.StringToBarcode(x.PICK_ORD_NO);
          });
          var report3 = ReportHelper.CreateAndLoadReport<RP0501010007>();
          report3.SetDataSource(list3.ToDataTable());
          report3.SetParameterValue("StaffName", Wms3plSession.CurrentUserInfo.AccountName);
          var win3 = new Wms3plViewer { Owner = System.Windows.Window.GetWindow(this) };
					PrintReport(report3, Vm.SelectedF910501, printType, PrinterType.A4);
			}

      if (Vm.RP0501010005Model.Any())
      {
          var list4 = Vm.RP0501010005Model;
          list4.ForEach(x =>
          {
              x.BARCODE = BarcodeConverter128.StringToBarcode(x.PICK_ORD_NO);
          });
          var report4 = ReportHelper.CreateAndLoadReport<RP0501010005>();
          report4.SetDataSource(list4.ToDataTable());
					PrintReport(report4, Vm.SelectedF910501, printType, PrinterType.Label);
			}
         
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 在切換 Tab 時，UserControl 會重新觸發 Loaded，故需判斷是否已經設定過
            if (Vm.SelectedF910501 == null)
                ShowDeviceWindowAndSetF910501();
        }
        void ShowDeviceWindowAndSetF910501()
        {
            var openDeviceWindow = OpenDeviceWindow(Vm.FunctionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, Vm.SelectedDcCode);
            if (openDeviceWindow.Any())
            {
                Vm.SelectedF910501 = openDeviceWindow.FirstOrDefault();
            }
            else
            {
                var deviceWindow = new DeviceWindow(Vm.SelectedDcCode);
                deviceWindow.Owner = this.Parent as Window;
                deviceWindow.ShowDialog();
                Vm.SelectedF910501 = deviceWindow.SelectedF910501;
            }
            
        }
		private void PrintReport(ReportClass report, F910501 device,PrintType printType, PrinterType printerType = PrinterType.A4)
		{
			CrystalReportService crystalReportService;
			if (printType == PrintType.Preview)
			{
				crystalReportService = new CrystalReportService(report);
				crystalReportService.ShowReport(null, PrintType.Preview);
			}
			else
			{
				crystalReportService = new CrystalReportService(report, device);
				crystalReportService.PrintToPrinter(printerType);
			}
		}
	}
}
