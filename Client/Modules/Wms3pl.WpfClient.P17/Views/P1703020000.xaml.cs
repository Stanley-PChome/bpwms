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

namespace Wms3pl.WpfClient.P17.Views
{
	/// <summary>
	/// P1703020000.xaml 的互動邏輯
	/// </summary>
	public partial class P1703020000 : Wms3plUserControl
	{
		public P1703020000()
		{
			InitializeComponent();
			this.DataContext = new P71.ViewModel.P7101070000_ViewModel(true);
		}
	}
}
