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
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.LabelPrinter.Bartender;
using Wms3pl.WpfClient.P05.ViewModel;
using Wms3pl.WpfClient.P08.Report;
using Wms3pl.WpfClient.P08.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.Services;

using CrystalDecisions.CrystalReports.Engine;
using System.Collections;


namespace Wms3pl.WpfClient.P05.Views
{
	/// <summary>
	/// P0503010000.xaml 的互動邏輯
	/// </summary>
	public partial class P0503010000 : Wms3plUserControl
	{
		public P0503010000()
		{
			InitializeComponent();
		}

		private void DgWmsOrdNoList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var win = new P0503020100(Vm.SelectWMSData.GUP_CODE, Vm.SelectWMSData.CUST_CODE, Vm.SelectWMSData.DC_CODE, Vm.SelectWMSData.WMS_ORD_NO);
			win.ShowDialog();
		}		
	}
}
