using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
    public class NameValuePairMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
					if (!values.Any() || values[0] == null || values[0].Equals(DependencyProperty.UnsetValue))
                return null;

            var value = values[0].ToString();
            var list = values[1] as List<NameValuePair<string>>;
						if (list == null) return null; // 避免抓不到值時出錯
            return list.Where(item => item.Value == value).Select(item=>item.Name).FirstOrDefault();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
