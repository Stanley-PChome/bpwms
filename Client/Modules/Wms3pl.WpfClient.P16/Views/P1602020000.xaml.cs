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
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.P05.Views;
using Wms3pl.WpfClient.P16.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P16.Views
{
  /// <summary>
  /// P1602020000.xaml 的互動邏輯
  /// </summary>
	public partial class P1602020000 : Wms3plUserControl
	{
		public P1602020000()
		{
			InitializeComponent();
			Vm.SetQueryFocus += SetQueryFocus;
			Vm.SetQueryFocusForAddNew += SetQueryFocusForAddNew;
			Vm.DisableDetailDataGrid += DisableDetailDataGrid;
		}

		private void OrderDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (Vm.SearchResultForSerachSelectedItem != null && OrderDataGrid.SelectedItem != null)
			{
				F050801WmsOrdNo f050801 = (F050801WmsOrdNo) OrderDataGrid.SelectedItem;

				var win = new P0503020100(Wms3plSession.Get<GlobalInfo>().GupCode, Wms3plSession.Get<GlobalInfo>().CustCode,
					Vm.DcCodeForSearch,
					f050801.WMS_ORD_NO)
				{
					Owner = System.Windows.Window.GetWindow(this),
					WindowStartupLocation = WindowStartupLocation.CenterOwner
				};
				win.ShowDialog();
			}
		}

		private void SetQueryFocus()
		{
			SetFocusedElement(DComboBox);
		}

		private void SetQueryFocusForAddNew()
		{
			SetFocusedElement(DComboBoxForAddNew);
		}

		private void VendorTextBox_OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			TextBox txt = sender as TextBox;
			if (!String.IsNullOrEmpty(txt.Text))
			{
				txt.Text = txt.Text.Replace("'", "");
			}
		}

		private void Master_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private void DisableDetailDataGrid()
		{
			F160204DataGrid.Columns.Where(a => a.GetType() == typeof(DataGridCheckBoxColumn)).ToList()
				.ForEach(c => c.Visibility = Visibility.Hidden);
			F160204DataGrid.IsReadOnly = true;
		}
	}
}
