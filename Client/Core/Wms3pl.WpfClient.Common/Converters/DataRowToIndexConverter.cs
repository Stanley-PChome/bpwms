using System;
using System.Data;
using System.Globalization;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
  public class DataRowToIndexConvrter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var row = (value as DataRowView).Row;
      return row.Table.Rows.IndexOf(row) + 1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

}
