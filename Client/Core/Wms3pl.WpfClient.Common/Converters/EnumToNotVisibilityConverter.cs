using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
	public class EnumToNotVisibilityConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
			if (parameter == null)
				return DependencyProperty.UnsetValue;

			string parameterString = parameter.ToString();

      if (Enum.IsDefined(value.GetType(), value) == false)
        return DependencyProperty.UnsetValue;

      object parameterValue = Enum.Parse(value.GetType(), parameterString);

			return parameterValue.Equals(value) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
			if (parameter == null)
				return DependencyProperty.UnsetValue;

			string parameterString = parameter.ToString();

      return Enum.Parse(targetType, parameterString);
    }

    #endregion
  }
}