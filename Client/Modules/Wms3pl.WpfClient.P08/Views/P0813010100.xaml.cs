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
	/// P0813010100.xaml 的互動邏輯
	/// </summary>
	public partial class P0813010100 : Wms3plWindow
	{
		public P0813010100(string dcCode,bool isMoveLoc,string moveLoc,string itemCode)
		{
			InitializeComponent();
			Vm.DcCode = dcCode;
			Vm.MoveLocCode = moveLoc;
			Vm.IsMoveLoc = isMoveLoc;
			Vm.MoveItemCode = itemCode;
			Vm.SetTxtScanUpLocCodeFocus += SetTxtScanUpLocCodeFocus;
			Vm.SetBtnMoveComplete += SetBtnMoveComplete;
			Vm.SearchCommand.Execute(null);
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			SetTxtScanUpLocCodeFocus();
		}
		private void SetTxtScanUpLocCodeFocus()
		{
			SetFocusedElement(TxtScanUpLocCode,true);
		}

		private void SetBtnMoveComplete()
		{
			SetFocusedElement(BtnMoveComplete);
		}

		private void TxtScanUpLocCode_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			Vm.CheckTarLocCommand.Execute(null);
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			Close();
		}
	}
}
