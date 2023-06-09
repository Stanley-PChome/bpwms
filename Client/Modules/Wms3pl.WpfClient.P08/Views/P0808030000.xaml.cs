using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P08.Views
{
	public partial class P0808030000 : Wms3plWindow
	{
		public P0808030000()
		{
			InitializeComponent();
			Vm.OnSearchPastNoComplete += FocusAfterPastNo;
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void TxtWMS_NO_KeyDown(object sender, KeyEventArgs e)
		{
			TextBox obj = sender as TextBox;
			if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(obj.Text))
			{
				Vm.SearchCommand.Execute(null);
			}
		}

		private void FocusAfterPastNo()
		{
			SetFocusedElement(txtPastNo,true);

		}

		private void Window_OnLoaded(object sender, RoutedEventArgs e)
		{
			SetFocusedElement(txtPastNo);
		}

	}
}