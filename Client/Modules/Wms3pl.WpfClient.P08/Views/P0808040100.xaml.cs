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
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClients.SharedViews.Views;

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0808040100.xaml 的互動邏輯
	/// </summary>
	public partial class P0808040100 : Wms3plWindow
	{
		public P0808040100()
		{
			InitializeComponent();
			Vm.DoDcChange = DcChange;
		}

		public P0808040100(string dcCode, DateTime? delvDate, string pickTime)
		{
			InitializeComponent();
			Vm.DoDcChange = DcChange;
			Vm.SelectDcCode = dcCode;
			Vm.DelvDate = delvDate == null ? DateTime.Today : delvDate;
			Vm.SelectPickTime = pickTime;
		}

		private void Window_OnLoaded(object sender, RoutedEventArgs e)
		{
			DcChange();
		}

		#region 物流中心變更事件
		private void DcChange()
		{
			var openDeviceWindow = OpenDeviceWindow(Vm.FunctionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, Vm.SelectDcCode);
			if (!openDeviceWindow.Any())
			{
				var deviceWindow = new DeviceWindow(Vm.SelectDcCode);
				deviceWindow.Owner = this.Parent as Window;
				deviceWindow.ShowDialog();
			}
		}
		#endregion


		private void CancelCommand_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
