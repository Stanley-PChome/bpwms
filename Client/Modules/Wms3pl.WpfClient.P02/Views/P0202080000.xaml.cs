using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P02.Views
{
  /// <summary>
  /// P0202080000.xaml 的互動邏輯
  /// </summary>
  public partial class P0202080000 : Wms3plUserControl
  {
    public P0202080000()
    {
      InitializeComponent();
      Vm.SetContainerFocus = () => { SetFocusedElement(txtContainerCode, true); };
      Vm.ShowNGItemForm = () => ShowNGItemForm();
      Vm.ShowSerialNoForm = () => ShowSerialNoForm();
    }

    private void txtContainerCode_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        if (string.IsNullOrWhiteSpace(Vm.AddContainerCode))
        {
          Vm.ShowInfoMessage("請輸入容器條碼或容器分格條碼");
          return;
        }
        Vm.GetContainerRecheckFaildCommand.Execute(null);
      }
    }

    private void txtRemoveQTY_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      Regex re = new Regex("^[1-9]?[0-9]*$");
      var txt = ((TextBox)sender).Text + e.Text;
      e.Handled = !re.IsMatch(txt);
    }

    private void ShowNGItemForm()
    {
      var p020203Data = new P020203Data();
      if (Vm.f02020109Datas == null)
        Vm.f02020109Datas = new List<F02020109Data>();

      var win = new P0202030700(Vm.p020203Datas, 2, Vm.f02020109Datas.ToList());
      var result = win.ShowDialog();
      if (result.GetValueOrDefault())
        Vm.f02020109Datas = win.f02020109Datas.ToList();
    }

    private void txtRemoveQTY_LostFocus(object sender, System.Windows.RoutedEventArgs e)
    {
      if (Vm.SelectedContainerRecheckFaildItem.QTY < Vm.RemoveRecvQty)
        Vm.ShowWarningMessage("移除數量不可大於實際數量");

    }

    Boolean IsIgnoreSelectedContainerRecheckFaildItem;
    int dgSelectIndex;
    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (IsIgnoreSelectedContainerRecheckFaildItem || (!Vm.ContainerRecheckFaildItems?.Any() ?? true))
        return;

      if ((Vm.f02020109Datas?.Count ?? 0) > 0)
        if (Vm.ShowConfirmMessage("已有設定不良品資料，選擇其他資料會清除原有設定，是否仍要繼續?") != UILib.Services.DialogResponse.Yes)
        {
          Dispatcher.BeginInvoke((Action)delegate ()
          {
            IsIgnoreSelectedContainerRecheckFaildItem = true;
            Vm.SelectedContainerRecheckFaildItem = Vm.ContainerRecheckFaildItems[dgSelectIndex];
            IsIgnoreSelectedContainerRecheckFaildItem = false;
          });
          return;
        }
        else
          Vm.f02020109Datas = null;

      dgSelectIndex = dgContainerRecheckFaildItem.SelectedIndex;
    }

    private void ShowSerialNoForm()
    {
      var win = new P0202080100(Vm.AddSelectedDc, Vm._gupCode, Vm._custCode, Vm.SelectedContainerRecheckFaildItem.RT_NO, Vm.RemoveSerialNos);
      win.ShowDialog();
      if (win.DialogResult.HasValue && win.DialogResult.Value)
        Vm.RemoveSerialNos = win.Vm.RemoveSerialNos.ToList();
    }

  }
}
