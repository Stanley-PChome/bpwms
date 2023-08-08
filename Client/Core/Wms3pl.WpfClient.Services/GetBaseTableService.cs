using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Xml.Linq;
using System.Xml.XPath;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;

namespace Wms3pl.WpfClient.Services
{
	public partial class GetBaseTableService 
	{
		#region 取 F000904　程式下拉選單參數設定
		public static List<NameValuePair<string>> GetF000904List(string functionCode, string topic, string subtopic = null, bool allItem = false)
		{
			var proxyEx = ConfigurationExHelper.GetExProxy<ShareExDataSource>(false, functionCode);
			var data = proxyEx.CreateQuery<F000904>("GetF000904List").AddQueryExOption("topic", topic).AddQueryExOption("subtopic", subtopic).ToList();
			var list = (from o in data
						select new NameValuePair<string>
						{
							Name = o.NAME,
							Value = o.VALUE
						}).ToList();
			if (allItem)
				list.Insert(0, new NameValuePair<string> { Name = "全部", Value = "" });
			return list;
		}
		#endregion

		#region 取業主列印發票報表檔
		public static string GetInvoiceReportType(string gupCode, string custCode, string functionCode)
		{
			var proxy = ConfigurationHelper.GetProxy<F19Entities>(false, functionCode);
			var f1909 =
					proxy.F1909s.Where(n => n.GUP_CODE == gupCode && n.CUST_CODE == custCode).ToList().FirstOrDefault();

			if (f1909 == null || string.IsNullOrEmpty(f1909.INVO_REPORT))
			{				
				return string.Empty;
			}
			return f1909.INVO_REPORT;
		}
		#endregion

    public static List<NameValuePair<string>> GetMoveOutDcList(string functionCode)
    {
      var proxy = ConfigurationHelper.GetProxy<F00Entities>(false, functionCode);
      var data = proxy.F0001s.Select(o => new NameValuePair<string> {
        Name = o.CROSS_NAME,
        Value = o.CROSS_CODE
      }).ToList();

      data.Insert(0, new NameValuePair<string> { Name = "全部", Value = "" });

      return data;
    }
	}
}
