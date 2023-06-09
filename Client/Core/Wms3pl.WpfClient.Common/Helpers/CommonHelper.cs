using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.Common.Helpers
{
	public class CommonHelper
	{

		public static bool ComparePropertiesValue<T>(T object1, T object2, List<string> excludePropertis = null) where T : class
		{
			var proInfos = typeof(T).GetProperties();
			foreach (var proInfo in proInfos)
			{
				if (excludePropertis != null && excludePropertis.Contains(proInfo.Name)) continue;
				if (!proInfo.PropertyType.IsValueType && proInfo.PropertyType != typeof(string)) continue;
				if (proInfo.GetMethod.GetParameters().Any()) continue;
				var v1 = proInfo.GetValue(object1);
				var v2 = proInfo.GetValue(object2);
				if (v1 == null && v2 != null) return false;
				if (v1 != null && !v1.Equals(v2))
					return false;
			}
			return true;
		}
	}
}
