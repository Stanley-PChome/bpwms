using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Wms3pl.WpfClient.P02.ViewModel;
using Wms3pl.WpfClient.P08.Report;
using Wms3pl.WpfClient.UILib;
using System;
using System.Windows.Controls;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.Common;
using System.Linq;
using Wms3pl.WpfClients.SharedViews.Views;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.DataServices.F91DataService;

namespace Wms3pl.WpfClient.P02.Views
{
	public partial class P0202030000 : Wms3plUserControl
	{
		public P0202030000()
		{
			InitializeComponent();
			Vm.AfterDoAcceptance += AcceptanceReport;
			Vm.OnEdit += OnEdit;
			Vm.OnCancel += OnCancel;
			Vm.SerialAndAlloctionReport += SerialAndAlloctionReport;
			Vm.OnDcCodeChanged += ShowDeviceWindowAndSetF910501;
			Vm.SetPurchaseNoFocus += SetPurchaseNoFocus;
		}

		/// <summary>
		/// 上傳檔案
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UploadFile_OnClick(object sender, RoutedEventArgs e)
		{
			var win = new P0202030200(Vm.SelectedDc, Vm.SelectedPurchaseNo, Vm.RtNo);
			win.ShowDialog();
			Vm.SearchCommand.Execute(null);
		}

		/// <summary>
		/// 商品檢驗
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void VerifyItem_OnClick(object sender, RoutedEventArgs e)
		{
			bool bGoReadSerial = false;
			bool bGoCollectSerial = false;
			if (Vm.SelectedData.BUNDLE_SERIALNO == "1")
			{
				if (!Vm.ExistsF020301Data()) //沒有資料時,顯示提示訊息
				{
					if (Vm.ShowMessage(Messages.WarningBeforeGoP02020301) == DialogResponse.Yes) bGoCollectSerial = true;
				}
			}
			if (bGoCollectSerial)
			{
				var win = new P0202030600(Vm.SelectedData, Vm.SelectedDc, Vm.RtNo);
				win.ShowDialog();
			}
			else
			{
				var win = new P0202030100(Vm.SelectedData, Vm.SelectedDc, Vm.SelectedDt, Vm.RtNo, bGoReadSerial);
				win.ShowDialog();
			}
			Vm.SearchCommand.Execute(null);
		}

		/// <summary>
		/// 序號刷讀
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ReadSerial_OnClick(object sender, RoutedEventArgs e)
		{


			bool bGoReadSerial = true;
			bool bGoCollectSerial = false;
			if (Vm.SelectedData.BUNDLE_SERIALNO == "1")
			{
				if (!Vm.ExistsF020301Data()) //沒有資料時,顯示提示訊息
				{
					if (Vm.ShowMessage(Messages.WarningBeforeGoP02020301) == DialogResponse.Yes) bGoCollectSerial = true;
				}
			}
			if (bGoCollectSerial)
			{
				var win = new P0202030600(Vm.SelectedData, Vm.SelectedDc, Vm.RtNo);
				win.ShowDialog();
			}
			else
			{
				var win = new P0202030100(Vm.SelectedData, Vm.SelectedDc, Vm.SelectedDt, Vm.RtNo, bGoReadSerial);
				win.ShowDialog();
			}
			Vm.SearchCommand.Execute(null);


		}

		/// <summary>
		/// 列印版箱標
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PrintBoxLabel_OnClick(object sender, RoutedEventArgs e)
		{
			var win = new P0202030500(Vm.SelectedDc, Vm.SelectedData, Vm.SelectedPurchaseNo);
			win.ShowDialog();
			Vm.SearchCommand.Execute(null);
		}

		/// <summary>
		/// 特殊採購
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SpecialPurchase_OnClick(object sender, RoutedEventArgs e)
		{
			var win = new P0202030400(Vm.DataForSpecialPurchase, Vm.SelectedDc, Vm.SelectedPurchaseNo);
			win.ShowDialog();
			Vm.SearchCommand.Execute(null);
		}

		/// <summary>
		/// 序號收集
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CollectSerial_OnClick(object sender, RoutedEventArgs e)
		{
			var win = new P0202030600(Vm.SelectedData, Vm.SelectedDc, Vm.RtNo);
			win.ShowDialog();
			Vm.SearchCommand.Execute(null);
		}

		private void txtPurchaseNo_KeyDown(object sender, KeyEventArgs e)
		{
			TextBox obj = sender as TextBox;
			if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(obj.Text))
				Vm.SearchCommand.Execute(true);
		}

		/// <summary>
		/// 驗收
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AcceptanceReport()
		{
			PrintAcceptanceReport();

			if (Vm.F051201ReportDataADatas != null && Vm.F051201ReportDataADatas.Any())
			{
				//列印揀貨單
				PrintPickReport();
			}
		}

		private void SerialAndAlloctionReport()
		{
			// 列印序號報表
			if (Vm.AcceptanceSerialDatas != null && Vm.AcceptanceSerialDatas.Any())
			{
				PrintAcceptanceSerialReport();
			}
			// 列印調撥單貼紙
			if (Vm.F151001ReportByAcceptance != null && Vm.F151001ReportByAcceptance.Any())
			{
				PrintAcceptanceAlloctionReport();
			}
		}

		private void PrintPickReport()
		{
			//var report = new RP0501010001();
			//report.Load(@"RP0501010001.rpt");
			var report = ReportHelper.CreateAndLoadReport<RP0501010001>();
			report.SetDataSource(Vm.F051201ReportDataADataDataTable);
			report.SetParameterValue("StaffName", Wms3plSession.CurrentUserInfo.AccountName);

			var crystalReportService = new CrystalReportService(report, Vm.SelectedF910501);
			crystalReportService.PrintToPrinter();
		}

		private void PrintAcceptanceReport()
		{
			//var report = new Report.P0202030000();
			//report.Load("P0202030000.rpt");
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
				//report.SetDataSource(Vm.AcceptanceReportDataTable);
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
				//report1.SetDataSource(Vm.AcceptanceReportDataTable1);
				report1.SetDataSource(Vm.AcceptanceReportDataTable1);
				report1.Subreports[0].SetDataSource(Vm.F151001ReportByAcceptanceDefectDataTable);
				report1.Subreports[1].SetDataSource(Vm.DefectDetailReportDataTable);
				var crystalReportService = new CrystalReportService(report1, Vm.SelectedF910501);
				crystalReportService.PrintToPrinter();
			}

			if ((Vm.AcceptanceReportData != null && Vm.AcceptanceReportData.Any()) || (Vm.AcceptanceReportData1 != null && Vm.AcceptanceReportData1.Any()))
			{
				var rtNo = string.Empty;
				if (Vm.AcceptanceReportData != null && Vm.AcceptanceReportData.Any())
					rtNo = Vm.AcceptanceReportData.First().RT_NO;
				if (Vm.AcceptanceReportData1 != null && Vm.AcceptanceReportData1.Any())
					rtNo = Vm.AcceptanceReportData1.First().RT_NO;
				if (!string.IsNullOrEmpty(rtNo))
				{
					Vm.UpdatePrintRecvNote(rtNo);
				}
			}


		}

		private void PrintAcceptanceSerialReport()
		{
			//TODO 序號報表
			//var report = new Report.P0202030001();
			//report.Load("P0202030001.rpt");
			var report = ReportHelper.CreateAndLoadReport<P0202030001>();
			report.SummaryInfo.ReportTitle = string.Format("{0}{1}{2}", Wms3plSession.Get<GlobalInfo>().CustName, Environment.NewLine, Properties.Resources.P0202030000_AcceptanceSerialReport);
			report.SetDataSource(Vm.AcceptanceSerialDataTable);
			var crystalReportService = new CrystalReportService(report, Vm.SelectedF910501);
			crystalReportService.PrintToPrinter();
		}

		private void PrintAcceptanceAlloctionReport()
		{
			var report = ReportHelper.CreateAndLoadReport<P0202030002>();
			var tmpF151001ReportByAcceptanceDataTable = Vm.F151001ReportByAcceptanceDataTable;
			tmpF151001ReportByAcceptanceDataTable.Merge(Vm.F151001ReportByAcceptanceDefectDataTable);
			//Vm.F151001ReportByAcceptance.Add(Vm.F151001ReportByAcceptanceDefect).ToList().ForEach(x => x.AllocationNoBarcode = BarcodeConverter128.StringToBarcode(x.ALLOCATION_NO)).ToDataTable;
			//var Vm.F151001ReportByAcceptanceDataTable.Merge(Vm.F151001ReportByAcceptanceDefectDataTable);
			report.SetDataSource(tmpF151001ReportByAcceptanceDataTable);
			var crystalReportService = new CrystalReportService(report, Vm.SelectedF910501);
			crystalReportService.PrintToPrinter(PrinterType.Label);
		}
		private void OnEdit()
		{
			dgList.Columns[4].IsReadOnly = false;
		}

		private void OnCancel()
		{
			dgList.Columns[4].IsReadOnly = true;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// 在切換 Tab 時，UserControl 會重新觸發 Loaded，故需判斷是否已經設定過
			if (Vm.SelectedF910501 == null)
				ShowDeviceWindowAndSetF910501();
		}

		void ShowDeviceWindowAndSetF910501()
		{
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

		private void CmbTarwarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var cmbox = sender as ComboBox;
			var p020203Data = cmbox.DataContext as P020203Data;
			if (p020203Data != null)
				Vm.TarWarehouseChange(p020203Data.PURCHASE_NO, p020203Data.PURCHASE_SEQ, p020203Data.TARWAREHOUSE_ID);
		}

		private void SetDefectQty_OnClick(object sender, RoutedEventArgs e)
		{
			var win = new P0202030700(Vm.SelectedData);
			win.ShowDialog();
			Vm.SearchCommand.Execute(null);
		}

		private void SetPurchaseNoFocus()
		{
			SetFocusedElement(txtPurchaseNo, true);
		}
	}
}
