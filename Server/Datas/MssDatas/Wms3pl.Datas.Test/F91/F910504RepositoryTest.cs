using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
	[TestClass]
	public class F910504RepositoryTest : BaseRepositoryTest
	{
		private readonly F910504Repository _f910504Repository;
		public F910504RepositoryTest()
		{
			_f910504Repository = new F910504Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetF910504ScanLog()
		{
			string dcCode = "";
			string gupCode = "";
			string custCode = "";
			string processNo = "";
			string clientIp = "";
			Output(new { dcCode, gupCode, custCode, processNo, clientIp });
			var result = _f910504Repository.GetF910504ScanLog(dcCode, gupCode, custCode, processNo, clientIp);
			Output(result);
		}

		private void Output(object obj)
		{
			Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
		}
	}
}
