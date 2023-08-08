using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F70;
using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Test.F70
{
	[TestClass]
	public class F700702RepositoryTest : BaseRepositoryTest
	{
		private F700702Repository _F700702Repo;
		public F700702RepositoryTest()
		{
			_F700702Repo = new F700702Repository(Schemas.CoreSchema);
		}
		[TestMethod]
		public void GetOrderCountForHour()
		{
			var a = _F700702Repo.GetOrderCountForHour(DateTime.Parse("2017/02/20"), DateTime.Parse("2017/02/20"));
			Trace.Write(JsonSerializer.Serialize(a));
		}

	}
}