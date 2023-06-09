using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Text;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.Services;

using Wms3pl.WpfClient.DataServices;
using System.Reflection;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;

namespace Wms3pl.WpfClient.P08.Services
{
	/// <summary>
	/// 各種Barcode的前置碼
	/// </summary>
    public static class BarcodeService
    {
		public static string Order = "O";//出貨單頭碼		

		public static bool IsOrder(string s)
		{
			if (string.IsNullOrWhiteSpace(s)) return false;
			return s.ToUpper().StartsWith(BarcodeService.Order);
		}

		/// <summary>
		/// 是否要求加箱
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static bool IsNewBox(string s)
		{
			return s.ToUpper() == "NEWBOX";
		}
	}
}
