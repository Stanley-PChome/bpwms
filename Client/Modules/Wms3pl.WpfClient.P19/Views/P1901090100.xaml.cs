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
using Wms3pl.WpfClient.DataServices.F19DataService;

namespace Wms3pl.WpfClient.P19.Views
{
	/// <summary>
	/// P1901090100.xaml 的互動邏輯
	/// </summary>
	public partial class P1901090100 : Wms3plWindow
	{
		public P1901090100()
		{
			InitializeComponent();
			Vm.DoExit += DoExitWin;
		}

		public F1908 SelectData { get { return Vm.SelectedData; } }

		private void ExitCommand_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var dr = DialogService.ShowMessage(Properties.Resources.P1901020100xamlcs_YesToLeave, Properties.Resources.P1901020100xamlcs_YesToLeave, UILib.Services.DialogButton.YesNo, UILib.Services.DialogImage.Warning);
			if (dr == UILib.Services.DialogResponse.No)
				e.Cancel = true;
		}

		private void DoExitWin()
		{
			this.DialogResult = true;
			this.Close();
		}

		private void txtVRN_CODE_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				txtVRN_NAME.Focus();
			}
		}

		private void txtVRN_NAME_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Vm.SearchCommand.Execute(null);
			}
		}
	}
}
