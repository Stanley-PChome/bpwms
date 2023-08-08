using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
	public class StringToLocCodeConverter : IValueConverter
	{
		public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (values == null) return DependencyProperty.UnsetValue;
			string val = values.ToString();
			if (val.Length == 9)
			{
				var tmpCode = string.Format("{0}-{1}-{2}-{3}-{4}", val.Substring(0, 1)
																, val.Substring(1, 2)
																, val.Substring(3, 2)
																, val.Substring(5, 2)
																, val.Substring(7, 2));
				return tmpCode;
			}
			//else if (val.Trim().Length == 13)
			//{
			//	return val;
			//}
			else
			{
				return val;
				//string tmplocCode = val.Trim().Replace("-", "");

				//if (tmplocCode.Length > 7)
				//{
				//	string tmpCode = string.Format("{0}-{1}-{2}-{3}-{4}", tmplocCode.Substring(0, 1)
				//													, tmplocCode.Substring(1, 2)
				//													, tmplocCode.Substring(3, 2)
				//													, tmplocCode.Substring(5, 2)
				//													, tmplocCode.Substring(7, 2));
				//	return tmpCode;
				//}
				//else
				//{

				//}
			}
		}

		public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			var returnValue=value.ToString().Trim().Replace("-", "");
			return returnValue;
		}
	}
}
