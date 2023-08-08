using Microsoft.Win32;
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
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P19.Views
{
	/// <summary>
	/// P1901020100.xaml 的互動邏輯
	/// </summary>
	public partial class P1901020100 : Wms3plWindow
	{
		public P1901020100()
		{
			InitializeComponent();
			Vm.AddImage = btnAddFileUpload;
			Vm.UpdImage = btnEditFileUpload;
		}

		public P1901020100(string _itemCode, string gupCode, string crtYear)
		{
			InitializeComponent();
			ItemCode = _itemCode;
			GupCode = gupCode;
			CrtYear = crtYear;
			Vm.AddImage = btnAddFileUpload;
			Vm.UpdImage = btnEditFileUpload;
			Vm.SearchCommand.Execute(null);
		}

		public string ItemCode { set { Vm.ItemCode = value; } }

		public string GupCode { set { Vm.GupCode = value; } }

		public string CrtYear { set { Vm.CrtYear = value; } }

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//var dr = DialogService.ShowMessage(Properties.Resources.P1901020100xamlcs_YesToLeave, Properties.Resources.P1901020100xamlcs_Message, UILib.Services.DialogButton.YesNo, UILib.Services.DialogImage.Warning);
			//if (dr == UILib.Services.DialogResponse.No)
			//	e.Cancel = true;
		}

		private void ExitCommand_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}	


		private OpenFileDialog FileUpload()
		{
			OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.Multiselect = false;
			dlg.DefaultExt = ".jpg";
			dlg.Filter = Properties.Resources.P1901020100xamlcs_ItemImage;
			dlg.FileOk += delegate(object s, CancelEventArgs ev)
			{
				foreach (var p in dlg.FileNames)
				{
					if (new FileInfo(p).Length > Wms3pl.WpfClient.Common.GlobalVariables.FileSizeLimit)
					{
						Vm.ShowMessage(Messages.WarningFileSizeExceedLimits);
						ev.Cancel = true;
					}
				}
			};
			return dlg;
		}

		private void btnAddFileUpload()
		{
			var dlg = FileUpload();
			// Get the selected file name and display in a TextBox
			if (dlg.ShowDialog() == true)
			{
				Vm.FileName = dlg.FileName;
				Vm.UpType = ViewModel.uploadType.Add;
				Vm.UploadCommand.Execute(null);
			}
		}

		private void ShowImg_Click(object sender, RoutedEventArgs e)
		{
			Button bt = sender as Button;
			Int16 imageNo = 0;
			Int16.TryParse(bt.CommandParameter.ToString(),out imageNo);
			Vm.SelectedData = Vm.Records.Where(x => x.IMAGE_NO == imageNo).FirstOrDefault();
			Vm.RefreshImage();
		}

		private void btnEditFileUpload()
		{
			var dlg = FileUpload();
			// Get the selected file name and display in a TextBox
			if (dlg.ShowDialog() == true)
			{
				Vm.FileName = dlg.FileName;
				Vm.UpType = ViewModel.uploadType.Edit;
				Vm.UploadCommand.Execute(null);
			}
		}
	}
}
