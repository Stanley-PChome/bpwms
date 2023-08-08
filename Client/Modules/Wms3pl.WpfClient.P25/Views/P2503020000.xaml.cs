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

namespace Wms3pl.WpfClient.P25.Views
{
  /// <summary>
  /// P2503020000.xaml 的互動邏輯
  /// </summary>
  public partial class P2503020000 : Wms3plUserControl
  {
    public P2503020000()
    {
      InitializeComponent();
      Vm.SetDefaultFocus += SetDefaultFocus;
    }

    private void SetDefaultFocus()
    {
      SetFocusedElement(TxtItemCode);
    }

    private void TxtItemCode_OnKeyDown(object sender, KeyEventArgs e)
    {
      TextBox obj = sender as TextBox;
      if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(obj.Text))
      {
        if (Vm.CheckItemCode())
          SetFocusedElement(TxtSerialNoOld);
      }
    }

    private void TxtSerialNoOld_OnKeyDown(object sender, KeyEventArgs e)
    {
      TextBox obj = sender as TextBox;
      if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(obj.Text))
      {
        if (Vm.CheckOldSerialNo())
          SetFocusedElement(TxtSerialNoNew);
      }
    }

    private void TxtSerialNoNew_OnKeyDown(object sender, KeyEventArgs e)
    {
      TextBox obj = sender as TextBox;
      if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(obj.Text))
      {
        Vm.CheckNewSerialNo();
      }
    }

  }
}
