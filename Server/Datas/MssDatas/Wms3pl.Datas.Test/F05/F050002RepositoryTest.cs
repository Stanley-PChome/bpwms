using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
	[TestClass]
	public class F050002RepositoryTest : BaseRepositoryTest
	{
		private F050002Repository _f05000201Repo;
		public F050002RepositoryTest()
		{
			_f05000201Repo = new F050002Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetNotAllotedDatas()
		{
			_f05000201Repo.GetNotAllotedDatas();
		}

		[TestMethod]
		public void GetNotAllotedDatas2()
		{
			#region Params
			var ordNos = new List<string> { "S2018091000053", "S2018091000054" };
			#endregion

			_f05000201Repo.GetNotAllotedDatas(ordNos);
		}

		[TestMethod]
		public void Delete()
		{
			#region Params
			var ordNos = new List<string> { "S2019043000001", "S2019043000001" };
			var gupCode = "01";
			var custCode = "010001";
			var dcCode = "001";
			#endregion

			_f05000201Repo.Delete(ordNos, gupCode, custCode, dcCode);
		}

		[TestMethod]
		public void DeleteF050002()
		{
			#region Params
			var ordNo = "S2018091000054";
			#endregion

			_f05000201Repo.DeleteF050002(ordNo);
		}


		[TestMethod]
		public void GetF05002BySingleItem()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010002";
			var itemCode = "PS14122-10131";
			#endregion

			_f05000201Repo.GetF05002BySingleItem(dcCode, gupCode, custCode, itemCode);
		}

		[TestMethod]
		public void UpdateCheckedSameItem()
		{
			#region Params

			var gupCode = "01";
			var custCode = "030002";
			var dcCode = "001";
			var itemCode = "CAPIE0035";
			var ordNos = new List<string> { "S2018091200007", "S2018091200008" };
			var checkedSameItem = "3";
			#endregion

			_f05000201Repo.UpdateCheckedSameItem(gupCode, custCode, dcCode, itemCode, ordNos, checkedSameItem);
		}

		[TestMethod]
		public void BulkDelete()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var ordNos = new List<string> { "S2020010200004" };
			#endregion

			_f05000201Repo.BulkDelete(dcCode, gupCode, custCode, ordNos);
		}

		[TestMethod]
		public void DeleteHasAllot()
		{
			_f05000201Repo.DeleteHasAllot();
		}

		[TestMethod]
		public void DeleteLackOrder()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			#endregion

			_f05000201Repo.DeleteLackOrder(gupCode, custCode);
		}
	}
}
