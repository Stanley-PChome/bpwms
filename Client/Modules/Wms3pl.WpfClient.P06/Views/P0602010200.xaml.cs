using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices.P06ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P06.Views
{
  /// <summary>
  /// P0602010200.xaml 的互動邏輯
  /// </summary>
  public partial class P0602010200 : Wms3plWindow
  {
    public P0602010200(List<F1951> REASONList, F051206Pick PickSelected, ObservableCollection<F051206LackList> LackList)
    {
      InitializeComponent();
      Vm.REASONList = REASONList;
      Vm.PickSelected = PickSelected;
      Vm.LackList = LackList;
      Vm.DoClose += CloseWin;
    }

    public void CloseWin(Boolean IsSuccess)
    {
      this.DialogResult = IsSuccess;
      Close();
    }

  }
}
