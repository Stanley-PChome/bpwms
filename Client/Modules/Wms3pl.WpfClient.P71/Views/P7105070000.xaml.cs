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

namespace Wms3pl.WpfClient.P71.Views
{
  /// <summary>
  /// P7105070000.xaml 的互動邏輯
  /// </summary>
  public partial class P7105070000 : Wms3plUserControl
  {
    public P7105070000()
    {
      InitializeComponent();
    }

    private void CbDcCode_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      Vm.SetDataSourceAfterDcCodeSelected();
    }

    private void DbAllId_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      Vm.SetDataSourceAfterAllIdSelected();
    }

    private void CbAccKind_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      Vm.SetInputData();
    }
  }
}
