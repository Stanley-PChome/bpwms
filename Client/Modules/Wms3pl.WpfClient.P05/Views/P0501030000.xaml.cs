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
using Wms3pl.WpfClient.P05.Report;
using Wms3pl.WpfClient.P05.ViewModel;
using Wms3pl.WpfClient.P08.Report;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P05.Views
{
	/// <summary>
	/// P0501030000.xaml 的互動邏輯
	/// </summary>
	public partial class P0501030000 : Wms3plUserControl
	{
		public P0501030000()
		{
			InitializeComponent();
			Vm.OnPrintAction += PrintAction;
		}


		private void PrintAction(PrintType printType, DataTable data)
		{

			//var report = new RP0501010003();
			var report = ReportHelper.CreateAndLoadReport<RP0501010003>();

			report.SetDataSource(data);

			report.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().CustName+Properties.Resources.P0501030000_PickReport;
			report.SetText("txtUserName", Wms3plSession.Get<UserInfo>().AccountName);
			var win = new Wms3plViewer { Owner = System.Windows.Window.GetWindow(this) };
			win.CallReport(report, printType);
		}

	
	}
}