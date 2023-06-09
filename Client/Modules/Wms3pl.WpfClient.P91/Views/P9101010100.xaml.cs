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
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.P91.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P91.Views
{
	/// <summary>
	/// P9101010100.xaml 的互動邏輯
	/// </summary>
	public partial class P9101010100 : Wms3plWindow
	{
		public P9101010100(F910201 baseData)
		{
			InitializeComponent();
			Vm.BaseData = baseData;
			Vm.actionAfterCreatePickTicket += CloseWin;
		}

		private string _pickNo = string.Empty;
		public string PickNo
		{
			get { return _pickNo; }
			set { _pickNo = value; }
		}

		private void CloseWin()
		{
			this.PickNo = Vm.PickNo;
			this.Close();
		}
	}
}
