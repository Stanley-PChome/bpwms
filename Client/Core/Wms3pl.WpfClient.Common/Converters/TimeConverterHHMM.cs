using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
    public class TimeConverterHHMM : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                string time = System.Convert.ToString(value);
                if (string.IsNullOrWhiteSpace(time) || time.Length < 4)
                {
                    return value;
                }

                return string.Format("{0}:{1}", time.Substring(0, 2), time.Substring(2, 2));
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                string time = System.Convert.ToString(value);
                if (string.IsNullOrWhiteSpace(time) || time.Length < 5)
                {
                    return value;
                }

                return string.Format("{0}{1}", time.Substring(0, 2), time.Substring(3, 2));
            }
            return value;
        }

        #endregion
    }
}
