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
using Wms3pl.WpfClient.P06.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P06.Views
{
	/// <summary>
	/// P0601010000.xaml 的互動邏輯
	/// </summary>
	public partial class P0601010000 : Wms3plUserControl
	{
		public P0601010000()
		{
			InitializeComponent();
		}

		private void TxtCUST_ORD_NO_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Vm.DoSearchCustOrdNo();
			}
		}
	}
}
