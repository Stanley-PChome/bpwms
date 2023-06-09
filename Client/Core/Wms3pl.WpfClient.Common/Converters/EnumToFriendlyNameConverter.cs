using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
  [ValueConversion(typeof(object), typeof(String))]
  public class EnumToFriendlyNameConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value != null)
      {
        FieldInfo fi = value.GetType().GetField(value.ToString());

        if (fi != null)
        {
          var attributes = (LocalizableDescriptionAttribute[])fi.GetCustomAttributes(typeof(LocalizableDescriptionAttribute), false);

          return ((attributes.Length > 0) && (!String.IsNullOrEmpty(attributes[0].Description)))
                     ? attributes[0].Description : value.ToString();
        }
      }

      return string.Empty;
    }

    /// <summary>
    /// ConvertBack value from binding back to source object
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new Exception("Cant convert back");
    }
  }

}
