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
	/// P1801010000.xaml 的互動邏輯
	/// </summary>
	public partial class P1801010000 : Wms3plUserControl
	{
		public P1801010000()
		{
			InitializeComponent();

			Vm.SetGridVisiablity += SetGridVisiablity;
			Vm.OpenValidDate += OpenVaidDate;
		}

		private void SetGridVisiablity(bool isExpend)
		{
			if (isExpend)
			{
				Dg1.Columns[Dg1.Columns.Count - 1].Visibility = Visibility.Visible;
				Dg1.Columns[Dg1.Columns.Count - 2].Visibility = Visibility.Visible;
				Dg1.Columns[Dg1.Columns.Count - 3].Visibility = Visibility.Visible;
				Dg1.Columns[Dg1.Columns.Count - 4].Visibility = Visibility.Visible;
			}
			else
			{
				Dg1.Columns[Dg1.Columns.Count - 1].Visibility = Visibility.Collapsed;
				Dg1.Columns[Dg1.Columns.Count - 2].Visibility = Visibility.Collapsed;
				Dg1.Columns[Dg1.Columns.Count - 3].Visibility = Visibility.Collapsed;
				Dg1.Columns[Dg1.Columns.Count - 4].Visibility = Visibility.Collapsed;
			}
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

		private void OpenVaidDate()
		{
			var win = new P1801010100(Vm.selectDgQueryData);
			win.Owner = this.Parent as Window;
			win.ShowDialog();
			Vm.SearchCommand.Execute(null);
		}
	}
}