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
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.P08.Report;
using Wms3pl.WpfClient.P08.ViewModel;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClients.SharedViews.Views;

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0808060000.xaml 的互動邏輯
	/// </summary>
	public partial class P0808060000 : Wms3plWindow
	{
		public P0808060000()
		{
			InitializeComponent();
			Vm.DoDcChange = DcChange;
			Vm.DoContainerFocus = ContainerCodeFocus;
			Vm.DoNormalContainerFocus = NormalContainerFocus;
			Vm.DoCancelContainerFocus = CancelContainerFocus;
			Vm.DoItemBarCodeFocus = ItemBarCodeFocus;
			Vm.DoReBindBox = DoRebindBox;
			Vm.DoNormalContainerEnable = NormalContainerEnable;
			Vm.DoNormalContainerDisable = NormalContainerDisable;
			Vm.DoCancelContainerEnable = CancelContainerEnable;
			Vm.DoCancelContainerDisable = CancelContainerDisable;
			Vm.DoItemBarCodeEnable = ItemBarCodeEnable;
			Vm.DoItemBarCodeDisable = ItemBarCodeDisable;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			DispatcherAction(() =>
			{
				ContainerCodeFocus();
			});
		}

		#region 物流中心變更事件
		private void DcChange()
		{
			var openDeviceWindow = OpenDeviceWindow(Vm.FunctionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, Vm.SelectedDc);
			if (!openDeviceWindow.Any())
			{
				var deviceWindow = new DeviceWindow(Vm.SelectedDc);
				deviceWindow.Owner = this.Parent as Window;
				deviceWindow.ShowDialog();
			}
			ContainerCodeFocus();
		}
		#endregion

		#region Focus

		private void ContainerCodeFocus()
		{
			SetFocusedElement(TxtContainerCode, true);
		}

		private void NormalContainerFocus()
		{
			SetFocusedElement(TxtNormalContainerCode, true);
		}

		private void CancelContainerFocus()
		{
			SetFocusedElement(TxtCancelContainerCode, true);
		}

		private void ItemBarCodeFocus()
		{
			SetFocusedElement(TxtScanItemBarCode, true);
		}
		#endregion

		#region Enabled
		private void NormalContainerEnable()
		{
			DispatcherAction(() =>
			{
				TxtNormalContainerCode.IsEnabled = true;
			});
		}
		private void NormalContainerDisable()
		{
			DispatcherAction(() =>
			{
				TxtNormalContainerCode.IsEnabled = false;
			});
		}
		private void CancelContainerEnable()
		{
			DispatcherAction(() =>
			{
				TxtCancelContainerCode.IsEnabled = true;
			});
		}
		private void CancelContainerDisable()
		{
			DispatcherAction(() =>
			{
				TxtCancelContainerCode.IsEnabled = false;
			});
		}
		private void ItemBarCodeEnable()
		{
			DispatcherAction(() =>
			{
				TxtScanItemBarCode.IsEnabled = true;
			});
		}
		private void ItemBarCodeDisable()
		{
			DispatcherAction(() =>
			{
				TxtScanItemBarCode.IsEnabled = false;
			});
		}
		#endregion


		#region 刷讀品號/國條/商品序號
		private void ScanItemBarCode_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;

			Vm.ScanItemBarCodeCommand.Execute(null);
		}
		#endregion

		#region 重綁箱號

		private void DoRebindBox(BindBoxType bindBoxType)
		{
			var containerInfo = bindBoxType == BindBoxType.NormalShipBox ? Vm.NormalShipBox.Info : Vm.CancelOrderBox.Info;
			var win = new P0808060200(Vm.SelectedDc, bindBoxType, containerInfo, Vm.CurrentContainerPickInfo);
			win.Owner = this;
			win.ShowDialog();
			if (win.Vm.IsOk)
			{
				var newContainerCode = win.Vm.ContainerCode;
				switch (bindBoxType)
				{
					case BindBoxType.NormalShipBox:
						Vm.NormalContainerCode = newContainerCode;
						Vm.NormalShipBox.Info.OUT_CONTAINER_CODE = newContainerCode;
						break;
					case BindBoxType.CanelOrderBox:
						Vm.CancelContainerCode = newContainerCode;
						Vm.CancelOrderBox.Info.OUT_CONTAINER_CODE = newContainerCode;
						break;
				}
			}
			ItemBarCodeFocus();
		}
		#endregion

		#region 刷讀容器條碼
		private void TxtContainerCode_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;

			Vm.SearchCommand.Execute(null);
		}
		#endregion

		#region 箱內明細

		private void BoxDetail_Click(object sender, RoutedEventArgs e)
		{
			if (Vm.CurrentContainerPickInfo != null)
			{
				var win = new P0808060100(Vm.SelectedDc);
				win.Owner = this;
				win.ShowDialog();
				DispatcherAction(() =>
				{
				});
			}
			else
			{
				var win = new P0808060100();
				win.Owner = this;
				win.ShowDialog();
				DispatcherAction(() =>
				{
					ContainerCodeFocus();
				});
			}
		}
		#endregion

		#region 揀貨單批次查詢
		private void BatchPickQuery_OnClick(object sender, RoutedEventArgs e)
		{
			var win = new P0808060300(Vm.SelectedDc);
			win.Owner = this;
			win.ShowDialog();
			DispatcherAction(() =>
			{
				ContainerCodeFocus();
			});
		}
		#endregion

		private void TxtNormalContainerCode_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			Vm.ScanNormalContainerCommand.Execute(null);
		}

		private void TxtCancelContainerCode_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			Vm.ScanCancelContainerCommand.Execute(null);
		}

		private void ContainerCodePreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			var txt = ((TextBox)sender).Text + e.Text;
			e.Handled = !ValidateHelper.IsMatchAZaz09(txt);
		}
	}
}
