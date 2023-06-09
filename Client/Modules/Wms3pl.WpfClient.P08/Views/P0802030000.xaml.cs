using System;
using System.Collections.Generic;
using Wms3pl.WpfClient.P08.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P08.Views
{
  /// <summary>
  /// P0803010000.xaml 的互動邏輯
  /// </summary>
  public partial class P0802030000 : Wms3plWindow
  {
    public P0802030000()
    {
      InitializeComponent();
      Vm.DgItemSource = new List<P0800000000_ViewModel.DgDataClass>()
      {
        new P0800000000_ViewModel.DgDataClass()
        {
          Str1 = "1-01-01-01-01",
          Str2 = "2",
          Str3 =Properties.Resources.Clear,
          Str4=Properties.Resources.P0802030000_GoodWarehouse
        },
        new P0800000000_ViewModel.DgDataClass()
        {
          Str1 = "1-01-01-01-02",
          Str2 = "2",
          Str3 =Properties.Resources.Clear,
          Str4=Properties.Resources.P0802030000_NoGoodWarehouse
        }
      };
    }
  }
}
