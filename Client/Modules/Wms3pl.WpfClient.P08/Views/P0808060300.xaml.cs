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
using Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.P08.ViewModel;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClients.SharedViews.Views;

namespace Wms3pl.WpfClient.P08.Views
{
  /// <summary>
  /// P0808060300.xaml 的互動邏輯
  /// </summary>
  public partial class P0808060300 : Wms3plWindow
  {
    public P0808060300()
    {
      InitializeComponent();
      Vm.DoContainerFocus = ContainerCodeFocus;
    }
    public P0808060300(string dcCode) : base()
    {
      InitializeComponent();
      Vm.DoContainerFocus = ContainerCodeFocus;
    }

    #region Focus
    private void ContainerCodeFocus()
    {
      SetFocusedElement(txtPickContainerCode, true);
    }
    #endregion

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      DispatcherAction(() =>
      {
        ContainerCodeFocus();
      });
    }

    #region 刷讀容器條碼
    private void TxtContainerCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Enter)
        return;

      Vm.SearchCommand.Execute(null);
    }
    #endregion

    private void TxtOutContainerCode_KeyDown(object sender, KeyEventArgs e)
    {

    }
  }
}
