using System;
using System.Text.RegularExpressions;

namespace Wms3pl.WebServices.DataCommon
{
	public static class DataCheckHelper
	{
		/// <summary>
		/// 檢核必填欄位
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="colName"></param>
		/// <returns></returns>
		public static bool CheckRequireColumn<T>(T obj, string colName)
		{
			var value = GetRequireColumnValue(obj, colName);

			return value != null && !string.IsNullOrWhiteSpace(Convert.ToString(value));
		}

		/// <summary>
		/// 檢核資料長度
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="colName"></param>
		/// <param name="maxLength"></param>
		/// <returns></returns>
		public static bool CheckDataMaxLength<T>(T obj, string colName, int maxLength)
		{
			var value = GetRequireColumnValue(obj, colName);

			return value == null ? true : maxLength >= Convert.ToString(value).Length;
		}

		/// <summary>
		/// 取得欄位資料
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="colName"></param>
		/// <returns></returns>
		public static object GetRequireColumnValue<T>(T obj, string colName)
		{
			object res = null;

			if (obj == null || string.IsNullOrWhiteSpace(colName))
			{
				return res;
			}

			return obj.GetType().GetProperty(colName).GetValue(obj);
		}

		/// <summary>
		/// 檢核資料是否為日期yyyy/MM/dd
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="colName"></param>
		/// <returns></returns>
		public static bool CheckDataIsDate<T>(T obj, string colName)
		{
			// yyyy/MM/dd
			Regex dateReg = new Regex(@"^(?:(?:(?:(?:(?:[13579][26]|[2468][048])00)|(?:[0-9]{2}(?:(?:[13579][26])|(?:[2468][048]|0[48]))))\/(?:(?:(?:09|04|06|11)\/(?:0[1-9]|1[0-9]|2[0-9]|30))|(?:(?:01|03|05|07|08|10|12)\/(?:0[1-9]|1[0-9]|2[0-9]|3[01]))|(?:02-(?:0[1-9]|1[0-9]|2[0-9]))))|(?:[0-9]{4}\/(?:(?:(?:09|04|06|11)\/(?:0[1-9]|1[0-9]|2[0-9]|30))|(?:(?:01|03|05|07|08|10|12)\/(?:0[1-9]|1[0-9]|2[0-9]|3[01]))|(?:02\/(?:[01][0-9]|2[0-8])))))$");
			string value = Convert.ToString(GetRequireColumnValue(obj, colName));
			var isOk = dateReg.IsMatch(value);
			// 針對正規化因為無法考慮閏年而檢核失敗，再進行DateTime.TryParse 確認是否為正確日期格式
			if (!isOk)
			{
				DateTime dt;
				isOk = DateTime.TryParse(value, out dt);
			}
			return isOk;
		}

		/// <summary>
		/// 檢核資料是否為日期yyyy/MM/dd HH:mm:ss
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="colName"></param>
		/// <returns></returns>
		public static bool CheckDataIsDateTime<T>(T obj, string colName)
		{
			// yyyy/MM/dd HH:mm:ss
			Regex dateReg = new Regex(@"^(?:(?:(?:(?:(?:[13579][26]|[2468][048])00)|(?:[0-9]{2}(?:(?:[13579][26])|(?:[2468][048]|0[48]))))\/(?:(?:(?:09|04|06|11)\/(?:0[1-9]|1[0-9]|2[0-9]|30))|(?:(?:01|03|05|07|08|10|12)\/(?:0[1-9]|1[0-9]|2[0-9]|3[01]))|(?:02-(?:0[1-9]|1[0-9]|2[0-9]))))|(?:[0-9]{4}\/(?:(?:(?:09|04|06|11)\/(?:0[1-9]|1[0-9]|2[0-9]|30))|(?:(?:01|03|05|07|08|10|12)\/(?:0[1-9]|1[0-9]|2[0-9]|3[01]))|(?:02\/(?:[01][0-9]|2[0-8]))))) (2[0-3]|[01]?[0-9]):([0-5]?[0-9]):([0-5]?[0-9])$");
			string value = Convert.ToString(GetRequireColumnValue(obj, colName));
			var isOk = dateReg.IsMatch(value);
			// 針對正規化因為無法考慮閏年而檢核失敗，再進行DateTime.TryParse 確認是否為正確日期格式
			if (!isOk)
			{
				DateTime dt;
				isOk = DateTime.TryParse(value, out dt);
			}
			return isOk;
		}

		/// <summary>
		/// 檢核資料是否為Decimal
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="colName"></param>
		/// <param name="firstCnt">小數前幾位</param>
		/// <param name="lastCnt">小數後幾位</param>
		/// <returns></returns>
		public static bool CheckDataIsDecimal<T>(T obj, string colName, int firstCnt, int lastCnt)
		{
			// decimal
			var reg = @"^(?!\.?$)\d{0,@p0}(\.\d{0,@p1})?$";
			reg = reg.Replace("@p0", firstCnt.ToString());
			reg = reg.Replace("@p1", lastCnt.ToString());

			Regex decimalReg = new Regex(reg);
			string value = Convert.ToString(GetRequireColumnValue(obj, colName));
			return decimalReg.IsMatch(value);
		}

		/// <summary>
		/// 檢核資料是否大於0
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="colName"></param>
		/// <returns></returns>
		public static bool CheckDataNotZero<T>(T obj, string colName)
		{
			var value = Convert.ToDecimal(GetRequireColumnValue(obj, colName));
			return value > 0;
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
