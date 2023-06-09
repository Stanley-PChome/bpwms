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
using Wms3pl.WpfClient.P05.Report;
using Wms3pl.WpfClient.P05.ViewModel;
using Wms3pl.WpfClient.P08.Report;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P05.Views
{
	/// <summary>
	/// P0501010000.xaml 的互動邏輯
	/// </summary>
	public partial class P0501010000 : Wms3plUserControl
	{
		public P0501010000()
		{
			InitializeComponent();
			Vm.DoPrintReport += GetReport;
		}
		/// <summary>
		/// 列印揀貨單或撥種單
		/// </summary>
		/// <param name="printType"></param>
		private void GetReport(PrintType printType)
		{
			if (Vm.RbReportTypeA)
			{
				var list1 = Vm.F051201ReportDataAs;
				list1.ForEach(x =>
				{
					x.PickOrdNoBarcode = BarcodeConverter128.StringToBarcode(x.PICK_ORD_NO);
					x.WmsOrdNoBarcode = BarcodeConverter128.StringToBarcode(x.WMS_ORD_NO);
					x.ItemCodeBarcode = BarcodeConverter128.StringToBarcode(x.ITEM_CODE);
					x.SerialNoBarcode = BarcodeConverter128.StringToBarcode(x.SERIAL_NO == null ? string.Empty : x.SERIAL_NO);
					x.PickLocBarcode = BarcodeConverter128.StringToBarcode(x.PICK_LOC.Replace("-", ""));
				});

				//var report1 = new RP0501010001();
				//report1.Load(@"RP0501010001.rpt");
				var report1 = ReportHelper.CreateAndLoadReport<RP0501010001>();
				report1.SetDataSource(list1.ToDataTable());
				report1.SetParameterValue("StaffName", Wms3plSession.CurrentUserInfo.AccountName);
				var win1 = new Wms3plViewer { Owner = System.Windows.Window.GetWindow(this) };
				win1.CallReport(report1, printType);
			}
			else
			{
				var list2 = Vm.F051201ReportDataBs;
				list2.ForEach(x =>
				{
					x.PickOrdNoBarcode = BarcodeConverter128.StringToBarcode(x.PICK_ORD_NO);
					x.ItemCodeBarcode = BarcodeConverter128.StringToBarcode(x.ITEM_CODE);
				});

				//var report2 = new RP0501010002();
				var report2 = ReportHelper.CreateAndLoadReport<RP0501010002>();
				report2.SetDataSource(list2.ToDataTable());
				report2.SetParameterValue("StaffName", Wms3plSession.CurrentUserInfo.AccountName);
				var win2 = new Wms3plViewer { Owner = System.Windows.Window.GetWindow(this) };
				win2.CallReport(report2, printType);
			}
		}
	}
}
