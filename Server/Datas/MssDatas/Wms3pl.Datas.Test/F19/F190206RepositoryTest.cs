
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.Datas.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wms3pl.Datas.F19
{
	[TestClass]
	public partial class F190206RepositoryTest : BaseRepositoryTest
	{
		private F190206Repository _f190206Repo;
		public F190206RepositoryTest()
		{
			_f190206Repo = new F190206Repository(Schemas.CoreSchema);
		}


		[TestMethod]
		public void GetItemCheckList()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var itemCode = "HLIN-QY03";
			var purchaseNo = "";
			var purchaseSeq = "";
			var rtNo = "";
			var checkType = "00";
			#endregion

			_f190206Repo.GetItemCheckList(dcCode, gupCode, custCode, itemCode,
					purchaseNo, purchaseSeq, rtNo, checkType);
		}

		[TestMethod]
		public void GetQuickItemCheckList()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var purchaseNo = "123";
			var rtNo = "123123";
			var itemCodes = new List<string> { "HLIN-QY03" };
			var checkType = "00";
			#endregion

			_f190206Repo.GetQuickItemCheckList(dcCode, gupCode, custCode, purchaseNo, rtNo,
					itemCodes, checkType);
		}

		[TestMethod]
		public void GetDatas()
		{
			#region Params
			var gupCode = "01";
			var custCode = "010001";
			var checkType = "00";
			var itemCodes = new List<string> { "HLIN-QY03" };

			#endregion

			_f190206Repo.GetDatas(gupCode, custCode, checkType, itemCodes);
		}
	}
}
