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
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P02.Views
{
	public partial class P0202010000 : Wms3plUserControl
	{

		public P0202010000()
		{
			InitializeComponent();
			Vm.OnSearchPurchaseNoComplete += FocusAfterSelectedPurchaseNo;
			Vm.SearchPurchaseNoByCustOrdNoComplete += FocusAfterSearchPurchaseNoByCustOrdNo;
			Vm.AfterSave += DoPrint;
		}

		/// <summary>
		/// 儲存成功後列印報表
		/// </summary>
		private void DoPrint()
		{
			if (Vm.ReportData != null)
			{
				//var report = new Wms3pl.WpfClient.P02.Report.P0202010000();
				//report.Load("P0202010000.rpt");
				var report = ReportHelper.CreateAndLoadReport<Report.P0202010000>();
				report.SummaryInfo.ReportComments = Wms3plSession.Get<GlobalInfo>().DcCodeList.Find(x => x.Value.Equals(Vm.SelectedDc)).Name;
				report.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().GupName + "－" +
																 Wms3plSession.Get<GlobalInfo>().CustName +
																 Properties.Resources.P0202010000_PurchaseRP;
				report.SetText("txtVnrCode", Vm.VnrCode);
				report.SetText("txtVnrName", Vm.VnrName);
				//report.SetText("txtCarNumber", Vm.CarNumber);

				report.SetText("txtPurchaseNo", Vm.SelectedPurchaseNo);
				report.SetText("txtPrintStaffName", Wms3plSession.CurrentUserInfo.AccountName);
				report.SetText("txtCustOrdNo", Vm.ReportData.Select(x => x.CUST_ORD_NO).FirstOrDefault() ?? "");
				report.SetText("txtShopNo", Vm.ReportData.Select(x => x.SHOP_NO).FirstOrDefault() ?? "");
				report.SetText("txtSourceNo", Vm.ReportData.Select(x => x.SOURCE_NO).FirstOrDefault() ?? "");

				Vm.ReportData.ForEach(x => x.ItemCodeBarcode = BarcodeConverter128.StringToBarcode(x.ITEM_CODE));
				report.SetDataSource(Vm.ReportData);

				report.SetParameterValue("PurchaseNoBarcode", BarcodeConverter128.StringToBarcode(Vm.SelectedPurchaseNo));

				var win = new Wms3plViewer { Owner = Wms3plViewer.GetWindow(this) };
				win.CallReport(report, PrintType.Preview);
			}
		}

		private void TxtPurchaseNo_KeyDown(object sender, KeyEventArgs e)
		{
			//TextBox obj = sender as TextBox;
			if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(TxtPurchaseNo.Text))
			{
				//SetFocusedElement(TxtSelectedPurchaseNo);
				//TxtSelectedPurchaseNo.SelectAll();

				//Vm.DoSave();
				//if (Vm.DoSave()) DoPrint();
				Vm.SearchPurchaseNoCommand.Execute(null);
				//if (Vm.SoundOn) PlaySoundHelper.Scan();
			}
		}

		private void TxtCustOrdNo_KeyDown(object sender, KeyEventArgs e)
		{
			//if (e.Key != Key.Enter)
			//    return;
			if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(TxtCustOrdNo.Text))
			{
				Vm.SearchPurchaseNoByCustOrdNoCommand.Execute(null);
				//if (Vm.SoundOn) PlaySoundHelper.Scan();

			}


			//var searchResult = Vm.DoSearchPurchaseNoByCustOrdNo();

			//SetFocusedElement(TexCustOrdNo);
			//TexCustOrdNo.SelectAll();
			//if (searchResult)
			//         {
			//	Vm.DoSave();
			//}

		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			//if (string.IsNullOrEmpty(Vm._gupCode))
			//    GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			//if (string.IsNullOrEmpty(CustCode))
			//    CustCode = Wms3plSession.Get<GlobalInfo>().CustCode;

			var win = new WinSearchProduct();
			win.GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			win.CustCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			win.Owner = this.Parent as Window;
			win.ShowDialog();
			if (win.DialogResult.HasValue && win.DialogResult.Value)
			{
				//SetData(win.SelectData);
				Vm.ItemCodeByQueryCondition = win.SelectData.ITEM_CODE;
			}
		}

		private void Window_OnLoaded(object sender, RoutedEventArgs e)
		{
			FocusSelectedCustOrdNo();
		}

		private void FocusSelectedCustOrdNo()
		{
			SetFocusedElement(TxtCustOrdNo);
		}

		private void FocusAfterSelectedPurchaseNo()
		{
			if (Vm.IsUseful)
			{
				Vm.DoSearchPurchaseNo();
			}

			SetFocusedElement(TxtPurchaseNo, true);
		}

		private void FocusSearchPurchaseNoByCustOrdNo()
		{
			SetFocusedElement(TxtCustOrdNo);
		}

		private void FocusAfterSearchPurchaseNoByCustOrdNo()
		{
			if (Vm.IsUseful)
			{
				Vm.DoSearchPurchaseNo();
			}
			SetFocusedElement(TxtCustOrdNo, true);
		}
	}
}