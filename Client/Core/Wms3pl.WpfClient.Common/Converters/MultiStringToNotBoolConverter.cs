using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
	public class MultiStringToNotBoolConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			
			var input = values[0] as string;
			var input2 = values[1] as string;
			return ((!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(input2)) && (input.Trim() == "0" || input2.Trim() == "0"));
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
