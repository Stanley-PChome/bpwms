using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
	[TestClass]
	public class F05030101RepositoryTest : BaseRepositoryTest
	{
		private F05030101Repository _f05030101Repo;
		public F05030101RepositoryTest()
		{
			_f05030101Repo = new F05030101Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetP060103Data()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			var dcCode = "001";
			var delvDate = new DateTime(2017, 8, 14);
			var pickTime = "10:47";
			var custOrdNo = "17081112185F241";
			var ordNo = "S2017081400341";
			var itemCode = "BB070504";
			#endregion

			_f05030101Repo.GetP060103Data(gupCode, custCode, dcCode, delvDate, pickTime, custOrdNo, ordNo, itemCode);
		}

		[TestMethod]
		public void GetF05030101WithF051202Data()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			var dcCode = "001";
			var ordNoList = new List<string> { "S2017022000024", "S2017022100156" };
			#endregion

			_f05030101Repo.GetF05030101WithF051202Data(gupCode, custCode, dcCode, ordNoList);
		}

		[TestMethod]
		public void GetWmsOrdNos()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			var dcCode = "001";
			var ordNo = "S2017022000003";
			#endregion

			_f05030101Repo.GetWmsOrdNos(gupCode, custCode, dcCode, ordNo);
		}

		[TestMethod]
		public void GetNonPickF051202ByOrdNo()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var ordNoList = new List<string> { "S2017030100401", "S2020070700002" };
			#endregion

			_f05030101Repo.GetNonPickF051202ByOrdNo(dcCode, gupCode, custCode, ordNoList);
		}

		[TestMethod]
		public void GetMergerOrders()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var ordNo = "S2017022000003";
			#endregion

			_f05030101Repo.GetMergerOrders(dcCode, gupCode, custCode, ordNo);
		}

		[TestMethod]
		public void GetDatas()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var wmsNo = "O2017032300007";
			var ordNo = "S2017032300007";
			#endregion

			_f05030101Repo.GetDatas(dcCode, gupCode, custCode, wmsNo, ordNo);
		}

		[TestMethod]
		public void GetDatas2()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var wmsOrdNos = new List<string> { "O2017030900126", "O2017032300007" };
			#endregion

			_f05030101Repo.GetDatas(dcCode, gupCode, custCode, wmsOrdNos);
		}
	}
}
