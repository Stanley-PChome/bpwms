using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
	public class ZeroPointConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
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
				if (strValue.Last() == '.' && decimal.TryParse(strValue, out mValue))
					return strValue + ".";
			}
			return value;
		}
	}
}
