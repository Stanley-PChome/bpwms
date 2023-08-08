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
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.P71.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.Views
{
  /// <summary>
  /// P7105020000.xaml 的互動邏輯
  /// </summary>
  public partial class P7105020000 : Wms3plUserControl
  {
    public P7105020000()
    {
      InitializeComponent();
			Vm.SetCtrlView += SetCtrlViewFocus;
    }
		private void SetCtrlViewFocus(F199002Data obj)
		{
			this.ControlView(() =>
			{
				DgJobValuation.Focus();
				DgJobValuation.SelectedItem = obj;
				DgJobValuation.ScrollIntoView(obj);
			});
		}
		private void rb_SingleChecked(object sender, RoutedEventArgs e)
		{
			if (Vm.CurrentRecord == null) return;
			Vm.CurrentRecord.BASIC_FEE = 0;
			Vm.CurrentRecord.OVER_FEE = 0;
		}

		private void rb_ConditionChecked(object sender, RoutedEventArgs e)
		{
			if (Vm.CurrentRecord == null) return;
			Vm.CurrentRecord.FEE = 0;
		}

		private void cbUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Vm.UserOperateMode == OperateMode.Add)
			{
				if (Vm.CurrentRecord == null) return;
				if (Vm.CurrentRecord.ACC_UNIT != "01")
					Vm.CurrentRecord.ACC_KIND = "A";
			}
		}

		private void cbAccItemKind_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Vm.UserOperateMode == OperateMode.Add)
			{
				if (Vm.CurrentRecord == null) return;
				Vm.DelvAccTypeAddList = Vm.GetDelvAccTypeList(Vm.CurrentRecord.ACC_ITEM_KIND_ID);
			}
		}
  }
}
