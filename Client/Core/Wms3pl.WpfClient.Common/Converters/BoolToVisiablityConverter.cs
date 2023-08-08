using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
	public class BoolToVisiablityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool)
      {
        var b = (bool)value;
				return b ? Visibility.Visible : Visibility.Collapsed;
      }
      throw new Exception("Value is not Type of Boolean !");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Visibility)
      {
        var visibility = (Visibility)value;
        return visibility == Visibility.Visible;
      }
      throw new Exception("Value is not Type of Visibility !");
    }
  }
}
