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

namespace Wms3pl.WpfClient.P19.Views
{
	/// <summary>
	/// P1920210000.xaml 的互動邏輯
	/// </summary>
	public partial class P1920210000 : Wms3plUserControl
	{
		public P1920210000()
		{
			InitializeComponent();
			Vm.AddAction += dgScrollIntoView;
		}

		private void dgScrollIntoView()
		{
			if (dgGrouList.SelectedItem != null)
				dgGrouList.ScrollIntoView(dgGrouList.SelectedItem);
		}

		private void TextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key < Key.D0 || e.Key > Key.D9)
			{
				if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
				{
					if (e.Key != Key.Back || e.Key != Key.Tab)
					{
						e.Handled = true;
					}
				}
			}
			if (e.Key == Key.Decimal || e.Key == Key.Add || e.Key == Key.Subtract || e.Key == Key.Multiply || e.Key == Key.Divide)
			{
				e.Handled = false;
			}
		}
	}
}
