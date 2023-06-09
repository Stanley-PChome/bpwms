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
using Wms3pl.WpfClient.P16.ViewModel;
using Wms3pl.WpfClient.UILib;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.Helpers;

namespace Wms3pl.WpfClient.P16.Views
{
	/// <summary>
	/// P1605010100.xaml 的互動邏輯
	/// </summary>
	public partial class P1605010100 : Wms3plWindow
	{
		public P1605010100(string destoryNo, string dcCode, string gupCode, string custCode,DateTime crtDate, bool isCanEdit, P1605010000_ViewModel parentVm)
		{
			InitializeComponent();
			Vm.F160501SelectData.DESTROY_NO = destoryNo;
			Vm.F160501SelectData.DC_CODE = dcCode;
			Vm.F160501SelectData.GUP_CODE = gupCode;
			Vm.F160501SelectData.CUST_CODE = custCode;
			Vm.F160501SelectData.CRT_DATE = crtDate;
			Vm.IsCanEdit = isCanEdit;
			Vm.IsSaveFlag = true;
			Vm.DoSearchParent = parentVm;
			if (isCanEdit)
				Vm.DestroyNoAdd = destoryNo;
			//取相關上傳資訊
			Vm.GetDestoryNoRelation(destoryNo);
		}


		private void btnExit_Click(object sender, RoutedEventArgs e)
		{
			CloseAction();
		}

		private void CloseAction()
		{

			if (Vm.ShowMessage(Messages.WarningBeforeExit) == UILib.Services.DialogResponse.OK)
			{
				if (Vm.IsSaveFlag == false)
				{
					var msg = new MessagesStruct()
					{
						Button = UILib.Services.DialogButton.OKCancel,
						Image = UILib.Services.DialogImage.Warning,
						Message = Properties.Resources.P1605010100xamlcs_UploadNotDone_SureWanttoLeave,
						Title = Properties.Resources.P1605010100xamlcs_Waring
					};

					if (Vm.ShowMessage(msg) == UILib.Services.DialogResponse.OK)
					{
						Vm.DgFileList = null;
						Vm.DgSerialFileList = null;
						this.Close();
					}
				}
				else
				{
					Vm.DoSearchParent.F160501Query.DESTROY_NO = Vm.F160501SelectData.DESTROY_NO;
					Vm.DoSearchParent.SearchCommand.Execute(null);
					Vm.DgFileList = null;
					Vm.DgSerialFileList = null;
					this.Close();
				}
			}
		}


		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (Vm.IsCanEdit)
				Vm.DelTicket();
		}

		private void btnAddTicket_Click(object sender, RoutedEventArgs e)
		{
			Vm.AddTicket();
		}


		private void AddFile(object sender, RoutedEventArgs e)
		{
			Vm.AddFile();
		}

		private void btnAddFileUpload_Click(object sender, RoutedEventArgs e)
		{
			var dlg = FileUpload();
			if (dlg.ShowDialog() == true)
			{
				Vm.SelectFileName = dlg.FileName;
			}
		}

		private OpenFileDialog FileUpload()
		{
			OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.Multiselect = false;
			dlg.DefaultExt = ".*";
			dlg.Filter = Properties.Resources.P1605010100xamlcs_Destroy;
			return dlg;
		}

		private void btnClearFile_Click(object sender, RoutedEventArgs e)
		{
			if (Vm.IsCanEdit)
				Vm.DelFileList();
		}

		private void btnViewFile_Click(object sender, RoutedEventArgs e)
		{
			
			if (Vm.DgFileSelect != null)
			{
				var filePath = !string.IsNullOrEmpty(Vm.DgFileSelect.UPLOAD_S_PATH)
								? (FileHelper.ShareFolderItemFiles + Vm.DgFileSelect.UPLOAD_S_PATH)
								: Vm.DgFileSelect.UPLOAD_C_PATH;

				if (File.Exists(filePath))
				{
					System.Diagnostics.Process.Start(filePath);
				}
				else
				{
					MessageBox.Show(Properties.Resources.P1605010100xamlcs_FileNotFound);
				}
			}

		}

		private void btnChooseFile_Click(object sender, RoutedEventArgs e)
		{

		}

	}
}
