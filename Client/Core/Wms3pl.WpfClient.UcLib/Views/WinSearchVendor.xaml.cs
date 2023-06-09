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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.UcLib.Views
{
	/// <summary>
	/// WinSearchVendor.xaml 的互動邏輯
	/// </summary>
	public partial class WinSearchVendor : Wms3plWindow
	{
		public WinSearchVendor():base(false)
		{
			InitializeComponent();
			Vm.DoExit += DoExitWin;
		}

		private void DoExitWin()
		{
			this.DialogResult = true;
			this.Close();
		}

		public F1908 SelectedItem { get { return Vm.SelectedItem; } }

		public string GupCode
		{
			get
			{
				return Vm.GupCode;
			}
			set
			{
				Vm.GupCode = value;
			}
		}

		private void ExitCommand_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
