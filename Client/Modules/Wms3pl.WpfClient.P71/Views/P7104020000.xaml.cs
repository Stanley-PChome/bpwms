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

namespace Wms3pl.WpfClient.P71.Views
{
	/// <summary>
	/// P7104020000.xaml 的互動邏輯
	/// </summary>
	public partial class P7104020000 : Wms3plWindow
	{
		public P7104020000()
		{
			InitializeComponent();
			Vm.Ini();
		}
		private void P7104020000_OnClosed(object sender, EventArgs e)
		{
			Vm.BgWorkerByAll.CancelAsync();
			Vm.BgWorkerByAll.Dispose();

			Vm.TimerCounterByAll.Stop();
			Vm.TimerCounterByAll.Close();
			Vm.TimerCounterByAll.Dispose();
		}
	}
}