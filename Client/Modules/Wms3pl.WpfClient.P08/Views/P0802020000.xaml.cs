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
using Wms3pl.WpfClient.P08.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P08.Views
{
  /// <summary>
  /// P0802020000.xaml 的互動邏輯
  /// </summary>
  public partial class P0802020000 : Wms3plWindow
  {
    public P0802020000()
    {
      InitializeComponent();
      Vm.DgItemSource = new List<P0800000000_ViewModel.DgDataClass>()
      {
        new P0800000000_ViewModel.DgDataClass()
        {
          Str1 = "1-01-01-01-01",
          Str2 = "2",
          Str3 =Properties.Resources.Clear
        },
        new P0800000000_ViewModel.DgDataClass()
        {
          Str1 = "1-01-01-01-02",
          Str2 = "2",
          Str3 =Properties.Resources.Clear
        }
      };
    }
  }
}
