using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.P02.ViewModel;
using Wms3pl.WpfClient.P08.Report;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P02.Views
{
	public partial class P0202070000 : Wms3plUserControl
	{

		public P0202070000()
		{
			InitializeComponent();
			Vm.OnPreviewComplete += OnPreviewComplete;

		}
		

		private void OnPreviewComplete()
		{
			AcceptanceReport();
		}
		/// <summary>
		/// 驗收
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AcceptanceReport()
		{
			// 良品驗收單
			if (Vm.AcceptanceReportData != null && Vm.AcceptanceReportData.Any())
			{
				var report = ReportHelper.CreateAndLoadReport<Report.P0202030000>();
				report.SummaryInfo.ReportComments = Wms3plSession.Get<GlobalInfo>().DcCodeList.Find(x => x.Value.Equals(Vm.SelectedDc)).Name;
				report.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().CustName;
				report.SummaryInfo.ReportAuthor = Wms3plSession.CurrentUserInfo.AccountName;
				report.SetText("txtPrintStaffName", Wms3plSession.CurrentUserInfo.AccountName);
				report.SetText("txtTicketNo", Vm.SelectedData.PURCHASE_NO);
				report.SetText("TxtRecvQty", "良品數");
				report.SetDataSource(Vm.AcceptanceReportDataTable);
				//Vm.F151001ReportByAcceptance.ForEach(x => x.AllocationNoBarcode = BarcodeConverter128.StringToBarcode(x.ALLOCATION_NO));
				report.Subreports[0].SetDataSource(Vm.F151001ReportByAcceptanceDataTable);
				//report.Database.Tables[1].SetDataSource(Vm.F151001ReportByAcceptanceDataTable);
				var win = new Wms3plViewer { Owner = Wms3plViewer.GetWindow(this) };
				win.CallReport(report, PrintType.Preview);
			}

			// 不良品驗收單
			if (Vm.AcceptanceReportData1 != null && Vm.AcceptanceReportData1.Any())
			{
				var report1 = ReportHelper.CreateAndLoadReport<Report.P0202030000>();
				report1.SummaryInfo.ReportComments = Wms3plSession.Get<GlobalInfo>().DcCodeList.Find(x => x.Value.Equals(Vm.SelectedDc)).Name;
				report1.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().CustName;
				report1.SummaryInfo.ReportAuthor = Wms3plSession.CurrentUserInfo.AccountName;
				report1.SetText("txtPrintStaffName", Wms3plSession.CurrentUserInfo.AccountName);
				report1.SetText("txtTicketNo", Vm.SelectedData.PURCHASE_NO);
				report1.SetText("TxtRecvQty", "不良品數");
				report1.SetDataSource(Vm.AcceptanceReportDataTable1);
				report1.Subreports[0].SetDataSource(Vm.F151001ReportByAcceptanceDefectDataTable);
				var win1 = new Wms3plViewer { Owner = Wms3plViewer.GetWindow(this) };
				win1.CallReport(report1, PrintType.Preview);
			}
		}
		

		private void VnrCode_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			Vm.SearchVnrCommand.Execute(null);
		}

	}
}