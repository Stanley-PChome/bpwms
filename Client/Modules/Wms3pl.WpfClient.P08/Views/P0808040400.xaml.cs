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

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0808040400.xaml 的互動邏輯
	/// </summary>
	public partial class P0808040400 : Wms3plWindow
	{
		public P0808040400(string dcCode)
		{
			InitializeComponent();
			Vm.SelectedDc = dcCode;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			DispatcherAction(() =>
			{
				SetFocusedElement(txtContainerBarcode,true);
			});
		}
	}
}
