using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.Common.Helpers
{
	public static class CommonDataHelper
	{
		/// <summary>
		/// 秤重機清單
		/// </summary>
		/// <returns></returns>
		public static List<NameValuePair<string>> WeightingList()
		{
			var result = PrinterSettings.InstalledPrinters.Cast<string>()
															.Select(printerName => new NameValuePair<string>(printerName, printerName))
															.ToList();
			result.Insert(0, new NameValuePair<string>() { Name = "", Value = "" });
			return result;
		}

		/// <summary>
		/// 印表機清單
		/// </summary>
		/// <returns></returns>
		public static List<NameValuePair<string>> PrinterList()
		{
			var result = PrinterSettings.InstalledPrinters.Cast<string>()
														  .Select(printerName => new NameValuePair<string>(printerName, printerName))
														  .ToList();
			result.Insert(0, new NameValuePair<string>() { Name = "", Value = "" });
			return result;
		}

		/// <summary>
		/// 標籤機清單
		/// </summary>
		/// <returns></returns>
		public static List<NameValuePair<string>> LabelingList()
		{
			var result = PrinterSettings.InstalledPrinters.Cast<string>()
															.Select(printerName => new NameValuePair<string>(printerName, printerName))
															.ToList();
			result.Insert(0, new NameValuePair<string>() { Name = "", Value = "" });
			return result;
		}
	}
}
