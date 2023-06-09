using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace Wms3pl.WpfClient.UILib.Controls
{
  public class Wms3plAutoCompleteBox : AutoCompleteBox
  {
    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (e.Key == Key.Enter ) //&& !IsDropDownOpen
      {
        IsDropDownOpen = false;
        e.Handled = false;
      }
      else
      {
        base.OnKeyDown(e);
      }
    } 

  }
}
