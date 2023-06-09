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
using Wms3pl.WpfClient.DataServices.F16DataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P16.Views
{
	/// <summary>
	/// P1601010200.xaml 的互動邏輯
	/// </summary>
	public partial class P1601010200 : Wms3plWindow
	{
		public P1601010200(F161201 _sourceData)
		{
			InitializeComponent();
			Vm.SourceData = _sourceData;
			Vm.DoExit += DoExitWin;
		}

		private void ExitCommand_Click(object sender, RoutedEventArgs e)
		{
			Window.Close();
		}

		private void DoExitWin()
		{
			Window.Close();
		}

		public F161201 SourceData { get; set; }

		public void DoQuery()
		{
			if (Vm.SourceData != null)
				Vm.SearchCommand.Execute(null);
		}
	}
}
