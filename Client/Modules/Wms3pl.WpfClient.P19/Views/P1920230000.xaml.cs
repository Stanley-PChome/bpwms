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
using System.Text.RegularExpressions;

namespace Wms3pl.WpfClient.P19.Views
{
  /// <summary>
  /// P1920230000.xaml 的互動邏輯
  /// </summary>
  public partial class P1920230000 : Wms3plUserControl
  {
    public P1920230000()
    {
      InitializeComponent();
    }

    private void PriCodeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      Vm.GetF1956IsShow();  
     
    }

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      //Regex re = new Regex("^[1-9]?[0-9]*$");
      Regex re = new Regex("^[0-9]?[0-9]*$");
      var txt = ((TextBox)sender).Text + e.Text;

      e.Handled = !re.IsMatch(txt);
    }


  }
}
