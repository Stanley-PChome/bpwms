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
//using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WpfClient.P02.Services
{
    public static class ReportService
    {

		/// <summary>
		/// 產生驗收報表所需資料
		/// Memo: 商品檢驗驗收時產生報表使用, 應該可與驗收單查詢的報表共用, 所以拉出來做為Service
		/// </summary>
		public static List<AcceptancePurchaseReport> GetAcceptancePurchaseReport(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo, string functionCode,string isDefect,string isAcceptanceContainer)
		{
			var proxy = ConfigurationExHelper.GetExProxy<P02ExDataSource>(false, functionCode);
			var result = proxy.CreateQuery<AcceptancePurchaseReport>("GetAcceptancePurchaseReport")
				.AddQueryOption("dcCode", string.Format("'{0}'", dcCode))
				.AddQueryOption("gupCode", string.Format("'{0}'", gupCode))
				.AddQueryOption("custCode", string.Format("'{0}'", custCode))
				.AddQueryOption("purchaseNo", string.Format("'{0}'", purchaseNo))
				.AddQueryOption("rtNo", string.Format("'{0}'", rtNo))
				.AddQueryOption("isDefect", string.Format("'{0}'", isDefect))
				.AddQueryOption("isAcceptanceContainer", string.Format("'{0}'", isAcceptanceContainer))
				.ToList();
			return result;
		}

		/// <summary>
		/// 列印版箱標籤使用的資料集
		/// </summary>
		public class BoxLable
		{
			public string ItemCode { get; set; }
			public string ItemName { get; set; }
			/// <summary>
			/// 建議儲位
			/// </summary>
			public string Loc { get; set; }
			/// <summary>
			/// 箱件數
			/// </summary>
			public string Qty { get; set; }
			public string VnrCode { get; set; }
			public string VnrName { get; set; }
			public DateTime ValidDate { get; set; }
		}
    }
}
