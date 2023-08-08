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
	/// P7110010000.xaml 的互動邏輯
	/// </summary>
	public partial class P7110010000 : Wms3plUserControl
	{
		public P7110010000()
		{
			InitializeComponent();
			Vm.OnAddFocus += OnAddFocus;
			Vm.OnEditFocus += OnEditFocus;

		}

		private void OnEditFocus()
		{
			SetFocusedElement(txtTicketName);
		}

		private void OnAddFocus()
		{
			SetFocusedElement(txtTicketName);
		}

		private void SHIPPING_ASSIGN_Checked(object sender, RoutedEventArgs e)
		{
			Vm.SetCanSelectedF1947();
		}

		private void SHIPPING_ASSIGN_UnChecked(object sender, RoutedEventArgs e)
		{
			Vm.SetCanSelectedF1947();
		}
	}
}