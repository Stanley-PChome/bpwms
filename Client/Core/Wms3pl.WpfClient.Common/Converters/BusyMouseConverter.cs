using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Wms3pl.WpfClient.Common.Converters
{
  public class BusyMouseConverter : MarkupExtension, IValueConverter
  {
    public BusyMouseConverter()
    {
      
    }
    private static BusyMouseConverter instance = new BusyMouseConverter();


    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool)
      {
        if ((bool)value)
          return Cursors.Wait;
        else
          return null;
      }

      return null;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Cursors)
      {
        if (value == Cursors.Wait)
          return true;
        else
          return false;
      }

      return null;

    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return instance;
    }
  }
}
