using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
	public class StringTypeToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (parameter == null)
				return DependencyProperty.UnsetValue;

			string parameterString = parameter.ToString();

			var input = value as string;

			return parameterString.Equals(value);

		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (parameter == null)
				return DependencyProperty.UnsetValue;

			string parameterString = parameter.ToString();

			bool isTrue = (bool)value;

			return isTrue ? parameterString : Binding.DoNothing;
		}

	}
}
