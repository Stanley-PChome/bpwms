using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Common.Helper
{
	public static class StringHelper
	{
		/// <summary>
		/// 是否為 Int32
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static bool IsInteger(string s)
		{
			int result;
			return int.TryParse(s, out result);
		}

		/// <summary>
		/// 強制轉換字串為 Int32，轉換失敗就回傳 defaultValue
		/// </summary>
		/// <param name="s"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static int ConvertToInteger(string s, int defaultValue = 0)
		{
			int result;
			if (int.TryParse(s, out result))
				return result;

			return defaultValue;
		}

		/// <summary>
		/// 強制轉換字串為 Int32，轉換失敗就回傳 defaultValue
		/// </summary>
		/// <param name="s"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static int? ConvertToIntegerNullable(string s, int? defaultValue = default(int?))
		{
			int result;
			if (int.TryParse(s, out result))
				return result;

			return defaultValue;
		}

		public static bool IsDateTime(string s)
		{
			DateTime result;
			return DateTime.TryParse(s, out result);
		}

		public static DateTime? ConvertToDateTimeNullable(string s, DateTime? defaultValue = null)
		{
			DateTime result;
			if (DateTime.TryParse(s, out result))
				return result;

			return defaultValue;
		}

		public static DateTime ConvertToDateTime(string s)
		{
			var result = ConvertToDateTimeNullable(s);
			if (result.HasValue)
				return result.Value;

			return default(DateTime);
		}
		/// <summary>
		/// 檢核資料是否有空白
		/// </summary>
		/// <param name="inputString"></param>
		/// <returns></returns>
		public static bool CheckStringIncludeWhiteSpace(string inputString)
		{
			if (inputString == null)
				return false;
			else if (inputString.IndexOf(" ") != -1)
				return true;
			else
				return false;
		}
	}
}
