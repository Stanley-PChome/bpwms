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
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P05.Views
{
  /// <summary>
  /// P0503030000.xaml 的互動邏輯
  /// </summary>
  public partial class P0503030000 : Wms3plUserControl
  {
    public P0503030000()
    {
      InitializeComponent();
    }

        private void btnSerachProduct_Click(object sender, RoutedEventArgs e)
        {
            WinSearchProduct winSearchProduct = new WinSearchProduct();
            var globalInfo = Wms3plSession.Get<GlobalInfo>();
            winSearchProduct.GupCode = globalInfo.GupCode;
            winSearchProduct.CustCode = globalInfo.CustCode;
            winSearchProduct.ShowDialog();

            if (winSearchProduct.SelectData != null)
                Vm.ItemCode = winSearchProduct.SelectData.ITEM_CODE;
        }
    }
}
