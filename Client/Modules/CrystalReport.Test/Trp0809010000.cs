using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.P08.Report;
using Wms3pl.WpfClient.UILib;

namespace CrystalReport.Test
{
	/// <summary>
	/// [物流中心出車明細表]
	/// </summary>
	[TestClass]
	public class Trp0809010000 : BaseTest
	{

		[TestMethod]
		public void TestP0809010000()
		{
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var takeDate = new DateTime(2015,6,17);
			var checkOutTime = "10:00";
			var allId = "HCT";

			var proxy = GetExProxy<P08ExDataSource>();

			var result = proxy.CreateQuery<F050801WithF700102>("GetF050801WithF700102sForReport")
							.AddQueryExOption("dcCode", dcCode)
							.AddQueryExOption("gupCode", gupCode)
							.AddQueryExOption("custCode", custCode)
							.AddQueryExOption("takeDate", takeDate)
							.AddQueryExOption("checkoutTime", checkOutTime)
							.AddQueryExOption("allId", allId)
							.AddQueryExOption("checkWmsStatus", "1")
							.ToList();


			var report = new RP0809010000();
			report.SetText("txtReportTitle", Wms3plSession.Get<GlobalInfo>().GupName + "－" + Wms3plSession.Get<GlobalInfo>().CustName + "  出車明細表");
			report.SetDataSource(result);
			CallReport(report);
		}
	}
}
