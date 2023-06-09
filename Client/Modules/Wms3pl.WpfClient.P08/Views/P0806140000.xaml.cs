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
	/// P0806140000.xaml 的互動邏輯
	/// </summary>
	public partial class P0806140000 : Wms3plWindow
	{
		public P0806140000()
		{
			InitializeComponent();
			Vm.TxtWmsNoBarCodeFocus = SetTxtWmsNoBarCodeFocus;
			Vm.TxtContainerCodeFocus = SetTxtContainerCodeFocus;
			Vm.TxtItemBarCodeFocus = SetTxtItemBarCodeFocus;
			Vm.OpenBoxDetail = OpenBoxDetail;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Vm.Init();
			SetTxtWmsNoBarCodeFocus();
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}


		private void OpenBoxDetail()
		{
			var detail = (from o in Vm.PickAllotLayers
										from j in o.PickAllotLocs
										select j).ToList(); ;
			var win = new P0806140100(detail);
			win.ShowDialog();
			if (Vm.CurrentPickAllotMode == ViewModel.PickAllotMode.ScanWmsNoBarCode)
				SetTxtWmsNoBarCodeFocus();
			else
				SetTxtItemBarCodeFocus();
		}

		private void ScanWmsNoBarCode_KeyDown(object sender, KeyEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(Vm.ScanWmsNoBarCode) && e.Key == Key.Enter)
				Vm.SearchOrderCommand.Execute(null);

		}

		private void SetTxtWmsNoBarCodeFocus()
		{
			DispatcherAction(() =>
			{
				SetFocusedElement(TxtWmsNoBarCode, true);
			});
			
		}

		private void SetTxtContainerCodeFocus()
		{
			DispatcherAction(() =>
			{
				SetFocusedElement(TxtContainerCode, true);
			});

		}

		private void SetTxtItemBarCodeFocus()
		{
			DispatcherAction(() =>
			{
				SetFocusedElement(TxtItemBarCode, true);
			});

		}

		public void TxtContainerCode_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				Vm.BindContainerCommand.Execute(null);

		}
		private void TxtItemBarCode_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				Vm.ScanItemBarCodeCommand.Execute(null);

		}
	}
}
