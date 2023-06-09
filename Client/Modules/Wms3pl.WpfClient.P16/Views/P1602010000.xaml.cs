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
using Wms3pl.WpfClient.ExDataServices.P16ExDataService;
using Wms3pl.WpfClient.P16.Report;
using Wms3pl.WpfClient.P16.ViewModel;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P16.Views
{
	/// <summary>
	/// P1602010000.xaml 的互動邏輯
	/// </summary>
	public partial class P1602010000 : Wms3plUserControl
	{

		public P1602010000()
		{
			InitializeComponent();
			SetFocusedElement(DcComboBox);
			Vm.EditAction += EditCommand_Executed;
			Vm.SetQueryFocus += SetQueryFocus;
			Vm.SetAddNewFocus += SetAddNewFocus;
			Vm.SetEditFocus += SetEditFocus;
			Vm.DoPrintReport += GetReport;
            Vm.ExcelImport += ExcelImport;
        }

		private void GetReport(PrintType printType)
		{
			var data = Vm.SelectedF160201DetailList;
			if (data == null || data.Count == 0)
			{
				DialogService.ShowMessage(Properties.Resources.P1601020000xamlcs_NullData);
				return;
			}
			
			var report = new Report.RP1602010005();
            report.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().GupName + "－" +
                                 Wms3plSession.Get<GlobalInfo>().CustName +
                                 Properties.Resources.RP1602010005_TITLE;
            report.SetDataSource(data.ToDataTable());
			report.SetParameterValue("StaffName", Wms3plSession.CurrentUserInfo.AccountName);
			//report.SetText("ReportT", gupName + " - " + custName + Properties.Resources.P1601020000xamlcs_TransferApplicateNo);
			//report.SetText("ApproveUser", AppSTAFF + " - " + AppName);
			var win = new Wms3plViewer { Owner = Wms3plViewer.GetWindow(this) };
			win.CallReport(report, printType);
			

		}

		//新增模式的新增明細
		private void AddNewModeAddDetailButton_Click(object sender, RoutedEventArgs e)
		{
			if (String.IsNullOrEmpty(Vm.AddNewF160201.DC_CODE))
			{
				Vm.ShowMessage(Properties.Resources.P1602010000xamlcs_ChooseDC);
				return;
			}

			if (String.IsNullOrEmpty(Vm.AddNewF160201.VNR_CODE))
			{
				Vm.ShowMessage(Properties.Resources.P1602010000xamlcs_VNRCode_Required);
				return;
			}

			if (Vm.AddNewF160201DetailList == null)
			{
				Vm.AddNewF160201DetailList = new SelectionList<F160201ReturnDetail>(new List<F160201ReturnDetail>());
			}

            Vm.ReturnData = new List<F160201ReturnDetail>();
            F160201ReturnDetail data = new F160201ReturnDetail();
            //data.ENTER_DATE = addItem.ENTER_DATE;
            //data.RTN_VNR_QTY = data.INVENTORY_QTY = addItem.INVENTORY_QTY;
            data.ITEM_CODE = Vm.ITEM_CODE;
            data.ITEM_COLOR = Vm.ITEM_COLOR;
            data.ITEM_NAME = Vm.ITEM_NAME;
            data.ITEM_SIZE = Vm.ITEM_SIZE;
            data.ITEM_SPEC = Vm.ITEM_SPEC;
            //data.LOC_CODE = addItem.LOC_CODE;
            //data.MEMO = addItem.MEMO;
            //data.ROWNUM = addItem.ROWNUM;
            //data.VALID_DATE = addItem.VALID_DATE;
            //data.WAREHOUSE_ID = addItem.WAREHOUSE_ID;
            //data.WAREHOUSE_NAME = addItem.WAREHOUSE_NAME;
            data.RTN_VNR_QTY = Vm.RTN_VNR_QTY;
			      data.RTN_VNR_CAUSE = Vm.RTN_VNR_CAUSE;
			      data.MEMO = Vm.MEMO;
			      data.MAKE_NO = Vm.MAKE_NO;
			      data.RTN_VNR_CAUSE_NAME = Vm.ReturnReasons.FirstOrDefault(x=> x.Value == Vm.RTN_VNR_CAUSE)?.Name;
            

   //         var win = new P1602010100(Vm.AddNewF160201, Vm.AddNewF160201VendorName, Vm.AddNewF160201DetailList);
			//win.ShowDialog();

			//if (win.ReturnData != null && win.ReturnData.Count > 0)
			//{
			//	Vm.ReturnData = win.ReturnData;
				
            if (Vm.CheckDetail(true,Vm.AddNewF160201DetailList,Vm.SelectedReturnDetaiItemForAddNew))
            {
                Vm.ReturnData.Add(data);
                Vm.ConvertReturnDetailData(Vm.AddNewF160201DetailList);
                AddNewReturnDetailDataGrid.ItemsSource = null;
                AddNewReturnDetailDataGrid.ItemsSource = Vm.AddNewF160201DetailList;
                Vm.ClearSearchProduct();
            }
           
            //}
        }

		

		//新增模式的刪除明細
		private void AddNewModeDeleteDetailButton_Click(object sender, RoutedEventArgs e)
		{
			Vm.DeleteDetailComplate();
			AddNewReturnDetailDataGrid.ItemsSource = null;
			AddNewReturnDetailDataGrid.ItemsSource = Vm.AddNewF160201DetailList;
            Vm.ClearSearchProduct();
		}

		private void VendorNo_OnPreviewKeyDown(object sender, KeyEventArgs e)
		{
			TextBox txt = sender as TextBox;
			if (e.Key == Key.Enter && !String.IsNullOrEmpty(txt.Text))
			{
				Vm.SearchVendorData(txt.Text);
			}
		}

		private void VendorTextBox_OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			TextBox txt = sender as TextBox;
			if (!String.IsNullOrEmpty(txt.Text))
			{
				Vm.SearchVendorData(txt.Text);
			}
		}

		private void CancelButton_OnClick(object sender, RoutedEventArgs e)
		{
			AddNewReturnDetailDataGrid.ItemsSource = null;
			EditReturnDetailDataGrid.ItemsSource = null;
		}

		//修改模式的新增明細
		private void EditModeAddDetailButton_Click(object sender, RoutedEventArgs e)
		{
			if (String.IsNullOrEmpty(Vm.EditF160201.DC_CODE))
			{
				Vm.ShowMessage(Properties.Resources.P1602010000xamlcs_ChooseDC);
				return;
			}

			if (String.IsNullOrEmpty(Vm.EditF160201.VNR_CODE))
			{
				Vm.ShowMessage(Properties.Resources.P1602010000xamlcs_VNRCode_Required);
				return;
			}

			if (Vm.EditF160201DetailList == null)
			{
				Vm.EditF160201DetailList = new SelectionList<F160201ReturnDetail>(new List<F160201ReturnDetail>());
			}

            Vm.ReturnData = new List<F160201ReturnDetail>();
            F160201ReturnDetail data = new F160201ReturnDetail();
            data.ITEM_CODE = Vm.ITEM_CODE;
            data.ITEM_COLOR = Vm.ITEM_COLOR;
            data.ITEM_NAME = Vm.ITEM_NAME;
            data.ITEM_SIZE = Vm.ITEM_SIZE;
            data.ITEM_SPEC = Vm.ITEM_SPEC;
            data.RTN_VNR_QTY = Vm.RTN_VNR_QTY;
			      data.MAKE_NO = Vm.MAKE_NO;
			      data.RTN_VNR_CAUSE = Vm.RTN_VNR_CAUSE;
			      data.RTN_VNR_CAUSE_NAME = Vm.ReturnReasons.FirstOrDefault(x=> x.Value == Vm.RTN_VNR_CAUSE)?.Name;
			      data.MEMO = Vm.MEMO;
            //var win = new P1602010100(Vm.EditF160201, Vm.EditF160201VendorName, Vm.EditF160201DetailList);
            //win.ShowDialog();

            if (Vm.CheckDetail(true, Vm.EditF160201DetailList, Vm.SelectedReturnDetaiItemForEdit))
            {
                Vm.ReturnData.Add(data);
                Vm.ConvertReturnDetailData(Vm.EditF160201DetailList);
                EditReturnDetailDataGrid.ItemsSource = null;
                EditReturnDetailDataGrid.ItemsSource = Vm.EditF160201DetailList;
                Vm.ClearSearchProduct();
            }
           
        }

		

		private void EditModeDeleteDetailButton_Click(object sender, RoutedEventArgs e)
		{
			Vm.DeleteDetailComplate(true);
			EditReturnDetailDataGrid.ItemsSource = null;
			EditReturnDetailDataGrid.ItemsSource = Vm.EditF160201DetailList;
            Vm.ClearSearchProduct();
		}

		private void EditCommand_Executed()
		{
			EditReturnDetailDataGrid.ItemsSource = Vm.EditF160201DetailList;
		}

		private void SetQueryFocus()
		{
			SetFocusedElement(DcComboBox);
		}

		private void SetAddNewFocus()
		{
			SetFocusedElement(DcComboBoxForAddNew);
		}

		private void SetEditFocus()
		{
			SetFocusedElement(ChangeTypeComboBoxForEdit);
		}

		private void AddNewButton_OnClick(object sender, RoutedEventArgs e)
		{
			AddNewReturnDetailDataGrid.ItemsSource = null;
			EditReturnDetailDataGrid.ItemsSource = null;
		}

		private void Add_AfterVendorChanged(object sender, RoutedEventArgs e)
		{
			Vm.CheckAddressAndSetSelfTake();
		}

        bool ImportResultData = false;
        private void ExcelImport()
        {
            var win = new WinImportSample(string.Format("{0},{1}", Vm._custCode, "P1602010000"));

            win.ImportResult = (t) => { ImportResultData = t; };
            win.ShowDialog();
            Vm.ImportFilePath = null;
            if (ImportResultData)
                Vm.ImportFilePath = OpenFileDialogFun();
        }

        private string OpenFileDialogFun()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".xls",
                Filter = "excel files (*.xls,*.xlsx)|*.xls*|csv files (*.csv)|*.csv"
            };

            if (dlg.ShowDialog() == true)
            {
                return dlg.FileName;
            }
            return "";
        }
    }
}
