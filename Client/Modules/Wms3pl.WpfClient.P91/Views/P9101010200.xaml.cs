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
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.P91.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P91.Views
{
  /// <summary>
  /// P9101010200.xaml 的互動邏輯
  /// </summary>
  public partial class P9101010200 : Wms3plWindow
  {
		public P9101010200(F910201 baseData)
    {
      InitializeComponent();
			Vm.BaseData = baseData;
			Vm.SearchCommand.Execute(null);
    }

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{

		}

		private void chkCheckAll_Click(object sender, RoutedEventArgs e)
		{
			CheckBox obj = sender as CheckBox;
			Vm.CheckAll((obj.IsChecked == null || obj.IsChecked == false) ? false : true);
		}
  }
}
