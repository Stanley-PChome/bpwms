using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Wms3pl.WpfClient.ExDataServices.P18ExDataService;
using Wms3pl.WpfClient.P18.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P18.Views
{
	/// <summary>
	/// P1801010100.xaml 的互動邏輯
	/// </summary>
	public partial class P1801010100 : Wms3plWindow
	{
		public P1801010100(StockQueryData1 tmpData)
		{
			InitializeComponent();
			Vm.TmpDgQueryData = tmpData;
			Vm.NewValidDate = tmpData.VALID_DATE;
			Vm.NewMakeNo = tmpData.MAKE_NO;
			Vm.CloseAction += Close_Executed;
            Vm.NewQTY = tmpData.QTY;
		}
		private void ExitCommand_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var dr = DialogService.ShowMessage(Properties.Resources.SureWantToleave, Properties.Resources.Message, UILib.Services.DialogButton.YesNo, UILib.Services.DialogImage.Warning);
			if (dr == UILib.Services.DialogResponse.No) return;
			this.Close();
		}
		private void Close_Executed()
		{
			this.Close();
		}

        private void txtNewQTY_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("^[1-9]?[0-9]*$");
            var txt = ((TextBox)sender).Text + e.Text;

            e.Handled = !re.IsMatch(txt);
        }
    }
}
