using System;
using System.Collections.Generic;
using System.Data;
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
using Wms3pl.WpfClient.ExDataServices.P01ExDataService;
using Wms3pl.WpfClient.P01.Report;
using Wms3pl.WpfClient.P01.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P01.Views
{
	/// <summary>
	/// P0101010000.xaml 的互動邏輯
	/// </summary>
	public partial class P0101010000 : Wms3plUserControl
	{
		public P0101010000()
		{
			InitializeComponent();
			SetFocusedElement(DcComboBox);
			Vm.OnAddFocus += OnAddFocus;
			Vm.OnEditFocus += OnEditFocus;
			//Vm.OnFocus += FocusAction;
			Vm.OnPrintAction += PrintAction;
			Vm.AddAction += AddDetailAction;
			Vm.DeleteAction += DelDetailAction;
		}

		private void FocusAction()
		{
			SetFocusedElement(TxtVnrCode);
		}

		private void TxtVnrCode_OnLostFocus(object sender, RoutedEventArgs e)
		{
			Vm.CheckVnrCode();
		}

		private void TxtVnrCode_OnKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				Vm.CheckVnrCode();
		}

		private void PrintAction(PrintType printType,DataTable data)
		{

			//var report = new RP0101010001();
			//report.Load(@"RP0101010001.rpt");
			var report = ReportHelper.CreateAndLoadReport<RP0101010001>();

			report.SetDataSource(data);

			report.SummaryInfo.ReportAuthor = Wms3plSession.CurrentUserInfo.AccountName;
			report.SummaryInfo.ReportTitle = "代採購單";

			var win = new Wms3plViewer { Owner = System.Windows.Window.GetWindow(this) };
			win.CallReport(report, printType);
		}

		private void AddDetailAction()
		{
			DgEditDetail.Items.MoveCurrentToLast();
			if (DgEditDetail.Items.CurrentItem != null)
				DgEditDetail.ScrollIntoView(DgEditDetail.Items.CurrentItem);
		}

		private void DelDetailAction()
		{
			DgEditDetail.Items.Refresh();
		}

		private void OnEditFocus()
		{
			SetFocusedElement(PurchaseDatePicker);
		}

		private void OnAddFocus()
		{
			SetFocusedElement(DcComboBoxForAddNew);
		}
	}
}
