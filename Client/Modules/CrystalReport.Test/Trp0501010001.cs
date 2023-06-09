using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.P08.Report;
using Wms3pl.WpfClient.UILib;


namespace CrystalReport.Test
{
	/// <summary>
	/// [揀貨單]
	/// </summary>
	[TestClass]
	public class Trp0501010001 : BaseTest
	{
		/*
		 * P0202030000
		 * P0806010000
		 * P0501010000
		 * P0501020000-UI
		 */

		[TestMethod]
		public void TestP0202030000()
		{
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var ordNo = "S2015061500009";


			var porxyEx = GetExProxy<P02ExDataSource>();
			var data = porxyEx.CreateQuery<Wms3pl.WpfClient.ExDataServices.P02ExDataService.F051201ReportDataA>("GetF051201ReportDataAs")
				.AddQueryExOption("dcCode", dcCode)
				.AddQueryExOption("gupCode", gupCode)
				.AddQueryExOption("custCode", custCode)
				.AddQueryExOption("ordNo", ordNo).ToList();

			data.ForEach(x =>
			{
				x.PickOrdNoBarcode = BarcodeConverter128.StringToBarcode(x.PICK_ORD_NO);
				x.WmsOrdNoBarcode = BarcodeConverter128.StringToBarcode(x.WMS_ORD_NO);
				x.ItemCodeBarcode = BarcodeConverter128.StringToBarcode(x.ITEM_CODE);
			});

			CallReport<RP0501010001>(data.ToDataTable());
		}



	



		[TestMethod]
		public void TestP0501010000()
		{
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var delvDate = new DateTime(2015, 6, 15);
			var pickTime = "16:34";
			var pickOrdNo = "P2015061500011";


			var proxyEx = GetExProxy<P05ExDataSource>();

			var list1 = proxyEx.GetF051201ReportDataAsForB2B(
				dcCode, gupCode, custCode,
				delvDate, pickTime, pickOrdNo).ToList();

			list1.ForEach(x =>
			{
				x.PickOrdNoBarcode = BarcodeConverter128.StringToBarcode(x.PICK_ORD_NO);
				x.WmsOrdNoBarcode = BarcodeConverter128.StringToBarcode(x.WMS_ORD_NO);
				x.ItemCodeBarcode = BarcodeConverter128.StringToBarcode(x.ITEM_CODE);
			});

			CallReport<RP0501010001>(list1.ToDataTable());
		}

	}
}
