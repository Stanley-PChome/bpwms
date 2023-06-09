using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P05.Views
{
	/// <summary>
	/// P0503020200.xaml 的互動邏輯
	/// </summary>
	public partial class P0503020200 : Wms3plWindow
	{

		public P0503020200()
		{
			InitializeComponent();
		}

		public P0503020200(string gupCode, string custCode, string dcCode, string ordNo, List<F05010101> xlsData)
		{
			InitializeComponent();
			Vm.ExitClick += Exit_OnClick;
			Vm.GUP_CODE = gupCode;
			Vm.CUST_CODE = custCode;
			Vm.DC_CODE = dcCode;
			Vm.ORD_NO = ordNo;
			Vm.XLS_DATA=xlsData;
			Vm.SearchCommand.Execute(null);
		}

		private void Exit_OnClick()
		{
			Close();
		}

	}
}
