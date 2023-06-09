﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
	public class StringTypeToNotVisiablityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string str = (string)value;
			if (str == parameter.ToString())
				return Visibility.Collapsed;
			else
				return Visibility.Visible;

			throw new Exception("Value is not Type of string !");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Visibility)
			{
				var visibility = (Visibility)value;
				return (visibility == Visibility.Collapsed) ? parameter : Binding.DoNothing;
			}

			throw new Exception("Value is not Type of Visibility !");
		}
	}
}