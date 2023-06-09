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
using System.Windows.Threading;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F16DataService;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.P08.ViewModel;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClients.SharedViews.Views;

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0802010000.xaml 的互動邏輯
	/// </summary>
	public partial class P0802010000 : Wms3plWindow
	{
		public P0802010000()
		{
			InitializeComponent();
			Vm.OnDcCodeChanged += ShowDeviceWindowAndSetF910501;
			Vm.SetRecordScroll += SetRecordScrollTo;
			Vm.SetSerialRecordScroll += SetSerialRecordScrollTo;
			Vm.SetInputSerialFocus += SetInputSerialFocusTo;
			Vm.SetTxtInputBillNoFocus += SetTxtInputBillNoFocus;
#if DEBUG
			//this.WindowStyle = System.Windows.WindowStyle.ToolWindow;
			//this.ResizeMode = System.Windows.ResizeMode.CanResize;
			//this.WindowState = System.Windows.WindowState.Normal;
			//this.ShowInTaskbar = true;
#endif
		}

		private void SetInputSerialFocusTo()
		{
			SetDefaultFocusClick(TxtInputSerailNo);
		}

		private void SetTxtInputBillNoFocus()
		{
			SetFocusedElement(TxtInputBillNo, true);
		}

		private void SetSerialRecordScrollTo(F16140101Data obj)
		{
			this.ControlView(() =>
			{
				DgSerialRecords.Focus();
				DgSerialRecords.SelectedItem = obj;
				DgSerialRecords.ScrollIntoView(obj);
			});
		}

		private void SetRecordScrollTo(SelectionItem<F161402Data> obj)
		{
			this.ControlView(() =>
			{
				DgRecords.Focus();
				DgRecords.SelectedItem = obj;
				DgRecords.ScrollIntoView(obj);
			});
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var win = new P0802010100() { Owner = System.Windows.Window.GetWindow(this) };
			win.Vm.SelectedDc = Vm.SelectedDc;
			win.ShowDialog();
			//重新取得清單
			Vm.DoSearchGatherList();
		}

		private void ScanInputBillNo_OnKeyDown(object sender, KeyEventArgs e)
		{
			TextBox obj = sender as TextBox;
			if (e.Key != Key.Enter || string.IsNullOrWhiteSpace(obj.Text))
				return;
			//語音
			if (Vm.PlaySound)
				PlaySoundHelper.Scan();
			Vm.SearchCommand.Execute(null);
			SetDefaultFocusClick(obj);
		}


		private void SetDefaultFocusClick(UIElement InputElement)
		{
			SetFocusedElement(InputElement);
		}

		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void StartCheck_Click(object sender, RoutedEventArgs e)
		{
			Vm.AddRturnMainCommand.Execute(null);
			
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			ShowDeviceWindowAndSetF910501();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var dr = DialogService.ShowMessage(WpfClient.Resources.Resources.WarningBeforeClose, WpfClient.Resources.Resources.Information, UILib.Services.DialogButton.YesNo, UILib.Services.DialogImage.Warning);
			if (dr == UILib.Services.DialogResponse.No)
				e.Cancel = true;
		}

		private void ScanInputSerailNo_Click(object sender, RoutedEventArgs e)
		{
			if (!Vm.IsPending) return;
			if (string.IsNullOrWhiteSpace(TxtInputSerailNo.Text)) return;
			Vm.IsInputSerailNoClick = true;
			Vm.SearchItemCommand.Execute(null);
		}

		/// <summary>
		/// 開啟序號收集頁面
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnImportSerial_Click(object sender, RoutedEventArgs e)
		{
			var win = new P0802010200(Vm.ReturnRecordMain, Vm.SelectedF910501) { Owner = Wms3plViewer.GetWindow(this) };
			win.ShowDialog();
			Vm.ImportSerialComplete.Execute(null);
			SetFocusedElement(TxtInputSerailNo);
		}

		private void Posting_Click(object sender, RoutedEventArgs e)
		{
			Vm.DoSave();
			Vm.PostingCommand.Execute(null);
		}

		private void ForceClose_Click(object sender, RoutedEventArgs e)
		{
			Vm.DoSave();
			Vm.ForceCloseCommand.Execute(null);
		}

		private void CancelCheck_Click(object sender, RoutedEventArgs e)
		{
			Vm.CancelCheck();
			txtSearchSerialNo.Text = string.Empty;
		}

		private void DgRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Vm.SetScanInputValue();
		}

		private void SearchSerailNo_OnKeyDown(object sender, KeyEventArgs e)
		{
			TextBox obj = sender as TextBox;
			if (e.Key != Key.Enter || string.IsNullOrWhiteSpace(obj.Text))
				return;
			//語音
			if (Vm.PlaySound)
				PlaySoundHelper.Scan();
			Vm.SerachSerialNo(obj.Text);
			SetDefaultFocusClick(obj);
		}

		void ShowDeviceWindowAndSetF910501()
		{
            var openDeviceWindow = OpenDeviceWindow(Vm.FunctionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, Vm.SelectedDc);
            if (openDeviceWindow.Any())
            {
                Vm.SelectedF910501 = openDeviceWindow.FirstOrDefault();
            }
            else
            {
                var deviceWindow = new DeviceWindow(Vm.SelectedDc);
                deviceWindow.Owner = this;
                deviceWindow.ShowDialog();
                Vm.SelectedF910501 = deviceWindow.SelectedF910501;
            }
           
		}
	}
}
