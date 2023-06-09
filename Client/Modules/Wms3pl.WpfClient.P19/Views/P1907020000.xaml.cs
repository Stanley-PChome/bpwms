using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Wms3pl.WpfClient.P19.Views
{
  /// <summary>
  /// P1907020000.xaml 的互動邏輯
  /// </summary>
  public partial class P1907020000 : Wms3plUserControl
  {
    public P1907020000()
    {
      InitializeComponent();

      Thread.CurrentThread.CurrentCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
      Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";
    }

    private void Dg_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
      if (e.PropertyType == typeof(DateTime))
      {
        var dataGridTextColumn = e.Column as DataGridTextColumn;
        if (dataGridTextColumn != null)
        {
          dataGridTextColumn.Binding.StringFormat = "{0:yyyy/MM/dd}";
        }
      }
    }

	  
  }
}
