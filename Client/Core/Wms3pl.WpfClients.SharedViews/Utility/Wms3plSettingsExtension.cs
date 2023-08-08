using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Wms3pl.WpfClient.Services;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;

namespace Wms3pl.WpfClient.Utility
{
  public static class Wms3plSettingsExtension
  {
    public static void ApplySettings(this Wms3plSettings settings)
    {
      SetFontSize(18);
    //  if (settings != null && !string.IsNullOrWhiteSpace(settings.CurrentCulture))
    //  {
				//CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(settings.CurrentCulture);
				//CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(settings.CurrentCulture);
				//Thread.CurrentThread.CurrentCulture = new CultureInfo(settings.CurrentCulture);
    //    Thread.CurrentThread.CurrentUICulture = new CultureInfo(settings.CurrentCulture);
    //  }
    }
    private static void SetFontSize(double size)
    {
      //AppleTextFontSize(typeof(TextBlock), size);
      //SetFontSizeUsingMetadata(size);
      //AppleTextFontSize(typeof(TextElement), size);
      //AppleTextFontSize(typeof(TextBox), size);
      //AppleTextFontSize(typeof(DatePicker), size); 
      //AppleTextFontSize(typeof(AutoCompleteBox), size);
      Application.Current.MainWindow.FontSize = size;
    }

    private static void SetFontSizeUsingMetadata(double size)
    {
      try
      {
        TextElement.FontSizeProperty.OverrideMetadata(
               typeof(TextElement), new FrameworkPropertyMetadata(size));

        TextBlock.FontSizeProperty.OverrideMetadata(
            typeof(TextBlock), new FrameworkPropertyMetadata(size));
      }
      catch (Exception)
      {


      }

    }

    private static void AppleTextFontSize(Type type, double size)
    {
      var style = new Style(type);
      style.Setters.Add(new System.Windows.Setter(Control.FontSizeProperty, size));
      if (Application.Current.Resources.Contains(type))
        Application.Current.Resources.Remove(type);
      Application.Current.Resources.Add(type, style);
    }


  }
}
