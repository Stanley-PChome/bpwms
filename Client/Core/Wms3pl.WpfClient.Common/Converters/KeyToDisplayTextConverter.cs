using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
  public class KeyToDisplayTextConverter : IValueConverter
  {
    private Dictionary<string, Dictionary<object, string>> _dictionary = new Dictionary<string, Dictionary<object, string>>();
    public void AddDictionary(string dictionaryKey, Dictionary<object, string> dictionaryValue )
    {
      _dictionary.Add(dictionaryKey, dictionaryValue);
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (parameter == null)
        throw new Exception("parameter cannot be null");
      string dictionaryName = (string) parameter;
      var dictionary = _dictionary[dictionaryName];
      if (dictionary == null)
        throw new Exception("Cannot find dictionary " + dictionaryName);
      return dictionary[value];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
