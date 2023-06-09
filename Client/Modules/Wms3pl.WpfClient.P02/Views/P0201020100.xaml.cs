using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P02.Views
{
	/// <summary>
	/// Interaction logic for P0201020100.xaml
	/// </summary>
	public partial class P0201020100 : Wms3plWindow
	{
		public P0201020100()
		{
			InitializeComponent();
			Vm.OnSave += DoSave;

			if (Vm.UserOperateMode == OperateMode.Add)
			{
				SetFocusedElement(InDatePicker);
			}
			else
			{
				SetFocusedElement(PierComboBox);
			}
		}

        public P0201020100(F020103 data, string dc, DateTime date, bool isNewData = false)
            : this()
        {
            Vm.SelectedPurchase = data;
            Vm.SelectedDc = dc;
            Vm.SelectedDate = date;
            Vm.UserOperateMode = isNewData ? OperateMode.Add : OperateMode.Edit;
            Vm.DoSearchPiers();
        }

		private void txtPurchaseNo_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			TextBox obj = (TextBox)sender;
			if (!string.IsNullOrEmpty(obj.Text.Trim())) Vm.DoSearchVendorInfo();
			else
			{
				Vm.SelectedPurchase.VNR_CODE = string.Empty;
			}
		}

		private void ExitCommand_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var dr = DialogService.ShowMessage(Properties.Resources.P0201020100_ConfirmExit, Properties.Resources.Message, UILib.Services.DialogButton.YesNo, UILib.Services.DialogImage.Warning);
			if (dr == UILib.Services.DialogResponse.No) return;
			this.Close();
		}

        private void DoSave()
        {
            this.Close();
        }
	}
}
