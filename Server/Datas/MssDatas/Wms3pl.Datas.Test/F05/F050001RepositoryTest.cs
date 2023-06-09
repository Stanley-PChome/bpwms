using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
	[TestClass]
	public class F050001RepositoryTest : BaseRepositoryTest
	{
		private F050001Repository _f050001Repo;
		public F050001RepositoryTest()
		{
			_f050001Repo = new F050001Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetDatas()
		{
			_f050001Repo.GetAllDatas();
		}

		[TestMethod]
		public void GetNotAllotedDatas()
		{
			_f050001Repo.GetNotAllotedDatas();
		}

		[TestMethod]
		public void GetNotAllotedDatas2()
		{
			#region Params
			List<string> ordNos = new List<string> { "S2018091400015" };
			#endregion

			_f050001Repo.GetNotAllotedDatas(ordNos);
		}

		[TestMethod]
		public void UpdateZipCodeDc()
		{
			#region Params
			var ordNo = "S2018091000050";
			var gupCode = "01";
			var custCode = "030002";
			var zipCode = "11";
			var dcCode = "22";
			var typeId = "2";
			var addressParse = "333";
			#endregion

			_f050001Repo.UpdateZipCodeDc(ordNo, gupCode, custCode, zipCode, dcCode, typeId, addressParse);
		}


		[TestMethod]
		public void Delete()
		{
			#region Params
			List<string> ordNos = new List<string> { "S2020010300001", "S2020010300002" };
			var gupCode = "01";
			var custCode = "010001";
			var dcCode = "001";
			#endregion

			_f050001Repo.Delete(ordNos, gupCode, custCode, dcCode);
		}

		[TestMethod]
		public void SetStatus()
		{
			#region Params
			var status = "2";
			List<string> ordNos = new List<string> { "S2018091000053", "S2018091000054" };
			#endregion

			_f050001Repo.SetStatus(status, ordNos);
		}

		[TestMethod]
		public void SetStatus2()
		{
			#region Params
			var newStatus = "2";
			var oldStatus = "1";
			#endregion

			_f050001Repo.SetStatus(newStatus, oldStatus);
		}

		[TestMethod]
		public void GetF050001sByNoProcFlag9()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			var ordNo = "S2019043000001";
			#endregion

			_f050001Repo.GetF050001sByNoProcFlag9(gupCode, custCode, ordNo);
		}

		[TestMethod]
		public void DeleteF050001()
		{
			#region Params
			var ordNo = "S2018091100029";
			#endregion

			_f050001Repo.DeleteF050001(ordNo);
		}

		[TestMethod]
		public void DeleteHasAllot()
		{
			_f050001Repo.DeleteHasAllot();
		}

		[TestMethod]
		public void DeleteLackOrder()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			#endregion

			_f050001Repo.DeleteLackOrder(gupCode, custCode);
		}

    [TestMethod]
    public void LockNonAllotOrderStatus()
    {
      #region Params
      var f190105Repo = new F190105Repository(Schemas.CoreSchema);
      var AutoAllotCustList = f190105Repo.GetDatas(new[] { "12" }.ToList()).FirstOrDefault()?.AUTO_ALLOT_CUST_LIST.Split(',').ToList();
      var strAllotBatchNo = "BS" + DateTime.Now.ToString("yyyyMMddHHmmss");
      #endregion

      _f050001Repo.LockNonAllotOrderStatus(strAllotBatchNo, 3, 999);
      _f050001Repo.UnLockByAllotBatchNo(strAllotBatchNo);
    }


  }
}
