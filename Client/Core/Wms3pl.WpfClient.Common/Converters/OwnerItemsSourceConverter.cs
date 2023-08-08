using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
	public class OwnerItemsSourceConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (values[0] == null || values[0].Equals(DependencyProperty.UnsetValue)
			 || values[1] == null || values[1].Equals(DependencyProperty.UnsetValue))
				return null;

			var ownerValue = System.Convert.ToString(values[0]);

			// 給兩個參數即取得 ItemSource, 給三個參數就是要直接取得第一個參數的 ItemSource 的其中一個 Name
			var nameValuePairListDict = values[1] as Dictionary<string, List<NameValuePair<string>>>;

			if (nameValuePairListDict != null)
			{
				if (!nameValuePairListDict.ContainsKey(ownerValue))
					return null;

				var nameValuePairList = nameValuePairListDict[ownerValue];

				if (values.Length <= 2)
				{
					return nameValuePairList;
				}

				var value = System.Convert.ToString(values[2]);
				return nameValuePairList.Where(item => item.Value == value).Select(item => item.Name).FirstOrDefault();
			}

			// 如果單純要取得某個 ItemSource 的某一個 Name, 可直接用 Dictionary 比較快
			var pairDict = values[1] as Dictionary<string, Dictionary<string, string>>;

			if (pairDict == null)
				return null;

			if (!pairDict.ContainsKey(ownerValue))
				return null;

			var dict = pairDict[ownerValue];
			var key = System.Convert.ToString(values[2]);
			if (dict.ContainsKey(key))
				return dict[key];

			return null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
