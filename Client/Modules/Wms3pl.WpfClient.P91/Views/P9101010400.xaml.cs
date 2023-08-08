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
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P91.Views
{
	/// <summary>
	/// P9101010400.xaml 的互動邏輯
	/// </summary>
	public partial class P9101010400 : Wms3plWindow
	{
		public P9101010400(F910201 src)
		{
			InitializeComponent();
			Vm.BaseData = src;
			Vm.GetMinProcessQty();
		}
	}
}
