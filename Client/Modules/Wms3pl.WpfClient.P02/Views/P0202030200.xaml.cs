using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Wms3pl.WpfClient.UILib;
using System;
using System.Windows.Controls;

namespace Wms3pl.WpfClient.P02.Views
{
	public partial class P0202030200 : Wms3plWindow
	{
		public P0202030200(string dcCode, string purchaseNo ,string rtNo)
		{
			InitializeComponent();
			Vm.AfterCheckRtNo += OpenFileUploadWindow;
			Vm.SelectedDc = dcCode;
			Vm.PurchaseNo = purchaseNo;
			Vm.RtNo = rtNo;
			Vm.DoSearchRtNoList();
		}

		private void OpenFileUploadWindow()
		{
			var win = new P0202030300(Vm.SelectedDc, Vm.PurchaseNo, Vm.RtNo);
			win.ShowDialog();
			this.Close();
		}

		//private void txtRtNo_KeyDown(object sender, KeyEventArgs e)
		//{
		//    if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(txtRtNo.Text))
		//        Vm.SearchCommand.Execute(null);
		//}
	}
}
