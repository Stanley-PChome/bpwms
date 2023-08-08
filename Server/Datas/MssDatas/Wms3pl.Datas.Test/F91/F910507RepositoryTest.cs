using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
	[TestClass]
	public class F910507RepositoryTest : BaseRepositoryTest
	{
		private readonly F910507Repository _f910507Repository;
		public F910507RepositoryTest()
		{
			_f910507Repository = new F910507Repository(Schemas.CoreSchema);
		}
		[TestMethod]
		public void UpdateCaseNoForP910103()
		{
			string dcCode = "";
			string gupCode = "";
			string custCode = "";
			string processNo = "";
			string clientIp = "";
			string caseNo = "";
			Output(new { dcCode, gupCode, custCode, processNo, clientIp, caseNo });
			var result = _f910507Repository.UpdateCaseNoForP910103(dcCode, gupCode, custCode, processNo, clientIp, caseNo);
			Output(result);
		}

		[TestMethod]
		public void ClearCaseNoForP910103()
		{
			string dcCode = "";
			string gupCode = "";
			string custCode = "";
			string processNo = "";
			string clientIp = "";
			string caseNo = "";
			Output(new { dcCode, gupCode, custCode, processNo, clientIp, caseNo });
			var result = _f910507Repository.UpdateCaseNoForP910103(dcCode, gupCode, custCode, processNo, clientIp, caseNo);
			Output(result);
		}

		[TestMethod]
		public void ClearStatusForP910103()
		{
			string dcCode = "";
			string gupCode = "";
			string custCode = "";
			string processNo = "";
			string clientIp = "";
			string caseNo = "";
			Output(new { dcCode, gupCode, custCode, processNo, clientIp, caseNo });
			var result = _f910507Repository.UpdateCaseNoForP910103(dcCode, gupCode, custCode, processNo, clientIp, caseNo);
			Output(result);
		}

		[TestMethod]
		public void GetF910507ScanLog()
		{
			string dcCode = "";
			string gupCode = "";
			string custCode = "";
			string processNo = "";
			string clientIp = "";
			Output(new { dcCode, gupCode, custCode, processNo, clientIp });
			var result = _f910507Repository.GetF910507ScanLog(dcCode, gupCode, custCode, processNo, clientIp);
			Output(result);
		}

		private void Output(object obj)
		{
			Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
		}
	}
}
