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
using CrystalDecisions.CrystalReports.Engine;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.P71.Report;
using Wms3pl.WpfClient.P71.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.Views
{
	/// <summary>
	/// P7107050000.xaml 的互動邏輯
	/// </summary>
	public partial class P7107050000 : Wms3plUserControl
	{
		public P7107050000()
		{
			InitializeComponent();

		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Vm.DoPrint += PrintReport;
		}

		private void PrintReport(PrintType printType)
		{
			var printStaff = new KeyValuePair<string, string>("PrintStaff", Wms3plSession.Get<UserInfo>().AccountName);
			switch (Vm.SelectReportType)
			{
				case "0":
					CallReport(new R7107050001(), printType, Vm.GetP710705BackWarehouseInventory(), Properties.Resources.P7107050000xamlcs_BackWarehouseInventory_REPORT, printStaff);
					break;
				case "1":
					CallReport(new R7107050002(), printType, Vm.GetP710705MergeExecution(), Properties.Resources.P7107050000xamlcs_MergeExecution_REPORT, printStaff);
					break;
				case "2":
					CallReport(new R7107050003(), printType, Vm.GetP710705Availability(), Properties.Resources.P7107050000xamlcs_Availability_REPORT, printStaff);
					break;
				case "3":
					CallReport(new R7107050004(), printType, Vm.GetP710705ChangeDetail(), Properties.Resources.P7107050000xamlcs_ChangeDetail_REPORT, printStaff);
					break;
				case "4":
					CallReport(new R7107050005(), printType, Vm.GetP710705WarehouseDetail(), Properties.Resources.P7107050000xamlcs_WarehouseDetail_REPORT, printStaff);
					break;
			}
		}

		private void CallReport<T>(ReportClass report, PrintType printType, List<T> data, string title, params KeyValuePair<string, string>[] param) where T : class , new()
		{
			if (data == null || !data.Any())
			{
				DialogService.ShowMessage(Properties.Resources.P7101080000xamlcs_listNoData);
				return;
			}
			var dataTable = data.ToDataTable();
			report.SetDataSource(dataTable);
			report.SummaryInfo.ReportTitle = title;
			foreach (var p in param)
				report.SetParameterValue(p.Key, p.Value);

			var win = new Wms3plViewer { Owner = System.Windows.Window.GetWindow(this) };
			win.CallReport(report, printType);
		}


	}
}
