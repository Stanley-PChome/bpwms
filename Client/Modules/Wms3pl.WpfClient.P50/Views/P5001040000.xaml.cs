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
using Wms3pl.WpfClient.UILib;
using System.IO;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace Wms3pl.WpfClient.P50.Views
{
	/// <summary>
	/// P5001040000.xaml 的互動邏輯
	/// </summary>
	public partial class P5001040000 : Wms3plUserControl
	{
		public P5001040000()
		{
			InitializeComponent();
			Vm.DoUpLoad += DoUpload;
			Vm.ReUploadBtn += ReUploadBtn;
		}
		private void DoUpload()
		{

			if (Vm.SelectF500103Data != null)
			{
				if (!string.IsNullOrEmpty(Vm.SelectF500103Data.UPLOAD_FILE))  //已上傳
				{
					var fileName = Vm.SelectF500103Data.QUOTE_NO + System.IO.Path.GetExtension(Vm.SelectF500103Data.UPLOAD_FILE);
					var filePath = System.IO.Path.Combine(Vm.FileFolderPath, fileName);
					if (File.Exists(filePath))
					{
						System.Diagnostics.Process.Start(filePath);
					}
					else
					{
						Vm.DialogService.ShowMessage(Properties.Resources.P5001020000xamlcs_FileNotFound);
					}
				}
				else
				{
					OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
					dlg.Multiselect = false;
					dlg.DefaultExt = ".*";
					dlg.Filter = Properties.Resources.P5001020000xamlcs_UploadFile_WithFormat;
					if (dlg.ShowDialog() == true)
					{
						Vm.SelectFileName = dlg.FileName;
					}
				}

			}
		}
		private void ReUploadBtn()
		{
			if (Vm.SelectF500103Data != null && !string.IsNullOrEmpty(Vm.SelectF500103Data.UPLOAD_FILE))
			{
				btnUpload.Content = Properties.Resources.P5001020000xamlcs_ViewFile;
			}
			else
			{
				btnUpload.Content = Properties.Resources.P5001020000xamlcs_UploadFile;
			}

		}

		private void btnViewUpload_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(Vm.SelectF500103Data.UPLOAD_FILE))  //已上傳
			{
				var fileName = Vm.SelectF500103Data.QUOTE_NO + System.IO.Path.GetExtension(Vm.SelectF500103Data.UPLOAD_FILE);
				var filePath = System.IO.Path.Combine(Vm.FileFolderPath, fileName);
				if (File.Exists(filePath))
				{
					System.Diagnostics.Process.Start(filePath);
				}
				else
				{
					Vm.DialogService.ShowMessage(Properties.Resources.P5001020000xamlcs_FileNotFound);
				}
			}
		}
	}
}
