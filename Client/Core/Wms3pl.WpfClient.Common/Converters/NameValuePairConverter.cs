using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
  public class NameValuePairConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var lists = (parameter as ObjectDataProvider).Data as List<NameValuePair<string>>;

			if (value != null && value.GetType() == typeof(decimal))
				value = value.ToString();

      string strValue = value as string;

			if (lists == null) return "";
      var valuePair = lists.Where(i => i.Value== strValue).SingleOrDefault();
      return valuePair == null ? "" : valuePair.Name;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
