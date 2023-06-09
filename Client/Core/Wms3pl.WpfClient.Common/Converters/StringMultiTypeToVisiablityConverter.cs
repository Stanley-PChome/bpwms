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
	public class StringMultiTypeToVisiablityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string str = (string)value;
			var param = (parameter != null) ? parameter.ToString().Split(';') : null;
			if (param == null) return Visibility.Collapsed;
			foreach (var item in param)
			{
				if (str == item)
					return Visibility.Visible;
			}
			return Visibility.Collapsed;

			throw new Exception("Value is not Type of string !");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
