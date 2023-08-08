using System;
using System.Collections.Generic;
using System.Configuration;
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
using Microsoft.Win32;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P15.Views
{
	/// <summary>
	/// P1502010300.xaml 的互動邏輯
	/// </summary>
	public partial class P1502010300 : Wms3plWindow
	{
		public P1502010300(string dcCode,string itemCode,string changeStatus)
		{
			InitializeComponent();
			Vm.ClosedWindow += Close;
			Vm.OpenFileDialog += OpenFileDialog;
			Vm.ScrollIntoView += ScrollIntoView;
			Vm.SetDefaultFocus += SetDefaultFocus;
			Vm.DcCode = dcCode;
			Vm.ItemCode = itemCode;
			Vm.ChangeStatus = changeStatus;
			Vm.Init();
			SetDefaultFocus();
		}

		private void OpenFileDialog()
		{
			var dlg = new OpenFileDialog {DefaultExt = ".csv", Filter = Properties.Resources.P1502010300xamlcs_ItemSerialNoCSV};
			if (dlg.ShowDialog() ?? false)
			{
				try
				{
					Vm.ImportSerialList = System.IO.File.ReadAllLines(dlg.FileName).ToList();
				}
				catch (Exception ex)
				{
					var errorMsg = ErrorHandleHelper.GetCustomErrorCodeDescription(ex, Properties.Resources.P1502010000_ImportFail, true);
					Vm.ShowWarningMessage(errorMsg);
				}
			}
		}

		private void SetDefaultFocus()
		{
			SetFocusedElement(txtScanSerialNo);
		}
		private void ScrollIntoView()
		{
			if (DgSerialNoResult.Items.Count > 0)
			{
				// 有資料時才做Focus動作
				DgSerialNoResult.ScrollIntoView(DgSerialNoResult.Items[DgSerialNoResult.Items.Count-1]);
				DgSerialNoResult.Focus();
			}
		}

		private void ScanSerialNo_OnKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key != Key.Enter)
				return;
	     Vm.ScanSerialNoCommand.Execute(null);
		}
	}
}
