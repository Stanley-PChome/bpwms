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
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P02.Views
{
  /// <summary>
  /// P0202080100.xaml 的互動邏輯
  /// </summary>
  public partial class P0202080100 : Wms3plWindow
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="rtNo">驗收單號</param>
    /// <param name="removeSerialNo">要移除的序號紀錄</param>
    public P0202080100(string dcCode, string gupCode, string custCode, string rtNo, List<string> removeSerialNo)
    {
      InitializeComponent();
      Vm.DcCode = dcCode;
      Vm.GupCode = gupCode;
      Vm.CustCode = custCode;
      Vm.rtNo = rtNo;
      Vm.RemoveSerialNos = removeSerialNo.ToObservableCollection();
      Vm.DoFocusInputSerialNo = FocusInputSerialNo;
      Vm.DoClose = DoClose;
    }

    private void DoClose(bool IsSave)
    {
      this.DialogResult = IsSave;
    }

    private void FocusInputSerialNo()
    {
      SetFocusedElement(txtInputSerialNo, true);
    }

    private void Wms3plWindow_Loaded(object sender, RoutedEventArgs e)
    {
      SetFocusedElement(txtInputSerialNo, true);
    }
  }
}
