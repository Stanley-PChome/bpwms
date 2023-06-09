using System;
using System.Drawing;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace Wms3pl.WpfClient.Common.Converters
{
  public class ForegroundToRedConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
			SolidColorBrush brush = null;
			if (value != null && value.GetType() == parameter.GetType())
      {
				if (value.Equals(parameter))
				{
					brush = new SolidColorBrush(Colors.Red);
					if (brush.CanFreeze)
						brush.Freeze();
					return brush;
				}
      }

			brush = new SolidColorBrush(Colors.Black);
			if (brush.CanFreeze)
				brush.Freeze();
			return brush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
			throw new NotImplementedException();      
    }
  }
}
