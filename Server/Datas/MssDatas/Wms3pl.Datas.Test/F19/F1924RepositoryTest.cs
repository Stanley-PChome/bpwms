using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text.Json;

namespace Wms3pl.Datas.Test.F19
{
	[TestClass]
	public class F1924RepositoryTest : BaseRepositoryTest
	{
		private F1924Repository _f1924Repo;
		public F1924RepositoryTest()
		{
			_f1924Repo = new F1924Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void F1924WithF192403()
		{
			#region Params
			#endregion

			_f1924Repo.F1924WithF192403();

		}

		[TestMethod]
		public void Delete()
		{
			#region Params
			var empId = "A12345";
			var userId = "System";
			#endregion

			_f1924Repo.Delete(empId, userId);
		}

		[TestMethod]
		public void GetUserPassword()
		{
			#region Params
			var empId = "A12345";
			#endregion

			_f1924Repo.GetUserPassword(empId);
		}

		[TestMethod]
		public void EmpWithFuncionName()
		{
			#region Params
			var EmpId = "A12345";
			#endregion

			_f1924Repo.EmpWithFuncionName(EmpId);
		}

		[TestMethod]
		public void IsCanUseful()
		{
			#region Params
			var pickStaff = "A12345";
			var btId = "BP0101010001";
			#endregion

			_f1924Repo.IsCanUseful(pickStaff, btId);
		}

		[TestMethod]
		public void GetActiveDatas()
		{
			#region Params
			var grpIds = new List<decimal> { 1, 2 };
			#endregion

			_f1924Repo.GetActiveDatas(grpIds);
		}

		[TestMethod]
		public void GetEmpHasAuth()
		{
			#region Params
			var empID = "000001";
			#endregion

			_f1924Repo.GetEmpHasAuth(empID);
		}

		[TestMethod]
		public void CheckAcc()
		{
			#region Params
			var AccNo = "wms";
			#endregion

			_f1924Repo.CheckAcc(AccNo);
		}

		[TestMethod]
		public void GetUserInfo()
		{
			#region Params
			var AccNo = "A12345";
			#endregion

			_f1924Repo.GetUserInfo(AccNo);
		}

		[TestMethod]
		public void GetEmpName()
		{
			#region Params
			var AccNo = "A12345";
			#endregion

			_f1924Repo.GetEmpName(AccNo);
		}

    [TestMethod]
    public void GetDatasForEmpIds()
    {
      var res = _f1924Repo.GetDatasForEmpIds(new[] { "tester10" }.ToList());
      Trace.WriteLine(JsonSerializer.Serialize(res));
    }

  }

}
