using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.P02.ViewModel;
using Wms3pl.WpfClient.UILib;
using System.Linq;
using System;
using System.Windows.Controls;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;
using Microsoft.Win32;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;

namespace Wms3pl.WpfClient.P02.Views
{
	/// <summary>
	/// P0202030600.xaml 的互動邏輯
	/// </summary>
	public partial class P0202030600 : Wms3plWindow
	{
		private bool _isFileUpload = false;
		public P0202030600(P020203Data baseData, string dcCode, string rtNo)
		{
			InitializeComponent();
			// 指定序號輸入後要做的事, 必須在Command OnComplete後再處理, 所以用Action串接
			Vm.ActionAfterCheckSerialNo += AfterCheckSerialNo;
			//Vm.ActionBeforeImportData += FileUpload;
			Vm.CloseWin += Close;
			Vm.SelectedDc = dcCode;
			Vm.BaseData = baseData;
			Vm.RtNo = rtNo;
			
			Vm.LoadF020302ScanRecords();
			Vm.DoRefreshReadCount();
			txtNewSerialNo.Focus();

		}

		/// <summary>
		/// 按下Properties.Resources.P0202030600_Cancel/ Properties.Resources.P0202030600_Exit時關閉視窗
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
		private void AfterCheckSerialNo(object selectedItem)
		{
			base.ScrollIntoView(dgSerialList, selectedItem);
			// Focus回TextBox
			ControlView(() =>
			{
				base.SetFocusedElement(txtNewSerialNo, true);
			});


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

        public int serialNoCount()
        {
            return 1;
        }
	}
}
