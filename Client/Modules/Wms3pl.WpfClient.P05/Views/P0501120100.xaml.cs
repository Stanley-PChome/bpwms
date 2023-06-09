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
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P05.Views
{
	/// <summary>
	/// P0501120100.xaml 的互動邏輯
	/// </summary>
	public partial class P0501120100 : Wms3plWindow
	{
		public P0501120100(P050112Batch p050112Batch)
		{
			InitializeComponent();
			Vm.Master = p050112Batch;
			Vm.SearchCommand.Execute(null);
		}
		protected void Station_SelectionChanged(object sender,EventArgs e)
		{
			var cmbox = sender as ComboBox;
			var batchPickStation = cmbox.DataContext as BatchPickStation;
			batchPickStation.STATION_NO = (cmbox.SelectedItem as NameValuePair<string>).Value;
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
