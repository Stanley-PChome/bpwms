using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Automation;

namespace Wms3pl.WpfClient.Common.Converters
{
	public class CheckStateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool result = (bool)value;
			return result ? ToggleState.On : ToggleState.Off;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			ToggleState state = (ToggleState)value;
			return state == ToggleState.On ? true : false;
		}
	}
}
