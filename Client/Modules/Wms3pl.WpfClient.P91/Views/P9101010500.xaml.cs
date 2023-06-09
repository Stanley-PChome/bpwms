using Microsoft.Win32;
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
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.P91.ViewModel;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P91.Views
{
	/// <summary>
	/// P9101010500.xaml 的互動邏輯
	/// </summary>
	public partial class P9101010500 : Wms3plWindow
	{
		public P9101010500(F910201 data)
		{
			InitializeComponent();
			Vm.ActionBeforeImportData += FileUpload;
			Vm.OnPrint += PrintReport;
			Vm.BaseData = data;
			//Vm.SearchCommand.Execute(null);
			Vm.InitDatas();
		}

		private void PrintReport(PrintType printType, List<ExDataServices.P91ExDataService.P91010105Report> reportData)
		{

			reportData.ForEach(x=>
			{
				x.AllocationNoBarcode = BarcodeConverter128.StringToBarcode(x.ALLOCATION_NO);
				x.ItemCodeBarcode = BarcodeConverter128.StringToBarcode(x.ITEM_CODE);
			});

			var report = new Report.RP9101010501();
			report.Load(@"RP9101010501.rpt");
			report.SetDataSource(reportData);
			report.SetParameterValue("StaffName", Wms3plSession.CurrentUserInfo.AccountName);
			var win = new Wms3plViewer { Owner = Wms3plViewer.GetWindow(this) };
			win.CallReport(report, printType);
		}



		/// <summary>
		/// 檔案上傳
		/// </summary>
		private void FileUpload()
		{
			var win = new OpenFileDialog { InitialDirectory = string.IsNullOrEmpty(Vm.FilePath) ? @"C:\" : Vm.FilePath };
			if (!win.CheckPathExists)
				win.InitialDirectory = @"C:\";

			win.Multiselect = false;
			win.DefaultExt = ".xlsx";
			win.Filter = "xlsx files (.xlsx; .xls)|*.xlsx; *.xls";
			win.FilterIndex = 1;
			win.RestoreDirectory = true;
			var result = win.ShowDialog();
			if (result == true)
			{
				Vm.FullPath = win.FileName;
				Vm.FilePath = win.FileName.Substring(0, win.FileName.LastIndexOf("\\"));
				Vm.FileName = win.SafeFileName;
				Vm.DoImportData();
			}

			//// Get the selected file name and display in a TextBox
			//if (dlg.ShowDialog() == true)
			//{
			//	var file = new List<string>();
			//	var errorMsg = string.Empty;
			//	try
			//	{
			//		// Open document
			//		file = System.IO.File.ReadAllLines(dlg.FileName).ToList();
			//	}
			//	catch (Exception ex)
			//	{
			//		errorMsg = ErrorCodeHelper.GetCustomErrorCodeDescription(ex, Properties.Resources.P9101010500xamlcs_ImportFail, true);
			//		Vm.ShowWarningMessage(errorMsg);
			//	}

			//	if (string.IsNullOrEmpty(errorMsg))
			//	{
			//		Vm.DoImportData();
			//	}
			//}
		}
	}
}
