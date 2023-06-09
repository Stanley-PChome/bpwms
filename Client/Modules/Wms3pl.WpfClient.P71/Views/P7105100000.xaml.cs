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
	/// P7105100000.xaml 的互動邏輯
	/// </summary>
	public partial class P7105100000 : Wms3plUserControl
	{
		public P7105100000()
		{
			InitializeComponent();
			Vm.DoPreview += DoPreview;
		}

		private void DoPreview(ViewModel.F710510ReportType reportType)
		{
			switch (reportType)
			{
				case F710510ReportType.F7105100001:
					ReportF7105100001();
					break;
				case F710510ReportType.F7105100002:
					ReportF7105100002();
					break;
				case F710510ReportType.F7105100003:
					ReportF7105100003();
					break;
				case F710510ReportType.F7105100004:
					ReportF7105100004();
					break;
				case F710510ReportType.F7105100005:
					ReportF7105100005();
					break;
				default:
					throw new ArgumentOutOfRangeException("reportType", reportType, null);
			}
		}

		private void ReportF7105100001()
		{
			var report = new R7105100001();			
			var data = Vm.GetRp7105100001Data();
			if (!data.Any())
			{
				Vm.ShowNoDataMessage();
				return;
			}
			report.SetDataSource(data);
			report.SetParameterValue("footerYearMonth",
				string.Format(Properties.Resources.P7105100000xamlcs_CheckMonthFeeDetailCorreectMessage, Vm.SelectClearingYearMonth.Value.Year,
					Vm.SelectClearingYearMonth.Value.Month));			
			Preview(report);
		}

		private void ReportF7105100002()
		{
			//出貨處理費明細表
			var report = new R7105100002();			
			var data = Vm.GetRp7105100002Data();
			if (!data.Any())
			{
				Vm.ShowNoDataMessage();
				return;
			}
			data.ForEach(x =>
			{
				var consignee = (x.CUST_NAME ?? string.Empty);				
				var charList = consignee.ToArray();
				if (charList.Length > 1)
					charList[1] = '*';
				x.CUST_NAME = new string(charList);
			});			
			report.SetDataSource(data);
			report.SummaryInfo.ReportTitle =
				string.Format(Properties.Resources.P7105100000xamlcs_CNT_DATE_RANGE, data.Select(x => x.CNT_DATE_RANGE).FirstOrDefault() ?? string.Empty);
			Preview(report);
		}

		private void ReportF7105100003()
		{
			//退貨運送費明細表
			var report = new R7105100003();			
			var data = Vm.GetRp7105100003Data();
			if (!data.Any())
			{
				Vm.ShowNoDataMessage();
				return;
			}
			data.ForEach(x =>
			{
				var consignee = (x.CUST_NAME ?? string.Empty);
				var charList = consignee.ToArray();
				if (charList.Length > 1)
					charList[1] = '*';
				x.CUST_NAME = new string(charList);
			});	
			report.SetDataSource(data);
			report.SummaryInfo.ReportTitle =
				string.Format(Properties.Resources.P7105100000xamlcs_CNT_DATE_BEGIN_END, data.Select(x => x.CNT_DATE_RANGE).FirstOrDefault());
			Preview(report);
		}

		private void ReportF7105100004()
		{
			var report = new R7105100004();			
			var data = Vm.GetRp7105100004Data();
			if (!data.Any())
			{
				Vm.ShowNoDataMessage();
				return;
			}
			data.ForEach(x =>
			{
				var consignee = (x.CUST_NAME ?? string.Empty);
				var charList = consignee.ToArray();
				if (charList.Length > 1)
					charList[1] = '*';
				x.CUST_NAME = new string(charList);
			});	
			report.SetDataSource(data);
			report.SummaryInfo.ReportTitle =
				string.Format(Properties.Resources.P7105100000xamlcs_RTN_Processing_Fee_Detail, data.Select(x => x.CNT_DATE_RANGE).FirstOrDefault());
			Preview(report);
		}

		private void ReportF7105100005()
		{
			var report = new R7105100005();			
			var data = Vm.GetRp7105100005Data();
			if (!data.Any())
			{
				Vm.ShowNoDataMessage();
				return;
			}
			data.ForEach(x =>
			{
				var consignee = (x.CUST_NAME ?? string.Empty);
				var charList = consignee.ToArray();
				if (charList.Length > 1)
					charList[1] = '*';
				x.CUST_NAME = new string(charList);
			});	
			report.SetDataSource(data);
			report.SummaryInfo.ReportTitle =
				string.Format(Properties.Resources.P7105100000xamlcs_OTHER_SendFeeDetail, data.Select(x => x.CNT_DATE_RANGE).FirstOrDefault());

			var dictionary = new Dictionary<string, int>();
			foreach (var d in data)
			{
				if (string.IsNullOrEmpty(d.CAUSE))
				{
					continue;
				}
				if (!dictionary.ContainsKey(d.CAUSE))
				{
					dictionary.Add(d.CAUSE, 1);
				}
				else
				{
					dictionary[d.CAUSE] += 1;
				}
			}
			var footerTotal = string.Empty;
			var spilt = '、';
			foreach (var dic in dictionary)
			{
				footerTotal += String.Format("{0}*{1}{2}", dic.Key, dic.Value, spilt);
			}
			footerTotal = footerTotal.TrimEnd(spilt);

			report.SetParameterValue("footerTotalCause", footerTotal);
			Preview(report);
		}

		private void Preview(ReportClass report)
		{
			var win = new Wms3plViewer { Owner = System.Windows.Window.GetWindow(this) };
			win.CallReport(report, PrintType.Preview);
		}


	}
}
