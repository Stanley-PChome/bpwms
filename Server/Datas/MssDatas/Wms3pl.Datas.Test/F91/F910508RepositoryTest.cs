using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
	[TestClass]
	public class F910508RepositoryTest : BaseRepositoryTest
	{
		private readonly F910508Repository _f910508Repository;
		public F910508RepositoryTest()
		{
			_f910508Repository = new F910508Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetF910508ScanLog()
		{
			string dcCode = "";
			string gupCode = "";
			string custCode = "";
			string processNo = "";
			string clientIp = "";
			Output(new { dcCode, gupCode, custCode, processNo, clientIp });
			var result = _f910508Repository.GetF910508ScanLog(dcCode, gupCode, custCode, processNo, clientIp);
			Output(result);
		}

		private void Output(object obj)
		{
			Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
		}
	}
}
