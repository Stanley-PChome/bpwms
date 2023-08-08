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
using Wms3pl.WpfClient.P14.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P14.Views
{
	/// <summary>
	/// P1401020000.xaml 的互動邏輯
	/// </summary>
	public partial class P1401020000 : Wms3plUserControl
	{
		public P1401020000()
		{
			InitializeComponent();

			Vm.DoPrint += DoReport;
		}

		private void DoReport(PrintType printType)
		{
			if (Vm.SelectedF140101 == null)
			{
				ShowMessage(Properties.Resources.P1401020000xamlcs_ChooseInventory);
				return;
			}
			var list = Vm.GetReportData();
			if (list == null || list.Count == 0)
			{
				ShowMessage(Properties.Resources.P1401010000xamlcs_QueryDataEmpty);
				return;
			}
			var report = new R1401020000();
			report.Load(@"R1401020000.rpt");
			report.SetDataSource(list);
			report.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().GupName + "－" +
											 Wms3plSession.Get<GlobalInfo>().CustName +
			Properties.Resources.P1401020000xamlcs_DifferentInventory;
			report.SetParameterValue("PRINT_STAFF_NAME", Wms3plSession.Get<UserInfo>().AccountName);
      report.SetParameterValue("IsAutoWarehouse", Vm.SelectedF140101.CHECK_TOOL != "0");
      var win = new Wms3plViewer { Owner = System.Windows.Window.GetWindow(this) };
			win.CallReport(report, printType);
		}
	}
}
