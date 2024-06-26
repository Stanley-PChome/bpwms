﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
	public class NullToBooleanConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return true;
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
