using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
  public class StringFormatNumberConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
			if (value == null || value.GetType() != typeof(string) || string.IsNullOrEmpty((string)value))	    
		    return string.Empty;      
      var input = value as string;
      if (input.Last() == '.' || input.Last() == '0')
        return value;

      decimal number;
      if (decimal.TryParse(input, out number))
      {
        return number.ToString(parameter.ToString());
      }
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value;
    }
  }
}
