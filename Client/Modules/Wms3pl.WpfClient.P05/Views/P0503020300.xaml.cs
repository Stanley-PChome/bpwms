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

namespace Wms3pl.WpfClient.P05.Views
{
	/// <summary>
	/// P0503020300.xaml 的互動邏輯
	/// </summary>
	public partial class P0503020300 : Wms3plWindow
	{
		public P0503020300(string gupCode, string custCode, string dcCode, string ordNo)
		{
			InitializeComponent();
			Vm.DC_CODE = dcCode;
			Vm.GUP_CODE = gupCode;
			Vm.CUST_CODE = custCode;
			Vm.ORD_NO = ordNo;
			Vm.SearchCommand.Execute(null);
			Vm.ExitClick += ExitForm;
		}

		private void ExitForm()
		{
			this.Close();
		}
	}
}
