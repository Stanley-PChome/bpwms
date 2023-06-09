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
using Wms3pl.WpfClient.P16.ViewModel;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P16.Views
{
	/// <summary>
	/// P1601010000.xaml 的互動邏輯
	/// </summary>
	public partial class P1601010000 : Wms3plUserControl
	{
		public P1601010000()
		{
			{
				InitializeComponent();
				Vm.AddDetailAction += AddDetailCommand_Executed;
				Vm.DelItemAction += DeleteDetailCommand_Executed;
				Vm.ExcelImport += ExcelImport;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var win = new P1601010100();
			win.ShowDialog();
		}

		private void BtnReturnLog_Click(object sender, RoutedEventArgs e)
		{
			var win = new P1601010200(Vm.SelectedData);
			win.Owner = this.Parent as Window;
			//win.SourceData = Vm.SelectedData;
			win.DoQuery();
			win.ShowDialog();
		}

		private void TXTEditRTN_QTY_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (Vm.AddDetailCommand.CanExecute(null))
					Vm.AddDetailCommand.Execute(null);
			}
		}

		private void ASCUST_NO_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Vm.GetAddSearchList();
			}
		}


		private void AddDetailCommand_Executed(SelectionItem<F161201DetailDatas> selectedItem)
		{
			if (Vm.UserOperateMode == OperateMode.Add)
			{
				DgAddItemList.ScrollIntoView(selectedItem);
				//DgAddItemList.Items.MoveCurrentToLast();
				//if (DgAddItemList.Items.CurrentItem != null)
				//	DgAddItemList.ScrollIntoView(DgAddItemList.Items.CurrentItem);
				//else
				//{
				//	if (DgAddItemList.Items.Count>0)
				//		DgAddItemList.ScrollIntoView(DgAddItemList.Items.GetItemAt(DgAddItemList.Items.Count - 1));
				//}
			}
			if (Vm.UserOperateMode == OperateMode.Edit)
			{
				DgEditItemList.Items.MoveCurrentToLast();
				if (DgEditItemList.Items.CurrentItem != null)
					DgEditItemList.ScrollIntoView(DgEditItemList.Items.CurrentItem);
			}

		}

		private void DeleteDetailCommand_Executed()
		{
			if (Vm.UserOperateMode == OperateMode.Add)
			{
				DgAddItemList.Items.MoveCurrentToLast();
				if (DgAddItemList.Items.CurrentItem != null)
					DgAddItemList.ScrollIntoView(DgAddItemList.Items.CurrentItem);
			}
			if (Vm.UserOperateMode == OperateMode.Edit)
			{
				DgEditItemList.Items.MoveCurrentToLast();
				if (DgEditItemList.Items.CurrentItem != null)
					DgEditItemList.ScrollIntoView(DgEditItemList.Items.CurrentItem);
			}
		}

		private void DgAddSearchList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Vm.ImportWmsOrdDetailsCommand.Execute(null);
		}

		private void TxtRTN_CUST_CODE_TextChanged(object sender, TextChangedEventArgs e)
		{
			// 當有輸入退貨客戶代號時，客戶名稱清空不可輸入
			//Vm.RtnCustCodeTextChanged(TxtRTN_CUST_CODE.Text);
		}

		private void TxtRTN_CUST_CODE_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				//TxtRTN_CUST_CODE.Text = TxtRTN_CUST_CODE.Text.Trim();

				//Vm.GetCustCodeInfo(TxtRTN_CUST_CODE.Text);
			}
		}

		private void txtEditRTN_CUST_CODE_TextChanged(object sender, TextChangedEventArgs e)
		{
			// 當有輸入退貨客戶代號時，客戶名稱清空不可輸入
			//Vm.RtnCustCodeTextChanged(txtEditRTN_CUST_CODE.Text);
		}

		private void txtEditRTN_CUST_CODE_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				//txtEditRTN_CUST_CODE.Text = txtEditRTN_CUST_CODE.Text.Trim();

				//Vm.GetCustCodeInfo(txtEditRTN_CUST_CODE.Text);
			}
		}

		private void txtAddSourceNo_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Vm.CheckSourceNo(Vm.AddNewData);
			}
		}

		private void txtEditSourceNo_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Vm.CheckSourceNo(Vm.SelectEditData);
			}
		}

        bool ImportResultData = false;
        private void ExcelImport()
		{

            var win = new WinImportSample(string.Format("{0},{1}", Vm._custCode, "P1601010000"));

            win.ImportResult = (t) => { ImportResultData = t; };
            win.ShowDialog();
            Vm.ImportFilePath = null;
            if (ImportResultData)
                Vm.ImportFilePath = OpenFileDialogFun();
            //Vm.ImportFilePath = OpenFileDialogFun();
			
		}

		private string OpenFileDialogFun()
		{
			var dlg = new Microsoft.Win32.OpenFileDialog
			{
				DefaultExt = ".xls",
				Filter = "excel files (*.xls,*.xlsx)|*.xls*"
			};

			if (dlg.ShowDialog() == true)
			{
                String[] ex = dlg.SafeFileName.Split('.');

                //防止*.*的判斷式
                if (ex[ex.Length - 1] != "xls" && ex[ex.Length - 1] != "xlsx")
                {
                    DialogService.ShowMessage("退貨單匯入檔必須為Excel檔案，總共有24欄");
                    return "";
                }
               
				return dlg.FileName;
			}
			return "";
		}

        private void ASWMS_ORD_NO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Vm.GetAddSearchList();
            }
        }

        private void ASCUST_ORD_NO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Vm.GetAddSearchList();
            }
        }
    }
}