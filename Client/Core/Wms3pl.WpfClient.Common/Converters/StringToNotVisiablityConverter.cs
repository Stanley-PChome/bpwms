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
	public class StringToNotVisiablityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var input = value as string;
			return !((!string.IsNullOrEmpty(input)) && (input.Trim() == "1")) ? Visibility.Visible : Visibility.Collapsed;

			throw new Exception("Value is not Type of string !");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Visibility)
			{
				var visibility = (Visibility)value;
				return !(visibility == Visibility.Visible) ? "1" : "0";
			}
			throw new Exception("Value is not Type of Visibility !");
		}
	}
}
