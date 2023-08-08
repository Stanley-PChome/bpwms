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

namespace Wms3pl.WpfClient.P71.Views
{
  /// <summary>
  /// P7101050000.xaml 的互動邏輯
  /// </summary>
  public partial class P7101050000 : Wms3plUserControl
  {
    public P7101050000()
    {
      InitializeComponent();
	  Vm.OnFocusAdd += FocusAdd;
	  Vm.OnFocusEdit += FocusEdit;
	  SetFocusedElement(tbQuery);
    }

	private void FocusEdit()
	{
		SetFocusedElement(lOC_TYPE_NAMETextBox);
	}

	private void FocusAdd()
	{
		SetFocusedElement(lOC_TYPE_IDTextBox);
	}

    private void Numeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      bool isLegal = CheckIslegal(e.Text);
      e.Handled = isLegal;
    }
    private void VolumeRate_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      bool isLega2 = CheckIslega2();
      e.Handled = isLega2;

    }

    private bool CheckIslega2()
    {
      int Rate = 0;
      Int32.TryParse(Vm.CurrentRecord.VOLUME_RATE.ToString(), out Rate);
      if (Rate > 100 || Rate < 0)
      {
        DialogService.ShowMessage(Properties.Resources.P7101050000xamlcs_OVERVOLUME_RATE_Range);
				Vm.CurrentRecord.VOLUME_RATE = 0;
        return true;
      }
      return false;
    }

    private bool CheckIslegal(string inputStr)
    {
      string strNum1 = "1";
      string strNum2 = "2";
      string strNum3 = "3";
      // 檢查是否輸入1,2,3
      bool num1 = inputStr.Equals(strNum1);
      bool num2 = inputStr.Equals(strNum2);
      bool num3 = inputStr.Equals(strNum3);
      if (num1 || num2 || num3)
        return false;
      DialogService.ShowMessage(Properties.Resources.P7101050000xamlcs_OVERNUM_Range);
      Vm.CurrentRecord.HANDY = string.Empty;
      return true;
    }
  }
}
