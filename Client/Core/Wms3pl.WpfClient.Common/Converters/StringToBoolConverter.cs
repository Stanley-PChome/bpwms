using System;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
	public class StringToBoolConverter : IValueConverter
	{
		//convert string to bool
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var input = value as string;
			return ((!string.IsNullOrEmpty(input)) && (input.Trim() == "1"));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool isTrue = (bool) value;
			return isTrue ? "1" : "0";
		}
	}
}
