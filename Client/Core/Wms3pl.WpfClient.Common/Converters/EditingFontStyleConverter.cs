using System;
using System.Drawing;
using System.Windows.Data;
using System.Globalization;

namespace Wms3pl.WpfClient.Common.Converters
{
  [ValueConversion(typeof(bool), typeof(bool))]
  public class EditingFontStyleConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is bool))
      {
        throw new NotSupportedException("Only bool is supported.");
      }
      var isDirty = (bool) value;
      return isDirty ? FontStyle.Italic : FontStyle.Regular;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      FontStyle style = (FontStyle) value;
      if (style == FontStyle.Italic)
        return true;
      else
      {
        return false;
      }
      
    }
  }
}
