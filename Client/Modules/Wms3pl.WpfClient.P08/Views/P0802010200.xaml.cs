using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Wms3pl.WpfClient.P08.ViewModel;
using Wms3pl.WpfClient.UILib;
using System.Linq;
using System;
using System.Windows.Controls;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Microsoft.Win32;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.P08.Services;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.DataServices.F16DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.UcLib.Views;

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0802010200.xaml 的互動邏輯
	/// </summary>
	public partial class P0802010200 : Wms3plWindow
	{
		public P0802010200(F161201 baseData, F910501 f910501)
		{
			InitializeComponent();
			SetFocusedElement(txtNewSerialNo);
			Vm.BaseData = baseData;
			Vm.SelectedF910501 = f910501;
			////Vm.DlvData = dlvData.ToObservableCollection();
			////Vm.F055001Data = f055001Data;
			Vm.ActionAfterCheckSerialNo += AfterCheckSerialNo;
			Vm.OnSaveComplete += this.Close;
            Vm.ExcelImport = ExcelImport;

        }

       
        private void ExcelImport()
        {
            bool ImportResultData = false;
            var win = new WinImportSample(string.Format("{0},{1}", Vm.CustCode, "P0802010200"));

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
                DefaultExt = ".csv",
                Filter = Properties.Resources.P0802010200_ItemSerialNoFile
        };

            if (dlg.ShowDialog() == true)
            {
                return dlg.FileName;
            }
            return "";
        }

		/// <summary>
		/// 按下取消/ 離開時關閉視窗
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			if (Vm.ShowMessage(Messages.WarningBeforeExit) == UILib.Services.DialogResponse.OK)
			{
				Vm.UserOperateMode = OperateMode.Query;
				this.Close();
			}
		}

		/// <summary>
		/// 序號輸入後第一個動作: 選取到新的項目
		/// </summary>
		private void AfterCheckSerialNo()
		{
			

			/// 序號輸入後第二個動作: 重新Focus回TextBox
			SetFocusedElement(txtNewSerialNo);
		}

		/// <summary>
		/// 輸入單號按ENTER後進行查詢
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtNewSerialNo_KeyDown(object sender, KeyEventArgs e)
		{
			TextBox obj = sender as TextBox;
			if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(obj.Text))
			{
				Vm.CheckSerialNoCommand.Execute(null);
			}
		}

		private void Wms3plWindow_Closing(object sender, CancelEventArgs e)
		{
			Vm.UserOperateMode = OperateMode.Query;
		}


		private void Wms3plWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Vm.DoRefreshReadCount();
		}
	}
}
