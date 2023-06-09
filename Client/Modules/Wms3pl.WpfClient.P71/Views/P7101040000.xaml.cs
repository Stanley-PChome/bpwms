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

namespace Wms3pl.WpfClient.P71.Views
{
	/// <summary>
	/// P7101040000.xaml 的互動邏輯
	/// </summary>
	public partial class P7101040000 : Wms3plUserControl
	{
		public P7101040000()
		{
			InitializeComponent();
			Vm.OnSearch += SwitchGridColumnVisible;
            Vm.OnSearchItemComplete += OnSearchItemComplete;
		}

		private void txtItemCode_LostFocus(object sender, RoutedEventArgs e)
		{
            Vm.SearchItemNameCommand.Execute(null);
		}
        private void OnSearchItemComplete()
        {
            // Memo: 不會自動跳到第一個項目
            cbItemNameList.SelectedIndex = 0;
        }
		private void SwitchGridColumnVisible()
		{
			Visibility itemVisible = Vm.SearchByItem ? Visibility.Visible : Visibility.Hidden;
			dgList.Columns[1].Visibility = itemVisible;
			dgList.Columns[2].Visibility = itemVisible;
		}
	}
}
