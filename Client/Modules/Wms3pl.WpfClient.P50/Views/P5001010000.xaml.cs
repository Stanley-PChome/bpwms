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
using Wms3pl.WpfClient.ExDataServices.P50ExDataService;
using Wms3pl.WpfClient.P50.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P50.Views
{
  /// <summary>
  /// P5001010000.xaml 的互動邏輯
  /// </summary>
  public partial class P5001010000 : Wms3plUserControl
  {
    public P5001010000()
    {
      InitializeComponent();
			Vm.SetCtrlView += SetCtrlViewFocus;
    }

		private void SetCtrlViewFocus(F199007Data obj)
		{
			this.ControlView(() =>
			{
				DgProject.Focus();
				DgProject.SelectedItem = obj;
				DgProject.ScrollIntoView(obj);
			});
		}
  }
}
