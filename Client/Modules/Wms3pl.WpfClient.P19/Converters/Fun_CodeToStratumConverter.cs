using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Wms3pl.WpfClient.P19.Converter
{
	public class Fun_CodeToStratumConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lists = P19.Services.P190502Datas.Stratum;

			string strValue = value as string;

			var valuePair = lists.Where(i => i.Value == (strValue.EndsWith("00000000") ? "1":"2")).SingleOrDefault();
			return valuePair == null ? "" : valuePair.Name;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
