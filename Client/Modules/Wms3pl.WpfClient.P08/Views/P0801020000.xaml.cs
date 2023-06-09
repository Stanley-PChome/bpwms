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

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0801020000.xaml 的互動邏輯
	/// </summary>
	public partial class P0801020000 : Wms3plWindow
	{
		public P0801020000()
		{
			InitializeComponent();
		}

		private void BtnExit_OnClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
