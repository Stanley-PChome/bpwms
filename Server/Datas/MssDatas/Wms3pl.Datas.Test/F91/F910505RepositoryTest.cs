using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
	[TestClass]
	public class F910505RepositoryTest : BaseRepositoryTest
	{
		private readonly F910505Repository _f910505Repository;
		public F910505RepositoryTest()
		{
			_f910505Repository = new F910505Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetF910505ScanLog()
		{
			string dcCode = "";
			string gupCode = "";
			string custCode = "";
			string processNo = "";
			string clientIp = "";
			Output(new { dcCode, gupCode, custCode, processNo, clientIp });
			var result = _f910505Repository.GetF910505ScanLog(dcCode, gupCode, custCode, processNo, clientIp);
			Output(result);
		}

		[TestMethod]
		public void GetDisassembleNonScan()
		{
			string dcCode = "";
			string gupCode = "";
			string custCode = "";
			string processNo = "";
			string serialNo = "";
			Output(new { dcCode, gupCode, custCode, processNo, serialNo });
			var result = _f910505Repository.GetDisassembleNonScan(dcCode, gupCode, custCode, processNo, serialNo);
			Output(result);
		}

		private void Output(object obj)
		{
			Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
		}
	}
}
