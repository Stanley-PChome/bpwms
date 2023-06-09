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

namespace Wms3pl.WpfClient.P08.Views
{
    /// <summary>
    /// P0809010100.xaml 的互動邏輯
    /// </summary>
    public partial class P0809010100 : Wms3plWindow
    {
        public P0809010100()
        {
            InitializeComponent();
        }
		public P0809010100(string gupCode, string custCode, string dcCode, DateTime delvDate, string checkoutTime, string pickTime)
		{
			InitializeComponent();
			Vm.GUP_CODE = gupCode;
			Vm.CUST_CODE = custCode;
			Vm.DC_CODE = dcCode;
			Vm.DELV_DATE = delvDate;
			Vm.CHECKOUT_TIME = checkoutTime;
			Vm.PICK_TIME = pickTime;
			Vm.SearchCommand.Execute(null);
		}
    }
}
