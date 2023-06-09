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
using Wms3pl.WpfClient.P16.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P16.Views
{
	/// <summary>
	/// P1604010100.xaml 的互動邏輯
	/// </summary>
	public partial class P1604010100 : Wms3plWindow
	{
		public P1604010100()
		{
			InitializeComponent();
			Vm.DoClose += Win_Close;
		}

		private void Win_Close()
		{
			this.DialogResult = true;
			this.Close();
		}
		public void SetBaseData(string dcCode, string gupCode, string custCode, 
			List<NameValuePair<string>> warehouseList,
			List<NameValuePair<string>> scrapResonList)
		{
			Vm.DcCode = dcCode;
			Vm.GupCode = gupCode;
			Vm.CustCode = custCode;
			Vm.WarehouseList = warehouseList;
			Vm.ScrapResonList = scrapResonList;
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
