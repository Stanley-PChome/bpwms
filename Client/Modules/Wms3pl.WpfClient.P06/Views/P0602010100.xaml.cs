using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.ExDataServices.P06ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P06.Views
{
	/// <summary>
	/// Interaction logic for P0602010100.xaml
	/// </summary>
	public partial class P0602010100 : Wms3plWindow
	{
		public P0602010100()
		{
			InitializeComponent();
			Vm.OnSave += DoSave;
		}

		public int LackQty { get; set; }

		public P0602010100(F051206LackList data)
				: this()
		{
			Vm.Data = data;
			Vm.LackQty = data.LACK_QTY;
		}


		private void ExitCommand_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var dr = DialogService.ShowMessage(Properties.Resources.ConfirmExit, Properties.Resources.Message, UILib.Services.DialogButton.YesNo, UILib.Services.DialogImage.Warning);
			if (dr == UILib.Services.DialogResponse.No) return;
			this.Close();
		}

		private void DoSave()
		{
			LackQty = Vm.LackQty.Value;
			this.Close();
		}
	}
}
