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
using Wms3pl.WpfClient.P71.Report;
using Wms3pl.WpfClient.P71.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.Views
{
	/// <summary>
	/// P7107020000.xaml 的互動邏輯
	/// </summary>
	public partial class P7107020000 : Wms3plUserControl
	{
		public P7107020000()
		{
			InitializeComponent();
			Vm.DoPrintReport += PrintReport;
		}

		private void PrintReport(PrintType obj)
		{
			switch (Vm.SelectedReportType)
			{
				case "2"://綜合
					var list2 = Vm.IntegrateRecords;
					var report2 = new R7107020003();
					report2.Load(@"R7107020003.rpt");
					report2.SetDataSource(list2);
					report2.SummaryInfo.ReportTitle = Properties.Resources.P7107020000xamlcs_COMPLEX_REPORT;
					report2.SummaryInfo.ReportAuthor = Wms3plSession.CurrentUserInfo.AccountName;
					report2.SetParameterValue("DC_NAME", Comb_DC.Text);
					report2.SetParameterValue("ALLID_NAME", Comb_AllId.Text);
					report2.SetParameterValue("CUST_NAME", Comb_Cust.Text);
					report2.SetParameterValue("SEARCH_DATE",
						string.Format("{0} ~ {1}", Vm.SelectDateStart.ToShortDateString(), Vm.SelectDateEnd.ToShortDateString()));
					var win2 = new Wms3plViewer { Owner = System.Windows.Window.GetWindow(this) };
					win2.CallReport(report2, obj);
					break;
				case "3"://派車
					var list3 = Vm.DistrCarRecords;
					var report3 = new R7107020002();
					report3.Load(@"R7107020002.rpt");
					report3.SetDataSource(list3);
					report3.SummaryInfo.ReportTitle = Properties.Resources.P7107020000xamlcs_DELV_REPORT;
					report3.SummaryInfo.ReportAuthor = Wms3plSession.CurrentUserInfo.AccountName;
					report3.SetParameterValue("DC_NAME", Comb_DC.Text);
					report3.SetParameterValue("ALLID_NAME", Comb_AllId.Text);
					report3.SetParameterValue("CUST_NAME", Comb_Cust.Text);
					report3.SetParameterValue("SEARCH_DATE",
						string.Format("{0} ~ {1}", Vm.SelectDateStart.ToShortDateString(), Vm.SelectDateEnd.ToShortDateString()));
					var win3 = new Wms3plViewer { Owner = System.Windows.Window.GetWindow(this) };
					win3.CallReport(report3, obj);
					break;
				case "4"://流通加工記錄表
					var list4 = Vm.ProcessRecords.OrderBy(o=>o.ROWNUM);
					var report4 = new R7107020004();
					report4.Load(@"R7107020004.rpt");
					report4.SetDataSource(list4);
					report4.SummaryInfo.ReportTitle = Properties.Resources.P7107020000xamlcs_CIRCULATION_REPORT;
					report4.SummaryInfo.ReportAuthor = Wms3plSession.CurrentUserInfo.AccountName;
					report4.SetParameterValue("DC_NAME", Comb_DC.Text);
					report4.SetParameterValue("OUTSOURCE_NAME", Comb_OutSource.Text);
					report4.SetParameterValue("SEARCH_DATE",
						string.Format("{0} ~ {1}", Vm.SelectDateStart.ToShortDateString(), Vm.SelectDateEnd.ToShortDateString()));
					var win4 = new Wms3plViewer { Owner = System.Windows.Window.GetWindow(this) };
					win4.CallReport(report4, obj);
					break;
				default:
					break;
			}
		}
	}
}
