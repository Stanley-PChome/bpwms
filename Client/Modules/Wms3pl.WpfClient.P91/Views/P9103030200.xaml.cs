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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P91.Views
{
	/// <summary>
	/// P9103030200.xaml 的互動邏輯
	/// </summary>
	public partial class P9103030200 : Wms3plWindow
	{
		public P9103030200(P19.Views.P1901280000 p1901280000)
		{
			InitializeComponent();

			this.Content = p1901280000;
			p1901280000.Function = Function;
			p1901280000.OnSaved += Saved;
			
		}

		private void Saved()
		{
			// 表示確認有儲存
			this.DialogResult = true;
			this.Close();
		}
	}
}