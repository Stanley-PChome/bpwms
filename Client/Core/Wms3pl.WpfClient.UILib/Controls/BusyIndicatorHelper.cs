using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Wms3pl.WpfClient.UILib.Controls
{
  public class BusyIndicatorHelper
  {
    
      private static void OnEnsureFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
        if (!(bool)e.NewValue)
        {
          (d as Control).Focus();
        }
      }

      public static bool GetEnsureFocus(DependencyObject obj)
      {
        return (bool)obj.GetValue(EnsureFocusProperty);
      }

      public static void SetEnsureFocus(DependencyObject obj, bool value)
      {
        obj.SetValue(EnsureFocusProperty, value);
      }

      public static readonly DependencyProperty EnsureFocusProperty =
      DependencyProperty.RegisterAttached("EnsureFocus", typeof(bool), typeof(BusyIndicatorHelper), new PropertyMetadata(OnEnsureFocusChanged));
    
  }
}
