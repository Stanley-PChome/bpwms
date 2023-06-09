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
using Wms3pl.WpfClient.P71.ViewModel;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.Views
{
	/// <summary>
	/// P7108020000.xaml 的互動邏輯
	/// </summary>
	public partial class P7108020000 : Wms3plUserControl
	{
		public P7108020000()
		{
			InitializeComponent();
		}

		private void ItemSearchButton_OnClick(object sender, RoutedEventArgs e)
		{
			WinSearchProduct winSearchProduct = new WinSearchProduct();
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			winSearchProduct.GupCode = globalInfo.GupCode;
			winSearchProduct.CustCode = globalInfo.CustCode;
			winSearchProduct.ShowDialog();

			if (winSearchProduct.SelectData != null)
			{
				F1903 f1903 = winSearchProduct.SelectData;
				Vm.ItemCode = f1903.ITEM_CODE;
				Vm.ItemName = f1903.ITEM_NAME;
			}
		}
	}
}
