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
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0803030000.xaml 的互動邏輯
	/// </summary>
	public partial class P0803030000 : Wms3plWindow
	{
		private DispatcherTimer timer;

		public P0803030000()
		{
			InitializeComponent();
			Vm.IsDifferentWareHouse = true;
			Vm.ExitClick += Exit_OnClick;
			Vm.SetDefaultFocusClick += SetDefaultFocusClick;
			Vm.SetCaseNoFocusClick += SetCaseNoFocusClick;
			Vm.SetAllocationNoFocusClick += SetAllocationNoFocusClick;
			Vm.OpenTarClick += OpenTarClick;
			timer = new DispatcherTimer();
			timer.Tick += new EventHandler(TimerOnTick);
			timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
			Action action = () =>
			{
				Vm.InitBindData();
				SetCaseNoFocusClick();
			};
			Vm.View = this;
			ControlView(action);
		}
		private void SetAllocationNoFocusClick()
		{
			SetFocusedElement(TxtAllocationNo);
		}

		private void SetDefaultFocusClick()
		{
			SetFocusedElement(TxtScanItemCode);
		}

		private void SetCaseNoFocusClick()
		{
			SetFocusedElement(TxtCaseNo);
		}


		private void TimerOnTick(object sender, EventArgs eventArgs)
		{

			if (MyTranslateTransform.X >= -BlockMarqueeMessage.ActualWidth)
				MyTranslateTransform.X -= 10;
			else
				MyTranslateTransform.X = ActualWidth;
		}

		private void P0800303000_OnClosed(object sender, EventArgs e)
		{
			timer.Stop();
		}

		private void P0800303000_OnLoaded(object sender, RoutedEventArgs e)
		{
			MyTranslateTransform.X = ActualWidth;
			timer.Start();
		}

		private void Exit_OnClick()
		{
			Dispatcher.Invoke(new Action(Close));
		}

		private void TxtScanItemCode_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			Vm.ScanItemCodeAddActualQty();
		}

		private void TxtActualSrcQty_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			Vm.AdjustActualQty();

		}
		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			var dep = (DependencyObject)e.OriginalSource;
			while ((dep != null) &&
					!(dep is DataGridRow))
			{
				dep = VisualTreeHelper.GetParent(dep);
			}
			if (dep == null) return;

			DataGridRow row = dep as DataGridRow;
			var f151002ItemLocData = row.Item as F151002ItemLocData;
			Vm.ClearLocItemActualQty(f151002ItemLocData);
			SetDefaultFocusClick();
		}

		private void TxtAllocationNo_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			Vm.SearchCommand.Execute(null);			
		}

		private void TxtCaseNo_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			Vm.ScanCaseNo();
		}

		private void OpenTarClick()
		{
			var win = new P0803040000(Vm.tempAllocationNo)
			{
				Owner = System.Windows.Window.GetWindow(this),
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
			};
			win.ShowDialog();
			Vm.GetNextAllocation();

		}
	}
}
