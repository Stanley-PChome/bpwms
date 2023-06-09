using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
	[TestClass]
	public class F910506RepositoryTest : BaseRepositoryTest
	{
		private readonly F910506Repository _f910506Repository;
		public F910506RepositoryTest()
		{
			_f910506Repository = new F910506Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void UpdateBoxNoForP910103()
		{
			string dcCode = "";
			string gupCode = "";
			string custCode = "";
			string processNo = "";
			string clientIp = "";
			string boxNo = "";
			Output(new { dcCode, gupCode, custCode, processNo, clientIp, boxNo });
			var result = _f910506Repository.UpdateBoxNoForP910103(dcCode, gupCode, custCode, processNo, clientIp, boxNo);
			Output(result);
		}

		[TestMethod]
		public void GetF910506ScanLog()
		{
			string dcCode = "";
			string gupCode = "";
			string custCode = "";
			string processNo = "";
			string clientIp = "";
			Output(new { dcCode, gupCode, custCode, processNo, clientIp });
			var result = _f910506Repository.GetF910506ScanLog(dcCode, gupCode, custCode, processNo, clientIp);
			Output(result);

		}

		private void Output(object obj)
		{
			Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
		}
	}
}
