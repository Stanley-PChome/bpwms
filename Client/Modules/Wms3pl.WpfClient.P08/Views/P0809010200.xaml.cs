using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using Wms3pl.WpfClient.P08.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0809010200.xaml 的互動邏輯
	/// </summary>
	public partial class P0809010200 : Wms3plWindow
	{
		private bool _hasUpload;
		public P0809010200()
		{
			InitializeComponent();
			Vm.DoUpload += FileUpload;
		}
		public P0809010200(string gupCode, string custCode, string dcCode, DateTime delvDate, string takeTime, string allId)
		{
			InitializeComponent();
			Vm.MaxUploadCount = 3;
			Vm.GupCode = gupCode;
			Vm.CustCode = custCode;
			Vm.DcCode = dcCode;
			Vm.DelvDate = delvDate;
			Vm.TakeTime = takeTime;
			Vm.AllId = allId;
			Vm.DoUpload += FileUpload;
			Vm.SearchCommand.Execute(null);
		}

		private void ExitCommand_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = _hasUpload;
			Close();
		}
		private void FileUpload(bool isAdd)
		{
			var dlg = new OpenFileDialog { Multiselect = isAdd, DefaultExt = ".jpg", Filter = Properties.Resources.P0801010101_ItemImage };
			int imgMax = Vm.MaxUploadCount - ((Vm.DataList == null) ? 0 : Vm.DataList.Count);
			if (!isAdd)
				imgMax = 1;
			//imgMax-取得目前已上傳數 =本次可上傳檔案數
			dlg.FileOk += delegate(object s, CancelEventArgs ev)
			{
				Vm.FilePathList = new List<string>();
				foreach (var p in dlg.FileNames)
				{
					if (dlg.FileNames.Count() > imgMax)
					{
						DialogService.ShowMessage(Properties.Resources.P0809010000xamlcs_JPG_Limit3);
						Vm.FilePathList.Clear();
						ev.Cancel = true;
						return;
					}
					if (new FileInfo(p).Length > Common.GlobalVariables.FileSizeLimit)
					{
						Vm.ShowMessage(Messages.WarningFileSizeExceedLimits);
						ev.Cancel = true;
					}
					Vm.FilePathList.Add(p);
				}
			};
			if (dlg.ShowDialog() == true)
			{
				Vm.UploadType = isAdd ? UploadType.Add : UploadType.Edit;
				Vm.UploadCommand.Execute(null);
				_hasUpload = true;
			}
		}

		

		private void ShowImg_Click(object sender, RoutedEventArgs e)
		{
			var bt = sender as Button;
			var fileName = bt.CommandParameter.ToString();
			Vm.SelectedData = Vm.DataList.FirstOrDefault(x => x.FILE_NAME == fileName);
		}

		
	}
}
