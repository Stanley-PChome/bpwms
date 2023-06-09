using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.Common
{
	public static class ValidateHelper
	{
		/// <summary>
		/// 文字是否只包含英文與數字
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static bool IsMatchAZaz09(string text)
		{
			if (text == null)
				return false;

			return Regex.IsMatch(text, @"^[a-zA-Z0-9]+$");
		}


		/// <summary>
		/// 文字是否只包含英文與數字
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static bool IsMatchAZaz09Dash(string text)
		{
			if (text == null)
				return false;

			return Regex.IsMatch(text, @"^[a-zA-Z0-9\d-]+$");
		}
		/// <summary>
		/// 文字是否只包含數字
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static bool IsMatchNumber(string text)
		{
			if (text == null)
				return false;

			return Regex.IsMatch(text, @"^\d+$");
		}

		/// <summary>
		/// 文字是否為電話
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static bool IsMatchPhone(string text)
		{
			if (text == null)
				return false;

			return Regex.IsMatch(text, @"^[\d\@\/\,\*\.\+\-\(\)\#\:\s]+$");
		}

		/// <summary>
		/// 帳號是否只包含國際銀行帳戶號碼的字元
		/// </summary>
		/// <param name="account"></param>
		/// <returns></returns>
		public static bool IsMatchIBAN(string account)
		{
			if (account == null)
				return false;

			return Regex.IsMatch(account, @"^[a-zA-Z]{0,4}\s?[a-zA-Z]{0,4}\s?[0-9\-\s]+$");
		}

		/// <summary>
		/// 文字是否只包含英文
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static bool IsMatchAZaz(string text)
		{
			if (text == null)
				return false;

			return Regex.IsMatch(text, @"^[a-zA-Z]+$");
		}

		/// <summary>
		/// 驗證文字是否符合HH:mm
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static bool IsMatchHHmm(string text)
		{
			if (text == null)
				return false;

			return Regex.IsMatch(text, @"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");
		}

		/// <summary>
		/// 檢查起訖有誤回傳錯誤訊息，若是參考型別，其中一個沒帶，則會起訖相等(會改變屬性值)
		/// </summary>
		/// <typeparam name="TClass"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="obj"></param>
		/// <param name="propertyBeginExpression"></param>
		/// <param name="propertyEndExpression"></param>
		/// <param name="name"></param>
		/// <param name="errorMsg"></param>
		/// <param name="isSetNullProperty">預設會將 null 的互帶相等值</param>
		/// <returns></returns>
		public static bool TryCheckBeginEnd<TClass, TProperty>(TClass obj, Expression<Func<TClass, TProperty>> propertyBeginExpression, Expression<Func<TClass, TProperty>> propertyEndExpression, string name, out string errorMsg, bool isAutoChangeBeginEnd = false, bool isSetNullProperty = true)
		{
			return TryCheckBeginEnd(obj, propertyBeginExpression, propertyEndExpression, name, out errorMsg, name, isAutoChangeBeginEnd, isSetNullProperty);
		}

		/// <summary>
		/// 檢查起訖，若起始大於結束，預設會自動交換
		/// </summary>
		/// <typeparam name="TClass"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="obj"></param>
		/// <param name="propertyBeginExpression"></param>
		/// <param name="propertyEndExpression"></param>
		/// <param name="isAutoChangeBeginEnd"></param>
		/// <param name="isSetNullProperty"></param>
		public static void AutoChangeBeginEnd<TClass, TProperty>(TClass obj, Expression<Func<TClass, TProperty>> propertyBeginExpression, Expression<Func<TClass, TProperty>> propertyEndExpression, bool isAutoChangeBeginEnd = true, bool isSetNullProperty = true)
		{
			string errorMsg;
			TryCheckBeginEnd(obj, propertyBeginExpression, propertyEndExpression, string.Empty, out errorMsg, string.Empty, isAutoChangeBeginEnd, isSetNullProperty);
		}

		public static bool TryCheckBeginEnd<TClass, TProperty>(TClass obj, Expression<Func<TClass, TProperty>> propertyBeginExpression, Expression<Func<TClass, TProperty>> propertyEndExpression, string beginName, out string errorMsg, string endName, bool isAutoChangeBeginEnd = false, bool isSetNullProperty = true)
		{
			errorMsg = string.Empty;

			#region Check Exception
			//if (propertyBeginExpression == null || propertyEndExpression == null)
			//{
			//	throw new ArgumentNullException("propertyExpression");
			//}

			var body1 = propertyBeginExpression.Body as MemberExpression;
			var body2 = propertyEndExpression.Body as MemberExpression;

			//if (body1 == null || body2 == null)
			//{
			//	throw new ArgumentException("Invalid argument", "propertyExpression");
			//}

			var property1 = body1.Member as PropertyInfo;
			var property2 = body2.Member as PropertyInfo;

			//if (property1 == null || property2 == null)
			//{
			//	throw new ArgumentException("Argument is not a property", "propertyExpression");
			//}
			#endregion


			if (typeof(TProperty) == typeof(string))	// string
			{
				var begin = Convert.ToString(property1.GetValue(obj));
				var end = Convert.ToString(property2.GetValue(obj));

				if (begin == null) begin = string.Empty;
				if (end == null) end = string.Empty;

				begin = begin.Trim();
				end = end.Trim();

				if (isSetNullProperty)
				{
					if (!string.IsNullOrEmpty(begin) && string.IsNullOrEmpty(end))
						end = begin;

					if (string.IsNullOrEmpty(begin) && !string.IsNullOrEmpty(end))
						begin = end;
				}

				property1.SetValue(obj, begin);
				property2.SetValue(obj, end);

				if (begin.CompareTo(end) > 0)
				{
					if (isAutoChangeBeginEnd)
					{
						property1.SetValue(obj, end);
						property2.SetValue(obj, begin);
					}
					else
					{
						errorMsg = FormatBeginEndErrorMsg(beginName, endName);
					}
				}
			}
			else if (typeof(TProperty).IsValueType || Nullable.GetUnderlyingType(typeof(TProperty)) != null)	// nullable or value type
			{
				var begin = property1.GetValue(obj);
				var end = property2.GetValue(obj);

				if (isSetNullProperty)
				{
					if (begin != null && end == null)
						end = begin;

					if (begin == null && end != null)
						begin = end;

					property1.SetValue(obj, begin);
					property2.SetValue(obj, end);
				}

				if (begin != null && end != null && (begin as IComparable).CompareTo(end) > 0)
				{
					if (isAutoChangeBeginEnd)
					{
						property1.SetValue(obj, end);
						property2.SetValue(obj, begin);
					}
					else
					{
						errorMsg = FormatBeginEndErrorMsg(beginName, endName);
					}
				}
			}

			return string.IsNullOrEmpty(errorMsg);
		}


		private static string FormatBeginEndErrorMsg(string beginName, string endName = "")
		{
			if (string.IsNullOrEmpty(endName))
				endName = beginName;

			return string.Format("起始{0}不可大於結束{1}", beginName, endName);
		}

		/// <summary>
		/// 檢查起訖有誤回傳錯誤訊息，若是參考型別，其中一個沒帶，則會起訖相等(會改變屬性值)
		/// </summary>
		/// <typeparam name="TClass"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="obj"></param>
		/// <param name="propertyBeginExpression"></param>
		/// <param name="propertyEndExpression"></param>
		/// <param name="name"></param>
		/// <param name="errorMsg"></param>
		/// <param name="isSetNullProperty">預設會將 null 的互帶相等值</param>
		/// <returns></returns>
		public static bool TryCheckBeginEndForLoc<TClass, TProperty>(TClass obj, Expression<Func<TClass, TProperty>> propertyBeginExpression, Expression<Func<TClass, TProperty>> propertyEndExpression, string name, out string errorMsg, bool isAutoChangeBeginEnd = false, bool isSetNullProperty = true)
		{
			return TryCheckBeginEndForLoc(obj, propertyBeginExpression, propertyEndExpression, name, out errorMsg, name, isAutoChangeBeginEnd, isSetNullProperty);
		}
		/// <summary>
		/// 檢查起訖，若起始大於結束，預設會自動交換
		/// </summary>
		/// <typeparam name="TClass"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="obj"></param>
		/// <param name="propertyBeginExpression"></param>
		/// <param name="propertyEndExpression"></param>
		/// <param name="isAutoChangeBeginEnd"></param>
		/// <param name="isSetNullProperty"></param>
		public static void AutoChangeBeginEndForLoc<TClass, TProperty>(TClass obj, Expression<Func<TClass, TProperty>> propertyBeginExpression, Expression<Func<TClass, TProperty>> propertyEndExpression, bool isAutoChangeBeginEnd = true, bool isSetNullProperty = true)
		{
			string errorMsg;
			TryCheckBeginEndForLoc(obj, propertyBeginExpression, propertyEndExpression, string.Empty, out errorMsg, string.Empty, isAutoChangeBeginEnd, isSetNullProperty);
		}

		public static bool TryCheckBeginEndForLoc<TClass, TProperty>(TClass obj, Expression<Func<TClass, TProperty>> propertyBeginExpression, Expression<Func<TClass, TProperty>> propertyEndExpression, string beginName, out string errorMsg, string endName, bool isAutoChangeBeginEnd = false, bool isSetNullProperty = true)
		{
			errorMsg = string.Empty;

			#region Check Exception
			//if (propertyBeginExpression == null || propertyEndExpression == null)
			//{
			//	throw new ArgumentNullException("propertyExpression");
			//}

			var body1 = propertyBeginExpression.Body as MemberExpression;
			var body2 = propertyEndExpression.Body as MemberExpression;

			//if (body1 == null || body2 == null)
			//{
			//	throw new ArgumentException("Invalid argument", "propertyExpression");
			//}

			var property1 = body1.Member as PropertyInfo;
			var property2 = body2.Member as PropertyInfo;

			//if (property1 == null || property2 == null)
			//{
			//	throw new ArgumentException("Argument is not a property", "propertyExpression");
			//}
			#endregion


			if (typeof(TProperty) == typeof(string))	// string
			{
				var begin = Convert.ToString(property1.GetValue(obj));
				var end = Convert.ToString(property2.GetValue(obj));
			


				if (begin == null) begin = string.Empty;
				if (end == null) end = string.Empty;

				begin = begin.Trim();
				end = end.Trim();

				if (isSetNullProperty)
				{
					if (!string.IsNullOrEmpty(begin) && string.IsNullOrEmpty(end))
						end = begin;

					if (string.IsNullOrEmpty(begin) && !string.IsNullOrEmpty(end))
						begin = end;
				}

				if (begin.Length > 0 && !IsMatchAZaz09(begin))
				{
					errorMsg = beginName + "格式錯誤";
					return string.IsNullOrEmpty(errorMsg);
				}
				else if (end.Length > 0 && !IsMatchAZaz09(end))
				{
					errorMsg = endName + "格式錯誤";
					return string.IsNullOrEmpty(errorMsg);
				}

				property1.SetValue(obj, begin);
				property2.SetValue(obj, end);

				if (begin.CompareTo(end) > 0)
				{
					if (isAutoChangeBeginEnd)
					{
						property1.SetValue(obj, end);
						property2.SetValue(obj, begin);
					}
					else
					{
						errorMsg = FormatBeginEndErrorMsg(beginName, endName);
					}
				}
			}
			return string.IsNullOrEmpty(errorMsg);
		}
		

	}
}
