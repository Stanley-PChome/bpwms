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
	/// P0800010000.xaml 的互動邏輯
	/// </summary>
	public partial class P0800010000 : Wms3plWindow
	{
		public P0800010000()
		{
			InitializeComponent();
			Vm.MyAppType = P21.ViewModel.appType.RF;
		}
		private void DG_Hyperlink_Click(object sender, RoutedEventArgs e)
		{
			Hyperlink link = e.OriginalSource as Hyperlink;
			string formId = link.NavigateUri.OriginalString;
			var formService = new FormService();
			formService.AddFunctionForm(formId);
		}
	}
}
