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

namespace Wms3pl.WpfClient.P71.Views
{
	/// <summary>
	/// P7101060000.xaml 的互動邏輯
	/// </summary>
	public partial class P7101060000 : Wms3plUserControl
	{
		public P7101060000()
		{
			InitializeComponent();
		}

		private void txtSelectedLoc_LostFocus(object sender, RoutedEventArgs e)
		{
			TextBox obj = (TextBox)sender;
			if (string.IsNullOrWhiteSpace(obj.Text)) return;
			Vm.DoCheckLocCode();
		}
	}
}
