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

namespace Wms3pl.WpfClient.P05.Views
{
	/// <summary>
	/// P0503040100.xaml 的互動邏輯
	/// </summary>
	public partial class P0503040100 : Wms3plWindow
	{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Flag">呼叫位置 0:手動挑單 1:配庫試算結果查詢</param>
    public P0503040100(string Flag)
		{
      InitializeComponent();
      Vm.Flag = Flag;
      Vm.CloseWindows = () => { Close(); };
		}

    private void Form_Loaded(object sender, RoutedEventArgs e)
    {
      if(!new[] { "0", "1" }.Contains( Vm.Flag))
      {
        Vm.ShowWarningMessage("無法辨識的Flag，本視窗將關閉");
        Close();
      }
    }
  }
}
