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
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClients.SharedViews.Views;

namespace Wms3pl.WpfClient.P91.Views
{
	/// <summary>
	/// P9103040000.xaml 的互動邏輯
	/// </summary>
	public partial class P9103040000 : Wms3plUserControl
	{
		private DeviceWindow _deviceWindow;
		public P9103040000()
		{
			InitializeComponent();
			this.Loaded += Window_OnLoaded;
		}

		private void VnrCode_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			Vm.SetVnrInfo(Wms3plSession.Get<GlobalInfo>().GupCode, Wms3plSession.Get<GlobalInfo>().CustCode);
		}

		private void VnrCode_LostFocus(object sender, RoutedEventArgs e)
		{
			Vm.SetVnrInfo(Wms3plSession.Get<GlobalInfo>().GupCode, Wms3plSession.Get<GlobalInfo>().CustCode);
		}
		private void ItemCode_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			Vm.GetItemData(Wms3plSession.Get<GlobalInfo>().GupCode, Wms3plSession.Get<GlobalInfo>().CustCode);
		}

		private void ItemCode_LostFocus(object sender, RoutedEventArgs e)
		{
			Vm.GetItemData(Wms3plSession.Get<GlobalInfo>().GupCode, Wms3plSession.Get<GlobalInfo>().CustCode);
		}

		private void Window_OnLoaded(object sender, RoutedEventArgs e)
		{
			DcCodeChanged();
		}

		private void DcCodeChanged()
		{
			if (_deviceWindow != null)
				return;
			_deviceWindow = new DeviceWindow("");
			_deviceWindow.Owner = this.Parent as Window;
			_deviceWindow.ShowDialog();
			Vm.SelectedF910501 = _deviceWindow.SelectedF910501;
		}
	}


}
