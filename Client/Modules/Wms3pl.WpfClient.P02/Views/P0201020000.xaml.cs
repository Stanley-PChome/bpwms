using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.P02.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P02.Views
{
	/// <summary>
	/// Interaction logic for P0201020000.xaml
	/// </summary>
	public partial class P0201020000 : Wms3plUserControl
	{
		public P0201020000()
		{
			InitializeComponent();
			SetFocusedElement(DcComboBox);
		}

		private void EditCommand_OnClick(object sender, RoutedEventArgs e)
		{
			Vm.DoEdit();
			var win = new P0201020100(Vm.SelectedPurchaseCopy, Vm.SelectedDc, Vm.SelectedDate);
			win.ShowDialog();
			Vm.DoSearch();
		}

		private void AddCommand_Click(object sender, RoutedEventArgs e)
		{
			Vm.DoAdd();
            var win = new P0201020100(new F020103() { ARRIVE_DATE = Vm.SelectedDate }, Vm.SelectedDc, Vm.SelectedDate, true);
			win.ShowDialog();
			Vm.DoSearch();
		}
	}
}
