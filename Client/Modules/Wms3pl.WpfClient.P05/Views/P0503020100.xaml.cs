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
using Wms3pl.WpfClient.P05.ViewModel;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P05.Views
{
	/// <summary>
	/// P0503020100.xaml 的互動邏輯
	/// </summary>
	public partial class P0503020100 : Wms3plWindow
	{
		public P0503020100(F050801NoShipOrders sourceData=null)
		{
			InitializeComponent();
			Vm.ExitClick += Exit_OnClick;
			//Vm.SourceData = sourceData;
			//Vm.DgItemSource = new List<P0500000000_ViewModel.DgDataClass>()
			//{
			//	new P0500000000_ViewModel.DgDataClass()
			//	{
			//		Str1 = "2014/12/4",
			//		Str2 = "O2104120412345",
			//		Str3 = "已裝車",
			//		Str4 = "1",
			//		Str5 = "123456789",
			//		Str6 = "HTC M8",
			//		Str7 = "3",
			//		Str8 = "",
			//		Str9 = "3",
			//		Str10 = "無",
			//		Str11 = "",
			//		Str12 = "1234567890",
			//		Str13 = "5吋",
			//		Str14 = "16G",
			//		Str15 = "黑",
			//		Str16 = "",
			//		Str17 = "2014120400001",
			//		Str18 = "3",
			//		Str19 = "0",
			//		Str20 = "",
			//		Str21 = "",
			//		Str22 = "11112222",
			//		Bool1 = true
			//	}
			//};

			//Vm.DgItemSource6 = new List<P0500000000_ViewModel.DgDataClass>()
			//{
			//	new P0500000000_ViewModel.DgDataClass()
			//	{
			//		Str1 = "1",
			//		Str2= Properties.Resources.P0503020100_Delv,
			//		Str3=""
			//	},
			//	new P0500000000_ViewModel.DgDataClass()
			//	{
			//		Str1 = "2",
			//		Str2 = Properties.Resources.P0503020100_Receipt,
			//		Str3=""
			//	},
			//	new P0500000000_ViewModel.DgDataClass()
			//	{
			//		Str1 = "3",
			//		Str2 = "保證書",
			//		Str3=""
			//	},
			//	new P0500000000_ViewModel.DgDataClass()
			//	{
			//		Str1 = "4",
			//		Str2 = "Hello Letter",
			//		Str3=""
			//	},
			//	new P0500000000_ViewModel.DgDataClass()
			//	{
			//		Str1 = "5",
			//		Str2 = "SA",
			//		Str3=""
			//	},
			//	new P0500000000_ViewModel.DgDataClass()
			//	{
			//		Str1 = "6",
			//		Str2 = Properties.Resources.P0503020100_PastNo,
			//		Str3="88888888,99999999"
			//	}
			//};


			//Vm.DgItemSource5 = new List<P0500000000_ViewModel.DgDataClass>
			//{
			//	new P0500000000_ViewModel.DgDataClass()
			//	{
			//		Str1 = "123456789012343",
			//		Str2 = "123456",
			//		Str3 = "",
			//		Bool1 = true
			//	},
			//	new P0500000000_ViewModel.DgDataClass()
			//	{
			//		Str1 = "098765432112345",
			//		Str2 = "123457",
			//		Str3 = "此序號已出貨",
			//		Bool1 = false
			//	}
			//	,
			//	new P0500000000_ViewModel.DgDataClass()
			//	{
			//		Str1 = "098765432112346",
			//		Str2 = "",
			//		Bool1 = true
			//	}
			//};
		}

		public P0503020100(string gupcode,string custcode,string dccode,string wmsordno)
		{
			InitializeComponent();
			Vm.ExitClick += Exit_OnClick;
			Vm.GUP_CODE= gupcode;
			Vm.CUST_CODE = custcode;
			Vm.DC_CODE = dccode;
			Vm.WMS_ORD_NO = wmsordno;
            //Vm.GET_DC_LIST();
            //Vm.GET_SOURCE_TYPE_LIST();
            //Vm.GET_LACK_DO_STATUS_LIST();
            //Vm.GET_STATUS_LIST();
			Vm.SearchCommand.Execute(null);

		}

		private void Exit_OnClick()
		{
			Close();
		}



	}
}
