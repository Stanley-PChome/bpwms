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
	/// <summary>
	/// 隱藏特定的下拉選單選項
	/// 當傳入的value符合parameter裡的任一值時, 該value試為要被隱藏, 會回傳false
	/// </summary>
	public class StringToComboBoxVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || parameter == null) return true;
			var p = parameter.ToString().Split(',').ToList();
			if (p.Exists(x => x == value.ToString()))
			{
				return false;
			}
			return true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
