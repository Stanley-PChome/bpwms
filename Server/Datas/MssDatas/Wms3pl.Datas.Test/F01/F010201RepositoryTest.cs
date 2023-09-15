using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F01;
using System;
using System.Collections.Generic;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.Datas.Test.F01
{
	[TestClass]
	public partial class F010201RepositoryTest : BaseRepositoryTest
	{
		private F010201Repository _f010201Repo;
		public F010201RepositoryTest()
		{
			_f010201Repo = new F010201Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetF010201Datas()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";

			// 以下可空
			var begStockDate = "2017-02-21";
			var endStockDate = "2020-09-03";
			var stockNo = "";
			var vnrCode = "";
			var vnrName = "";
			var custOrdNo = "";
			var sourceNo = "";
			var status = "";
			#endregion

			//_f010201Repo.GetF010201Datas(dcCode, gupCode, custCode, begStockDate, endStockDate, stockNo, vnrCode, vnrName, custOrdNo, sourceNo, status);
		}

		[TestMethod]
		public void GetF010201SourceNo()
		{
			#region Params
			string sourceNo = null;
			#endregion

			_f010201Repo.GetF010201SourceNo(sourceNo);
		}

		[TestMethod]
		public void DeleteF010201()
		{
			#region Params
			var stockNo = "A2017022000001";
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			#endregion

			_f010201Repo.DeleteF010201(stockNo, dcCode, gupCode, custCode);
		}

		[TestMethod]
		public void FindInHouseF010201()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var stockNo = "A2017022000001";
			#endregion

			_f010201Repo.FindInHouseF010201(dcCode, gupCode, custCode, stockNo);
		}

		[TestMethod]
		public void GetVendorInfo()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var stockNo = "A2017033000003";
			#endregion

			_f010201Repo.GetVendorInfo(stockNo, dcCode, gupCode, custCode);
		}

		[TestMethod]
		public void GetInWarehouseReport()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var purchaseNo = "A2017032300010";
			#endregion

			_f010201Repo.GetInWarehouseReport(dcCode, gupCode, custCode, purchaseNo);
		}

		[TestMethod]
		public void ExistsNonCancelByItemCode()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			var itemCode = "BB070105";
			#endregion

			_f010201Repo.ExistsNonCancelByItemCode(gupCode, custCode, itemCode);
		}

		[TestMethod]
		public void ExistsNonCancelByVendor()
		{
			#region Params
			var gupCode = "01";
			var vnrCode = "FCW";
			#endregion

			_f010201Repo.ExistsNonCancelByVendor(gupCode, vnrCode);
		}

		[TestMethod]
		public void ExistsF020301Data()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var purchaseNo = "A2020070900001";
			var itemCode = "SN001";
			#endregion

			_f010201Repo.ExistsF020301Data(dcCode, gupCode, custCode, purchaseNo, itemCode);
		}

		[TestMethod]
		public void GetDcWmsNoOrdPropItems()
		{
			#region Params
			var dcCode = "001";
			var stockDate = new DateTime(2018, 09, 11);
			#endregion

			_f010201Repo.GetDcWmsNoOrdPropItems(dcCode, stockDate);
		}

		[TestMethod]
		public void GetDcWmsNoDateItems()
		{
			#region Params
			var dcCode = "001";
			var gupCpde = "01";
			var custCode = "010001";
			var begStockDate = new DateTime(2019, 09, 09);
			var endStockDate = new DateTime(2020, 09, 09);
			#endregion

			_f010201Repo.GetDcWmsNoDateItems(dcCode, gupCpde, custCode, begStockDate, endStockDate);
		}

		[TestMethod]
		public void ExistsEmptyShopNoByBundleSerialNo()
		{
			#region Params
			var dcCode = "001";
			var gupCpde = "01";
			var custCode = "010001";
			var purchaseNo = "A2017032300006";
			#endregion

			_f010201Repo.ExistsEmptyShopNoByBundleSerialNo(dcCode, gupCpde, custCode, purchaseNo);
		}

		[TestMethod]
		public void GetOrderIsProblem()
		{
			#region Params
			var selectDate = new DateTime(2020, 09, 10);
			#endregion

			_f010201Repo.GetOrderIsProblem(selectDate);
		}

		[TestMethod]
		public void GetDatasByCustOrdNoAndVnrCodeNotCancel()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var custOrdNo = "2019030800001";
			var vnrCode = "成心科技";
			#endregion

			_f010201Repo.GetDatasByCustOrdNoAndVnrCodeNotCancel(dcCode, gupCode, custCode, custOrdNo, vnrCode);
		}

		[TestMethod]
		public void GetEnabledStockData()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var stockNo = "A2019030800001";
			#endregion

			_f010201Repo.GetEnabledStockData(dcCode, gupCode, custCode, stockNo);
		}

		[TestMethod]
		public void GetPalletDatas()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var stockNo = "A2019040900001";
			#endregion

			_f010201Repo.GetPalletDatas(dcCode, gupCode, custCode, stockNo);
		}

    [TestMethod]
    public void CancelNotProcessWarehouseIn()
    {
      #region Params
      var dcCode = "001";
      var gupCode = "01";
      var custCode = "010001";
      var stockNo = "A2019040900001";
      var importFlag = "G";
      #endregion

      _f010201Repo.CancelNotProcessWarehouseIn(dcCode, gupCode, custCode, stockNo, importFlag);
    }
  }
}
