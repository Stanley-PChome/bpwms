using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.DataCommon
{
	public static class DateTimeHelper
	{
		#region Private Property
		private static List<String> _defaultFormatException;
		/// <summary>
		/// 預設日期格式
		/// </summary>
		private static List<String> defaultFormatException
		{
			get
			{
				if (_defaultFormatException == null)
				{
					_defaultFormatException = new List<String>
					{
						"yyyy/MM/dd",
						"yyyyMMdd",
						"yyyy/MM",
						"yyyyMM",
						"yyyy/M/d",
						"yyyy/M/d tt hh:mm:ss",
						"yyyy/MM/dd tt hh:mm:ss",
						"yyyy/MM/dd HH:mm:ss",
						"yyyy/M/d HH:mm:ss",
						"yyy/MM",
						"yy/MM"
					};
				}
				return _defaultFormatException;
			}
		}
		#endregion

		#region 物件型態

		#region 檢驗
		/// <summary>
		/// 預設日期格式檢查
		/// </summary>
		public static bool CheckDate(string txtDateStart)
		{
			return CheckDate(txtDateStart, defaultFormatException);
		}
		/// <summary>
		/// 日期格式檢查
		/// </summary>
		/// <param name="txtDateStart">欲檢查的日期字串</param>
		/// <param name="formatException">檢查的日期格式</param>
		public static bool CheckDate(string txtDateStart, string formatException)
		{
			return CheckDate(txtDateStart, new List<string>() { formatException });
		}
		/// <summary>
		/// 日期格式檢查
		/// </summary>
		/// <param name="txtDateStart">欲檢查的日期字串</param>
		/// <param name="formatException">檢查的日期格式(若可能有多種輸入情形)</param>
		public static bool CheckDate(string txtDateStart, List<string> formatException)
		{
			DateTime tmp;
			if (String.IsNullOrEmpty(txtDateStart) ||
				!DateTime.TryParseExact(txtDateStart,
																formatException.ToArray(),
																System.Globalization.CultureInfo.InvariantCulture,
																System.Globalization.DateTimeStyles.None,
																out tmp))
			{
				return false;
			}
			return true;
		}
		#endregion

		#region 轉換
		/// <summary>
		/// 轉換字串為預設日期格式
		/// </summary>
		public static DateTime ConversionDate(string date)
		{
			return ConversionDate(date, defaultFormatException);
		}
		/// <summary>
		/// 轉換字串為指定日期格式
		/// </summary>
		/// <param name="date">欲轉換的日期字串</param>
		/// <param name="formatException">轉換的日期格式</param>
		public static DateTime ConversionDate(string date, string formatException)
		{
			return ConversionDate(date, new List<string>() { formatException });
		}
		/// <summary>
		/// 轉換字串為指定日期格式
		/// </summary>
		/// <param name="date">欲轉換的日期字串</param>
		/// <param name="formatException">轉換的日期格式(若可能有多種輸入情形)</param>
		public static DateTime ConversionDate(string date, List<string> formatException)
		{
			DateTime result;
			DateTime.TryParseExact(date,
														formatException.ToArray(),
														System.Globalization.CultureInfo.InvariantCulture,
														System.Globalization.DateTimeStyles.None,
														out result);
			return result;
		}
		#endregion

		#endregion

		#region 資料單位

		#region 轉換
		/// <summary>
		/// 西元年A.D轉民國R.O.C
		/// </summary>
		/// <returns>[年/月/日]</returns>
		public static string AnnoDominiToRocDate(DateTime date)
		{
			return (new System.Globalization.TaiwanCalendar()).GetYear(date) + date.ToString("/MM/dd");
		}
		#endregion

		#endregion

		#region Other
		/// <summary>
		/// 一次取一天, 取出指定時間區間
		/// </summary>
		public static IEnumerable<DateTime> EachDay(DateTime startDate, DateTime endDate)
		{
			for (var day = startDate.Date; day.Date <= endDate.Date; day = day.AddDays(1)) yield return day;
		}

		/// <summary>
		/// 一次取一天, 取出指定時間區間, 但是不會超過今天 (適用報表類資料,減少無資料的查詢時間)
		/// </summary>
		public static IEnumerable<DateTime> EachDayStopToday(DateTime startDate, DateTime endDate)
		{
			for (var day = startDate.Date;
				day.Date <= ((endDate.Date > DateTime.Now.Date) ? DateTime.Now.Date : endDate.Date);
				day = day.AddDays(1))
				yield return day;
		}

		/// <summary>
		/// 該年是否為閏年
		/// </summary>
		/// <param name="iYear">年分</param>
		/// <returns>[True/False]</returns>
		public static bool IsLeapYear(int iYear)
		{
			return (iYear % 4 == 0 && iYear % 100 != 0) || (iYear % 400 == 0);
		}

		/// <summary>
		/// 取得該月份的第一天 - Date(DateTime)
		/// </summary>
		/// <param name="theDate">該月份任何一天</param>
		/// <returns></returns>
		public static DateTime DateMonthFirstDate(DateTime theDate)
		{
			return new DateTime(theDate.Year, theDate.Month, 1);
		}
		/// <summary>
		/// 取得該月份的最後一天 - Day(int)
		/// </summary>
		/// <param name="theDate">該月份任何一天</param>
		/// <returns></returns>
		public static int DateMonthLastDay(DateTime theDate)
		{
			return DateMonthLastDate(theDate).Day;
		}
		/// <summary>
		/// 取得該月份的最後一天 - Date(DateTime)
		/// </summary>
		/// <param name="theDate">該月份任何一天</param>
		/// <returns></returns>
		public static DateTime DateMonthLastDate(DateTime theDate)
		{
			var colDate = theDate.AddMonths(1);
			return ((new DateTime(colDate.Year, colDate.Month, 1)).AddDays(-1));
		}

		/// <summary>
		/// 取得指定日期的最後一秒(查詢日期區間時，結束日期可用)
		/// </summary>
		public static DateTime GetLastSecond(DateTime theDate)
		{
			return theDate.Date.AddDays(1).AddSeconds(-1);
		}

		#endregion
	}
}
