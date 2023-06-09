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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.P18.ViewModel;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P18.Views
{
	/// <summary>
	/// P1802010000.xaml 的互動邏輯
	/// </summary>
	public partial class P1802010000 : Wms3plUserControl
	{
		public P1802010000()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void btnSerachProduct_Click(object sender, RoutedEventArgs e)
		{
			WinSearchProduct winSearchProduct = new WinSearchProduct();
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			winSearchProduct.GupCode = globalInfo.GupCode;
			winSearchProduct.CustCode = globalInfo.CustCode;
			winSearchProduct.ShowDialog();

			if (winSearchProduct.SelectData != null)
			{
				Vm.ItemCode = winSearchProduct.SelectData.ITEM_CODE;
			}
		}
	}
}