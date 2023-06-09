using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
	[TestClass]
	public class F050101RepositoryTest : BaseRepositoryTest
	{
		private F050101Repository _f050101Repo;
		public F050101RepositoryTest()
		{
			_f050101Repo = new F050101Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetF050101ExDatas()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			var dcCode = "001";
			var ordDateFrom = new DateTime(2017, 2, 20);
			var ordDateTo = new DateTime(2017, 2, 20);
			var ordNo = "S2017022000008";
			var arriveDateFrom = new DateTime(2017, 2, 20);
			var arriveDateTo = new DateTime(2017, 2, 20);
			var custOrdNo = "17021709290001F";
			var status = "A";
			string retailCode = null;
			var custName = "徐柔瑄";
			var wmsOrdNo = "O2017022000002";
			var pastNo = "1545141";
			var address = "";
			var channel = "";
			var delvType = "1";
			var allId = "";
			#endregion

			//_f050101Repo.GetF050101ExDatas(gupCode, custCode, dcCode, ordDateFrom, ordDateTo, ordNo, arriveDateFrom,
			//arriveDateTo, custOrdNo, status, retailCode, custName, wmsOrdNo, pastNo, address, channel, delvType, allId);
		}

		[TestMethod]
		public void GetF050102WithF050801s()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			var dcCode = "001";
			var wmsOrdNo = "O2017022000004";
			#endregion

			_f050101Repo.GetF050102WithF050801s(gupCode, custCode, dcCode, wmsOrdNo);
		}

		[TestMethod]
		public void GetDatasByCustOrdNoAndRetailCodeNotCancel()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "030001";
			var custOrdNo = "B2019071905BX";
			var retailCode = "AA";
			#endregion

			_f050101Repo.GetDatasByCustOrdNoAndRetailCodeNotCancel(dcCode, gupCode, custCode, custOrdNo, retailCode);
		}

		[TestMethod]
		public void GetDatasByUnApprove()
		{
			#region Params
			var gupCode = "01";
			var custCode = "030001";
			#endregion

			_f050101Repo.GetDatasByUnApprove(gupCode, custCode);
		}





		[TestMethod]
		public void UpdateLackToCancelOrder()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			#endregion

			_f050101Repo.UpdateLackToCancelOrder(gupCode, custCode);
		}
	}
}
