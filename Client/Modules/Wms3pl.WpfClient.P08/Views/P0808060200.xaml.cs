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
	/// P0808060200.xaml 的互動邏輯
	/// </summary>
	public partial class P0808060200 : Wms3plWindow
	{
		public P0808060200(string dcCode, BindBoxType bindBoxType, OutContainerInfo orgContainerInfo, BindingPickContainerInfo currentPickContainterInfo)
		{
			InitializeComponent();
			Vm.DcCode = dcCode;
			Vm.CurrentBindBoxType = bindBoxType;
			Vm.OriContainerInfo = orgContainerInfo;
			Vm.CurrentPickContainterInfo = currentPickContainterInfo;
			Vm.DoClose = Close;
			Vm.DoContainerCodeFocus = ContainerCodeFocus;
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			Vm.ExecRebindContainerCommand.Execute(null);
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			Vm.IsOk = false;
			Close();
		}

		private void ContainerCodeFocus()
		{
			SetFocusedElement(TxtContainerCode, true);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			DispatcherAction(() =>
			{
				ContainerCodeFocus();
			});
		}

		private void TxtContainerCode_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			if (string.IsNullOrWhiteSpace(Vm.ContainerCode))
			{
				DialogService.ShowMessage(Vm.Title, "警告", DialogButton.OK, DialogImage.Warning);
				SetFocusedElement(TxtContainerCode, true);
				return;
			}
			Vm.ExecRebindContainerCommand.Execute(null);
		}
	}
}
