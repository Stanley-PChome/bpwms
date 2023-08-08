using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F15;
using Wms3pl.WebServices.DataCommon;
using System;
using System.Collections.Generic;


namespace Wms3pl.Datas.Test.F15
{
	[TestClass]
	public class F151001RepositoryTest : BaseRepositoryTest
	{
		private F151001Repository _f151001Repo;
		public F151001RepositoryTest()
		{
			_f151001Repo = new F151001Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetF151001Datas()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "030001";
			var sourceNo = "A2018081400006";
			#endregion

			_f151001Repo.GetF151001Datas(dcCode, gupCode, custCode, sourceNo);
		}

	

		[TestMethod]
		public void GetF151001ReportData()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var allocationNo = "T2017030700005";
			var isShowValidDate = false;
			#endregion

			_f151001Repo.GetF151001ReportData(dcCode, gupCode, custCode, allocationNo, isShowValidDate);
		}

		[TestMethod]
		public void GetF151001ReportDataByExpendDate()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var allocationNo = "T2017030600009";
			#endregion

			_f151001Repo.GetF151001ReportDataByExpendDate(dcCode, gupCode, custCode, allocationNo);
		}

		[TestMethod]
		public void GetF1510Data()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "030001";
			var allocationNo = "T2018081400023";
			var allocationDate = "2018/08/14";
			var status = "3";
			var userId = "wms";
			var makeNo = "0";
			var enterDate = Convert.ToDateTime("2018-08-14");
			var srcLocCode = "10Q010101";


			#endregion

			_f151001Repo.GetF1510Data(dcCode, gupCode, custCode, allocationNo,
					allocationDate, status, userId, makeNo, enterDate, srcLocCode);
		}

		[TestMethod]
		public void GetDatasByTar()
		{
			#region Params
			var tarDcCode = "001";
			var gupCode = "01";
			var custCode = "030001";
			var allocationDate = Convert.ToDateTime("2018-08-23");
			var status = new string[] { "3" };
			#endregion

			_f151001Repo.GetDatasByTar(tarDcCode, gupCode,
					custCode, allocationDate, status);
		}

		[TestMethod]
		public void GetF1510DatasByTar()
		{
			#region Params
			var tarDcCode = "001";
			var gupCode = "01";
			var custCode = "030001";
			var allocationNo = "T2018082300001";
			var allocationDate = Convert.ToDateTime("2018-08-23");
			#endregion

			_f151001Repo.GetF1510DatasByTar(tarDcCode, gupCode, custCode, allocationNo, allocationDate);
		}

		[TestMethod]
		public void GetF1510BundleSerialLocDatas()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var allocationNo = "T2020092400007";
			var checkSerialNo = "0";
			var userId = "System";
			#endregion

			_f151001Repo.GetF1510BundleSerialLocDatas(dcCode, gupCode, custCode,
			allocationNo, checkSerialNo, userId);
		}

		[TestMethod]
		public void GetF1510ItemLocDatas()
		{
			#region Params
			var tarDcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var allocationNo = "T2020010600002";
			var status = new string[] { "3" };
			var itemCode = "BB020101";
			var validDate = Convert.ToDateTime("2023-09-17");
			var srcLocCode = "10Q010101";
			var makeNo = "AS0152";
			#endregion

			_f151001Repo.GetF1510ItemLocDatas(tarDcCode, gupCode, custCode,
			allocationNo, status, itemCode, validDate, srcLocCode, makeNo);
		}

		[TestMethod]
		public void GetDatasBySourceNo()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "030001";
			var sourceNo = "A2018081400006";
			#endregion

			_f151001Repo.GetDatasBySourceNo(dcCode, gupCode, custCode, sourceNo);
		}

		[TestMethod]
		public void GetDcWmsNoOrdPropItems()
		{
			#region Params
			var dcCode = "001";
			var allocationDate = Convert.ToDateTime("2018-08-16");
			#endregion

			_f151001Repo.GetDcWmsNoOrdPropItems(dcCode, allocationDate);
		}

		[TestMethod]
		public void GetAllocationBundleSerialLocCount()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var allocationNo = "T2020092400002";
			var userId = "System";
			#endregion

			_f151001Repo.GetAllocationBundleSerialLocCount(dcCode, gupCode,
			custCode, allocationNo, userId);
		}

		[TestMethod]
		public void GetF15100101Data()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var allocationNo = "T2020100500001";
			var userId = "System";
			#endregion

			_f151001Repo.GetF15100101Data(dcCode, gupCode, custCode, allocationNo,
			userId);
		}

		[TestMethod]
		public void DeleteF151001Datas()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "030001";
			var allocationNos = new List<string> { "T2018100200006" };
			#endregion

			_f151001Repo.DeleteF151001Datas(gupCode, custCode, dcCode, allocationNos);
		}

		[TestMethod]
		public void UpdateDatasForCancel()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			var dcCode = "001";
			var ordNos = new List<string> { "T2017030600009" };
			#endregion

			_f151001Repo.UpdateDatasForCancel(gupCode, custCode, dcCode, ordNos);
		}


		[TestMethod]
		public void GetHealthInsurancePurchaseData()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var startDate = Convert.ToDateTime("2017-02-20");
			var endDate = Convert.ToDateTime("2017-03-04");
			var itemCodes = new string[] { "ISMKFBAG" };
			#endregion

			_f151001Repo.GetHealthInsurancePurchaseData(dcCode, gupCode, custCode, startDate, endDate, itemCodes);
		}


		[TestMethod]
		public void GetDatas()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "030001";
			var allocationNo = new List<string> { "T2018081400022" };
			#endregion

			_f151001Repo.GetDatas(dcCode, gupCode, custCode, allocationNo);
		}

		#region F151001Repository.part1
		[TestMethod]
		public void GetF150201CSV()
		{
			#region Params
			var gupCode = "01";
			var custCode = "030001";
			var SourceDcCode = "001";
			var TargetDcCode = "001";
			var CRTDateS = Convert.ToDateTime("2018-07-18");
			var CRTDateE = Convert.ToDateTime("2018-07-20");
			var TxtSearchAllocationNo = "201807180006";
			var PostingDateS = Convert.ToDateTime("2018-07-18");
			var PostingDateE = Convert.ToDateTime("2018-07-20");
			var SourceWarehouseList = "I01";
			var TargetWarehouseList = "G07";
			var StatusList = "2";
			var TxtSearchSourceNo = "A2018071800004";


			#endregion

			_f151001Repo.GetF150201CSV(gupCode, custCode, SourceDcCode, TargetDcCode, CRTDateS, CRTDateE,
					TxtSearchAllocationNo, PostingDateS, PostingDateE, SourceWarehouseList, TargetWarehouseList,
					 StatusList, TxtSearchSourceNo);
		}
		#endregion

		[TestMethod]
		public void GetSingleData()
		{
			#region Params
			var dcNo = "001";
			var custNo = "030001";
			var gupCode = "01";
			var allocNo = "T2018081400022";
			#endregion

			_f151001Repo.GetSingleData(dcNo, custNo, gupCode, allocNo);
		}

		[TestMethod]
		public void PostAllocUpdate()
		{
			#region Params
			var dcNo = "001";
			var custNo = "030001";
			var gupCode = "01";
			var allocNo = "T2018081400022";
			var tarMoveStaff = "A12345";
			var tarMoveName = "A12345";
			var srcMoveStaff = "A12345";
			var srcMoveName = "A12345";
			var status = "1";
			var lockStatus = "3";
			#endregion

			_f151001Repo.PostAllocUpdate(dcNo, custNo, gupCode, allocNo, tarMoveStaff, tarMoveName
					, srcMoveStaff, srcMoveName, status, lockStatus);
		}
	}
}
