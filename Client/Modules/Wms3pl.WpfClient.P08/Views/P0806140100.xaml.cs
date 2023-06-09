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
using Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.P08.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0806140100.xaml 的互動邏輯
	/// </summary>
	public partial class P0806140100 : Wms3plWindow
	{
		public P0806140100(List<PickAllotLoc> pickAllotLocs)
		{
			InitializeComponent();
			Vm.PickAllotLocs = pickAllotLocs;
			Vm.Init();
		}
	}
}
