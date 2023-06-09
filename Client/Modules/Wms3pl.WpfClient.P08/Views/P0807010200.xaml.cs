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
	/// P0807010200.xaml 的互動邏輯
	/// </summary>
	public partial class P0807010200 : Wms3plWindow
	{
		public P0807010200(string dcCode, string gupCode, string custCode)
		{
			InitializeComponent();
			Vm.OnUnlockComplete += OnUnlock;
			Vm.SelectedDc = dcCode;
			Vm.SelectedCust = custCode;
			Vm.SelectedGup = gupCode;
			SetFocusedElement(txtAdminId);
		}
		private void OnUnlock() {
			this.Close();
		}
		private void Wms3plWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!Vm.unlocked)
			{
				e.Cancel = true;
				SetFocusedElement(txtAdminId);
			}
		}

	}
}
