using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Text;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;

using Wms3pl.WpfClient.DataServices;
using System.Reflection;
using Wms3pl.WpfClient.P02.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.DataServices.F01DataService;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.DataServices.F19DataService;

namespace Wms3pl.WpfClient.P02.Services
{
	public static class PiersService
	{

		/// <summary>
		/// 查詢允許進貨的碼頭清單. 依照所選取的DC_CODE
		/// 主要搜尋F1981, 若F020104有資料, 則該碼頭依F020104的設定區間
		/// </summary>
		public static List<NameValuePair<string>> AllowInPiers(string dcCode, DateTime date, string functionCode)
		{
			return AllowPiers(dcCode, date, functionCode, true);
		}

		/// <summary>
		/// 查詢允許出貨的碼頭清單. 依照所選取的DC_CODE
		/// 主要搜尋F1981, 若F020104有資料, 則該碼頭依F020104的設定區間
		/// </summary>
		public static List<NameValuePair<string>> AllowOutPiers(string dcCode, DateTime date, string functionCode)
		{
			return AllowPiers(dcCode, date, functionCode, false);
		}

		/// <summary>
		/// 查詢允許進貨的碼頭清單. 依照所選取的DC_CODE
		/// 主要搜尋F1981, 若F020104有資料, 則該碼頭依F020104的設定區間
		/// </summary>
		private static List<NameValuePair<string>> AllowPiers(string dcCode, DateTime date, string functionCode,bool isIn)
		{
			// 暫存檔, 放入搜尋結果
			var result = new List<NameValuePair<string>>();

			// 先取出F1981主檔
			var proxyF19 = ConfigurationHelper.GetProxy<F19Entities>(false, functionCode);
			var f1981s = proxyF19.F1981s.Where(x => x.DC_CODE.Equals(dcCode));
			// 再取出F020104設定檔
			var proxyF02 = ConfigurationHelper.GetProxy<F02Entities>(false, functionCode);
			var tmpPierSetting = proxyF02.F020104s.Where(x => x.DC_CODE.Equals(dcCode)).ToList();
			DateTime dt = date.Date;
			foreach (var f1981 in f1981s)
			{
				// 比對碼頭是否存在期間設定，如果是且為進/出條件 符合條件則加入選單中
				var tmp = tmpPierSetting.FirstOrDefault(x => x.PIER_CODE == f1981.PIER_CODE && x.BEGIN_DATE.CompareTo(dt) <= 0 && x.END_DATE.CompareTo(dt) >= 0);
				if (tmp != null && (isIn && tmp.ALLOW_IN == "1") || (!isIn && tmp.ALLOW_OUT == "1"))
					result.Add(new NameValuePair<string> { Name = f1981.PIER_CODE, Value = f1981.PIER_CODE });
				// 如果不存在期間設定，則抓此碼頭預設進/出條件 符合條件則加入選單中
				else if ((isIn && f1981.ALLOW_IN == "1") || (!isIn && f1981.ALLOW_OUT == "1"))
					result.Add(new NameValuePair<string> { Name = f1981.PIER_CODE, Value = f1981.PIER_CODE });
				else
					continue;
			}
			return result.OrderBy(x => x.Name).ToList();
		}

		/// <summary>
		/// 取得碼頭編號清單
		/// </summary>
		/// <param name="dcCode">dcCode</param>
		/// <returns></returns>
		public static List<NameValuePair<string>> PiersList(string functionCode, string dcCode = null)
		{
			// 取出F1981主檔
			var proxyF19 = ConfigurationHelper.GetProxy<F19Entities>(false, functionCode);
			var result = proxyF19.F1981s.Where(x => x.DC_CODE.Equals(dcCode))
				.OrderBy(x => x.PIER_CODE)
				.Select(x => new NameValuePair<string>()
				{
					Name = x.PIER_CODE,
					Value = x.PIER_CODE
				}).ToList();
			return result;
		}

		/// <summary>
		/// 取得碼頭編號清單(含所有欄位)
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="pierCode"></param>
		/// <returns></returns>
		public static List<F1981> PiersList(string functionCode, string dcCode, string pierCode = null)
		{
			// 取出F1981主檔
			var proxyF19 = ConfigurationHelper.GetProxy<F19Entities>(false, functionCode);
			var result = proxyF19.F1981s.Where(x => x.DC_CODE.Equals(dcCode) && x.PIER_CODE.Equals(pierCode))
				.OrderBy(x => x.TEMP_AREA)
				.OrderBy(x => x.PIER_CODE)
				.ToList();
			return result;
		}
	}
}
