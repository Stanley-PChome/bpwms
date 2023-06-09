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
using Wms3pl.WpfClient.P08.ViewModel;
using Wms3pl.WpfClient.UILib;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0810010000.xaml 的互動邏輯
	/// </summary>
	public partial class P0810010000 : Wms3plWindow
	{
		private DispatcherTimer timer;

		public P0810010000()
		{
			InitializeComponent();
			Vm.ExitClick += Exit_OnClick;
			Vm.InventoryNoFocus += SetInventoryNoFocus;
			Vm.ScanLocCodeFocus += SetScanLocCodeFocus;
			Vm.ScanItemCodeOrSerialNoFocus += SetScanItemCodeOrSerialNoFocus;
			Vm.ScanQtyFocus += SetScanQtyFocus;
			timer = new DispatcherTimer();
			timer.Tick += new EventHandler(TimerOnTick);
			timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
			Action action = () =>
			{
				Vm.InitBindData();
				SetScanLocCodeFocus();
			};
			Vm.View = this;
			ControlView(action);
		}

		private void P0810010000_OnLoaded(object sender, RoutedEventArgs e)
		{
			//MyTranslateTransform.X = ActualWidth;
			timer.Start();
		}

		private void P0810010000_OnClosed(object sender, EventArgs e)
		{
			timer.Stop();
		}

		private void TimerOnTick(object sender, EventArgs eventArgs)
		{

			//if (MyTranslateTransform.X >= -BlockMarqueeMessage.ActualWidth)
			//	MyTranslateTransform.X -= 10;
			//else
			//	MyTranslateTransform.X = ActualWidth;
		}

		private void Exit_OnClick()
		{
			Dispatcher.Invoke(new Action(Close));
		}

		private void SetInventoryNoFocus()
		{
			SetFocusedElement(TxtInventoryNo);
		}

		private void SetScanLocCodeFocus()
		{
			SetFocusedElement(TxtScanLocCode);
		}
		private void SetScanItemCodeOrSerialNoFocus()
		{
			SetFocusedElement(TxtScanItemCodeOrSerialNo);
		}

		private void SetScanQtyFocus()
		{
			SetFocusedElement(TxtScanQty);
		}

		private void InventoryNo_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter && e.Key != Key.Tab)
				return;
			if (!string.IsNullOrWhiteSpace(Vm.InventoryNo))
				Vm.SearchCommand.Execute(null);
			else
			{
				var message = new MessagesStruct
				{
					Button = DialogButton.OK,
					Image = DialogImage.Information,
					Message = Properties.Resources.P0810010000_InventoryNoIsNull,
					Title = WpfClient.Resources.Resources.Information
				};
				Vm.ShowMessage(message);
			}
		}

		private void ScanLocCode_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter && e.Key != Key.Tab)
				return;
			Vm.DoScanLocCode();
		}

		private void ScanItemCodeOrSerialNo_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter && e.Key != Key.Tab)
				return;
			Vm.DoScanItemCodeOrSerialNo();
		}

		private void ScanQty_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter && e.Key != Key.Tab)
				return;
			Vm.DoUpdateQty();
		}


		private void ClearInventoryQty_OnClick(object sender, RoutedEventArgs e)
		{
			Vm.DoClearInventoryQty();
		}

		private void UnitQty_OnKeyUp(object sender, KeyEventArgs e)
		{
			Vm.ChangeEnterQty();
		}

		private void ScanQty_OnKeyUp(object sender, KeyEventArgs e)
		{
			Vm.ChangeUnitQty();
		}
	}
}
