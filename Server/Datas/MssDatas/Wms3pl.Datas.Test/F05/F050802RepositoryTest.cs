using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
	[TestClass]
	public class F050802RepositoryTest : BaseRepositoryTest
	{
		private F050802Repository _f050802Repo;
		public F050802RepositoryTest()
		{
			_f050802Repo = new F050802Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetF050802s()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var wmsOrdNo = "O2017022000005";
			#endregion

			_f050802Repo.GetF050802s(dcCode, gupCode, custCode, wmsOrdNo);
		}



		[TestMethod]
		public void GetShippingItem()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var itemCodes = "BB020203";
			var wmsOrderNos = "O2017022000001,O2017022000003,O2017022000004";
			#endregion

			_f050802Repo.GetShippingItem(dcCode, gupCode, custCode, itemCodes, wmsOrderNos);
		}

		[TestMethod]
		public void GetGroupItem()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var wmsOrdNo = "O2017022200028";
			#endregion

			_f050802Repo.GetGroupItem(dcCode, gupCode, custCode, wmsOrdNo);
		}

		[TestMethod]
		public void HasSerialItem()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var wmsOrdNo = "O2017030800055";
			#endregion

			_f050802Repo.HasSerialItem(dcCode, gupCode, custCode, wmsOrdNo);
		}

		[TestMethod]
		public void GetDatasByHasSerial()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var wmsOrdNo = "O2020090100003";
			#endregion

			_f050802Repo.GetDatasByHasSerial(dcCode, gupCode, custCode, wmsOrdNo);
		}

		[TestMethod]
		public void GetDatas()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var wmsOrdNos = new List<string> { "O2017030600171", "O2017030600170" };
			#endregion

			_f050802Repo.GetDatas(dcCode, gupCode, custCode, wmsOrdNos);
		}
	}
}
