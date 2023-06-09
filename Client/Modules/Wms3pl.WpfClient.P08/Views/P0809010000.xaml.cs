using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using Wms3pl.WpfClient.P08.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0809010000.xaml 的互動邏輯
	/// </summary>
	public partial class P0809010000 : Wms3plWindow
	{
		public P0809010000()
		{
			InitializeComponent();
			Vm.PrintAction += PrintCommand_Executed;
			Vm.SearchAction += SearchCommand_Executed;
		}
		private void dgScrollIntoView()
		{
			if (dgList.SelectedItem != null)
				dgList.ScrollIntoView(dgList.SelectedItem);
		}

		private void SearchCommand_Executed()
		{
			dgScrollIntoView();
		}

		private void PrintCommand_Executed()
		{
			if (Vm.RptDataList != null)
			{
				dgScrollIntoView();
				//var report = new Wms3pl.WpfClient.P08.Report.RP0809010000();
				//report.Load("RP0809010000.rpt");
				var report = ReportHelper.CreateAndLoadReport<Report.RP0809010000>();
				report.SetText("txtReportTitle", Wms3plSession.Get<GlobalInfo>().GupName + "－"
					+ Wms3plSession.Get<GlobalInfo>().CustName + Properties.Resources.P0809010000_ReportTitle);
				report.SetDataSource(Vm.RptDataList.Where(o=> o.STATUS == 5 || o.STATUS == 6));
				report.SetParameterValue("StaffName", Wms3plSession.CurrentUserInfo.AccountName);
				report.SetParameterValue("LoadOwnerQty", Vm.RptDataList.Select(o=>o.WMS_ORD_NO).Distinct().Count());
				report.SetParameterValue("LoadOwnerOkQty", Vm.RptDataList.Where(o=> o.STATUS ==5 || o.STATUS == 6).Select(o => o.WMS_ORD_NO).Distinct().Count());
				report.SetParameterValue("PackageBoxNo", Vm.RptDataList.Where(o=> o.STATUS == 5 || o.STATUS == 6).Sum(o=> o.BOXQTY));
				var win = new Wms3plViewer { Owner = Wms3plViewer.GetWindow(this) };
				win.CallReport(report, PrintType.Preview);
			}
		}
		private void btnFileUpload_Click(object sender, RoutedEventArgs e)
		{
			var myWin = new P0809010200(
				Wms3plSession.Get<GlobalInfo>().GupCode,
				Wms3plSession.Get<GlobalInfo>().CustCode,
				Vm.SelectedData.DC_CODE,
				Vm.SelectedData.DELV_DATE,
				Vm.SelectedData.TAKE_TIME,
				Vm.SelectedData.ALL_ID
				);
			
			 if(myWin.ShowDialog()??false)
				 Vm.SearchCommand.Execute(null);
			 
			//OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			//dlg.Multiselect = true;
			//dlg.DefaultExt = ".jpg";
			//dlg.Filter = "封條照片圖檔 (.jpg)|*.jpg";
			//List<string> files = new List<string>();
			//int imgMax = 3;
			//dlg.FileOk += delegate(object s, CancelEventArgs ev)
			//{
			//	if (dlg.FileNames.Count() > imgMax)
			//	{
			//		DialogService.ShowMessage("封條照片最多可上傳3張");
			//		files.Clear();
			//		ev.Cancel = true;
			//		return;
			//	}
			//	foreach (var p in dlg.FileNames)
			//	{
			//		if (new FileInfo(p).Length > GlobalVariables.FileSizeLimit)
			//		{
			//			Vm.ShowMessage(Messages.WarningFileSizeExceedLimits);
			//			files.Clear();
			//			ev.Cancel = true;
			//			return;
			//		}
			//		files.Add(p);
			//	}
			//};

			//if (dlg.ShowDialog() == true)
			//{
			//	Vm.FileList = files;
			//	Vm.UploadCommand.Execute(null);
			//}
		}

		private void btnViewFile_Click(object sender, RoutedEventArgs e)
		{
			var myWin = new P0809010100(
				Wms3plSession.Get<GlobalInfo>().GupCode,
				Wms3plSession.Get<GlobalInfo>().CustCode,
				Vm.SelectedData.DC_CODE,
				Vm.SelectedData.DELV_DATE,
				Vm.SelectedData.TAKE_TIME,
				Vm.SelectedData.PICK_TIME
				);
			//var myWin = new P0809010100(
			//	"01",
			//	"010001",
			//	Vm.SelectedData.DC_CODE,
			//	Vm.SelectedData.DELV_DATE,
			//	Vm.SelectedData.CHECKOUT_TIME,
			//	Vm.SelectedData.PICK_TIME
			//	);
			//var function = FormService.GetFunctionFromSession("P1902060000");
			//if (function == null)
			//{
			//	DialogService.ShowMessage("無使用權限");
			//	return;
			//}
			myWin.Show();
			e.Handled = true;
		}
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var dr = DialogService.ShowMessage(WpfClient.Resources.Resources.WarningBeforeClose, WpfClient.Resources.Resources.Information, UILib.Services.DialogButton.YesNo, UILib.Services.DialogImage.Warning);
			if (dr == UILib.Services.DialogResponse.No)
				e.Cancel = true;
		}

	}
}
