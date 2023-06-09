using Microsoft.Win32;
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
using Wms3pl.WpfClient.P08.ViewModel;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0808020000.xaml 的互動邏輯
	/// </summary>
	public partial class P0808020000 : Wms3plWindow
	{
		public P0808020000()
		{
			InitializeComponent();
			Vm.OnSearchCommon += FocusAfterPastNo;
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void txtPAST_NO_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Vm.SearchCommand.Execute(null);
			}
		}

		private void FocusAfterPastNo()
		{
			SetFocusedElement(txtPAST_NO);
		}

		private void UploadShipCode_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new OpenFileDialog { DefaultExt = ".xls", Filter = "excel files (*.xls,*.xlsx)|*.xls*" };
			if (dlg.ShowDialog() ?? false)
			{
				try
				{
					Vm.UploadDelvCheckCodeFilePath = dlg.FileName;
					Vm.UpLoadDelvCheckCodeCommand.Execute(null);
				}
				catch (Exception ex)
				{
					var errorMsg = ErrorHandleHelper.GetCustomErrorCodeDescription(ex, Properties.Resources.P0808020000_ImportFail, true);
					Vm.ShowWarningMessage(errorMsg);
				}
			}

		}
	}
}
