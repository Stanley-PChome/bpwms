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
using Wms3pl.WpfClient.P08.ViewModel;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClients.SharedViews.Views;

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0808050000.xaml 的互動邏輯
	/// </summary>
	public partial class P0808050000 : Wms3plWindow
	{
		public P0808050000()
		{
			InitializeComponent();
			Vm.DoDcChange = DcChange;
			Vm.DoContainerCodeFocus = ContainerCodeFocus;
			Vm.DoOutContainerCodeFocus = OutContainerCodeFocus;
			Vm.DoPutIntoConfirmFocus = PutIntoConfirmFocus;
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
			Vm.Init();
		}
		#endregion

		#region Focus
		private void ContainerCodeFocus()
		{
			SetFocusedElement(TxtContainerCode, true);
		}

		private void OutContainerCodeFocus()
		{
			SetFocusedElement(TxtOutContainerCode, true);
		}

		private void PutIntoConfirmFocus()
		{
			SetFocusedElement(BtnPutIntoConfirm, true);
		}
		#endregion

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			DispatcherAction(() =>
			{
				ContainerCodeFocus();
			});
		}

		#region 刷讀揀貨容器條碼
		private void TxtContainerCode_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;

			Vm.SearchCommand.Execute(null);
		}
		#endregion

		#region 刷讀跨庫箱號
		private void TxtOutContainerCode_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;

			Vm.ScanOutContainerCommand.Execute(null);
		}
		#endregion

		#region 箱內明細

		private void BoxDetail_Click(object sender, RoutedEventArgs e)
		{
			if (Vm.CurrentPickContainerResult != null)
			{
				var win = new P0808050100("P0808050000", Vm.SelectedDc);
				win.Owner = this;
				win.ShowDialog();
				DispatcherAction(() =>
				{
				});
			}
			else
			{
				var win = new P0808050100("P0808050000");
				win.Owner = this;
				win.ShowDialog();
				DispatcherAction(() =>
				{
					ContainerCodeFocus();
				});
			}
		}
		#endregion

		#region 點擊關箱動作
		private void CloseBox_Click(object sender, RoutedEventArgs e)
		{
			//var win = new P0808040400(Vm.SelectedDc);
			//win.Owner = this;
			//win.ShowDialog();
			//DispatcherAction(() =>
			//{
			//	ContainerCodeFocus();
			//});
		}
		#endregion

		private void ContainerCodePreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			var txt = ((TextBox)sender).Text + e.Text;
			e.Handled = !ValidateHelper.IsMatchAZaz09(txt);
		}
	}
}
