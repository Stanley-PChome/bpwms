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
using System.Windows.Threading;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0802010300.xaml 的互動邏輯
	/// </summary>
	public partial class P0802010300 : Wms3plWindow
	{
		public P0802010300()
		{
			InitializeComponent();
			Vm.ReDataGridFocus += ReFocusDataGridCell;
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void ReFocusDataGridCell()
		{
			ReFocusDataGridCell(dg);
		}

	
	}
}
