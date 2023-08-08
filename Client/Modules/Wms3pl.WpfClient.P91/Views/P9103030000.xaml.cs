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
using Wms3pl.WpfClient.P91.ViewModel;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.P19.Views;
using Microsoft.Win32;
using Wms3pl.WpfClient.P91.Report;
using CrystalDecisions.CrystalReports.Engine;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices.P91ExDataService;
using Wms3pl.WpfClient.UcLib.Views;

namespace Wms3pl.WpfClient.P91.Views
{
	/// <summary>
	/// P9103030000.xaml 的互動邏輯
	/// </summary>
	public partial class P9103030000 : Wms3plUserControl
	{
		public P9103030000()
		{

			InitializeComponent();
			Vm.DoPrint += PrintContract;
			Vm.OnImportDetail += ImportDetail;
		}

		private void PrintContract(PrintType printType)
		{
			//取得列印資料
			if (Vm.SelectedData == null)
			{
				ShowMessage(Properties.Resources.P9103030000xamlcs_SelectContract);
				return;
			}

			var list = Vm.GetContractReport();
			if (list == null || list.Count == 0)
			{
				ShowMessage(Properties.Resources.P9103030000xamlcs_NoData);
				return;
			}

			//// 測試分頁假資料
			//for (int i = 0; i < 50; i++)
			//{
			//	list.Add(list[0]);
			//}

			ReportClass report;
			string fileName;
			if (Vm.SelectedData.OBJECT_TYPE == "0")
			{
				report = new R9103030001();
				fileName = @"R9103030001.rpt";
			}
			else
			{
				report = new R9103030002();
				fileName = @"R9103030002.rpt";
			}

			report.Load(fileName);
			report.SetDataSource(list);
			report.SummaryInfo.ReportTitle = Properties.Resources.P9103030000xamlcs_Contract;
			var win = new Wms3plViewer { Owner = System.Windows.Window.GetWindow(this) };
			win.CallReport(report, printType);
		}

		private void UNI_FORM_KeyDown(object sender, KeyEventArgs e)
		{
			TextBox obj = sender as TextBox;
			if (e.Key != Key.Enter || string.IsNullOrWhiteSpace(obj.Text))
				return;

			CheckUniForm();
		}

		private void CheckUniForm()
		{
			var isFind = Vm.GetContractObject();
			if (isFind) return;
			var dr = DialogService.ShowMessage(Properties.Resources.P9103030000xamlcs_NoContractor, Properties.Resources.P9101010000xamlcs_Message, UILib.Services.DialogButton.YesNo, UILib.Services.DialogImage.Warning);
			if (dr == UILib.Services.DialogResponse.Yes)
			{
				if (Vm.SelectedData.OBJECT_TYPE == "0")
				{
					//開啟貨主主檔維護
					var p710903 = new P71.Views.P7109030000();
					p710903.SetNewUniForm(Vm.SelectedData.UNI_FORM);

					var p91030301 = new P9103030100(p710903)
					{
						Owner = System.Windows.Window.GetWindow(this),
						WindowStartupLocation = WindowStartupLocation.CenterOwner,
						FontSize = Application.Current.MainWindow.FontSize,
					};

					if (p91030301.ShowDialog() == true)
					{
						var f1909 = p710903.SelectedF1909;
						if (f1909 != null)
						{
							Vm.SelectedData.UNI_FORM = f1909.UNI_FORM;
							Vm.GetContractObject();
						}

					}

				}
				else
				{
					//開啟委外商主檔維護
					var p190128 = new P19.Views.P1901280000();
					p190128.SetNewUniForm(Vm.SelectedData.UNI_FORM);

					var p91030302 = new P9103030200(p190128)
					{
						Owner = System.Windows.Window.GetWindow(this),
						WindowStartupLocation = WindowStartupLocation.CenterOwner,
						FontSize = Application.Current.MainWindow.FontSize,
					};
					if (p91030302.ShowDialog() == true)
					{
						var f1928 = p190128.SelectedF1928;
						if (f1928 != null)
						{
							Vm.SelectedData.UNI_FORM = f1928.UNI_FORM;
							Vm.GetContractObject();
						}
					}
				}
			}
		}


		private string _checkedUniForm;
		private void UNI_FORM_LostFocus(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(Vm.SelectedData.UNI_FORM))
			{
				_checkedUniForm = string.Empty;
				Vm.SelectedData.CUST_CODE = null;
			}
			else if (Vm.SelectedData.UNI_FORM != _checkedUniForm)
			{
				_checkedUniForm = Vm.SelectedData.UNI_FORM;
				CheckUniForm();
			}
		}

		private void CONTRACT_NO_Changed(object sender, TextChangedEventArgs e)
		{
			if (Vm.SelectedData == null || Vm.NewSerialRecord == null) return;
			Vm.NewSerialRecord.CONTRACT_NO = Vm.SelectedData.CONTRACT_NO;

			if (Vm.SerialRecords == null || !Vm.SerialRecords.Any()) return;
			foreach (var record in Vm.SerialRecords)
				record.Item.CONTRACT_NO = Vm.SelectedData.CONTRACT_NO;
		}

		private void ENABLE_DATE_Changed(object sender, SelectionChangedEventArgs e)
		{
			if (Vm.SelectedData == null || Vm.SelectedData.DISABLE_DATE == null) return;
			if (Vm.SerialRecords == null) return;
			var items = Vm.SerialRecords.Where(x => x.Item.CONTRACT_TYPE == "0").Select(x => x.Item).ToList();
			if (items.Any())
				items.ForEach(x => x.ENABLE_DATE = Vm.SelectedData.ENABLE_DATE);

			Vm.SetQUOTE();
		}

		private void DISABLE_DATE_Changed(object sender, SelectionChangedEventArgs e)
		{
			if (Vm.SelectedData == null || Vm.SelectedData.DISABLE_DATE == null) return;
			Vm.SelectedData.DUEDATE = (Vm.SelectedData.DISABLE_DATE.Date - DateTime.Now.Date).Days;
			if (Vm.SerialRecords == null) return;
			var items = Vm.SerialRecords.Where(x => x.Item.CONTRACT_TYPE == "0").Select(x => x.Item).ToList();
			if (items.Any())
				items.ForEach(x => x.DISABLE_DATE = Vm.SelectedData.DISABLE_DATE);

			Vm.SetQUOTE();
		}


		private void ImportDetail()
		{
            bool ImportResultData = false;
            var win = new WinImportSample(string.Format("{0},{1}", Vm._custCode, "P9103030000", "BP9103030012.xlsx"));

            win.ImportResult = (t) => { ImportResultData = t; };
            win.ShowDialog();
            Vm.FullPath = null;
            if (ImportResultData)
                Vm.FullPath = OpenFileDialogFun();
            //bool ImportResultData = false;
            //var win = new WinImportSample(string.Format("{0},{1}", Vm._custCode, "P0102010000"));

            //win.ImportResult = (t) => { ImportResultData = t; };
            //win.ShowDialog();
            //if (ImportResultData)
            //{
            //    var dlg = new OpenFileDialog { InitialDirectory = string.IsNullOrEmpty(Vm.FilePath) ? @"C:\" : Vm.FilePath };
            //    if (!dlg.CheckPathExists)
            //        dlg.InitialDirectory = @"C:\";

            //    dlg.Multiselect = false;
            //    dlg.DefaultExt = ".xlsx";
            //    dlg.Filter = "xlsx files (.xlsx; .xls)|*.xlsx; *.xls";
            //    dlg.FilterIndex = 1;
            //    dlg.RestoreDirectory = true;
            //    var result = dlg.ShowDialog();
            //    if (result == true)
            //    {
            //        Vm.FullPath = dlg.FileName;
            //        Vm.FilePath = dlg.FileName.Substring(0, dlg.FileName.LastIndexOf("\\"));
            //        Vm.FileName = dlg.SafeFileName;
            //        Vm.DoImportData();
            //    }
            //}
        }

        private string OpenFileDialogFun()
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".xlsx",
                Filter = "xlsx files (.xlsx; .xls)|*.xlsx; *.xls",
                Multiselect = false,
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (dlg.ShowDialog() == true)
            {
                return dlg.FileName;
            }
            return "";
        }

        private void ContractType_Changed(object sender, SelectionChangedEventArgs e)
		{
			Vm.SetContractTypeName();
		}

		private void ItemType_Changed(object sender, SelectionChangedEventArgs e)
		{
			if (Vm.NewSerialRecord != null)
			{
				Vm.SetItemTypeName();
				if (Vm.SelectedData != null && Vm.SelectedData.OBJECT_TYPE == "0" && !string.IsNullOrEmpty(Vm.NewSerialRecord.ITEM_TYPE) &&
					(Vm.QUOTELists == null || Vm.QUOTELists.Where(x => x.Key == Vm.NewSerialRecord.ITEM_TYPE).Select(x => x.Value).Count() == 0))
					ShowMessage(Properties.Resources.P9103030000xamlcs_NoOKQouto);
			}
		}

		private void Quote_Changed(object sender, SelectionChangedEventArgs e)
		{
			if (Vm.NewSerialRecord != null)
			{
				Vm.SetQUOTEName();
				Vm.SetContractFee();
				Vm.SetSelectedQuoteData();
			}
		}

		private void Unit_Changed(object sender, SelectionChangedEventArgs e)
		{
			if (Vm.NewSerialRecord != null)
				Vm.SetUnitName();
		}

		private void Process_Changed(object sender, SelectionChangedEventArgs e)
		{
			if (Vm.NewSerialRecord != null)
				Vm.SetPROCESSName();
		}

		private void SelectData_DC_CODE_Chamged(object sender, SelectionChangedEventArgs e)
		{
			Vm.SetQUOTE();
			if (Vm.SelectedData == null || Vm.NewSerialRecord == null) return;
			Vm.NewSerialRecord.DC_CODE = Vm.SelectedData.DC_CODE;

			if (Vm.UserOperateMode == OperateMode.Add && Vm.SelectedData.OBJECT_TYPE == "0"
				&& Vm.SerialRecords != null && Vm.SerialRecords.Any())
			{
				Vm.ShowWarningMessage(Properties.Resources.P9103030000xamlcs_ClearContractDetailSet);
				Vm.SerialRecords = new SelectionList<F910302Data>(new List<F910302Data>());
			}
		}

		private void CopyContract_Click(object sender, RoutedEventArgs e)
		{
			Vm.CopyContract();
		}


	}
}
