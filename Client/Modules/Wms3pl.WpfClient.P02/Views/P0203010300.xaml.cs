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

namespace Wms3pl.WpfClient.P02.Views
{
	/// <summary>
	/// P0203010201.xaml 的互動邏輯
	/// </summary>
	public partial class P0203010300 : Wms3plWindow
	{
		public P0203010300()
		{
			InitializeComponent();
			Vm.ExitClick += ClosedClick;
			Vm.SetDefaultFocus += SetDefaultFocus;
			SetDefaultFocus();
		}
		private void SetDefaultFocus()
		{
			SetFocusedElement(txtItemCode);
		}
		private void ClosedClick()
		{
			Dispatcher.Invoke(new Action(() =>
			{
				DialogResult = Vm.IsSaveOk;
				Close();
			}));
		}

		private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Vm.SaveCommand.Execute(null);
			}
		}
	}
}
