using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
  /// <summary>
  /// 傳入ConverterParameter，則無條件捨去指定小數位之後字串
  /// 例如傳入2，表示只允許小數2位，123.456789=>123.45
  /// </summary>
  public class ZeroPointDigitsConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      //if (value == null || value == DBNull.Value)
      //  return value;

      //decimal dValue;
      //if (decimal.TryParse(value.ToString(), out dValue))
      //{
      //  if (parameter != null)
      //  {
      //    int iParameter;
      //    if (int.TryParse(parameter.ToString(), out iParameter))
      //    {
      //      return dValue.ToString("0." + new string('#', iParameter));
      //    }
      //  }
      //}
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is string && (string)value == string.Empty)
        return null;
      else if (value is string)
      {
        var strValue = (string)value;
        decimal mValue;
        if (decimal.TryParse(strValue, out mValue))
        {
          if (strValue.Last() == '.')
          {
            return strValue + ".";
          }
          else
          {
            if (parameter != null)
            {
              int iParameter;
              if (int.TryParse(parameter.ToString(), out iParameter))
              {
                var arrTmp = strValue.Split('.');
                if (arrTmp.Length > 1 && arrTmp[1].Length > iParameter)
                  return arrTmp[0] + "." + arrTmp[1].Substring(0, iParameter);
              }
            }
          }
        }
      }
      return value;
    }
  }
}
