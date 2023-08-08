using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
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
using Wms3pl.WpfClient.ExDataServices.P91ExDataService;
using Wms3pl.WpfClient.P91.Report;
using Wms3pl.WpfClient.P91.ViewModel;
using Wms3pl.WpfClient.UILib;
using System.IO;

namespace Wms3pl.WpfClient.P91.Views
{
	/// <summary>
	/// P9103020000.xaml 的互動邏輯
	/// </summary>
	public partial class P9103020000 : Wms3plUserControl
	{
		public P9103020000()
		{
			InitializeComponent();
			Vm.OnAddProcessMode += AddProcessMode_Executed;
			Vm.OnEditProcessMode += EditProcessMode_Executed;
			Vm.OnQueryProcessMode += QueryProcessMode_Executed;
			Vm.OnAddSuppliesMode += AddSuppliesMode_Executed;
			Vm.OnQuerySuppliesMode += QuerySuppliesMode_Executed;
			Vm.OnPrintAction += PrintAction;
			Vm.OnFocus += FocusAction;
			Vm.OnUpdateEditableDate += UpdateEditableDate;
			SetFocusedElement(cboSearchDcCode);
		}

		private void UpdateEditableDate()
		{
			ControlView(() =>
				{
					dpEditableStartEnableDate.SelectedDate = Vm.EditableF910401.ENABLE_DATE;
					dpEditableEndEnableDate.SelectedDate = Vm.EditableF910401.DISABLE_DATE;
				});
			
		}

		private void FocusAction()
		{
			if (Vm.CanEditData)
			{
				SetFocusedElement(txtQUOTE_NAME);
			}
		}

		// 使用者於商品編號欄位輸入後按Enter，系統檢核是否有此業主商品，
		// 是則帶出商品名稱與商品大分類於對應欄位，
		// 否則提示【無此商品編號！】，Focus回到商品編號輸入欄位。
		private void ITEM_CODE_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				var textBox = gdSupplies.CurrentColumn.GetCellContent(gdSupplies.CurrentCell.Item) as TextBox;
				var be = textBox.GetBindingExpression(TextBox.TextProperty);
				be.UpdateSource();
				Vm.SetItemInfo();
			}
		}

		//
		//使用者於計量單位下拉選單選擇值，輸入耗用基準與標準費用後，游標(Focus)離開此行資料列，系統檢核無誤即更新耗材統計資料列。
		//
		private void STANDARD_LostFocus(object sender, RoutedEventArgs e)
		{
			var list = Vm.EditableF910403DataList.Select(item => item.Item.STANDARD).ToList();
		}

		private void STANDARD_COST_LostFocus(object sender, RoutedEventArgs e)
		{
			var list = Vm.EditableF910403DataList.Select(item => item.Item.STANDARD_COST).ToList();
		}

		// 當有輸入或修改成本價與毛利率時，系統自動帶出貨主加工申請價，使用者仍可修改，計算規則如下：
		// 貨主加工申請價 = 成本價 / (1-毛利率)
		private void NET_RATE_TextChanged(object sender, TextChangedEventArgs e)
		{
			Vm.SetApplyPrice();
		}

		private void COST_PRICE_TextChanged(object sender, TextChangedEventArgs e)
		{
			Vm.SetApplyPrice();
		}

		private void NET_RATE_LostFocus(object sender, RoutedEventArgs e)
		{
			if (!Vm.ValidateNetRate())
			{
				Vm.SetApplyPrice();
			}
		}

		private void dgProcessScrollIntoView()
		{
			if (gdProcess.SelectedItem != null)
			{
				gdProcess.ScrollIntoView(gdProcess.SelectedItem);

			}
		}

		private void AddProcessMode_Executed()
		{
			gdProcess.Columns[0].IsReadOnly = true;
			gdProcess.Columns[1].IsReadOnly = false;
			gdProcess.Columns[2].IsReadOnly = false;
			gdProcess.Columns[3].IsReadOnly = false;
			gdProcess.Columns[4].IsReadOnly = false;
			dgProcessScrollIntoView();
		}

		private void EditProcessMode_Executed()
		{
			gdProcess.Columns[0].IsReadOnly = true;
			gdProcess.Columns[1].IsReadOnly = true;
			gdProcess.Columns[2].IsReadOnly = false;
			gdProcess.Columns[3].IsReadOnly = false;
			gdProcess.Columns[4].IsReadOnly = false;
			dgProcessScrollIntoView();
		}

		private void QueryProcessMode_Executed()
		{
			gdProcess.Columns[0].IsReadOnly = false;
			gdProcess.Columns[1].IsReadOnly = true;
			gdProcess.Columns[2].IsReadOnly = true;
			gdProcess.Columns[3].IsReadOnly = true;
			gdProcess.Columns[4].IsReadOnly = true;
			dgProcessScrollIntoView();
		}

		private void dgSuppliesScrollIntoView()
		{
			if (gdSupplies.SelectedItem != null)
			{
				gdSupplies.ScrollIntoView(gdSupplies.SelectedItem);

			}
		}

		private void AddSuppliesMode_Executed()
		{
			gdSupplies.Columns[0].IsReadOnly = true;
			gdSupplies.Columns[1].IsReadOnly = false;
			gdSupplies.Columns[4].IsReadOnly = false;
			gdSupplies.Columns[5].IsReadOnly = false;
			gdSupplies.Columns[6].IsReadOnly = false;
			dgSuppliesScrollIntoView();
		}

		private void QuerySuppliesMode_Executed()
		{
			gdSupplies.Columns[0].IsReadOnly = false;
			gdSupplies.Columns[1].IsReadOnly = true;
			gdSupplies.Columns[4].IsReadOnly = true;
			gdSupplies.Columns[5].IsReadOnly = true;
			gdSupplies.Columns[6].IsReadOnly = true;
			dgSuppliesScrollIntoView();
		}

		//private void ENABLE_DATE_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		//{
		//	CheckExecuteAutoSetEnableDisableDate();
		//}

		//private void DISABLE_DATE_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		//{
		//	CheckExecuteAutoSetEnableDisableDate();
		//}

		//private void CheckExecuteAutoSetEnableDisableDate()
		//{
		//	if (Vm.UserOperateMode == OperateMode.Add)
		//	{
		//		Vm.ExecuteAutoSetEnableDisableDate();
		//	}
		//}

		private void Upload_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
			ofd.CheckFileExists = true;
			ofd.DefaultExt = ".jpg";
			ofd.Filter = Properties.Resources.P9103020000xamlcs_JPGFile;
			ofd.FileOk += (s, ce) =>
			{
				MessagesStruct msg;
				if (!ValidateFileInfo(ofd.FileName, out msg))
				{
					Vm.ShowMessage(msg);
					ce.Cancel = true;
				}
			};

			var f = ofd.ShowDialog();
			if (f.HasValue && f.Value)
			{
				var clientFilePath = ofd.FileName;
				var extension = System.IO.Path.GetExtension(clientFilePath);
				var shareFolder = FileHelper.GetShareFolderPath(new string[] { "QUOTE", Vm.SelectedF910401.DC_CODE, Vm.SelectedF910401.GUP_CODE, Vm.SelectedF910401.CUST_CODE, Vm.SelectedF910401.CRT_DATE.Year.ToString() });
				var serverFilePath = System.IO.Path.Combine(shareFolder, Vm.SelectedF910401.QUOTE_NO + extension);

				var directoryName = System.IO.Path.GetDirectoryName(serverFilePath);
				if (!System.IO.Directory.Exists(directoryName))
				{
					System.IO.Directory.CreateDirectory(directoryName);
				}
				if (System.IO.File.Exists(serverFilePath))
				{
					ShowMessage(Properties.Resources.P9103020000xamlcs_QouteImageFileUpload);
					return;
				}

				if (clientFilePath.Length > 500 || serverFilePath.Length > 500)
				{
					ShowMessage(Properties.Resources.P9103020000xamlcs_ImageFilePathTooLong);
					return;
				}

				if (Vm.SelectedF910401 == null)
				{
					DialogService.ShowMessage(Properties.Resources.P9103020000xamlcs_SelectQoute);
					return;
				}

				if (!Vm.UpdateStatus())
					return;

				System.IO.File.Copy(clientFilePath, serverFilePath, false);

				Vm.UploadImagePath(clientFilePath, serverFilePath);

			}
		}

		private bool ValidateFileInfo(string fileName, out MessagesStruct msg)
		{
			var fileInfo = new System.IO.FileInfo(fileName);
			if (!fileInfo.Exists)
			{
				msg = new MessagesStruct()
				{
					Message = Properties.Resources.P9103020000xamlcs_FileNotFound,
					Button = UILib.Services.DialogButton.OK,
					Image = UILib.Services.DialogImage.Warning
				};
				return false;
			}

			if (fileInfo.Length > Common.GlobalVariables.FileSizeLimit)
			{
				msg = Messages.WarningFileSizeExceedLimits;
				return false;
			}

			msg = default(MessagesStruct);
			return true;
		}

		private void PrintAction(PrintType printType)
		{
			// test data
			//F910401Report f910401Report = new F910401Report()
			//{
			//	QUOTE_NO = "Q2015032500001",
			//	APPLY_PRICE = 9050,
			//	CONTACT = "王老先生",
			//	OUTSOURCE_NAME = "山寨專業加工",
			//	QUOTE_NAME = "奶粉報價單"
			//};


			//List<F910402Report> F910402ReportList = new List<F910402Report>();
			//for (int i = 0; i < 10; i++)
			//{
			//	F910402ReportList.Add(new F910402Report()
			//	{
			//		PROCESS_ACT = Properties.Resources.ACT + i,
			//		UNIT = "動作單位" + i
			//	});
			//}
			//List<F910403Report> F910403ReportList = new List<F910403Report>();
			//for (int i = 0; i < 10; i++)
			//{
			//	F910403ReportList.Add(new F910403Report()
			//	{
			//		ITEM_NAME = "項目" + i,
			//		UNIT = "項目單位" + i
			//	});
			//}

			var f910401Report = Vm.F910401Report;
			var f910402ReportList = Vm.F910402Reports;
			var f910403ReportList = Vm.F910403Reports;

			var report2 = new RP9103020002();
			report2.Load(@"RP9103020002.rpt");
			

			report2.Subreports["RP9103020001.rpt"].SetDataSource(f910402ReportList.ToDataTable());
			report2.Subreports["RP9103020003.rpt"].SetDataSource(f910403ReportList.ToDataTable());
		
			report2.SetText("Text17", Properties.Resources.P9103020000xamlcs_PrintPerson);
			report2.SetText("Text16", Properties.Resources.P9103020000xamlcs_QouteNo);
			report2.SetText("Text11", Wms3plSession.Get<GlobalInfo>().CustName);
			report2.SetText("Text3", string.Format(Properties.Resources.P9103020000xamlcs_Qoute, Wms3plSession.Get<GlobalInfo>().GupName,Wms3plSession.Get<GlobalInfo>().CustName));
			report2.SetParameterValue("APPLY_PRICE", f910401Report.APPLY_PRICE);
			report2.SetParameterValue("CONTACT", f910401Report.CONTACT);
			report2.SetParameterValue("OUTSOURCE_NAME", f910401Report.OUTSOURCE_NAME);
			report2.SetParameterValue("QUOTE_NAME", f910401Report.QUOTE_NAME);
			report2.SetParameterValue("QUOTE_NO", f910401Report.QUOTE_NO);
			report2.SetParameterValue("AccountName", Wms3plSession.CurrentUserInfo.AccountName);

			var win = new Wms3plViewer { Owner = System.Windows.Window.GetWindow(this) };
			win.CallReport(report2, printType);
		}

		private void btnViewUpload_Click(object sender, RoutedEventArgs e)
		{
			if (Vm.SelectViewFile != null && Vm.SelectedF910401 !=null)  //已上傳
			{

				var extension = System.IO.Path.GetExtension(Vm.SelectViewFile.UPLOAD_S_PATH);
				var shareFolder = FileHelper.GetShareFolderPath(new string[] { "QUOTE", Vm.SelectedF910401.DC_CODE, Vm.SelectedF910401.GUP_CODE, Vm.SelectedF910401.CUST_CODE, Vm.SelectedF910401.CRT_DATE.Year.ToString() });
				var serverFilePath = System.IO.Path.Combine(shareFolder, Vm.SelectedF910401.QUOTE_NO + extension);


				if (File.Exists(serverFilePath))
				{
					System.Diagnostics.Process.Start(serverFilePath);
				}
				else
				{
					Vm.DialogService.ShowMessage(Properties.Resources.P9103020000xamlcs_FileCantFound);
				}
			}
		}

	}
}
