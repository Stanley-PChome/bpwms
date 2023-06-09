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
using Wms3pl.WpfClient.P71.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.Views
{
	/// <summary>
	/// P7109010000.xaml 的互動邏輯
	/// </summary>
	public partial class P7109010000 : Wms3plUserControl
	{
		public P7109010000()
		{
			InitializeComponent();
			Vm.OpenSearchWin += DoOpenSearch;
			Vm.F194701DgScrollIntoView += F194701DgScrollIntoView;
			Vm.F194708DgScrollIntoView += F194708DgScrollIntoView;
			Vm.F194709DgScrollIntoView += F194709DgScrollIntoView;
			this.F194701Grid.IsReadOnly = true;
		}

		private void DoOpenSearch()
		{
			var win = new P7109010100();
			win.Owner = this.Parent as Window;
			win.ShowDialog();
			if (win.DialogResult.HasValue && win.DialogResult.Value)
			{
				Vm.SetSearchResult(win.SelectedData);
			}
		}

		private void DcCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Vm.DcCodeChanged();
		}

		private void GupCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//Vm.GupCodeChanged();
		}

		private void btnP710902_Click(object sender, RoutedEventArgs e)
		{
			var function = FormService.GetFunctionFromSession("P7109020000");
			if (function == null)
			{
				DialogService.ShowMessage(Properties.Resources.P7109010000xamlcs_NO_AUTHENTICATION);
				return;
			}
			var win = new P7109020000()
			{
				Function = function
			};

			win.Show();
			win.UpdateAllItemSource(Vm.CurrentMaster == null ? string.Empty : Vm.CurrentMaster.DC_CODE);
			e.Handled = true;
		}

		private void F1934EX2_ALLCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			Vm.F1934EX2_AllSelect(true);
		}

		private void F1934EX2_ALLCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			Vm.F1934EX2_AllSelect(false);
		}

		private void F194701DgScrollIntoView()
		{
			if (F194701Grid.SelectedItem == null) return;
			// Focus到所選的項目
			FocusManager.SetFocusedElement(this, F194701Grid);
			F194701Grid.ScrollIntoView(F194701Grid.SelectedItem);
		}
		private void F194708DgScrollIntoView()
		{
			if (F194708Grid.SelectedItem == null) return;
			// Focus到所選的項目
			FocusManager.SetFocusedElement(this, F194708Grid);
			F194708Grid.ScrollIntoView(F194708Grid.SelectedItem);
		}

		private void F194709DgScrollIntoView()
		{
			if (F194709Grid.SelectedItem == null) return;
			// Focus到所選的項目
			FocusManager.SetFocusedElement(this, F194709Grid);
			F194709Grid.ScrollIntoView(F194709Grid.SelectedItem);
		}
	}
}
