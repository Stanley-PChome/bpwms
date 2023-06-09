using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
  public class UseByConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string theValue = value as string;
      switch (theValue)
      {
        case "01":
          return "01 Wms";
        case "02":
          return "02 門市";
        case "03":
          return "03 廠商";
        case "05":
          return "05 DC";
        default:
          return string.Format("{0} 其他", theValue);
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
