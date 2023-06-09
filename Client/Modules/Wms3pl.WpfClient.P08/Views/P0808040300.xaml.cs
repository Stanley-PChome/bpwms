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
using Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.P08.ViewModel;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0808040300.xaml 的互動邏輯
	/// </summary>
	public partial class P0808040300 : Wms3plWindow
	{
		public P0808040300(string dcCode, ContainerPickInfo currentContainerPickInfo, BindBoxType bindBoxType, bool isAddBox = false, string orgBoxNo = null)
		{
			InitializeComponent();
			Vm.DcCode = dcCode;
			Vm.CurrentContainerPickInfo = currentContainerPickInfo;
			Vm.OrgBoxNo = orgBoxNo;
			Vm.IsAddBox = isAddBox;
			Vm.CurrentBindBoxType = bindBoxType;
			Vm.Close = Close;
			Vm.BoxFocus = BoxFocus;
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			if(string.IsNullOrWhiteSpace(Vm.BoxNo))
			{
				DialogService.ShowMessage(Vm.Title, "警告", DialogButton.OK, DialogImage.Warning);
				SetFocusedElement(TxtBoxNo, true);
				return;
			}
			Vm.ExecBindBoxCommand.Execute(null);
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			Vm.IsOk = false;
			Close();
		}
		private void BoxFocus()
		{
			SetFocusedElement(TxtBoxNo, true);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			DispatcherAction(() =>
			{
				BoxFocus();
			});
		}

		private void TxtBoxNo_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			if (string.IsNullOrWhiteSpace(Vm.BoxNo))
			{
				DialogService.ShowMessage(Vm.Title, "警告", DialogButton.OK, DialogImage.Warning);
				SetFocusedElement(TxtBoxNo, true);
				return;
			}
			Vm.ExecBindBoxCommand.Execute(null);
		}
	}
}
