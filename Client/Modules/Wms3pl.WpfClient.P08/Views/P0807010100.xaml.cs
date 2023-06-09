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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.UcLib.Views;

namespace Wms3pl.WpfClient.P08.Views
{
    public partial class P0807010100 : Wms3plWindow
    {
        public P0807010100(F050801 baseData, List<F1903> f1903s, F1909 f1909Data)
        {
            InitializeComponent();
            SetFocusedElement(txtNewSerialNo);
            Vm.BaseData = baseData;
            Vm.F1903s = f1903s;
            Vm.F1909Data = f1909Data;
            Vm.ActionAfterCheckSerialNo += AfterCheckSerialNo;
           // Vm.ActionBeforeImportData += FileUpload;
            Vm.ActionForRequireUnlock += OnRequireUnlock;
            Vm.OnSaveComplete += this.Close;
            Vm.ExcelImport += ExcelImport;
        }

        private void ExcelImport()
        {
            bool ImportResultData = false;
            var win = new WinImportSample(string.Format("{0},{1}", Vm.BaseData.CUST_CODE, "P0807010100"));

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
                Filter = "csv files(.csv)|*.csv"
            };

            var path = string.Empty;
            if (dlg.ShowDialog() == true)
            {
                String[] ex = dlg.SafeFileName.Split('.');

                //防止*.*的判斷式
                if (ex[ex.Length - 1] != "csv")
                {
                    DialogService.ShowMessage("進倉單匯入檔必須為csv檔案");
                    dlg = null;
                    return "";
                }
                return dlg.FileName;
            }
            return "";
        }

        

		/// <summary>
		/// ���U"����"/ "���}"����������
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
		/// �Ǹ���J��Ĥ@�Ӱʧ@: �����s������
		/// </summary>
		private void AfterCheckSerialNo()
		{
			Vm.DgSelectedItem = Vm.DgSerialList.LastOrDefault();
			ScrollIntoView(dgSerialList, Vm.DgSelectedItem);

			/// �Ǹ���J��ĤG�Ӱʧ@: ���sFocus�^TextBox
			SetFocusedElement(txtNewSerialNo);
		}

		/// <summary>
		/// ��J�渹��ENTER��i��d��
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

		/// <summary>
		/// �D�޸���
		/// </summary>
		private void OnRequireUnlock()
		{
			var win = new P0807010200(Vm.BaseData.DC_CODE, Vm.BaseData.GUP_CODE, Vm.BaseData.CUST_CODE) { Owner = Wms3plViewer.GetWindow(this) };
			win.ShowDialog();
			SetFocusedElement(txtNewSerialNo);
		}

		private void Wms3plWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Vm.DoRefreshReadCount();
		}
	}
}
