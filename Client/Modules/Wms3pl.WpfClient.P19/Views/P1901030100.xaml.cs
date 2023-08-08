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
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P19.Views
{
	/// <summary>
	/// P1901030100.xaml 的互動邏輯
	/// </summary>
	public partial class P1901030100 : Wms3plWindow
	{
		public P1901030100()
		{
			InitializeComponent();
			Vm.DoExit += DoExitWin;
		}

		private void DoExitWin()
		{
			this.DialogResult = true;
			this.Close();
		}

		public F1905Data SelectData { get { return Vm.SelectItem; } }

		private void ExitCommand_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//if (this.DialogResult == null)
			//{
			//var dr = DialogService.ShowMessage(Properties.Resources.P1901020100xamlcs_YesToLeave, Properties.Resources.P1901020100xamlcs_Message, UILib.Services.DialogButton.YesNo, UILib.Services.DialogImage.Warning);
			//	if (dr == UILib.Services.DialogResponse.No)
			//		e.Cancel = true;
			//}
		}
	}
}
