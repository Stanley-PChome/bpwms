using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Wms3pl.WpfClient.P02.ViewModel;
using Wms3pl.WpfClient.UILib;
using System;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using Wms3pl.WpfClient.Common;

namespace Wms3pl.WpfClient.P02.Views
{
	public partial class P0202030300 : Wms3plWindow
	{
		public P0202030300(string dcCode, string purchaseNo, string rtNo)
		{
			InitializeComponent();
			Vm.SelectedDc = dcCode;
			Vm.RtNo = rtNo;
			Vm.PurchaseNo = purchaseNo;
			Vm.SearchCommand.Execute(null);
			Vm.AfterUpload += this.Close;
		}

		/// <summary>
		/// 選擇檔案
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnChooseFile_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.CheckFileExists = true;
			dlg.Multiselect = (Vm.SelectedItem.Item.UPLOAD_TYPE != "00");
			dlg.DefaultExt = ".jpg";
			dlg.Filter = Properties.Resources.P0202030300_ChooseFileType;
			List<string> files = new List<string>();
			Vm.GetPath();
			dlg.FileOk += delegate(object s, CancelEventArgs ev)
			{
				foreach (var p in dlg.FileNames)
				{
					if (new FileInfo(p).Length > Wms3pl.WpfClient.Common.GlobalVariables.FileSizeLimit)
					{
						Vm.ShowMessage(Messages.WarningFileSizeExceedLimits);
						files.Clear();
						ev.Cancel = true;
						return;
					}
					if( (new FileInfo(p).Length / 1024)> Vm.fileSize)
					{
						Vm.ShowWarningMessage(string.Format(@Properties.Resources.P0202030300_FileSizeOver, Vm.fileSize));
						files.Clear();
						ev.Cancel = true;
						return;
					}
					files.Add(p);
				}
			};

			if (dlg.ShowDialog() == true)
			{
				Vm.SelectedItem.Item.SELECTED_COUNT = Vm.SelectedItem.Item.SELECTED_COUNT + files.Count;
				if (files.Count > 0) Vm.SelectedItem.IsSelected = true;
				if (Vm.SelectedItem.Item.UPLOAD_TYPE == "00") Vm.SelectedItem.Item.SELECTED_FILES = dlg.FileName;
				else Vm.SelectedItem.Item.SELECTED_FILES += "|" + string.Join("|", files);
				files.Clear();
			}
		}

		private void btnExit_Click(object sender, RoutedEventArgs e)
		{
			if (Vm.ShowMessage(Messages.WarningBeforeExit) == UILib.Services.DialogResponse.OK)
				this.Close();
		}

		private void btnClearFile_Click(object sender, RoutedEventArgs e)
		{
			Vm.SelectedItem.Item.SELECTED_FILES = string.Empty;
			Vm.SelectedItem.Item.SELECTED_COUNT = 0;
		}
	}
}
