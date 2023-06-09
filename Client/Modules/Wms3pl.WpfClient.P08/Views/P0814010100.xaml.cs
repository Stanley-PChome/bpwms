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
using Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.P08.ViewModel;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0814010100.xaml 的互動邏輯
	/// </summary>
	public partial class P0814010100 : Wms3plWindow
	{

		public P0814010100(string dcCode, string shippingMode, bool? hasWorkingOrd = null)
		{
			InitializeComponent();
			Vm.DcCode = dcCode;
			Vm.ShippingMode = shippingMode;
            Vm.HasWorkingOrd = hasWorkingOrd;
			Vm.Leave += this.Close;
			Vm.GetDeviceSetting();
			Vm.InitButten();
			if (shippingMode == "2")
			{
				Vm.GetWorkStataionShipData();
				Vm.WaitWmsOrderCntVisibility = Visibility.Visible;
			};
			Vm.WaitWmsOrderCntVisibility = shippingMode == "2"?Visibility.Visible: Visibility.Collapsed;

		}
	}
}
