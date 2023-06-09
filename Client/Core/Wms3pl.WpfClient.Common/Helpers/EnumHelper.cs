using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.Common.Helpers
{
	public static class EnumHelper
	{
		public static string GetPresentationString(Enum enumValue)
		{
			var enumType = enumValue.GetType();
			var memberInfos =
					enumType.GetMember(enumValue.ToString());
			if (memberInfos.Length > 0)
			{
				var attrs = memberInfos[0].GetCustomAttributes(typeof(EnumMemberAttribute), false);

				if (attrs.Length > 0 && ((EnumMemberAttribute)attrs[0]).Value != null)
				{
					return ((EnumMemberAttribute)attrs[0]).Value;
				}
			}
			return enumValue.ToString();
		}
		public static List<NameValuePair<string>> EnumToNameValuePairList<T>()
		{
			var result= new List<NameValuePair<string>>();
			foreach(var f in (T[])Enum.GetValues(typeof(T)))
			{
				var name = f.ToString();
				if (typeof(T) == typeof(DayOfWeek))
					name = DateTimeFormatInfo.GetInstance(new CultureInfo("zh-TW")).DayNames[(byte)f.GetHashCode()];
				result.Add(new NameValuePair<string> { Name = name, Value = f.GetHashCode().ToString() });
			}
			return result;
		}
	}  
}
