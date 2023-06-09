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
	/// P5001050000.xaml 的互動邏輯
	/// </summary>
	public partial class P5001050000 : Wms3plUserControl
	{
		public P5001050000()
		{
			InitializeComponent();
			Vm.ResetUI += ResetUI;
			Vm.DoUpLoad += DoUpload;
			Vm.ReUploadBtn += ReUploadBtn;
			ResetQueryUI();
		}

		private void AccKindCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox cb = sender as ComboBox;
			if (cb.SelectedIndex == -1) return;

			if (Vm.F500102Add != null)
			{
				AccKindDetailA.Visibility = Vm.F500102Add.ACC_KIND == "F" ? Visibility.Visible : Visibility.Collapsed;
				AccKindDetailB.Visibility = Vm.F500102Add.ACC_KIND == "C" ? Visibility.Visible : Visibility.Collapsed;
				AccKindDetailC.Visibility = Vm.F500102Add.ACC_KIND == "D" ? Visibility.Visible : Visibility.Collapsed;
				AccKindDetailD.Visibility = Vm.F500102Add.ACC_KIND == "E" ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		private void AccKindComboboxQuery_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox cb = sender as ComboBox;
			if (cb.SelectedIndex == -1) return;

			if (Vm.SelectF500102Data != null)
			{
				AccKindDetailAQuery.Visibility = Vm.SelectF500102Data.ACC_KIND == "F" ? Visibility.Visible : Visibility.Collapsed;
				AccKindDetailBQuery.Visibility = Vm.SelectF500102Data.ACC_KIND == "C" ? Visibility.Visible : Visibility.Collapsed;
				AccKindDetailCQuery.Visibility = Vm.SelectF500102Data.ACC_KIND == "D" ? Visibility.Visible : Visibility.Collapsed;
				AccKindDetailDQuery.Visibility = Vm.SelectF500102Data.ACC_KIND == "E" ? Visibility.Visible : Visibility.Collapsed;
			}
		}



		private void ResetUI()
		{
			if (Vm.F500102Add != null)
			{
				AccKindDetailA.Visibility = Visibility.Collapsed;
				AccKindDetailB.Visibility = Visibility.Collapsed;
				AccKindDetailC.Visibility = Visibility.Collapsed;
				AccKindDetailD.Visibility = Visibility.Collapsed;
			}
		}

		private void ResetQueryUI()
		{
			AccKindDetailAQuery.Visibility = Visibility.Collapsed;
			AccKindDetailBQuery.Visibility = Visibility.Collapsed;
			AccKindDetailCQuery.Visibility = Visibility.Collapsed;
			AccKindDetailDQuery.Visibility = Visibility.Collapsed;
		}

		private void DoUpload()
		{

			if (Vm.SelectF500102Data != null)
			{
				if (!string.IsNullOrEmpty(Vm.SelectF500102Data.UPLOAD_FILE))  //已上傳
				{
					var fileName = Vm.SelectF500102Data.QUOTE_NO + System.IO.Path.GetExtension(Vm.SelectF500102Data.UPLOAD_FILE);
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
			if (Vm.SelectF500102Data != null && !string.IsNullOrEmpty(Vm.SelectF500102Data.UPLOAD_FILE))
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
			if (!string.IsNullOrEmpty(Vm.SelectF500102Data.UPLOAD_FILE))  //已上傳
			{
				var fileName = Vm.SelectF500102Data.QUOTE_NO + System.IO.Path.GetExtension(Vm.SelectF500102Data.UPLOAD_FILE);
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
