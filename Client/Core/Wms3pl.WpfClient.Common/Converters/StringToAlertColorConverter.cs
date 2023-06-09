using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Wms3pl.WpfClient.Common.Converters
{
	public class StringToAlertColorConverter : IValueConverter
	{
		//convert string to Yello/ White brush
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var input = value as string;
			SolidColorBrush defaultBrush = Brushes.Yellow;
			if (parameter != null) defaultBrush = parameter as SolidColorBrush;
			return ((!string.IsNullOrEmpty(input)) && (input.Trim() == "1")) ? defaultBrush : Brushes.White;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool isTrue = (bool) value;
			return isTrue ? "1" : "0";
		}
	}
}
