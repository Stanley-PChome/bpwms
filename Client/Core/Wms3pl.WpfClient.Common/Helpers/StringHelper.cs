using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.Common.Helpers
{
	public class StringHelper
	{
		/// <summary>
		/// 清除字串中包含特殊字元，在呼叫DataService會造成錯誤的字元
		/// </summary>
		public static string ClearUpDataServices(string text)
		{
			var specialCharacters = new char[] { '\\', '/', '.' };
			var temp = text.Split(specialCharacters, StringSplitOptions.RemoveEmptyEntries);
			return String.Join(string.Empty, temp);
		}

		public static IEnumerable<string> SplitDistinct(string text, params char[] separators)
		{
			if (string.IsNullOrEmpty(text)) return null;

			return text.Split(separators, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).Distinct();

		}

		public static string JoinSplitDistinct(string text, string separator)
		{
			if (string.IsNullOrEmpty(text)) return null;

			return string.Join(separator, SplitDistinct(text, separator.ToArray()));
		}
	}
}
