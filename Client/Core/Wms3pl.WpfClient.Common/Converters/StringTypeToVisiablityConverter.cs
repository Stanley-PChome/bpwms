using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
  public class StringTypeToVisiablityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
      string str = (string)value;
      if (str == parameter.ToString())
        return Visibility.Visible;
      else
        return Visibility.Collapsed;

			throw new Exception("Value is not Type of string !");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Visibility)
			{
				var visibility = (Visibility)value;
        return (visibility == Visibility.Visible) ? parameter : Binding.DoNothing;
			}

			throw new Exception("Value is not Type of Visibility !");
		}
	}
}
