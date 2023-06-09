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
using Wms3pl.WpfClient.P19.Views;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P02.Views
{
	/// <summary>
	/// P0202030101.xaml 的互動邏輯
	/// </summary>
	public partial class P0202030101 : Wms3plWindow
	{
		public P0202030101()
		{
			InitializeComponent();
			var p1901030000 =  new P1901030000();
			this.Content = p1901030000;
			p1901030000.Function = Function;

		}
	}
}
