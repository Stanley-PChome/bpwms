using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClients.SharedViews.Views;

namespace Wms3pl.WpfClient.P16.Views
{
	public partial class P1602030000 : Wms3plWindow
	{
		public P1602030000()
		{
			InitializeComponent();
			Vm.OnSearchComplete += FocusAfter;
			Vm.OpenDeviceWindow += OpenDeviceWindow;
      Vm.DoFocustxtMaxSheetNum += () => { SetFocusedElement(txtMaxSheetNum, true); };
    }
		
		private void Leave_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void TxtLittleWhiteLableNo_KeyDown(object sender, KeyEventArgs e)
		{
			//TextBox obj = sender as TextBox;
			if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(TxtWmsOrdNo.Text))
			{
				Vm.SearchCommand.Execute(null);
				SetFocusedElement(TxtWmsOrdNo, true);
			}
			
		}

		// 補印廠退明細
		private void PrintReturnDetail_Click(object sender, RoutedEventArgs e)
		{
			Vm.PrintReturnDetail(true);
		}

		//補印宅配單
		private void PrintHomeDeliveryOrder_Click(object sender,RoutedEventArgs e)
		{
			Vm.PrintHomeDeliveryOrder(true);
		}

		private void Window_OnLoaded(object sender, RoutedEventArgs e)
		{
			SetFocusedElement(TxtWmsOrdNo);
			var proxy = ConfigurationHelper.GetProxy<F00Entities>(false, Vm.FunctionCode);
			Vm.F0003 = proxy.F0003s.Where(x => x.AP_NAME == "VideoCombinVnrRet" && x.DC_CODE == Vm.SelectedDc).FirstOrDefault();
			if (Vm.F0003?.SYS_PATH == "1")
				OpenDeviceWindow();
		}

		private void FocusAfter()
		{
			SetFocusedElement(TxtWmsOrdNo,true);

		}

		private void OpenDeviceWindow()
		{
			var checkF9105501 = true;
			do
			{
				var deviceSerivce = new DeviceWindowService();
				Vm.F910501 = deviceSerivce.GetDeviceSettings(Vm.FunctionCode, Wms3plSession.Get<GlobalInfo>().ClientIp, Vm.SelectedDc).FirstOrDefault();
				
				if (Vm.F910501?.WORKSTATION_GROUP == "F" && !string.IsNullOrWhiteSpace(Vm.F910501?.WORKSTATION_CODE))
				{
					checkF9105501 = false;
				}
				else
				{
					Vm.ShowErrorWorkstationCode();
					var deviceWindow = new DeviceWindow(Vm.SelectedDc);
					deviceWindow.Owner = this;
					deviceWindow.ShowDialog();
					Vm.F910501 = deviceWindow.SelectedF910501;

					continue;
				}

			} while (checkF9105501);
		}

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      Regex re = new Regex("^[1-9]?[0-9]*$");
      var txt = ((TextBox)sender).Text + e.Text;

      e.Handled = !re.IsMatch(txt);
    }

  }
}