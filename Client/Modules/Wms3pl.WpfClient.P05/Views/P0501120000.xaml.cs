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

namespace Wms3pl.WpfClient.P05.Views
{
	/// <summary>
	/// P0501120000.xaml 的互動邏輯
	/// </summary>
	public partial class P0501120000 : Wms3plUserControl
	{
		public P0501120000()
		{
			InitializeComponent();
			Vm.OpenAGVStationSetting += OpenAGVStationSetting;
		}
		private void OpenAGVStationSetting()
		{
			var win = new P0501120100(Vm.SelectedDgItem);
			win.ShowDialog();
		}


	}
}
