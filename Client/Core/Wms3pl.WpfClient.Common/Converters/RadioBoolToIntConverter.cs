using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
  class RadioBoolToIntConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      int integer = (int)value;
      if (integer == int.Parse(parameter.ToString()))
        return true;
      else
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value.Equals(true) ? parameter : Binding.DoNothing;
    } 

  }
}
