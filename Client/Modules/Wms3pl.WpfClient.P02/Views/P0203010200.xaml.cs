using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Microsoft.Win32;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P02.Views
{
	/// <summary>
	/// P0203010200.xaml 的互動邏輯
	/// </summary>
	public partial class P0203010200 : Wms3plWindow
	{
		public P0203010200()
		{
			InitializeComponent();
		}

    public P0203010200(string dcCode, string gupCode, string custCode, DateTime allocationDate, string allocationNo, string tarDcCode, string tarWarehouseId, DateTime? validDate)
    {
      InitializeComponent();
			Vm.SetDefaultFocus += SetDefaultFocus;
			Vm.ExitClick += ExitClick;
			Vm.ClosedSuccessClick += ClosedSuccessClick;
			//Vm.ShowSetItem += ShowSetItem;
			Vm.Bind(dcCode, gupCode, custCode, allocationDate, allocationNo, tarDcCode, tarWarehouseId, validDate);
			SetDefaultFocus();
		}

		private void SetDefaultFocus()
		{
			SetFocusedElement(SerialNoOrBoxNo);
		}
		private void ExitClick()
		{
			DialogResult = Vm.IsChangeData;
			Close();
		}
		private void ClosedSuccessClick()
		{
			DialogResult = true;
			Close();
		}


		private void SerialNoOrBoxNo_OnKeyDown(object sender, KeyEventArgs e)
		{
			Vm.ItemCode = "";
			Vm.ItemName = "";
			if (e.Key == Key.Enter)
			{
				if (!Vm.SerialNoOrBoxNoItemCodeCheck())
				{
					e.Handled = true;
					return;
					//if (!ShowSetItemWin())
					//{
					//	e.Handled = true;
					//	return;
					//}
				}
				//序號檢查
				if (Vm.SerialNoOrBoxNoCheck())
					LocCode.Focus();
				else
					e.Handled = true;
			}
				
		}

		private void LocCode_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Vm.DoSave();
				if (Vm.IsSaveOk)
					Vm.SearchCommand.Execute(null);
				else
					e.Handled = true;
			}
				
		}

		private void P0203010200_OnClosing(object sender, CancelEventArgs e)
		{
			DialogResult = Vm.IsChangeData;
		}

		private void ExcelImport_OnClick(object sender, RoutedEventArgs e)
		{
            bool ImportResultData = false;
            var win = new WinImportSample(string.Format("{0},{1}", Vm._custCode, "P0203010200"));

            win.ImportResult = (t) => { ImportResultData = t; };
            win.ShowDialog();
            Vm.ImportFilePath = null;
            if (ImportResultData)
            {
                var dlg = new OpenFileDialog { DefaultExt = ".xls", Filter = "excel files (*.xls,*.xlsx)|*.xls*" };
                if (dlg.ShowDialog() ?? false)
                {
                    try
                    {
                        Vm.ImportFilePath = dlg.FileName;
                        Vm.ExcelImportItemCommand.Execute(null);
                    }
                    catch (Exception ex)
                    {
                        var errorMsg = ErrorHandleHelper.GetCustomErrorCodeDescription(ex, Properties.Resources.P0203010200_ImportFail, true);
                        Vm.ShowWarningMessage(errorMsg);
                    }
                }
            }
                //Vm.ImportFilePath = OpenFileDialogFun();


            
		}

		//private void ShowSetItem()
		//{
		//	Dispatcher.Invoke(new Action(() =>
		//	{
		//		if (ShowSetItemWin())
		//			Vm.DoSave();
		//	}));
		//}

		//private bool ShowSetItemWin()
		//{
		//	var win = new P0203010300()
		//		{
		//			Owner = System.Windows.Window.GetWindow(this),
		//			WindowStartupLocation = WindowStartupLocation.CenterOwner,
		//		};
		//	if (win.ShowDialog() ?? false)
		//	{
		//		Vm.ItemCode = win.Vm.ItemCode;
		//		Vm.ItemName = win.Vm.ItemName;
		//		return true;
		//	}
		//	return false;
		//}
	}
}
