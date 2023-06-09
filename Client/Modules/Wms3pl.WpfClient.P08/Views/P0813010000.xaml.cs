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
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0813010000.xaml 的互動邏輯
	/// </summary>
	public partial class P0813010000 : Wms3plWindow
	{
		public P0813010000()
		{
			InitializeComponent();
			Vm.SetScanItemLocCodeFocus += SetScanItemLocCodeFocus;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			SetScanItemLocCodeFocus();
		}
		private void SetScanItemLocCodeFocus()
		{
			SetFocusedElement(TxtScanItemOrLocCode, true);
		}
		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void Loc_OpenToMoveClick(object sender, RoutedEventArgs e)
		{
			var btn = sender as Button;
			var isMoveLoc = LocCodeHelper.LocCodeConverter9(btn.Content.ToString()) == LocCodeHelper.LocCodeConverter9(Vm.TempScanItemOrLocCode);

			var win = new P0813010100(Vm.TempSelectedDc, isMoveLoc, LocCodeHelper.LocCodeConverter9(btn.Content.ToString()),isMoveLoc ? "" : Vm.SelectedItem.ITEM_CODE);
			if(win.ShowDialog() ?? true)
			{
				Vm.SearchCommand.Execute(null);
			}
		}

		private void ScanItemOrLocCode_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			if (string.IsNullOrWhiteSpace(Vm.ScanItemOrLocCode))
				return;
			Vm.SearchCommand.Execute(null);
		}
	}
}
