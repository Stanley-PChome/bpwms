using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F02;
using System;

namespace Wms3pl.Datas.Test
{
	[TestClass]
	public class F020203RepositoryTest : BaseRepositoryTest
	{
		public F020203RepositoryTest()
		{
		}
		[TestMethod]
		public void GetAcceptancePurchaseReport()
		{
			var repo = new F020203Repository(Schemas.CoreSchema);
			var r = repo.GetDataByKey("12", "10", "010001", "KK005", new DateTime(2021, 9, 6));
		}
	}
}
