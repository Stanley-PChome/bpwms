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
	/// P1701030000.xaml 的互動邏輯
	/// </summary>
	public partial class P1701030000 : Wms3plUserControl
	{
		P7101030000_ViewModel Vm;
		public P1701030000()
		{
			InitializeComponent();
			Vm = new P7101030000_ViewModel(true);
			this.DataContext = Vm;
		}

        private void IsSaveHorDistance_Checked(object sender, RoutedEventArgs e)
        {
            Vm.CheckUpdateSet("HorDistance");
        }

        private void IsSaveLocType_Checked(object sender, RoutedEventArgs e)
        {
            Vm.CheckUpdateSet("LocType");
        }

        private void IsSaveHandy_Checked(object sender, RoutedEventArgs e)
        {
            Vm.CheckUpdateSet("Handy");
        }

        private void IsSaveRentBaginDate_Checked(object sender, RoutedEventArgs e)
        {
            Vm.CheckUpdateSet("RentBaginDate");
        }

        private void IsSaveRentEndDate_Checked(object sender, RoutedEventArgs e)
        {
            Vm.CheckUpdateSet("RentEndDate");
        }
    }
}
