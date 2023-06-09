using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Wms3pl.WpfClient.P02.ViewModel;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P02.Views
{
	/// <summary>
	/// Interaction logic for P0201010000.xaml
	/// </summary>
	public partial class P0201010000 : Wms3plUserControl
	{
		public P0201010000()
		{
			InitializeComponent();
			SetFocusedElement(cboDc);
			Vm.OnAddFocus += OnAddFocus;
			Vm.OnEditFocus += OnEditFocus;
			Vm.OnSearchVnrCodeForAdd += txtAddVendor.SearchResultCode;
		}

		private void OnAddFocus()
		{
			SetFocusedElement(cboEditDc);
		}

		private void OnEditFocus()
		{
			SetFocusedElement(cboEditPier);
		}

		private void cboEditDc_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Vm.SetPierListByAddMode();
		}

		private void dpEditArriveDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			Vm.SetPierListByAddMode();
		}

		private void btnSearchVendor_Click(object sender, RoutedEventArgs e)
		{
			var win = new WinSearchVendor();
			win.GupCode = Wms3plSession.Get<GlobalInfo>().GupCode; ;
			win.Owner = this.Parent as Window;
			var result = win.ShowDialog();
			if (result.HasValue && result.Value)
			{
				txtSearchVendor.F1908Data = win.SelectedItem;
			}
		}



	}
}
