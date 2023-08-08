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
	/// P0503020400.xaml 的互動邏輯
	/// </summary>
	public partial class P0503020400 : Wms3plWindow
	{
		public P0503020400(string dcCode,string gupCode,string custCode,string ordNo,string delvRetailCode,string delvRetailName)
		{
			InitializeComponent();
			Vm.DcCode = dcCode;
			Vm.GupCode = gupCode;
			Vm.CustCode = custCode;
			Vm.OrderNo = ordNo;
			Vm.OldDelvRetailCode = delvRetailCode;
			Vm.OldDelvRetailName = delvRetailName;
			Vm.DelvRetailCode = delvRetailCode;
			Vm.DelvRetailName = delvRetailName;
			Vm.CloseWin += CloseWin;
			Vm.SearchCommand.Execute(null);
		}
		private void CloseWin(bool isSave)
		{
			this.DialogResult = isSave;
			Close();
		}
	}
}
