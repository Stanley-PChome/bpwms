using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P15.Views
{
  /// <summary>
  /// P1502010500.xaml 的互動邏輯
  /// </summary>
  public partial class P1502010500 : Wms3plWindow
  {
    public P1502010500(string DcCode, string GupCode, string CustCode, string AllocationNo)
    {
      InitializeComponent();
      Vm.DoSearchInitData(DcCode, GupCode, CustCode, AllocationNo);

      Vm.OnClose = o =>
      {
        this.DialogResult = o;
        this.Close();
      };
    }

    private void txtQTY_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("^[1-9]?[0-9]*$");
            var txt = ((TextBox)sender).Text + e.Text;

            e.Handled = !re.IsMatch(txt);
        }

        private void colLackQty_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!(sender is TextBox))
                return;
            var txtbox = sender as TextBox;
            int tmpI;
            if (!int.TryParse(txtbox.Text, out tmpI))
                return;
            Vm.CheckLackQty(tmpI);
        }

    private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
      if (!Vm.P1502010500Datas?.Any() ?? true)
      {
        Vm.ShowInfoMessage("查無資料");
        this.Close();
      }

    }
  }
}
