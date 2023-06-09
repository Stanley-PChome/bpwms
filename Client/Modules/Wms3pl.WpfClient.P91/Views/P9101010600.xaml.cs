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
	/// P9101010600.xaml 的互動邏輯
	/// </summary>
	public partial class P9101010600 : Wms3plWindow
	{
		public P9101010600(F910201 src)
		{
			InitializeComponent();
			Vm.BaseData = src;
			Vm.SearchCommand.Execute(null);
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			Vm.SelectedData = null;
			this.DialogResult = true;
			this.Close();
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if (Vm.SelectedData != null)
				this.DialogResult = true; // 設定為True, 回到原視窗時可得知有沒有選取到值, 或是按下Cancel (Close)
			this.Close();
		}
	}
}
