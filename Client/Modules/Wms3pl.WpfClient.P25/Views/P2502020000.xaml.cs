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
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P25ExDataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.UcLib.Views;

namespace Wms3pl.WpfClient.P25.Views
{
	/// <summary>
	/// P2502020000.xaml 的互動邏輯
	/// </summary>
	public partial class P2502020000 : Wms3plUserControl
	{
		public P2502020000()
		{
			InitializeComponent();
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
				F1903 f1903 = winSearchProduct.SelectData;
				Vm.AppendItemCode(f1903);
			}
		}
	}
}
