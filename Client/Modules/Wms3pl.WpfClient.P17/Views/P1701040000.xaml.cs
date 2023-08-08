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
using Wms3pl.WpfClient.P71.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P17.Views
{
	/// <summary>
	/// P1701040000.xaml 的互動邏輯
	/// </summary>
	public partial class P1701040000 : Wms3plUserControl
	{
		P7101040000_ViewModel Vm;
		public P1701040000()
		{
			InitializeComponent();
			Vm = new P7101040000_ViewModel(true);
			this.DataContext = Vm;
			Vm.OnSearch += SwitchGridColumnVisible;
		}
		private void txtItemCode_LostFocus(object sender, RoutedEventArgs e)
		{
			Vm.DoSearchItemName();
		}

		private void SwitchGridColumnVisible()
		{
			Visibility itemVisible = Vm.SearchByItem ? Visibility.Visible : Visibility.Hidden;
			dgList.Columns[1].Visibility = itemVisible;
			dgList.Columns[2].Visibility = itemVisible;
		}

	}
}
