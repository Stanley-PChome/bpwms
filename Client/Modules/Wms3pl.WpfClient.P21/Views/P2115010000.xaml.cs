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
using Wms3pl.WpfClient.P21.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P21.Views
{
	/// <summary>
	/// P2115010000.xaml 的互動邏輯
	/// </summary>
	public partial class P2115010000 : Wms3plUserControl
	{
		public P2115010000()
		{
			InitializeComponent();

		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Button bt = e.OriginalSource as Button;
			string fmid = bt.ToolTip.ToString();
			var formservice = new FormService();
			formservice.AddFunctionForm(fmid);
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			Vm.RefreshScheduleCommand.Execute(null);
		}
	}
}
