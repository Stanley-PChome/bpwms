using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
	/// <summary>
	/// 將時間字串 (HHMM) 轉為 HH:MM格式
	/// </summary>
	public class TimeStringToTimeFormatConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null) return string.Empty;
			var input = value as string;
			DateTime dt;
			DateTime.TryParseExact(DateTime.Now.ToString("yyyy/MM/dd ") + input, "yyyy/MM/dd HHmm", null, System.Globalization.DateTimeStyles.None, out dt);

			return string.Format(dt.ToString("HH:mm"));
		}

		public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
